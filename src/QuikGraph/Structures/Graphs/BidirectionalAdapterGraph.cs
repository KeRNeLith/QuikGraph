using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph
{
    /// <summary>
    /// Wrapper of a graph adapting it to become bidirectional.
    /// </summary>
    /// <remarks>Vertex list graph for out-edges only and dictionary cache for in-edges.</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class BidirectionalAdapterGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly IVertexAndEdgeListGraph<TVertex, TEdge> _baseGraph;

        /// <inheritdoc />
        public Func<TVertex, TVertex, bool> AreVerticesEqual {
            get => _baseGraph.AreVerticesEqual;
            //set => _baseGraph.AreVerticesEqual = value;
        }

        /// <summary>
        /// Initializes a new <see cref="BidirectionalAdapterGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="baseGraph">Wrapped graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        public BidirectionalAdapterGraph([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph)
        {
            _baseGraph = baseGraph ?? throw new ArgumentNullException(nameof(baseGraph));
            _inEdges = new Dictionary<TVertex, EdgeList<TEdge>>(_baseGraph.VertexCount);
            foreach (TEdge edge in _baseGraph.Edges)
            {
                if (!_inEdges.TryGetValue(edge.Target, out EdgeList<TEdge> edgeList))
                {
                    edgeList = new EdgeList<TEdge>();
                    _inEdges.Add(edge.Target, edgeList);
                }

                edgeList.Add(edge);
            }

            // Add vertices that has no in edges
            foreach (TVertex vertex in _baseGraph.Vertices.Except(_inEdges.Keys.ToArray()))
            {
                _inEdges.Add(vertex, new EdgeList<TEdge>());
            }
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => _baseGraph.IsDirected;

        /// <inheritdoc />
        public bool AllowParallelEdges => _baseGraph.AllowParallelEdges;

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _baseGraph.IsVerticesEmpty;

        /// <inheritdoc />
        public int VertexCount => _baseGraph.VertexCount;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _baseGraph.Vertices;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            return _baseGraph.ContainsVertex(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => _baseGraph.IsEdgesEmpty;

        /// <inheritdoc />
        public int EdgeCount => _baseGraph.EdgeCount;

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> Edges => _baseGraph.Edges;

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            return _baseGraph.ContainsEdge(edge);
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge> 

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target) => _baseGraph.ContainsEdge(source, target);

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge) => _baseGraph.TryGetEdge(source, target, out edge);

        /// <inheritdoc />
        public IEnumerable<TEdge> GetEdges(TVertex source, TVertex target) => _baseGraph.GetEdges(source, target);

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? OutDegree(TVertex vertex) => _baseGraph.OutDegree(vertex);

        /// <inheritdoc />
        public IEnumerable<TEdge> OutEdges(TVertex vertex) => _baseGraph.OutEdges(vertex);

        /// <inheritdoc />
        public TEdge OutEdge(TVertex vertex, int index) => _baseGraph.OutEdge(vertex, index);

        #endregion

        #region IBidirectionalIncidenceGraph<TVertex,TEdge>

        [NotNull]
        private readonly Dictionary<TVertex, EdgeList<TEdge>> _inEdges;

        /// <inheritdoc />
        public int? InDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_inEdges.TryGetValue(vertex, out EdgeList<TEdge> inEdges))
                return inEdges.Count;

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> InEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_inEdges.TryGetValue(vertex, out EdgeList<TEdge> inEdges))
                return inEdges.AsEnumerable();

            return null;
        }

        /// <inheritdoc />
        public TEdge InEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_inEdges.TryGetValue(vertex, out EdgeList<TEdge> inEdges))
                return inEdges[index];

            return default(TEdge);
        }

        /// <inheritdoc />
        public int? Degree(TVertex vertex) => InDegree(vertex) + OutDegree(vertex);

        #endregion
    }
}