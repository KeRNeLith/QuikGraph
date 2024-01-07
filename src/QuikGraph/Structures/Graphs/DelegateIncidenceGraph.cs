using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// A delegate-based directed incidence graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class DelegateIncidenceGraph<TVertex, TEdge> : DelegateImplicitGraph<TVertex, TEdge>, IIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateIncidenceGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <param name="allowParallelEdges">
        /// Indicates if parallel edges are allowed.
        /// Note that get of edges is delegated so you may have bugs related
        /// to parallel edges due to the delegated implementation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        public DelegateIncidenceGraph(
            [NotNull] TryFunc<TVertex, IEnumerable<TEdge>> tryGetOutEdges,
            bool allowParallelEdges = true)
            : base(tryGetOutEdges, allowParallelEdges)
        {
        }

        #region IIncidenceGraph<TVertex,TEdge>

        [Pure]
        internal virtual bool ContainsEdgeInternal([NotNull] TVertex source, [NotNull] TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return ContainsEdgeInternal(source, target);
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (TryGetOutEdges(source, out IEnumerable<TEdge> outEdges))
            {
                foreach (TEdge outEdge in outEdges)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(outEdge.Target, target))
                    {
                        edge = outEdge;
                        return true;
                    }
                }
            }

            edge = default(TEdge);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (TryGetOutEdges(source, out IEnumerable<TEdge> outEdges))
            {
                edges = outEdges.Where(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
                return true;
            }

            edges = null;
            return false;
        }

        #endregion
    }
}