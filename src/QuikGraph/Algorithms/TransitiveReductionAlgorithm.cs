using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <inheritdoc cref="TransitiveReductionAlgorithm{TVertex, TEdge}.TransitiveReduction"/>
    public static class TransitiveReductionAlgorithm
    {
        /// <inheritdoc cref="TransitiveReductionAlgorithm{TVertex, TEdge}.TransitiveReduction"/>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ComputeTransitiveReduction<TVertex, TEdge>(
            [NotNull] this IEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            TransitiveReductionAlgorithm<TVertex, TEdge> algorithm = CreateTransitiveReduction(graph);
            algorithm.Compute();
            return algorithm.TransitiveReduction;
        }

        /// <inheritdoc cref="TransitiveReductionAlgorithm{TVertex, TEdge}.TransitiveReduction"/>
        public static TransitiveReductionAlgorithm<TVertex, TEdge> CreateTransitiveReduction<TVertex, TEdge>(
            [NotNull] this IEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
            => new TransitiveReductionAlgorithm<TVertex, TEdge>(graph);
    }

    /// <inheritdoc cref="TransitiveReduction"/>
    public class TransitiveReductionAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="TransitiveReductionAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public TransitiveReductionAlgorithm(
            [NotNull] IEdgeListGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
            TransitiveReduction = new BidirectionalGraph<TVertex, TEdge>();
        }

        /// <summary> Computes a transitive reduction of the <see cref="IAlgorithm{TGraph}.VisitedGraph"/>. </summary>
        /// <remarks>
        /// which is another directed graph with the same vertices and as few edges as possible.
        /// The transitive reduction is usually not unique.
        /// It removes all Edges that can be constructed by transitively chaining other Edges.
        /// The result is a kind of Spanning tree that connects the same Vertices,
        /// resulting in the same connected Components.
        ///
        /// The inverse Operation is <see cref="TransitiveClosureAlgorithm{TVertex,TEdge}.TransitiveClosure"/>.
        /// </remarks>
        /// <summary> Algorithm that computes the transitive reduction of a graph </summary>
        /// <remarks>
        /// </remarks>
        [NotNull]
        public BidirectionalGraph<TVertex, TEdge> TransitiveReduction { get; }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Clone the visited graph
            TransitiveReduction.AddVertexRange(VisitedGraph.Vertices);
            TransitiveReduction.AddEdgeRange(VisitedGraph.Edges);

            var algorithmHelper = new TransitiveAlgorithmHelper<TVertex, TEdge>(TransitiveReduction);
            algorithmHelper.InternalCompute((graph, u, v, found, edge) =>
            {
                if (found)
                {
                    graph.RemoveEdge(edge);
                }
            });
        }

        #endregion
    }
}