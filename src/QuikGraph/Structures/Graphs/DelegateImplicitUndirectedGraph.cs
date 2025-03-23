using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// A delegate-based undirected implicit graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class DelegateImplicitUndirectedGraph<TVertex, TEdge> : IImplicitUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        public Func<TVertex, TVertex, bool> AreVerticesEqual
        {
            get => areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            set => areVerticesEqual = value;
        }
        [CanBeNull]
        private Func<TVertex, TVertex, bool> areVerticesEqual;

        /// <summary>
        /// Initializes a new <see cref="DelegateImplicitUndirectedGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="tryGetAdjacentEdges">Getter of adjacent edges.</param>
        /// <param name="allowParallelEdges">
        /// Indicates if parallel edges are allowed.
        /// Note that get of edges is delegated so you may have bugs related
        /// to parallel edges due to the delegated implementation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetAdjacentEdges"/> is <see langword="null"/>.</exception>
        public DelegateImplicitUndirectedGraph(
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetAdjacentEdges,
            bool allowParallelEdges = true)
        {
            _getAdjacencyEdges = tryGetAdjacentEdges ?? throw new ArgumentNullException(nameof(tryGetAdjacentEdges));
            AllowParallelEdges = allowParallelEdges;
        }

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; } =
            EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

        /// <summary>
        /// Getter of adjacent edges.
        /// </summary>
        [NotNull]
        private readonly Func<TVertex, IEnumerable<TEdge>> _getAdjacencyEdges;

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => false;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IImplicitVertexSet<TVertex>

        [Pure]
        internal virtual bool ContainsVertexInternal([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _getAdjacencyEdges(vertex) != null;
        }

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            return ContainsVertexInternal(vertex);
        }

        #endregion

        #region IImplicitUndirectedGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? AdjacentDegree(TVertex vertex) => AdjacentEdges(vertex)?.Count();

        [Pure]
        [ItemNotNull]
        [CanBeNull]
        internal virtual IEnumerable<TEdge> AdjacentEdgesInternal([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _getAdjacencyEdges(vertex);
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> AdjacentEdges(TVertex vertex) => AdjacentEdgesInternal(vertex);

        /// <inheritdoc />[CanBeNull]
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            var adjacentEdges = AdjacentEdges(vertex);
            if (adjacentEdges == null)
            {
                return default(TEdge);
            }

            return adjacentEdges.ElementAt(index);
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var adjacentEdges = AdjacentEdges(source);
            if (adjacentEdges != null)
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

        [Pure]
        internal virtual bool ContainsEdgeInternal([NotNull] TVertex source, [NotNull] TVertex target) => TryGetEdge(source, target, out _);

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target) => ContainsEdgeInternal(source, target);

        #endregion
    }
}