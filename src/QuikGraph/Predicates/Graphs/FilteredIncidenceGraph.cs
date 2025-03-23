using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary>
    /// Incidence graph data structure that is filtered with a vertex and an edge
    /// predicate. This means only vertex and edge matching predicates are "accessible".
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class FilteredIncidenceGraph<TVertex, TEdge, TGraph>
        : FilteredImplicitGraph<TVertex, TEdge, TGraph>
        , IIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IIncidenceGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="FilteredIncidenceGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredIncidenceGraph(
            [NotNull] TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            : base(baseGraph, vertexPredicate, edgePredicate)
        {
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> GetEdges(TVertex source, TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (VertexPredicate(source) && VertexPredicate(target))
            {
                var unfilteredEdges = BaseGraph.GetEdges(source, target);
                var edges = unfilteredEdges.Where(edge => EdgePredicate(edge));
                return edges;
            }

            return Empty;
        }

        /// <inheritdoc />
        //[Obsolete("Obsolete, use " + nameof(GetEdges))]
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (VertexPredicate(source)
                && VertexPredicate(target))
            {
                var unfilteredEdges = BaseGraph.GetEdges(source, target);
                foreach (TEdge unfilteredEdge in unfilteredEdges.Where(unfilteredEdge => EdgePredicate(unfilteredEdge)))
                {
                    edge = unfilteredEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }
    }
}