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
        /// Initializes a new <see cref="DelegateIncidenceGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <param name="allowParallelEdges">
        /// Indicates if parallel edges are allowed.
        /// Note that get of edges is delegated so you may have bugs related
        /// to parallel edges due to the delegated implementation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        public DelegateIncidenceGraph(
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetOutEdges,
            bool allowParallelEdges = true)
            : base(tryGetOutEdges, allowParallelEdges)
        {
        }

        #region IIncidenceGraph<TVertex,TEdge>

        [Pure]
        internal virtual bool ContainsEdgeInternal([NotNull] TVertex source, [NotNull] TVertex target) => TryGetEdge(source, target, out _);

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target) => ContainsEdgeInternal(source, target);

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var outEdges = OutEdges(source);
            if(outEdges != null)
            {
                foreach (TEdge outEdge in outEdges.Where(outEdge => AreVerticesEqual(outEdge.Target, target)))
                {
                    edge = outEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }


        /// <summary> Returns an empty Edge-Set </summary>
        public IEnumerable<TEdge> Empty => Edge.Empty<TEdge>();

        /// <inheritdoc />
        public IEnumerable<TEdge> GetEdges(TVertex source, TVertex target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var outEdges = OutEdges(source);
            if (outEdges != null)
            {
                return outEdges.Where(edge => AreVerticesEqual(edge.Target, target));
            }

            return Empty;
        }

        #endregion
    }
}