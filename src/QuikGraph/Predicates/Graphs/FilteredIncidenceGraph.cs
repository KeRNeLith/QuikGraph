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
        /// Initializes a new instance of the <see cref="FilteredIncidenceGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredIncidenceGraph(
            [NotNull] TGraph baseGraph,
            [NotNull] VertexPredicate<TVertex> vertexPredicate,
            [NotNull] EdgePredicate<TVertex, TEdge> edgePredicate)
            : base(baseGraph, vertexPredicate, edgePredicate)
        {
        }

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

            if (VertexPredicate(source)
                && VertexPredicate(target)
                && BaseGraph.TryGetEdges(source, target, out IEnumerable<TEdge> unfilteredEdges))
            {
                foreach (TEdge unfilteredEdge in unfilteredEdges)
                {
                    if (EdgePredicate(unfilteredEdge))
                    {
                        edge = unfilteredEdge;
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (VertexPredicate(source)
                && VertexPredicate(target)
                && BaseGraph.TryGetEdges(source, target, out IEnumerable<TEdge> unfilteredEdges))
            {
                edges = unfilteredEdges.Where(edge => EdgePredicate(edge));
                return true;
            }

            edges = null;
            return false;
        }
    }
}