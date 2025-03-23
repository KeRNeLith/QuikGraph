using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary> Extension Methods to build complex Structures </summary>
    public static class UndirectedGraph
    {
        /// <summary> Filters <paramref name="baseGraph"/> by <paramref name="vertexPredicate"/> and <paramref name="edgePredicate"/> </summary>
        /// <returns></returns>
        public static FilteredUndirectedGraph<TVertex, TEdge, TGraph> FilterByUndirected<TVertex, TEdge, TGraph>(
            this TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            where TGraph : IUndirectedGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
            => new FilteredUndirectedGraph<TVertex, TEdge, TGraph>(baseGraph, vertexPredicate, edgePredicate);
    }

    /// <summary>
    /// Undirected graph data structure that is filtered with a vertex and an edge
    /// predicate. This means only vertex and edge matching predicates are "accessible".
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public sealed class FilteredUndirectedGraph<TVertex, TEdge, TGraph>
        : FilteredGraph<TVertex, TEdge, TGraph>
        , IUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IUndirectedGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="FilteredUndirectedGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredUndirectedGraph(
            [NotNull] TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            : base(baseGraph, vertexPredicate, edgePredicate)
        {
        }

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; } =
            EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => !Vertices.Any();

        /// <inheritdoc />
        public int VertexCount => Vertices.Count();

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => BaseGraph.Vertices.Where(vertex => VertexPredicate(vertex));

        /// <inheritdoc />
        public override bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return VertexPredicate(vertex)
                   && BaseGraph.ContainsVertex(vertex);
        }

        #endregion

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

            if (VertexPredicate(vertex))
                return BaseGraph.AdjacentEdges(vertex)?.Where(FilterEdge);

            return null;
        }

        /// <inheritdoc />
        public int? AdjacentDegree(TVertex vertex) => AdjacentEdges(vertex)
            ?.Sum(edge => edge.IsSelfEdge(AreVerticesEqual) ? 2 : 1); // Self edge count twice

        /// <inheritdoc />
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (!VertexPredicate(vertex))
            {
                return default(TEdge);
            }

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
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (VertexPredicate(source) && VertexPredicate(target))
            {
                // We need to find the edge
                foreach (TEdge e in Edges)
                {
                    if (EdgeEqualityComparer(e, source, target)
                        && EdgePredicate(e))
                    {
                        edge = e;
                        return true;
                    }
                }
            }

            edge = default(TEdge);
            return false;
        }

        #endregion
    }
}