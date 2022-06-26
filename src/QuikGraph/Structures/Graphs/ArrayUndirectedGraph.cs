using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph
{
    /// <summary>
    /// Immutable undirected graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ArrayUndirectedGraph<TVertex, TEdge> : IUndirectedGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayUndirectedGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="baseGraph">Wrapped graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        public ArrayUndirectedGraph([NotNull] IUndirectedGraph<TVertex, TEdge> baseGraph)
        {
            if (baseGraph is null)
                throw new ArgumentNullException(nameof(baseGraph));

            AllowParallelEdges = baseGraph.AllowParallelEdges;
            EdgeEqualityComparer = baseGraph.EdgeEqualityComparer;
            EdgeCount = baseGraph.EdgeCount;
            _adjacentEdges = new Dictionary<TVertex, TEdge[]>(baseGraph.VertexCount);
            foreach (TVertex vertex in baseGraph.Vertices)
            {
                _adjacentEdges.Add(
                    vertex,
                    baseGraph.AdjacentEdges(vertex).ToArray());
            }

            _edges = baseGraph.Edges.ToList();
        }

        // Serialization constructor
        internal ArrayUndirectedGraph(
            bool allowParallelEdges,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            Debug.Assert(vertices != null);
            Debug.Assert(edges != null);

            AllowParallelEdges = allowParallelEdges;
            EdgeEqualityComparer = EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();
            _edges = edges.ToList();
            EdgeCount = _edges.Count;
            _adjacentEdges = ComputeAdjacentEdges();

            #region Local function

            Dictionary<TVertex, TEdge[]> ComputeAdjacentEdges()
            {
                var adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>();
                foreach (TVertex vertex in vertices)
                {
                    adjacentEdges.Add(
                        vertex,
                        new EdgeList<TVertex, TEdge>());
                }

                foreach (TEdge edge in _edges)
                {
                    adjacentEdges[edge.Source].Add(edge);
                    if (!edge.IsSelfEdge())
                    {
                        adjacentEdges[edge.Target].Add(edge);
                    }
                }

                var computedAdjacentEdges = new Dictionary<TVertex, TEdge[]>(adjacentEdges.Count);
                foreach (KeyValuePair<TVertex, IEdgeList<TVertex, TEdge>> pair in adjacentEdges)
                {
                    computedAdjacentEdges.Add(
                        pair.Key,
                        pair.Value.ToArray());
                }
                return computedAdjacentEdges;
            }

            #endregion
        }

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => false;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _adjacentEdges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => _adjacentEdges.Count;

        [NotNull]
        private readonly Dictionary<TVertex, TEdge[]> _adjacentEdges;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _adjacentEdges.Keys.AsEnumerable();

        #endregion

        #region IImplicitVertexSet<TVertex>

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _adjacentEdges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; }

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _edges;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _edges.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_adjacentEdges.TryGetValue(edge.Source, out TEdge[] edges))
                return edges.Any(e => EqualityComparer<TEdge>.Default.Equals(e, edge));
            return false;
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        #endregion

        #region IImplicitUndirectedGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TEdge> AdjacentEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges.AsEnumerable();
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public int AdjacentDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges.Sum(edge => edge.IsSelfEdge() ? 2 : 1);    // Self edge count twice
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool IsAdjacentEdgesEmpty(TVertex vertex)
        {
            return AdjacentDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out TEdge[] adjacentEdges))
                return adjacentEdges[index];
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (Comparer<TVertex>.Default.Compare(source, target) > 0)
            {
                TVertex temp = source;
                source = target;
                target = temp;
            }

            if (_adjacentEdges.TryGetValue(source, out TEdge[] adjacentEdges))
            {
                foreach (TEdge adjacentEdge in adjacentEdges.Where(adjacentEdge => EdgeEqualityComparer(adjacentEdge, source, target)))
                {
                    edge = adjacentEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph, returns this instance because this class is immutable.
        /// </summary>
        /// <returns>This graph.</returns>
        [Pure]
        [NotNull]
        public ArrayUndirectedGraph<TVertex, TEdge> Clone()
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