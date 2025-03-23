using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm{TVertex,TEdge}"/>
    public static class BipartiteToMaximumFlowGraphAugmentorAlgorithm
    {
        /// <summary> Creates a <inheritdoc cref="BipartiteToMaximumFlowGraphAugmentorAlgorithm{TVertex,TEdge}"/> </summary>
        public static BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge> CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> sourceToVertices,
            [NotNull, ItemNotNull] IEnumerable<TVertex> verticesToSink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory, [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge>(visitedGraph, sourceToVertices, verticesToSink, vertexFactory, edgeFactory, host);

        /// <summary> Creates a <inheritdoc cref="BipartiteToMaximumFlowGraphAugmentorAlgorithm{TVertex,TEdge}"/> </summary>
        public static BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge> ComputeBipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> sourceToVertices,
            [NotNull, ItemNotNull] IEnumerable<TVertex> verticesToSink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory, [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
        {
            var algorithm = visitedGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory, host);
            algorithm.Compute();
            return algorithm;
        }
    }

    /// <summary>
    /// This algorithm modifies a bipartite graph into a related graph, where each vertex in
    /// one partition is connected to a newly added "SuperSource" and each vertex in the other
    /// partition is connected to a newly added "SuperSink". When the maximum flow of this
    /// related graph is computed, the edges used for the flow are also those which make up
    /// the maximum match for the bipartite graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge>
        : GraphAugmentorAlgorithmBase<TVertex, TEdge, IMutableVertexAndEdgeSet<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Initializes a new <see cref="BipartiteToMaximumFlowGraphAugmentorAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="sourceToVertices">Vertices to which creating augmented edge from super source.</param>
        /// <param name="verticesToSink">Vertices from which creating augmented edge to super sink.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sourceToVertices"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesToSink"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        public BipartiteToMaximumFlowGraphAugmentorAlgorithm(
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [NotNull] [ItemNotNull] IEnumerable<TVertex> sourceToVertices,
            [NotNull] [ItemNotNull] IEnumerable<TVertex> verticesToSink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory, [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, vertexFactory, edgeFactory, host)
        {
            SourceToVertices = sourceToVertices ?? throw new ArgumentNullException(nameof(sourceToVertices));
            VerticesToSink = verticesToSink ?? throw new ArgumentNullException(nameof(verticesToSink));
        }

        /// <summary>
        /// Vertices to which augmented edge from super source are created with augmentation.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TVertex> SourceToVertices { get; }

        /// <summary>
        /// Vertices from which augmented edge to super sink are created with augmentation.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TVertex> VerticesToSink { get; }

        #region GraphAugmentorAlgorithmBase<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        protected override void AugmentGraph()
        {
            foreach (TVertex vertex in SourceToVertices)
            {
                ThrowIfCancellationRequested();

                AddAugmentedEdge(SuperSource, vertex);
            }

            foreach (TVertex vertex in VerticesToSink)
            {
                ThrowIfCancellationRequested();

                AddAugmentedEdge(vertex, SuperSink);
            }
        }

        #endregion
    }
}