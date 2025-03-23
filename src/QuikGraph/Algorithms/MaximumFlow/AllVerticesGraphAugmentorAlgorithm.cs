using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateAllVerticesGraphAugmentorAlgorithm"/>/>
    public static class AllVerticesGraphAugmentorAlgorithm
    {
        /// <summary> Creates a new <see cref="AllVerticesGraphAugmentorAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static AllVerticesGraphAugmentorAlgorithm<TVertex, TEdge>
            CreateAllVerticesGraphAugmentorAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new AllVerticesGraphAugmentorAlgorithm<TVertex, TEdge>(visitedGraph, vertexFactory, edgeFactory, host);
    }

    /// <summary> Augments all vertices of a graph
    /// by adding edge between all vertices from super source and to super sink. </summary>
    public sealed class AllVerticesGraphAugmentorAlgorithm<TVertex, TEdge>
        : GraphAugmentorAlgorithmBase<TVertex, TEdge, IMutableVertexAndEdgeSet<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Creates a new <see cref="AllVerticesGraphAugmentorAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        internal AllVerticesGraphAugmentorAlgorithm(
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
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

                AddAugmentedEdge(SuperSource, vertex);
                AddAugmentedEdge(vertex, SuperSink);
            }
        }
    }
}