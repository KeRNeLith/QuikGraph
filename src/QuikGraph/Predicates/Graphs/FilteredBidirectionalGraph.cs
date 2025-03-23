using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary> Extension Methods to build complex Structures </summary>
    public static class BidirectionalGraph
    {
        /// <summary> Filters <paramref name="baseGraph"/> by <paramref name="vertexPredicate"/> and <paramref name="edgePredicate"/> </summary>
        /// <returns></returns>
        public static FilteredBidirectionalGraph<TVertex, TEdge, TGraph> FilteredBiDir<TVertex, TEdge, TGraph>(
            this TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            where TGraph : IBidirectionalGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
            => new FilteredBidirectionalGraph<TVertex, TEdge, TGraph>(baseGraph, vertexPredicate, edgePredicate);
    }

    /// <summary>
    /// Bidirectional graph data structure that is filtered with a vertex and an edge
    /// predicate. This means only vertex and edge matching predicates are "accessible".
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class FilteredBidirectionalGraph<TVertex, TEdge, TGraph>
        : FilteredVertexListGraph<TVertex, TEdge, TGraph>
        , IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="FilteredBidirectionalGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredBidirectionalGraph(
            [NotNull] TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            : base(baseGraph, vertexPredicate, edgePredicate)
        {
        }

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => !Edges.Any();

        /// <inheritdoc />
        public int EdgeCount => Edges.Count();

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => BaseGraph.Edges.Where(FilterEdge);

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            return FilterEdge(edge) && BaseGraph.ContainsEdge(edge);
        }

        #endregion

        #region IBidirectionalIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? InDegree(TVertex vertex) => InEdges(vertex)?.Count();

        /// <inheritdoc />
        public IEnumerable<TEdge> InEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (VertexPredicate(vertex))
                return BaseGraph.InEdges(vertex)?.Where(FilterEdge);

            return null; //Enumerable.Empty<TEdge>();
        }

        /// <inheritdoc />
        [CanBeNull]
        public TEdge InEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (!VertexPredicate(vertex))
            {
                return default(TEdge);
            }

            var inEdges = BaseGraph.InEdges(vertex);
            if (inEdges == null)
            {
                return default(TEdge);
            }
            return inEdges.Where(FilterEdge).ElementAt(index);
        }

        /// <inheritdoc />
        public int? Degree(TVertex vertex) => OutDegree(vertex) + InDegree(vertex);

        #endregion
    }
}