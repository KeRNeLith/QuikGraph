using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Immutable bidirectional directed graph data structure.
    /// </summary>
    /// <remarks>
    /// It can be used for large sparse graph representation where
    /// out-edge need to be enumerated only.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ArrayBidirectionalGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
#if SUPPORTS_SERIALIZATION
        [Serializable]
#endif
        private sealed class InOutEdges
        {
            [NotNull, ItemNotNull]
            public TEdge[] OutEdges { get; }

            [NotNull, ItemNotNull]
            public TEdge[] InEdges { get; }

            public InOutEdges(
                [NotNull, ItemNotNull] TEdge[] outEdges,
                [NotNull, ItemNotNull] TEdge[] inEdges)
            {
                OutEdges = outEdges;
                InEdges = inEdges;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBidirectionalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="baseGraph">Wrapped graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        public ArrayBidirectionalGraph([NotNull] IBidirectionalGraph<TVertex, TEdge> baseGraph)
        {
            if (baseGraph is null)
                throw new ArgumentNullException(nameof(baseGraph));

            AllowParallelEdges = baseGraph.AllowParallelEdges;
            _vertexEdges = new Dictionary<TVertex, InOutEdges>(baseGraph.VertexCount);
            EdgeCount = baseGraph.EdgeCount;
            foreach (TVertex vertex in baseGraph.Vertices)
            {
                TEdge[] outEdges = baseGraph.OutEdges(vertex).ToArray();
                TEdge[] inEdges = baseGraph.InEdges(vertex).ToArray();
                _vertexEdges.Add(vertex, new InOutEdges(outEdges, inEdges));
            }
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _vertexEdges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => _vertexEdges.Count;

        [NotNull]
        private readonly IDictionary<TVertex, InOutEdges> _vertexEdges;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _vertexEdges.Keys;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _vertexEdges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; }

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges =>
            _vertexEdges.Values.SelectMany(inOutEdges => inOutEdges.OutEdges);

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_vertexEdges.TryGetValue(edge.Source, out InOutEdges inOutEdges))
            {
                return inOutEdges.OutEdges.Any(outEdge => EqualityComparer<TEdge>.Default.Equals(outEdge, edge));
            }

            return false;
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexEdges.TryGetValue(source, out InOutEdges inOutEdges))
            {
                foreach (TEdge outEdge in inOutEdges.OutEdges.Where(outEdge => EqualityComparer<TVertex>.Default.Equals(outEdge.Target, target)))
                {
                    edge = outEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexEdges.TryGetValue(source, out InOutEdges inOutEdges))
            {
                edges = inOutEdges.OutEdges.Where(outEdge => EqualityComparer<TVertex>.Default.Equals(outEdge.Target, target));
                return true;
            }

            edges = null;
            return false;
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            return OutDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.OutEdges.Length;

            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> OutEdges(TVertex vertex)
        {
            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.OutEdges;

            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
            {
                edges = inOutEdges.OutEdges;
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public TEdge OutEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.OutEdges[index];

            throw new VertexNotFoundException();
        }

        #endregion

        #region IBidirectionalGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsInEdgesEmpty(TVertex vertex)
        {
            return InDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int InDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.InEdges.Length;

            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> InEdges(TVertex vertex)
        {
            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.InEdges;

            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetInEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
            {
                edges = inOutEdges.InEdges;
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public TEdge InEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out InOutEdges inOutEdges))
                return inOutEdges.InEdges[index];

            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public int Degree(TVertex vertex)
        {
            return InDegree(vertex) + OutDegree(vertex);
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph, returns this instance because this class is immutable.
        /// </summary>
        /// <returns>This graph.</returns>
        [Pure]
        [NotNull]
        public ArrayBidirectionalGraph<TVertex, TEdge> Clone()
        {
            return this;
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        #endregion
    }
}