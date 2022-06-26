using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// A base class that creates a proxy to a graph that is serializable in XML.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    [XmlRoot("graph")]
    public class XmlSerializableGraph<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializableGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        public XmlSerializableGraph()
            : this(new TGraph())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializableGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Graph to serialize.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        public XmlSerializableGraph([NotNull] TGraph graph)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            Graph = graph;
        }

        /// <summary>
        /// Gets the graph to serialize.
        /// </summary>
        [NotNull]
        public TGraph Graph { get; }

        private XmlVertexList _vertices;

        /// <summary>
        /// Gets the vertices to serialize.
        /// </summary>
        [NotNull, ItemNotNull]
        [XmlArray("vertices")]
        [XmlArrayItem("vertex")]
        public XmlVertexList Vertices
        {
            get => _vertices ?? (_vertices = new XmlVertexList(Graph));
            set => _vertices = value;
        }

        private XmlEdgeList _edges;

        /// <summary>
        /// Gets the edges to serialize.
        /// </summary>
        [NotNull, ItemNotNull]
        [XmlArray("edges")]
        [XmlArrayItem("edge")]
        public XmlEdgeList Edges
        {
            get => _edges ?? (_edges = new XmlEdgeList(Graph));
            set => _edges = value;
        }

        /// <summary>
        /// Represents an XML serializable list of vertices.
        /// </summary>
        public class XmlVertexList : IEnumerable<TVertex>
        {
            [NotNull]
            private readonly TGraph _graph;

            internal XmlVertexList([NotNull] TGraph graph)
            {
                if (graph == null)
                    throw new ArgumentNullException(nameof(graph));

                _graph = graph;
            }

            #region IEnumerable

            /// <inheritdoc />
            public IEnumerator<TVertex> GetEnumerator()
            {
                return _graph.Vertices.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            /// <summary>
            /// Adds a vertex to this serializable graph.
            /// </summary>
            /// <param name="vertex">Vertex to add.</param>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
            public void Add([NotNull] TVertex vertex)
            {
                if (vertex == null)
                    throw new ArgumentNullException(nameof(vertex));

                _graph.AddVertex(vertex);
            }
        }

        /// <summary>
        /// Represents an XML serializable list of edge.
        /// </summary>
        public class XmlEdgeList : IEnumerable<TEdge>
        {
            [NotNull]
            private readonly TGraph _graph;

            internal XmlEdgeList([NotNull] TGraph graph)
            {
                if (graph == null)
                    throw new ArgumentNullException(nameof(graph));

                _graph = graph;
            }

            #region IEnumerable

            /// <inheritdoc />
            public IEnumerator<TEdge> GetEnumerator()
            {
                return _graph.Edges.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            /// <summary>
            /// Adds an edge to this serializable graph.
            /// </summary>
            /// <param name="edge">Edge to add.</param>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
            public void Add([NotNull] TEdge edge)
            {
                if (edge == null)
                    throw new ArgumentNullException(nameof(edge));

                _graph.AddVerticesAndEdge(edge);
            }
        }
    }
}