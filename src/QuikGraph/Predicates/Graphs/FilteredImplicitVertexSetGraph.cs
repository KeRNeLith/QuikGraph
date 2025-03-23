using System;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary> Extension Methods to build complex Structures </summary>
    public static class ImplicitVertexSet
    {
        /// <summary> Filters <paramref name="baseGraph"/> by <paramref name="vertexPredicate"/> and <paramref name="edgePredicate"/> </summary>
        /// <returns></returns>
        public static FilteredImplicitVertexSet<TVertex, TEdge, TGraph> FilterByImplicit<TVertex, TEdge, TGraph>(
            this TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            where TGraph : IGraph<TVertex, TEdge>, IImplicitVertexSet<TVertex>
            where TEdge : IEdge<TVertex>
            => new FilteredImplicitVertexSet<TVertex, TEdge, TGraph>(baseGraph, vertexPredicate, edgePredicate);
    }

    /// <summary>
    /// Implicit vertex set graph data structure that is filtered with a vertex and an edge
    /// predicate. This means only vertex and edge matching predicates are "accessible".
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class FilteredImplicitVertexSet<TVertex, TEdge, TGraph>
        : FilteredGraph<TVertex, TEdge, TGraph>
        , IImplicitVertexSet<TVertex>
        where TEdge : IEdge<TVertex>
        where TGraph : IGraph<TVertex, TEdge>, IImplicitVertexSet<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="FilteredImplicitVertexSet{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredImplicitVertexSet(
            [NotNull] TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            : base(baseGraph, vertexPredicate, edgePredicate)
        {
        }

        /// <inheritdoc />
        public override bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return VertexPredicate(vertex)
                   && BaseGraph.ContainsVertex(vertex);
        }
    }
}