using System;
using System.Xml;
#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
#endif
#if SUPPORTS_XML_GRAPH_SERIALIZATION
using System.Xml.XPath;
#endif
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// Extensions to serialize/deserialize graphs.
    /// </summary>
    public static class SerializationExtensions
    {
#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
        /// <summary>
        /// Serializes the <paramref name="graph"/> to the <paramref name="stream"/> using the .NET serialization binary formatter.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to serialize.</param>
        /// <param name="stream">Stream in which serializing the graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
#if DEPRECATE_BINARY_SERIALIZATION
        [Obsolete(
            "Binary serialization on old .NET targets is deprecated.\n" +
            "Consider using another kind of serialization or updating to at least .NET Framework 4.6.1+, .NET Standard 2.0+ or .NET 5.0+.")]
#endif
        public static void SerializeToBinary<TVertex, TEdge>(
            [NotNull] this IGraph<TVertex, TEdge> graph,
            [NotNull] Stream stream)
            where TEdge : IEdge<TVertex>
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("Must be a writable stream", nameof(stream));

            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, graph);
        }

        /// <summary>
        /// Deserializes a graph instance from a <paramref name="stream"/> that was serialized using the .NET serialization binary formatter.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="stream">Stream from which deserializing the graph.</param>
        /// <param name="binder">
        /// <para>
        /// <see cref="T:System.Runtime.Serialization.SerializationBinder"/> used during deserialization.
        /// It can be used to check/filter/replace/upgrade types that are loaded.
        /// </para>
        /// <para>It is also useful in security scenarios.</para>
        /// <para>By default no binder is used.</para>
        /// </param>
        /// <returns>Deserialized graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="stream"/> is not readable.</exception>
        [Pure]
#if DEPRECATE_BINARY_SERIALIZATION
        [Obsolete(
            "Binary deserialization on old .NET targets is deprecated.\n" +
            "Consider using another kind of serialization or updating to at least .NET Framework 4.6.1+, .NET Standard 2.0+ or .NET 5.0+.")]
#endif
        public static TGraph DeserializeFromBinary<TVertex, TEdge, TGraph>(
            [NotNull] this Stream stream,
            [CanBeNull] SerializationBinder binder = null)
            where TGraph : IGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Must be a readable stream", nameof(stream));

            var formatter = new BinaryFormatter { Binder = binder };
            object result = formatter.Deserialize(stream);
            return (TGraph)result;
        }
#endif

        [Pure]
        private static TGraph DeserializeFromXmlInternal<TVertex, TEdge, TGraph>(
            [NotNull] XmlReader reader,
            [NotNull, InstantHandle] Predicate<XmlReader> graphPredicate,
            [NotNull, InstantHandle] Predicate<XmlReader> vertexPredicate,
            [NotNull, InstantHandle] Predicate<XmlReader> edgePredicate,
            [NotNull, InstantHandle] Func<XmlReader, TGraph> graphFactory,
            [NotNull, InstantHandle] Func<XmlReader, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<XmlReader, TEdge> edgeFactory)
            where TGraph : class, IMutableVertexAndEdgeSet<TVertex, TEdge> where TEdge : IEdge<TVertex>
        {
            // Find the graph node
            TGraph graph = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && graphPredicate(reader))
                {
                    graph = graphFactory(reader);
                    break;
                }
            }

            if (graph is null)
                throw new InvalidOperationException("Could not find graph node.");

            using (XmlReader graphReader = reader.ReadSubtree())
            {
                while (graphReader.Read())
                {
                    if (graphReader.NodeType == XmlNodeType.Element)
                    {
                        if (vertexPredicate(graphReader))
                        {
                            TVertex vertex = vertexFactory(graphReader);
                            graph.AddVertex(vertex);
                        }
                        else if (edgePredicate(graphReader))
                        {
                            TEdge edge = edgeFactory(graphReader);
                            graph.AddEdge(edge);
                        }
                    }
                }
            }

            return graph;
        }

#if SUPPORTS_XML_GRAPH_SERIALIZATION
        private static TGraph DeserializeFromXmlInternal<TVertex, TEdge, TGraph>(
            [NotNull] this IXPathNavigable document,
            [NotNull] string graphXPath,
            [NotNull] string vertexXPath,
            [NotNull] string edgeXPath,
            [NotNull, InstantHandle] Func<XPathNavigator, TGraph> graphFactory,
            [NotNull, InstantHandle] Func<XPathNavigator, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<XPathNavigator, TEdge> edgeFactory)
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            XPathNavigator navigator = document.CreateNavigator();
            XPathNavigator graphNode = navigator?.SelectSingleNode(graphXPath);
            if (graphNode is null)
                throw new InvalidOperationException("Could not find graph node.");
            TGraph graph = graphFactory(graphNode);
            foreach (XPathNavigator vertexNode in graphNode.Select(vertexXPath))
            {
                TVertex vertex = vertexFactory(vertexNode);
                graph.AddVertex(vertex);
            }

            foreach (XPathNavigator edgeNode in graphNode.Select(edgeXPath))
            {
                TEdge edge = edgeFactory(edgeNode);
                graph.AddEdge(edge);
            }

            return graph;
        }

        /// <summary>
        /// Deserializes a graph instance from a generic XML stream, using an <see cref="T:System.Xml.XPath.XPathDocument"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="document">Input XML document.</param>
        /// <param name="graphXPath">XPath expression to the graph node. The first node is considered.</param>
        /// <param name="vertexXPath">XPath expression from the graph node to the vertex nodes.</param>
        /// <param name="edgeXPath">XPath expression from the graph node to the edge nodes.</param>
        /// <param name="graphFactory">Delegate that instantiates the empty graph instance, given the graph node.</param>
        /// <param name="vertexFactory">Delegate that instantiates a vertex instance, given the vertex node.</param>
        /// <param name="edgeFactory">Delegate that instantiates an edge instance, given the edge node.</param>
        /// <returns>Deserialized graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graphFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/> or creates <see langword="null"/> vertex.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/> or creates <see langword="null"/> edge.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="graphXPath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="vertexXPath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edgeXPath"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// If the <paramref name="document"/> does not allow to get an XML navigator
        /// or if the <paramref name="graphXPath"/> does not allow to get graph node.
        /// </exception>
        [Pure]
        public static TGraph DeserializeFromXml<TVertex, TEdge, TGraph>(
            [NotNull] this IXPathNavigable document,
            [NotNull] string graphXPath,
            [NotNull] string vertexXPath,
            [NotNull] string edgeXPath,
            [NotNull, InstantHandle] Func<XPathNavigator, TGraph> graphFactory,
            [NotNull, InstantHandle] Func<XPathNavigator, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<XPathNavigator, TEdge> edgeFactory)
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrEmpty(graphXPath))
                throw new ArgumentException("Graph path cannot be null or empty", nameof(graphXPath));
            if (string.IsNullOrEmpty(vertexXPath))
                throw new ArgumentException("Vertex path cannot be null or empty", nameof(vertexXPath));
            if (string.IsNullOrEmpty(edgeXPath))
                throw new ArgumentException("Edge path cannot be null or empty", nameof(edgeXPath));
            if (graphFactory is null)
                throw new ArgumentNullException(nameof(graphFactory));
            if (vertexFactory is null)
                throw new ArgumentNullException(nameof(vertexFactory));
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));

            return DeserializeFromXmlInternal(
                document,
                graphXPath,
                vertexXPath,
                edgeXPath,
                graphFactory,
                vertexFactory,
                edgeFactory);
        }
#endif

        /// <summary>
        /// Deserializes a graph instance from a generic XML stream, using an <see cref="T:System.Xml.XmlReader"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="reader">Input XML reader.</param>
        /// <param name="graphPredicate">Predicate that returns a value indicating if the current XML node is a graph. The first match is considered.</param>
        /// <param name="vertexPredicate">Predicate that returns a value indicating if the current XML node is a vertex.</param>
        /// <param name="edgePredicate">Predicate that returns a value indicating if the current XML node is an edge.</param>
        /// <param name="graphFactory">Delegate that instantiates the empty graph instance, given the graph node.</param>
        /// <param name="vertexFactory">Delegate that instantiates a vertex instance, given the vertex node.</param>
        /// <param name="edgeFactory">Delegate that instantiates an edge instance, given the edge node.</param>
        /// <returns>Deserialized graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graphPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graphFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/> or creates <see langword="null"/> vertex.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/> or creates <see langword="null"/> edge.</exception>
        /// <exception cref="T:System.InvalidOperationException">If the graph node cannot be found.</exception>
        [Pure]
        public static TGraph DeserializeFromXml<TVertex, TEdge, TGraph>(
            [NotNull] this XmlReader reader,
            [NotNull, InstantHandle] Predicate<XmlReader> graphPredicate,
            [NotNull, InstantHandle] Predicate<XmlReader> vertexPredicate,
            [NotNull, InstantHandle] Predicate<XmlReader> edgePredicate,
            [NotNull, InstantHandle] Func<XmlReader, TGraph> graphFactory,
            [NotNull, InstantHandle] Func<XmlReader, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<XmlReader, TEdge> edgeFactory)
            where TGraph : class, IMutableVertexAndEdgeSet<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (graphPredicate is null)
                throw new ArgumentNullException(nameof(graphPredicate));
            if (vertexPredicate is null)
                throw new ArgumentNullException(nameof(vertexPredicate));
            if (edgePredicate is null)
                throw new ArgumentNullException(nameof(edgePredicate));
            if (graphFactory is null)
                throw new ArgumentNullException(nameof(graphFactory));
            if (vertexFactory is null)
                throw new ArgumentNullException(nameof(vertexFactory));
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));

            return DeserializeFromXmlInternal(reader, graphPredicate, vertexPredicate, edgePredicate, graphFactory, vertexFactory, edgeFactory);
        }

        /// <summary>
        /// Deserializes a graph from a generic XML stream, using an <see cref="T:System.Xml.XmlReader"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="reader">Input XML reader.</param>
        /// <param name="namespaceUri">XML namespace.</param>
        /// <param name="graphElementName">Name of the XML node holding graph information. The first node is considered.</param>
        /// <param name="vertexElementName">Name of the XML node holding vertex information.</param>
        /// <param name="edgeElementName">Name of the XML node holding edge information.</param>
        /// <param name="graphFactory">Delegate that instantiates the empty graph instance, given the graph node.</param>
        /// <param name="vertexFactory">Delegate that instantiates a vertex instance, given the vertex node.</param>
        /// <param name="edgeFactory">Delegate that instantiates an edge instance, given the edge node.</param>
        /// <returns>Deserialized graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="namespaceUri"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graphFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/> or creates <see langword="null"/> vertex.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/> or creates <see langword="null"/> edge.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="graphElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="vertexElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edgeElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">If the graph node cannot be found.</exception>
        [Pure]
        public static TGraph DeserializeFromXml<TVertex, TEdge, TGraph>(
            [NotNull] this XmlReader reader,
            [NotNull] string graphElementName,
            [NotNull] string vertexElementName,
            [NotNull] string edgeElementName,
            [NotNull] string namespaceUri,
            [NotNull, InstantHandle] Func<XmlReader, TGraph> graphFactory,
            [NotNull, InstantHandle] Func<XmlReader, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<XmlReader, TEdge> edgeFactory)
            where TGraph : class, IMutableVertexAndEdgeSet<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            if (string.IsNullOrEmpty(graphElementName))
                throw new ArgumentException($"{nameof(graphElementName)} cannot be null or empty.", nameof(graphElementName));
            if (string.IsNullOrEmpty(vertexElementName))
                throw new ArgumentException($"{nameof(vertexElementName)} cannot be null or empty.", nameof(vertexElementName));
            if (string.IsNullOrEmpty(edgeElementName))
                throw new ArgumentException($"{nameof(edgeElementName)} cannot be null or empty.", nameof(edgeElementName));
            if (namespaceUri is null)
                throw new ArgumentNullException(nameof(namespaceUri));

            return DeserializeFromXml(
                reader,
                r => r.Name == graphElementName && r.NamespaceURI == namespaceUri,
                r => r.Name == vertexElementName && r.NamespaceURI == namespaceUri,
                r => r.Name == edgeElementName && r.NamespaceURI == namespaceUri,
                graphFactory,
                vertexFactory,
                edgeFactory);
        }

        /// <summary>
        /// Serializes a graph instance to a generic XML stream, using an <see cref="T:System.Xml.XmlWriter"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph to serialize.</param>
        /// <param name="writer">XML writer.</param>
        /// <param name="vertexIdentity">The vertex identity.</param>
        /// <param name="edgeIdentity">The edge identity.</param>
        /// <param name="graphElementName">Name of the graph element.</param>
        /// <param name="vertexElementName">Name of the vertex element.</param>
        /// <param name="edgeElementName">Name of the edge element.</param>
        /// <param name="namespaceUri">XML namespace.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="namespaceUri"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="graphElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="vertexElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edgeElementName"/> is <see langword="null"/> or empty.</exception>
        public static void SerializeToXml<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] XmlWriter writer,
            [NotNull, InstantHandle] VertexIdentity<TVertex> vertexIdentity,
            [NotNull, InstantHandle] EdgeIdentity<TVertex, TEdge> edgeIdentity,
            [NotNull] string graphElementName,
            [NotNull] string vertexElementName,
            [NotNull] string edgeElementName,
            [NotNull] string namespaceUri)
            where TGraph : IEdgeListGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            SerializeToXml(
                graph,
                writer,
                vertexIdentity,
                edgeIdentity,
                graphElementName,
                vertexElementName,
                edgeElementName,
                namespaceUri,
                null,
                null,
                null);
        }

        /// <summary>
        /// Serializes a graph instance to a generic XML stream, using an <see cref="T:System.Xml.XmlWriter"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph to serialize.</param>
        /// <param name="writer">XML writer.</param>
        /// <param name="vertexIdentity">The vertex identity.</param>
        /// <param name="edgeIdentity">The edge identity.</param>
        /// <param name="graphElementName">Name of the graph element.</param>
        /// <param name="vertexElementName">Name of the vertex element.</param>
        /// <param name="edgeElementName">Name of the edge element.</param>
        /// <param name="namespaceUri">XMl namespace.</param>
        /// <param name="writeGraphAttributes">Delegate to write graph attributes (optional).</param>
        /// <param name="writeVertexAttributes">Delegate to write vertex attributes (optional).</param>
        /// <param name="writeEdgeAttributes">Delegate to write edge attributes (optional).</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="namespaceUri"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="graphElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="vertexElementName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edgeElementName"/> is <see langword="null"/> or empty.</exception>
        public static void SerializeToXml<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] XmlWriter writer,
            [NotNull, InstantHandle] VertexIdentity<TVertex> vertexIdentity,
            [NotNull, InstantHandle] EdgeIdentity<TVertex, TEdge> edgeIdentity,
            [NotNull] string graphElementName,
            [NotNull] string vertexElementName,
            [NotNull] string edgeElementName,
            [NotNull] string namespaceUri,
            [CanBeNull, InstantHandle] Action<XmlWriter, TGraph> writeGraphAttributes,
            [CanBeNull, InstantHandle] Action<XmlWriter, TVertex> writeVertexAttributes,
            [CanBeNull, InstantHandle] Action<XmlWriter, TEdge> writeEdgeAttributes)
            where TGraph : IEdgeListGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));
            if (vertexIdentity is null)
                throw new ArgumentNullException(nameof(vertexIdentity));
            if (edgeIdentity is null)
                throw new ArgumentNullException(nameof(edgeIdentity));
            if (string.IsNullOrEmpty(graphElementName))
                throw new ArgumentException($"{nameof(graphElementName)} cannot be null or empty.", nameof(graphElementName));
            if (string.IsNullOrEmpty(vertexElementName))
                throw new ArgumentException($"{nameof(vertexElementName)} cannot be null or empty.", nameof(vertexElementName));
            if (string.IsNullOrEmpty(edgeElementName))
                throw new ArgumentException($"{nameof(edgeElementName)} cannot be null or empty.", nameof(edgeElementName));
            if (namespaceUri is null)
                throw new ArgumentNullException(nameof(namespaceUri));

            writer.WriteStartElement(graphElementName, namespaceUri);

            writeGraphAttributes?.Invoke(writer, graph);

            foreach (TVertex vertex in graph.Vertices)
            {
                writer.WriteStartElement(vertexElementName, namespaceUri);
                writer.WriteAttributeString("id", namespaceUri, vertexIdentity(vertex));
                writeVertexAttributes?.Invoke(writer, vertex);
                writer.WriteEndElement();
            }

            foreach (TEdge edge in graph.Edges)
            {
                writer.WriteStartElement(edgeElementName, namespaceUri);
                writer.WriteAttributeString("id", namespaceUri, edgeIdentity(edge));
                writer.WriteAttributeString("source", namespaceUri, vertexIdentity(edge.Source));
                writer.WriteAttributeString("target", namespaceUri, vertexIdentity(edge.Target));
                writeEdgeAttributes?.Invoke(writer, edge);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}