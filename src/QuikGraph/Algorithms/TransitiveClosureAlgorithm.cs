using System;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <inheritdoc cref="CreateTransitiveClosureAlgorithm"/>
    public static class TransitiveClosureAlgorithm
    {
        /// <summary> Creates a new <see cref="TransitiveClosureAlgorithm{TVertex,TEdge}"/>. </summary>
        public static TransitiveClosureAlgorithm<TVertex, TEdge>
            CreateTransitiveClosureAlgorithm<TVertex, TEdge>(
            [NotNull] this IEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TVertex, TVertex, TEdge> edgeFactory) where TEdge : IEdge<TVertex>
            => new TransitiveClosureAlgorithm<TVertex, TEdge>(visitedGraph,edgeFactory);

        /// <inheritdoc cref="TransitiveClosureAlgorithm{TVertex,TEdge}.TransitiveClosure"/>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ComputeTransitiveClosure<TVertex, TEdge>(
            [NotNull] this IEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TVertex, TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateTransitiveClosureAlgorithm(edgeFactory);
            algorithm.Compute();
            return algorithm.TransitiveClosure;
        }
    }

    /// <inheritdoc cref="TransitiveClosure"/>
    public class TransitiveClosureAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Creates a new <see cref="TransitiveClosureAlgorithm{TVertex,TEdge}"/>. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeFactory">Function that create an edge between the 2 given vertices.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        internal TransitiveClosureAlgorithm(
            [NotNull] IEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TVertex, TVertex, TEdge> edgeFactory)
            : base(visitedGraph)
        {
            TransitiveClosure = new BidirectionalGraph<TVertex, TEdge>();
            _createEdge = edgeFactory ?? throw new ArgumentNullException(nameof(edgeFactory));
        }

        /// <summary> Computes the transitive closure of the <see cref="IAlgorithm{TGraph}.VisitedGraph"/>. </summary>
        /// <remarks>
        /// This is another directed graph with the same vertices
        /// and every reachable vertices by a given one linked by a single edge.
        /// 
        /// The transitive closure adds all Edges
        /// that can be constructed by transitively chaining other Edges.
        /// It uses <see cref="_createEdge"/> to create these edges.
        /// 
        /// The result is a very dense graph, that directly connects all Vertices,
        /// in the same connected Component.
        ///
        /// The inverse Operation is <see cref="TransitiveReductionAlgorithm{TVertex, TEdge}.TransitiveReduction"/>.
        /// </remarks>
        /// <returns>Transitive graph closure.</returns>
        public BidirectionalGraph<TVertex, TEdge> TransitiveClosure { get; }

        [NotNull]
        private readonly Func<TVertex, TVertex, TEdge> _createEdge;

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Clone the visited graph
            TransitiveClosure.AddVertexRange(VisitedGraph.Vertices);
            TransitiveClosure.AddEdgeRange(VisitedGraph.Edges);

            var algorithmHelper = new TransitiveAlgorithmHelper<TVertex, TEdge>(TransitiveClosure);
            algorithmHelper.InternalCompute((graph, u, v, found, edge) =>
            {
                if (!found)
                {
                    graph.AddEdge(_createEdge(u, v));
                }
            });
        }

        #endregion
    }
}