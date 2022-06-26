#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System;
#if SUPPORTS_XML_DTD_PROCESSING
using System.Xml.Schema;
#endif
using System.IO;
using System.Xml;
using JetBrains.Annotations;
using QuikGraph.Algorithms;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// Extensions to help serializing/deserializing graph to/from GraphML.
    /// </summary>
    public static class GraphMLExtensions
    {
        #region Serialization

        [NotNull]
        private const string SerializationIndent = "    ";

        /// <summary>
        /// Serializes the given <paramref name="graph"/> into GraphML in a file at given <paramref name="filePath"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to serialize.</param>
        /// <param name="filePath">Path to the file where serializing the graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="filePath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while writing elements to GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Serializing value on property without getter, or with unsupported property type.</exception>
        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] string filePath)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Must provide a file path.", nameof(filePath));

            var settings = new XmlWriterSettings { Indent = true, IndentChars = SerializationIndent };
            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                SerializeToGraphML<TVertex, TEdge, TGraph>(graph, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Serializes the given <paramref name="graph"/> into GraphML in a file at given <paramref name="filePath"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to serialize.</param>
        /// <param name="filePath">Path to the file where serializing the graph.</param>
        /// <param name="vertexIdentity">Vertex identity method.</param>
        /// <param name="edgeIdentity">Edge identity method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="filePath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while writing elements to GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Serializing value on property without getter, or with unsupported property type.</exception>
        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] string filePath,
            [NotNull] VertexIdentity<TVertex> vertexIdentity,
            [NotNull] EdgeIdentity<TVertex, TEdge> edgeIdentity)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Must provide a file path.", nameof(filePath));

            var settings = new XmlWriterSettings { Indent = true, IndentChars = SerializationIndent };
            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                SerializeToGraphML(graph, writer, vertexIdentity, edgeIdentity);
                writer.Flush();
            }
        }

        /// <summary>
        /// Serializes the given <paramref name="graph"/> into GraphML in the given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to serialize.</param>
        /// <param name="writer">The XML writer.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while writing elements to GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Serializing value on property without getter, or with unsupported property type.</exception>
        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] XmlWriter writer)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            VertexIdentity<TVertex> vertexIdentity = graph.GetVertexIdentity();
            EdgeIdentity<TVertex, TEdge> edgeIdentity = graph.GetEdgeIdentity();

            SerializeToGraphML(graph, writer, vertexIdentity, edgeIdentity);
        }

        /// <summary>
        /// Serializes the given <paramref name="graph"/> into GraphML in the given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to serialize.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="vertexIdentity">Vertex identity method.</param>
        /// <param name="edgeIdentity">Edge identity method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while writing elements to GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Serializing value on property without getter, or with unsupported property type.</exception>
        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] XmlWriter writer,
            [NotNull] VertexIdentity<TVertex> vertexIdentity,
            [NotNull] EdgeIdentity<TVertex, TEdge> edgeIdentity)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            var serializer = new GraphMLSerializer<TVertex, TEdge, TGraph>();
            serializer.Serialize(writer, graph, vertexIdentity, edgeIdentity);
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Deserializes from XML <paramref name="reader"/> (GraphML graph) into the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to fill.</param>
        /// <param name="reader">The XML reader.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Deserializing value on property without setter, or with invalid default value.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="XmlConstants.GraphMLTag"/> or <see cref="XmlConstants.GraphTag"/> not found.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while reading elements from GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Deserializing graph with unsupported property type.</exception>
        public static void DeserializeFromGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] XmlReader reader,
            [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
            [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
        {
            var serializer = new GraphMLDeserializer<TVertex, TEdge, TGraph>();
            serializer.Deserialize(reader, graph, vertexFactory, edgeFactory);
        }

        /// <summary>
        /// Deserializes from the given <paramref name="reader"/> (GraphML graph) into the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to fill.</param>
        /// <param name="reader">Reader stream.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Deserializing value on property without setter, or with invalid default value.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="XmlConstants.GraphMLTag"/> or <see cref="XmlConstants.GraphTag"/> not found.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while reading elements from GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Deserializing graph with unsupported property type.</exception>
        public static void DeserializeFromGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] TextReader reader,
            [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
            [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

#if SUPPORTS_XML_DTD_PROCESSING
            var settings = new XmlReaderSettings
            {
                ValidationFlags = XmlSchemaValidationFlags.None,
                XmlResolver = new GraphMLXmlResolver(),
                DtdProcessing = DtdProcessing.Ignore
            };

            using (XmlReader xmlReader = XmlReader.Create(reader, settings))
                DeserializeFromGraphML(graph, xmlReader, vertexFactory, edgeFactory);
#else
            var xmlReader = new XmlTextReader(reader)
            {
                XmlResolver = null
            };

            DeserializeFromGraphML(graph, xmlReader, vertexFactory, edgeFactory);
#endif
        }

        /// <summary>
        /// Deserializes the given file at <paramref name="filePath"/> (GraphML graph) into the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to fill.</param>
        /// <param name="filePath">Path to the file to load.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="filePath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">Deserializing value on property without setter, or with invalid default value.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="XmlConstants.GraphMLTag"/> or <see cref="XmlConstants.GraphTag"/> not found.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while reading elements from GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Deserializing graph with unsupported property type.</exception>
        public static void DeserializeFromGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] string filePath,
            [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
            [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Must provide a file path.", nameof(filePath));

            using (var reader = new StreamReader(filePath))
            {
                DeserializeFromGraphML(graph, reader, vertexFactory, edgeFactory);
            }
        }

#if SUPPORTS_XML_DTD_PROCESSING
        /// <summary>
        /// Deserializes from the given <paramref name="reader"/> (GraphML graph) into the given <paramref name="graph"/>
        /// and checks if content is valid.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph instance to fill.</param>
        /// <param name="reader">Reader stream.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Deserializing value on property without setter, or with invalid default value.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="XmlConstants.GraphMLTag"/> or <see cref="XmlConstants.GraphTag"/> not found.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failure while reading elements from GraphML.</exception>
        /// <exception cref="T:System.NotSupportedException">Deserializing graph with unsupported property type.</exception>
        public static void DeserializeAndValidateFromGraphML<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] TextReader reader,
            [NotNull] IdentifiableVertexFactory<TVertex> vertexFactory,
            [NotNull] IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var serializer = new GraphMLDeserializer<TVertex, TEdge, TGraph>();

            var resolver = new GraphMLXmlResolver();
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                XmlResolver = resolver,
                DtdProcessing = DtdProcessing.Ignore
            };

            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            // Add GraphML schema
            AddGraphMLSchema(settings, resolver);

            try
            {
                settings.ValidationEventHandler += ValidationEventHandler;

                // Read and validate
                using (XmlReader xmlReader = XmlReader.Create(reader, settings))
                {
                    serializer.Deserialize(xmlReader, graph, vertexFactory, edgeFactory);
                }
            }
            finally
            {
                settings.ValidationEventHandler -= ValidationEventHandler;
            }
        }

        private static void AddGraphMLSchema([NotNull] XmlReaderSettings settings, [NotNull] XmlResolver resolver)
        {
            using (Stream xsdStream = GraphMLResourceResolver.GetResource("graphml.xsd"))
            {
                settings.Schemas.XmlResolver = resolver;
                // ReSharper disable once AssignNullToNotNullAttribute, Justification: assert above
                using (XmlReader xsdReader = XmlReader.Create(xsdStream, settings))
                {
                    settings.Schemas.Add(GraphMLConstants.GraphMLNamespace, xsdReader);
                }
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
                throw new InvalidOperationException(args.Message);
        }
#endif

        #endregion
    }
}
#endif