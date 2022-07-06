using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if !SUPPORTS_TYPE_FULL_FEATURES
using System.Reflection;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Mutable bidirectional undirected graph data structure.
    /// </summary>
    /// <remarks>It is mutable via the wrapped graph.</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class UndirectedBidirectionalGraph<TVertex, TEdge> : IUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndirectedBidirectionalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="originalGraph">Bidirectional graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="originalGraph"/> is <see langword="null"/>.</exception>
        public UndirectedBidirectionalGraph([NotNull] IBidirectionalGraph<TVertex, TEdge> originalGraph)
        {
            OriginalGraph = originalGraph ?? throw new ArgumentNullException(nameof(originalGraph));

#if SUPPORTS_TYPE_FULL_FEATURES
            _reorder = typeof(IUndirectedEdge<TVertex>).IsAssignableFrom(typeof(TEdge))
#else
            _reorder = typeof(IUndirectedEdge<TVertex>).GetTypeInfo().IsAssignableFrom(typeof(TEdge).GetTypeInfo())
#endif
                ? (ReorderVertices)((TVertex source, TVertex target, out TVertex orderedSource, out TVertex orderedTarget) =>
                {
                    if (Comparer<TVertex>.Default.Compare(source, target) > 0)
                    {
                        orderedSource = target;
                        orderedTarget = source;
                    }
                    else
                    {
                        orderedSource = source;
                        orderedTarget = target;
                    }
                })
                : (TVertex source, TVertex target, out TVertex orderedSource, out TVertex orderedTarget) =>
                {
                    orderedSource = source;
                    orderedTarget = target;
                };
        }

        private delegate void ReorderVertices(
            [NotNull] TVertex source,
            [NotNull] TVertex target,
            [NotNull] out TVertex orderedSource,
            [NotNull] out TVertex orderedTarget);

        [NotNull]
        private readonly ReorderVertices _reorder;

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; } =
            EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

        /// <summary>
        /// Underlying bidirectional graph.
        /// </summary>
        public IBidirectionalGraph<TVertex, TEdge> OriginalGraph { get; }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => false;

        /// <inheritdoc />
        public bool AllowParallelEdges => OriginalGraph.AllowParallelEdges;

        #endregion

        #region IVertexSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsVerticesEmpty => OriginalGraph.IsVerticesEmpty;

        /// <inheritdoc />
        public int VertexCount => OriginalGraph.VertexCount;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => OriginalGraph.Vertices;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            return OriginalGraph.ContainsVertex(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => OriginalGraph.IsEdgesEmpty;

        /// <inheritdoc />
        public int EdgeCount => OriginalGraph.EdgeCount;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => OriginalGraph.Edges;

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            return OriginalGraph.ContainsEdge(edge);
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        #endregion

        #region IUndirectedGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TEdge> AdjacentEdges(TVertex vertex)
        {
            return
                OriginalGraph.OutEdges(vertex)
                    .Concat(
                        OriginalGraph.InEdges(vertex)
                            // We skip self edges here since
                            // We already got them out-edge run
                            .Where(inEdge => !inEdge.IsSelfEdge()));
        }

        /// <inheritdoc />
        public int AdjacentDegree(TVertex vertex)
        {
            return OriginalGraph.Degree(vertex);
        }

        /// <inheritdoc />
        public bool IsAdjacentEdgesEmpty(TVertex vertex)
        {
            return OriginalGraph.IsOutEdgesEmpty(vertex) && OriginalGraph.IsInEdgesEmpty(vertex);
        }

        /// <summary>
        /// <see cref="AdjacentEdge"/> is not supported for this kind of graph.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">This operation is not supported.</exception>
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            _reorder(source, target, out source, out target);

            if (ContainsVertex(source))
            {
                foreach (TEdge adjacentEdge in AdjacentEdges(source).Where(adjacentEdge => EdgeEqualityComparer(adjacentEdge, source, target)))
                {
                    edge = adjacentEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        #endregion
    }
}