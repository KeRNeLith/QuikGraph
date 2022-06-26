#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using JetBrains.Annotations;
using static QuikGraph.Serialization.ILHelpers;
using static QuikGraph.Serialization.XmlConstants;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// A GraphML (http://graphml.graphdrawing.org/) format deserializer.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <remarks>
    /// <para>
    /// Custom vertex, edge and graph attributes can be specified by 
    /// using the <see cref="T:System.Xml.Serialization.XmlAttributeAttribute"/> attribute on properties (field not supported).
    /// </para>
    /// <para>
    /// The serializer uses LCG (lightweight code generation) to generate the 
    /// methods that writes the attributes to avoid paying the price of 
    /// Reflection on each vertex/edge. Since nothing is for free, the first
    /// time you will use the serializer *on a particular pair of types*, it
    /// will have to bake that method.
    /// </para>
    /// <para>
    /// Hyper edge, nodes, nested graphs not supported.
    /// </para>
    /// </remarks>
    public sealed class GraphMLDeserializer<TVertex, TEdge, TGraph> : SerializerBase
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
    {
        #region Compiler

        private delegate void ReadVertexAttributesDelegate(
            [NotNull] XmlReader reader,
            [NotNull] string namespaceUri,
            [NotNull] TVertex vertex);

        private delegate void ReadEdgeAttributesDelegate(
            [NotNull] XmlReader reader,
            [NotNull] string namespaceUri,
            [NotNull] TEdge edge);

        private delegate void ReadGraphAttributesDelegate(
            [NotNull] XmlReader reader,
            [NotNull] string namespaceUri,
            [NotNull] TGraph graph);

        private static class ReadDelegateCompiler
        {
            [NotNull]
            public static ReadVertexAttributesDelegate VertexAttributesReader { get; }

            [NotNull]
            public static ReadEdgeAttributesDelegate EdgeAttributesReader { get; }

            [NotNull]
            public static ReadGraphAttributesDelegate GraphAttributesReader { get; }

            [NotNull]
            public static Action<TVertex> SetVertexDefault { get; }

            [NotNull]
            public static Action<TEdge> SetEdgeDefault { get; }

            [NotNull]
            public static Action<TGraph> SetGraphDefault { get; }

            static ReadDelegateCompiler()
            {
                VertexAttributesReader =
                    (ReadVertexAttributesDelegate)CreateReadDelegate(
                    typeof(ReadVertexAttributesDelegate),
                    typeof(TVertex)); //,"id"

                EdgeAttributesReader =
                    (ReadEdgeAttributesDelegate)CreateReadDelegate(
                    typeof(ReadEdgeAttributesDelegate),
                    typeof(TEdge)); //,"id", "source", "target"

                GraphAttributesReader =
                    (ReadGraphAttributesDelegate)CreateReadDelegate(
                    typeof(ReadGraphAttributesDelegate),
                    typeof(TGraph));

                SetVertexDefault =
                    (Action<TVertex>)CreateSetDefaultDelegate(
                        typeof(Action<TVertex>),
                        typeof(TVertex));

                SetEdgeDefault =
                    (Action<TEdge>)CreateSetDefaultDelegate(
                        typeof(Action<TEdge>),
                        typeof(TEdge));

                SetGraphDefault =
                    (Action<TGraph>)CreateSetDefaultDelegate(
                        typeof(Action<TGraph>),
                        typeof(TGraph));
            }

            [NotNull]
            private static Delegate CreateSetDefaultDelegate(
                [NotNull] Type delegateType,
                [NotNull] Type elementType)
            {
                Debug.Assert(delegateType != null);
                Debug.Assert(elementType != null);

                var method = new DynamicMethod(
                    $"{DynamicMethodPrefix}Set{elementType.Name}Default",
                    typeof(void),
                    new[] { elementType },
                    elementType.Module);
                ILGenerator generator = method.GetILGenerator();

                // We need to create the switch for each property
                IEnumerable<PropertyInfo> properties = SerializationHelpers
                    .GetAttributeProperties(elementType)
                    .Select(info => info.Property);
                foreach (PropertyInfo property in properties)
                {
                    var defaultValueAttribute = Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                    if (defaultValueAttribute is null)
                        continue;

                    MethodInfo setMethod = property.GetSetMethod();
                    if (setMethod is null)
                        throw new InvalidOperationException($"Property {property.Name} is not settable.");
                    if (property.PropertyType.IsArray)
                        throw new NotSupportedException("Default values for array types are not implemented.");

                    object value = defaultValueAttribute.Value;
                    if (value is null)
                        throw new NotSupportedException($"Null default value is not supported for property {property.Name}.");
                    if (value.GetType() != property.PropertyType)
                        throw new InvalidOperationException($"Invalid default value type for property {property.Name}.");

                    generator.Emit(OpCodes.Ldarg_0);
                    EmitValue(generator, property, value);
                    EmitCall(generator, setMethod);
                }

                generator.Emit(OpCodes.Ret);

                // Let's bake the method
                return method.CreateDelegate(delegateType);
            }

            [NotNull]
            private static Delegate CreateReadDelegate(
                [NotNull] Type delegateType,
                [NotNull] Type elementType)
            {
                Debug.Assert(delegateType != null);
                Debug.Assert(elementType != null);

                var method = new DynamicMethod(
                    $"{DynamicMethodPrefix}Read{elementType.Name}",
                    typeof(void),
                    new[] { typeof(XmlReader), typeof(string), elementType },
                    elementType.Module);
                ILGenerator generator = method.GetILGenerator();

                // Ldarg_0 = reader
                // Ldarg_1 = namespace URI
                // Ldarg_2 = element

                generator.DeclareLocal(typeof(string));

                GetXmlAttribute("key");
                generator.Emit(OpCodes.Stloc_0);

                // We need to create the switch for each property
                Label next = generator.DefineLabel();
                Label @return = generator.DefineLabel();
                bool first = true;
                foreach (PropertySerializationInfo info in SerializationHelpers.GetAttributeProperties(elementType))
                {
                    PropertyInfo property = info.Property;
                    if (!first)
                    {
                        generator.MarkLabel(next);
                        next = generator.DefineLabel();
                    }
                    first = false;

                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldstr, info.Name);
                    EmitCall(generator, Metadata.StringEqualsMethod);

                    // If false jump to next
                    generator.Emit(OpCodes.Brfalse, next);

                    // Do our stuff
                    if (!Metadata.TryGetReadContentMethod(property.PropertyType, out MethodInfo readMethod))
                        throw new NotSupportedException($"Property {property.Name} has a non supported type.");

                    // Do we have a set method?
                    MethodInfo setMethod = property.GetSetMethod();
                    if (setMethod is null)
                        throw new InvalidOperationException($"Property {property.DeclaringType}.{property.Name} has no setter.");

                    // element.xxx = reader.ReadXXX
                    generator.Emit(OpCodes.Ldarg_2); // element
                    generator.Emit(OpCodes.Ldarg_0); // reader
                    generator.Emit(OpCodes.Ldstr, "data");
                    generator.Emit(OpCodes.Ldarg_1); // namespace URI
                    EmitCall(generator, readMethod);
                    EmitCall(generator, setMethod);

                    // Jump to do while
                    generator.Emit(OpCodes.Br, @return);
                }

                // We don't know this parameter.. we throw
                generator.MarkLabel(next);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Newobj, Metadata.ArgumentExceptionCtor);
                generator.Emit(OpCodes.Throw);

                generator.MarkLabel(@return);
                generator.Emit(OpCodes.Ret);

                // Let's bake the method
                return method.CreateDelegate(delegateType);

                #region Local function

                void GetXmlAttribute(string attributeName)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldstr, attributeName);
                    EmitCall(generator, Metadata.GetAttributeMethod);
                }

                #endregion
            }
        }

        #endregion

        /// <summary>
        /// Deserializes a graph instance from the given <paramref name="reader"/> into the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        /// <param name="graph">Graph instance to fill.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Deserializing value on property without setter, or with invalid default value.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="GraphMLTag"/> or <see cref="GraphTag"/> not found.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while reading elements from GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Deserializing graph with unsupported property type.</exception>
        public void Deserialize(
            [NotNull] XmlReader reader,
            [NotNull] TGraph graph,
            [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
            [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            if (vertexFactory is null)
                throw new ArgumentNullException(nameof(vertexFactory));
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));

            var worker = new ReaderWorker(reader, graph, vertexFactory, edgeFactory);
            worker.Deserialize();
        }

        private sealed class ReaderWorker
        {
            [NotNull]
            private readonly XmlReader _reader;

            [NotNull]
            private readonly TGraph _graph;

            [NotNull]
            private readonly IdentifiableVertexFactory<TVertex> _vertexFactory;

            [NotNull]
            private readonly IdentifiableEdgeFactory<TVertex, TEdge> _edgeFactory;

            [NotNull]
            private string _graphMLNamespace = string.Empty;

            public ReaderWorker(
                [NotNull] XmlReader reader,
                [NotNull] TGraph graph,
                [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
                [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
            {
                Debug.Assert(reader != null);
                Debug.Assert(graph != null);
                Debug.Assert(vertexFactory != null);
                Debug.Assert(edgeFactory != null);

                _reader = reader;
                _graph = graph;
                _vertexFactory = vertexFactory;
                _edgeFactory = edgeFactory;
            }

            public void Deserialize()
            {
                ReadHeader();
                ReadGraphHeader();
                ReadElements();
            }

            private void ReadHeader()
            {
                // Read flow until we hit the graphML node
                while (_reader.Read())
                {
                    if (_reader.NodeType == XmlNodeType.Element && _reader.Name == GraphMLTag)
                    {
                        _graphMLNamespace = _reader.NamespaceURI;
                        return;
                    }
                }

                throw new InvalidOperationException($"\"{GraphMLTag}\" node not found.");
            }

            private void ReadGraphHeader()
            {
                if (!_reader.ReadToDescendant(GraphTag, _graphMLNamespace))
                    throw new InvalidOperationException($"\"{GraphTag}\" node not found.");
            }

            private void ReadElements()
            {
                Debug.Assert(
                    _reader.Name == GraphTag && _reader.NamespaceURI == _graphMLNamespace,
                    "Incorrect reader position.");

                ReadDelegateCompiler.SetGraphDefault(_graph);

                var vertices = new Dictionary<string, TVertex>(StringComparer.Ordinal);

                // Read vertices or edges
                while (_reader.Read())
                {
                    if (_reader.NodeType == XmlNodeType.Element
                        && _reader.NamespaceURI == _graphMLNamespace)
                    {
                        switch (_reader.Name)
                        {
                            case NodeTag:
                                ReadVertex(vertices);
                                break;
                            case EdgeTag:
                                ReadEdge(vertices);
                                break;
                            case DataTag:
                                ReadDelegateCompiler.GraphAttributesReader(_reader, _graphMLNamespace, _graph);
                                break;
                            default:
                                throw new InvalidOperationException($"Invalid reader position {_reader.NamespaceURI}: {_reader.Name}.");
                        }
                    }
                }
            }

            private void ReadVertex([NotNull] IDictionary<string, TVertex> vertices)
            {
                Debug.Assert(vertices != null);
                Debug.Assert(
                    _reader.NodeType == XmlNodeType.Element
                    && _reader.Name == NodeTag
                    && _reader.NamespaceURI == _graphMLNamespace);

                // Get subtree
                using (XmlReader subReader = _reader.ReadSubtree())
                {
                    // Read id
                    string id = ReadAttributeValue(_reader, IdAttribute);
                    // Create new vertex
                    TVertex vertex = _vertexFactory(id);
                    // Apply defaults
                    ReadDelegateCompiler.SetVertexDefault(vertex);

                    // Read data
                    while (subReader.Read())
                    {
                        while (subReader.NodeType == XmlNodeType.Element
                            && subReader.Name == DataTag
                            && subReader.NamespaceURI == _graphMLNamespace)
                        {
                            ReadDelegateCompiler.VertexAttributesReader(subReader, _graphMLNamespace, vertex);
                        }
                    }

                    // Add to graph
                    _graph.AddVertex(vertex);
                    vertices.Add(id, vertex);
                }
            }

            private void ReadEdge([NotNull] IDictionary<string, TVertex> vertices)
            {
                Debug.Assert(vertices != null);
                Debug.Assert(
                    _reader.NodeType == XmlNodeType.Element
                    && _reader.Name == EdgeTag
                    && _reader.NamespaceURI == _graphMLNamespace);

                // Get subtree
                using (XmlReader subReader = _reader.ReadSubtree())
                {
                    // Read id
                    string id = ReadAttributeValue(_reader, IdAttribute);
                    string sourceId = ReadAttributeValue(_reader, SourceAttribute);
                    if (!vertices.TryGetValue(sourceId, out TVertex source))
                        throw new ArgumentException($"Could not find vertex {sourceId}.");
                    string targetId = ReadAttributeValue(_reader, TargetAttribute);
                    if (!vertices.TryGetValue(targetId, out TVertex target))
                        throw new ArgumentException($"Could not find vertex {targetId}.");

                    TEdge edge = _edgeFactory(source, target, id);
                    ReadDelegateCompiler.SetEdgeDefault(edge);

                    // Read data
                    while (subReader.Read())
                    {
                        while (subReader.NodeType == XmlNodeType.Element
                            && subReader.Name == DataTag
                            && subReader.NamespaceURI == _graphMLNamespace)
                        {
                            ReadDelegateCompiler.EdgeAttributesReader(subReader, _graphMLNamespace, edge);
                        }
                    }

                    _graph.AddEdge(edge);
                }
            }

            private static string ReadAttributeValue([NotNull] XmlReader reader, [NotNull] string attributeName)
            {
                Debug.Assert(reader != null);
                Debug.Assert(attributeName != null);

                reader.MoveToAttribute(attributeName);
                if (!reader.ReadAttributeValue())
                    throw new ArgumentException($"Missing {attributeName} attribute.");
                return reader.Value;
            }
        }
    }

    internal static partial class Metadata
    {
        [NotNull]
        public static readonly MethodInfo GetAttributeMethod =
            typeof(XmlReader).GetMethod(
                nameof(XmlReader.GetAttribute),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string) },
                null) ?? throw new InvalidOperationException($"Cannot find {nameof(XmlReader.GetAttribute)} method on {nameof(XmlReader)}.");

        [NotNull]
        public static MethodInfo StringEqualsMethod { get; } =
            typeof(string).GetMethod(
                "op_Equality",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(string) },
                null) ?? throw new InvalidOperationException("Cannot find == operator method on string.");

        [NotNull]
        public static readonly ConstructorInfo ArgumentExceptionCtor =
            typeof(ArgumentException).GetConstructor(new[] { typeof(string) })
            ?? throw new InvalidOperationException($"Cannot find {nameof(ArgumentException)} constructor.");

        [NotNull]
        private static readonly Dictionary<Type, MethodInfo> ReadContentMethods = InitializeReadMethods();

        [NotNull]
        private static Dictionary<Type, MethodInfo> InitializeReadMethods()
        {
            Type readerType = typeof(XmlReader);
            Type readerExtensionsType = typeof(XmlReaderExtensions);

            return new Dictionary<Type, MethodInfo>
            {
                [typeof(bool)] = readerType.GetMethod(nameof(XmlReader.ReadElementContentAsBoolean), new[] { typeof(string), typeof(string) }),
                [typeof(int)] = readerType.GetMethod(nameof(XmlReader.ReadElementContentAsInt), new[] { typeof(string), typeof(string) }),
                [typeof(long)] = readerType.GetMethod(nameof(XmlReader.ReadElementContentAsLong), new[] { typeof(string), typeof(string) }),
                [typeof(float)] = readerType.GetMethod(nameof(XmlReader.ReadElementContentAsFloat), new[] { typeof(string), typeof(string) }),
                [typeof(double)] = readerType.GetMethod(nameof(XmlReader.ReadElementContentAsDouble), new[] { typeof(string), typeof(string) }),
                [typeof(string)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementAsNullableString)),

                // Extensions
                [typeof(bool[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsBooleanArray)),
                [typeof(int[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsInt32Array)),
                [typeof(long[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsInt64Array)),
                [typeof(float[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsSingleArray)),
                [typeof(double[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsDoubleArray)),
                [typeof(string[])] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsStringArray)),

                [typeof(IList<bool>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsBooleanArray)),
                [typeof(IList<int>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsInt32Array)),
                [typeof(IList<long>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsInt64Array)),
                [typeof(IList<float>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsSingleArray)),
                [typeof(IList<double>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsDoubleArray)),
                [typeof(IList<string>)] = readerExtensionsType.GetMethod(nameof(XmlReaderExtensions.ReadElementContentAsStringArray))
            };
        }

        [Pure]
        public static bool TryGetReadContentMethod([NotNull] Type type, out MethodInfo method)
        {
            Debug.Assert(type != null);

            bool result = ReadContentMethods.TryGetValue(type, out method);

            Debug.Assert(!result || method != null);

            return result;
        }
    }
}
#endif