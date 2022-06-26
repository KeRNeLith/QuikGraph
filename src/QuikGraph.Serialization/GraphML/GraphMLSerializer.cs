#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using JetBrains.Annotations;
using static QuikGraph.Serialization.ILHelpers;
using static QuikGraph.Serialization.XmlConstants;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// A GraphML (http://graphml.graphdrawing.org/) format serializer.
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
    public sealed class GraphMLSerializer<TVertex, TEdge, TGraph> : SerializerBase
        where TEdge : IEdge<TVertex>
        where TGraph : IEdgeListGraph<TVertex, TEdge>
    {
        #region Compiler

        private delegate void WriteVertexAttributesDelegate(
            [NotNull] XmlWriter writer,
            [NotNull] TVertex vertex);

        private delegate void WriteEdgeAttributesDelegate(
            [NotNull] XmlWriter writer,
            [NotNull] TEdge edge);

        private delegate void WriteGraphAttributesDelegate(
            [NotNull] XmlWriter writer,
            [NotNull] TGraph graph);

        private static class WriteDelegateCompiler
        {
            [NotNull]
            public static WriteVertexAttributesDelegate VertexAttributesWriter { get; }

            [NotNull]
            public static WriteEdgeAttributesDelegate EdgeAttributesWriter { get; }

            [NotNull]
            public static WriteGraphAttributesDelegate GraphAttributesWriter { get; }

            static WriteDelegateCompiler()
            {
                VertexAttributesWriter =
                    (WriteVertexAttributesDelegate)CreateWriteDelegate(
                        typeof(TVertex),
                        typeof(WriteVertexAttributesDelegate));

                EdgeAttributesWriter =
                    (WriteEdgeAttributesDelegate)CreateWriteDelegate(
                        typeof(TEdge),
                        typeof(WriteEdgeAttributesDelegate));

                GraphAttributesWriter =
                    (WriteGraphAttributesDelegate)CreateWriteDelegate(
                        typeof(TGraph),
                        typeof(WriteGraphAttributesDelegate));
            }

            private static void EmitWriteProperty(PropertySerializationInfo info, [NotNull] ILGenerator generator)
            {
                var @default = default(Label);
                PropertyInfo property = info.Property;

                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod is null)
                    throw new NotSupportedException($"Property {property.DeclaringType}.{property.Name} has no getter.");
                if (!Metadata.TryGetWriteValueMethod(property.PropertyType, out MethodInfo writeMethod))
                    throw new NotSupportedException($"Property {property.DeclaringType}.{property.Name} type is not supported.");

                // Ldarg_0 = writer
                // Ldarg_1 = element

                var defaultValueAttribute = Attribute.GetCustomAttribute(
                    property,
                    typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                if (defaultValueAttribute != null)
                {
                    @default = generator.DefineLabel();
                    object value = defaultValueAttribute.Value;
                    if (value is null)
                        throw new NotSupportedException($"Null default value is not supported for property {property.Name}.");
                    if (value.GetType() != property.PropertyType)
                        throw new InvalidOperationException($"Invalid default value type for property {property.Name}.");

                    EmitValue(generator, property, value);
                    generator.Emit(OpCodes.Ldarg_1);
                    EmitCall(generator, getMethod);
                    generator.Emit(OpCodes.Ceq);
                    generator.Emit(OpCodes.Brtrue, @default);
                }

                // For each property of the type,
                // write it to the XML writer (we need to take care of value types, etc...)
                // writer.WriteStartElement("data")
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, "data");
                generator.Emit(OpCodes.Ldstr, GraphMLConstants.GraphMLNamespace);
                EmitCall(generator, Metadata.WriteStartElementMethod);

                // writer.WriteStartAttribute("key");
                WriteXmlAttribute("key", info.Name);

                // writer.WriteValue(element.xxx);
                generator.Emit(OpCodes.Ldarg_0);
                GetFieldValue();
                EmitCall(generator, writeMethod);

                // writer.WriteEndElement()
                generator.Emit(OpCodes.Ldarg_0);
                EmitCall(generator, Metadata.WriteEndElementMethod);

                if (defaultValueAttribute != null)
                {
                    generator.MarkLabel(@default);
                }

                #region Local functions

                void GetFieldValue()
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    EmitCall(generator, getMethod);
                }

                void WriteXmlAttribute(string attributeName, string attributeValue)
                {
                    Debug.Assert(generator != null);
                    Debug.Assert(attributeName != null);
                    Debug.Assert(attributeValue != null);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldstr, attributeName);
                    generator.Emit(OpCodes.Ldstr, attributeValue);
                    EmitCall(generator, Metadata.WriteAttributeStringMethod);
                }

                #endregion
            }

            [NotNull]
            private static Delegate CreateWriteDelegate([NotNull] Type elementType, [NotNull] Type delegateType)
            {
                Debug.Assert(elementType != null);
                Debug.Assert(delegateType != null);

                var method = new DynamicMethod(
                    $"{DynamicMethodPrefix}Write{delegateType.Name}_{elementType.Name}",
                    typeof(void),
                    new[] { typeof(XmlWriter), elementType },
                    elementType.Module);

                ILGenerator generator = method.GetILGenerator();

                foreach (PropertySerializationInfo info in SerializationHelpers.GetAttributeProperties(elementType))
                {
                    EmitWriteProperty(info, generator);
                }

                generator.Emit(OpCodes.Ret);

                // Let's bake the method
                return method.CreateDelegate(delegateType);
            }
        }

        #endregion

        /// <summary>
        /// Serializes the given <paramref name="graph"/> instance into the given <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="graph">Graph instance to serialize.</param>
        /// <param name="vertexIdentity">Vertex identity method.</param>
        /// <param name="edgeIdentity">Edge identity method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while writing elements to GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Serializing value on property without getter, or with unsupported property type.</exception>
        public void Serialize(
            [NotNull] XmlWriter writer,
            [NotNull] TGraph graph,
            [NotNull] VertexIdentity<TVertex> vertexIdentity,
            [NotNull] EdgeIdentity<TVertex, TEdge> edgeIdentity)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            if (vertexIdentity is null)
                throw new ArgumentNullException(nameof(vertexIdentity));
            if (edgeIdentity is null)
                throw new ArgumentNullException(nameof(edgeIdentity));

            var worker = new WriterWorker(this, writer, graph, vertexIdentity, edgeIdentity);
            worker.Serialize();
        }

        internal sealed class WriterWorker
        {
            [NotNull]
            private readonly GraphMLSerializer<TVertex, TEdge, TGraph> _serializer;

            [NotNull]
            private readonly XmlWriter _writer;

            [NotNull]
            private readonly TGraph _graph;

            [NotNull]
            private readonly VertexIdentity<TVertex> _vertexIdentity;

            [NotNull]
            private readonly EdgeIdentity<TVertex, TEdge> _edgeIdentity;

            public WriterWorker(
                [NotNull] GraphMLSerializer<TVertex, TEdge, TGraph> serializer,
                [NotNull] XmlWriter writer,
                [NotNull] TGraph graph,
                [NotNull] VertexIdentity<TVertex> vertexIdentity,
                [NotNull] EdgeIdentity<TVertex, TEdge> edgeIdentity)
            {
                Debug.Assert(serializer != null);
                Debug.Assert(writer != null);
                Debug.Assert(graph != null);
                Debug.Assert(vertexIdentity != null);
                Debug.Assert(edgeIdentity != null);

                _serializer = serializer;
                _writer = writer;
                _graph = graph;
                _vertexIdentity = vertexIdentity;
                _edgeIdentity = edgeIdentity;
            }

            public void Serialize()
            {
                WriteHeader();
                WriteGraphAttributeDefinitions();
                WriteVertexAttributeDefinitions();
                WriteEdgeAttributeDefinitions();
                WriteGraphHeader();
                WriteVertices();
                WriteEdges();
                WriteGraphFooter();
                WriteFooter();
            }

            private void WriteHeader()
            {
                if (_serializer.EmitDocumentDeclaration)
                {
                    _writer.WriteStartDocument();
                }
                _writer.WriteStartElement(string.Empty, GraphMLTag, GraphMLConstants.GraphMLNamespace);
            }

            private void WriteFooter()
            {
                _writer.WriteEndElement();
                _writer.WriteEndDocument();
            }

            private void WriteGraphHeader()
            {
                _writer.WriteStartElement(GraphTag, GraphMLConstants.GraphMLNamespace);
                _writer.WriteAttributeString(IdAttribute, "G");
                _writer.WriteAttributeString("edgedefault", _graph.IsDirected ? "directed" : "undirected");
                _writer.WriteAttributeString("parse.nodes", _graph.VertexCount.ToString());
                _writer.WriteAttributeString("parse.edges", _graph.EdgeCount.ToString());
                _writer.WriteAttributeString("parse.order", "nodesfirst");
                _writer.WriteAttributeString("parse.nodeids", "free");
                _writer.WriteAttributeString("parse.edgeids", "free");

                WriteDelegateCompiler.GraphAttributesWriter(_writer, _graph);
            }

            private void WriteGraphFooter()
            {
                _writer.WriteEndElement();
            }

            private void WriteGraphAttributeDefinitions()
            {
                WriteAttributeDefinitions(GraphTag, typeof(TGraph));
            }

            private void WriteVertexAttributeDefinitions()
            {
                WriteAttributeDefinitions(NodeTag, typeof(TVertex));
            }

            private void WriteEdgeAttributeDefinitions()
            {
                WriteAttributeDefinitions(EdgeTag, typeof(TEdge));
            }

            private static string ConstructTypeCodeForSimpleType([NotNull] Type type)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return "boolean";
                    case TypeCode.Int32:
                        return "int";
                    case TypeCode.Int64:
                        return "long";
                    case TypeCode.Single:
                        return "float";
                    case TypeCode.Double:
                        return "double";
                    case TypeCode.String:
                        return "string";
                    case TypeCode.Object:
                        return "object";
                    default:
                        return "invalid";
                }
            }

            private static string ConstructTypeCode([NotNull] Type type)
            {
                string code = ConstructTypeCodeForSimpleType(type);
                if (code == "invalid")
                    throw new NotSupportedException("Simple type not supported by the GraphML schema.");

                // Recognize arrays of certain simple types. Type string is still "string" for all arrays,
                // because GraphML schema doesn't have an array type.
                if (code == "object")
                {
                    Type iListType = typeof(IList<>);
                    Type typeIListType = type.Name == iListType.Name ? type : type.GetInterface(iListType.Name, false);
                    if (typeIListType != null && typeIListType.Name == iListType.Name)
                    {
                        Type elementType = typeIListType.GetGenericArguments()[0];
                        string elementCode = ConstructTypeCodeForSimpleType(elementType);
                        if (elementCode == "object" || elementCode == "invalid")
                            throw new NotSupportedException("Array type not supported by GraphML schema.");
                        code = "string";
                    }
                }

                return code;
            }

            private void WriteAttributeDefinitions([NotNull] string elementName, [NotNull] Type elementType)
            {
                Debug.Assert(elementName != null);
                Debug.Assert(elementType != null);

                foreach (PropertySerializationInfo info in SerializationHelpers.GetAttributeProperties(elementType))
                {
                    PropertyInfo property = info.Property;
                    string name = info.Name;
                    Type propertyType = property.PropertyType;

                    // <key id="d1" for="edge" attr.name="weight" attr.type="double"/>
                    _writer.WriteStartElement("key", GraphMLConstants.GraphMLNamespace);
                    _writer.WriteAttributeString(IdAttribute, name);
                    _writer.WriteAttributeString("for", elementName);
                    _writer.WriteAttributeString("attr.name", name);

                    string typeCodeStr;
                    try
                    {
                        typeCodeStr = ConstructTypeCode(propertyType);
                    }
                    catch (NotSupportedException)
                    {
                        throw new NotSupportedException(
                            $"Property type {property.DeclaringType}.{property.Name} not supported by the GraphML schema.");
                    }

                    _writer.WriteAttributeString("attr.type", typeCodeStr);

                    // <default>...</default>
                    if (info.TryGetDefaultValue(out object defaultValue))
                    {
                        _writer.WriteStartElement("default");
                        Type defaultValueType = defaultValue.GetType();
                        switch (Type.GetTypeCode(defaultValueType))
                        {
                            case TypeCode.Boolean:
                                _writer.WriteString(XmlConvert.ToString((bool)defaultValue));
                                break;
                            case TypeCode.Int32:
                                _writer.WriteString(XmlConvert.ToString((int)defaultValue));
                                break;
                            case TypeCode.Int64:
                                _writer.WriteString(XmlConvert.ToString((long)defaultValue));
                                break;
                            case TypeCode.Single:
                                _writer.WriteString(XmlConvert.ToString((float)defaultValue));
                                break;
                            case TypeCode.Double:
                                _writer.WriteString(XmlConvert.ToString((double)defaultValue));
                                break;
                            case TypeCode.String:
                                _writer.WriteString((string)defaultValue);
                                break;
                            case TypeCode.Object:
                                if (defaultValueType.IsArray)
                                    throw new NotSupportedException("Default values for array types are not supported.");
                                throw new NotSupportedException(
                                    $"Property type {property.DeclaringType}.{property.Name} not supported by the GraphML schema.");
                            default:
                                throw new NotSupportedException(
                                    $"Property type {property.DeclaringType}.{property.Name} not supported by the GraphML schema.");
                        }

                        _writer.WriteEndElement();
                    }

                    _writer.WriteEndElement();
                }
            }

            private void WriteVertices()
            {
                foreach (TVertex vertex in _graph.Vertices)
                {
                    _writer.WriteStartElement(NodeTag, GraphMLConstants.GraphMLNamespace);
                    _writer.WriteAttributeString(IdAttribute, _vertexIdentity(vertex));
                    WriteDelegateCompiler.VertexAttributesWriter(_writer, vertex);
                    _writer.WriteEndElement();
                }
            }

            private void WriteEdges()
            {
                foreach (TEdge edge in _graph.Edges)
                {
                    _writer.WriteStartElement(EdgeTag, GraphMLConstants.GraphMLNamespace);
                    _writer.WriteAttributeString(IdAttribute, _edgeIdentity(edge));
                    _writer.WriteAttributeString(SourceAttribute, _vertexIdentity(edge.Source));
                    _writer.WriteAttributeString(TargetAttribute, _vertexIdentity(edge.Target));
                    WriteDelegateCompiler.EdgeAttributesWriter(_writer, edge);
                    _writer.WriteEndElement();
                }
            }
        }
    }

    internal static partial class Metadata
    {
        [NotNull]
        public static readonly MethodInfo WriteStartElementMethod =
            typeof(XmlWriter).GetMethod(
                nameof(XmlWriter.WriteStartElement),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(string) },
                null) ?? throw new InvalidOperationException(
                $"Cannot find {nameof(XmlWriter.WriteStartElement)} method on {nameof(XmlWriter)}.");

        [NotNull]
        public static readonly MethodInfo WriteEndElementMethod =
            typeof(XmlWriter).GetMethod(
                nameof(XmlWriter.WriteEndElement),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null) ?? throw new InvalidOperationException(
                $"Cannot find {nameof(XmlWriter.WriteEndElement)} method on {nameof(XmlWriter)}.");

        [NotNull]
        public static readonly MethodInfo WriteAttributeStringMethod =
            typeof(XmlWriter).GetMethod(
                nameof(XmlWriter.WriteAttributeString),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(string) },
                null) ?? throw new InvalidOperationException(
                $"Cannot find {nameof(XmlWriter.WriteAttributeString)} method on {nameof(XmlWriter)}.");

        [NotNull]
        private static readonly Dictionary<Type, MethodInfo> WriteContentMethods = InitializeWriteMethods();

        [NotNull]
        private static Dictionary<Type, MethodInfo> InitializeWriteMethods()
        {
            Type writerType = typeof(XmlWriter);
            Type writerExtensionsType = typeof(XmlWriterExtensions);

            return new Dictionary<Type, MethodInfo>
            {
                [typeof(bool)] = writerType.GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(bool) }),
                [typeof(int)] = writerType.GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(int) }),
                [typeof(long)] = writerType.GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(long) }),
                [typeof(float)] = writerType.GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(float) }),
                [typeof(double)] = writerType.GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(double) }),
                [typeof(string)] = writerType.GetMethod(nameof(XmlWriter.WriteString), new[] { typeof(string) }),

                // Extensions
                [typeof(bool[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteBooleanArray)),
                [typeof(int[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteInt32Array)),
                [typeof(long[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteInt64Array)),
                [typeof(float[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteSingleArray)),
                [typeof(double[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteDoubleArray)),
                [typeof(string[])] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteStringArray)),

                [typeof(IList<bool>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteBooleanArray)),
                [typeof(IList<int>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteInt32Array)),
                [typeof(IList<long>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteInt64Array)),
                [typeof(IList<float>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteSingleArray)),
                [typeof(IList<double>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteDoubleArray)),
                [typeof(IList<string>)] = writerExtensionsType.GetMethod(nameof(XmlWriterExtensions.WriteStringArray))
            };
        }

        [Pure]
        public static bool TryGetWriteValueMethod([NotNull] Type type, out MethodInfo method)
        {
            Debug.Assert(type != null);

            bool status = WriteContentMethods.TryGetValue(type, out method);
            return status;
        }
    }
}
#endif