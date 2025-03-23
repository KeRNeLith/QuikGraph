using System;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary> Extension Methods to build complex Structures </summary>
    public static class Graph
    {
        /// <summary> Filters <paramref name="baseGraph"/> by <paramref name="vertexPredicate"/> and <paramref name="edgePredicate"/> </summary>
        /// <returns></returns>
        public static FilteredGraph<TVertex, TEdge, TGraph> FilterGraphBy<TVertex, TEdge, TGraph>(
            this TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
            where TGraph : IGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
            => new FilteredGraph<TVertex, TEdge, TGraph>(baseGraph, vertexPredicate, edgePredicate);
    }

    /// <summary>
    /// Graph data structure that is filtered with a vertex and an edge predicate.
    /// This means only vertex and edge matching predicates are "accessible".
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <inheritdoc />
    public class FilteredGraph<TVertex, TEdge, TGraph> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="FilteredGraph{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="baseGraph">Graph in which applying predicates.</param>
        /// <param name="vertexPredicate">Predicate to match vertex that should be taken into account.</param>
        /// <param name="edgePredicate">Predicate to match edge that should be taken into account.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgePredicate"/> is <see langword="null"/>.</exception>
        public FilteredGraph(
            [NotNull] TGraph baseGraph,
            [NotNull] Func<TVertex, bool> vertexPredicate,
            [NotNull] Func<TEdge, bool> edgePredicate)
        {
            if (baseGraph == null)
                throw new ArgumentNullException(nameof(baseGraph));

            BaseGraph = baseGraph;
            VertexPredicate = vertexPredicate ?? throw new ArgumentNullException(nameof(vertexPredicate));
            EdgePredicate = edgePredicate ?? throw new ArgumentNullException(nameof(edgePredicate));
        }

        /// <summary>
        /// Underlying graph (graph that is filtered).
        /// </summary>
        [NotNull]
        public TGraph BaseGraph { get; }

        /// <summary>
        /// Vertex predicate used to filter the vertices.
        /// </summary>
        [NotNull]
        public Func<TVertex, bool> VertexPredicate { get; }

        /// <summary>
        /// Edge predicate used to filter the edges.
        /// </summary>
        [NotNull]
        public Func<TEdge, bool> EdgePredicate { get; }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => BaseGraph.IsDirected;

        /// <inheritdoc />
        public bool AllowParallelEdges => BaseGraph.AllowParallelEdges;

        /// <inheritdoc />
        public Func<TVertex, TVertex, bool> AreVerticesEqual {
            get => BaseGraph.AreVerticesEqual;
            //set => BaseGraph.AreVerticesEqual = value;
        }

        /// <inheritdoc />
        public virtual bool ContainsVertex([NotNull] TVertex vertex) => BaseGraph.ContainsVertex(vertex);


        #endregion

        /// <summary>
        /// Tests if the given <paramref name="edge"/> matches
        /// <see cref="VertexPredicate"/> for edge source and target
        /// and the <see cref="EdgePredicate"/>.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <returns>True if the <paramref name="edge"/> matches all predicates, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        protected bool FilterEdge([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return VertexPredicate(edge.Source)
                   && VertexPredicate(edge.Target)
                   && EdgePredicate(edge);
        }
    }
}