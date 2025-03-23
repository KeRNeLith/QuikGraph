using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateMultiSourceSinkGraphAugmentorAlgorithm"/>
    public static class MultiSourceSinkGraphAugmentorAlgorithm
    {
        /// <summary>Creates a new <see cref="MultiSourceSinkGraphAugmentorAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge>
            CreateMultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge>(visitedGraph, vertexFactory, edgeFactory, host);
    }

    /// <summary> Multi source and sink graph augmentor algorithm. </summary>
    public sealed class MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge>
        : GraphAugmentorAlgorithmBase<TVertex, TEdge, IMutableBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>Creates a new <see cref="MultiSourceSinkGraphAugmentorAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        internal MultiSourceSinkGraphAugmentorAlgorithm(
            [NotNull] IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, vertexFactory, edgeFactory, host)
        {
        }

        /// <inheritdoc />
        protected override void AugmentGraph()
        {
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                ThrowIfCancellationRequested();

                // Is source
                if (VisitedGraph.IsInEdgesEmpty(vertex) ?? true)
                {
                    AddAugmentedEdge(SuperSource, vertex);
                }

                // Is sink
                if (VisitedGraph.IsOutEdgesEmpty(vertex) ?? true)
                {
                    AddAugmentedEdge(vertex, SuperSink);
                }
            }
        }
    }
}