using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.MaximumFlow;

namespace QuikGraph.Algorithms
{
    /// <inheritdoc cref="MaximumBipartiteMatchingAlgorithm{TVertex, TEdge}"/>
    public static class MaximumBipartiteMatchingAlgorithm
    {
        /// <inheritdoc cref="MaximumBipartiteMatchingAlgorithm{TVertex, TEdge}"/>
        public static MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> ComputeMaximumBipartiteMatching<TVertex, TEdge>(
            this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph, TVertex[] vertexSetA, TVertex[] vertexSetB, VertexFactory<TVertex> vertexFactory, EdgeFactory<TVertex, TEdge> edgeFactory) where TEdge : IEdge<TVertex>
        {
            MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> maxMatch = CreateMaximumBipartiteMatching(graph, vertexSetA, vertexSetB, vertexFactory, edgeFactory);

            maxMatch.Compute();
            return maxMatch;
        }

        /// <inheritdoc cref="MaximumBipartiteMatchingAlgorithm{TVertex, TEdge}"/>
        public static MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> CreateMaximumBipartiteMatching<TVertex, TEdge>(
            this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph, TVertex[] vertexSetA, TVertex[] vertexSetB, VertexFactory<TVertex> vertexFactory, EdgeFactory<TVertex, TEdge> edgeFactory) where TEdge : IEdge<TVertex>
        {
            return new MaximumBipartiteMatchingAlgorithm<TVertex, TEdge>(
                graph,
                vertexSetA,
                vertexSetB,
                vertexFactory,
                edgeFactory);
        }
    }

    /// <summary> Computes a maximum bipartite matching in a graph,
    /// i.e. the maximum number of edges not sharing any vertex.
    /// </summary>
    public sealed class MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> : AlgorithmBase<IMutableVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Initializes a new <see cref="MaximumBipartiteMatchingAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="sourceToVertices">Vertices to which creating augmented edge from super source.</param>
        /// <param name="verticesToSink">Vertices from which creating augmented edge to super sink.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sourceToVertices"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesToSink"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        internal MaximumBipartiteMatchingAlgorithm(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> sourceToVertices,
            [NotNull, ItemNotNull] IEnumerable<TVertex> verticesToSink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory)
            : base(visitedGraph)
        {
            SourceToVertices = sourceToVertices ?? throw new ArgumentNullException(nameof(sourceToVertices));
            VerticesToSink = verticesToSink ?? throw new ArgumentNullException(nameof(verticesToSink));
            VertexFactory = vertexFactory ?? throw new ArgumentNullException(nameof(vertexFactory));
            EdgeFactory = edgeFactory ?? throw new ArgumentNullException(nameof(edgeFactory));
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

        /// <summary>
        /// Vertex factory method.
        /// </summary>
        [NotNull]
        public VertexFactory<TVertex> VertexFactory { get; }

        /// <summary>
        /// Edge factory method.
        /// </summary>
        [NotNull]
        public EdgeFactory<TVertex, TEdge> EdgeFactory { get; }


        [NotNull, ItemNotNull]
        private readonly List<TEdge> _matchedEdges = new List<TEdge>();

        /// <summary>
        /// Maximal edges matching.
        /// </summary>
        [NotNull, ItemNotNull]
        public TEdge[] MatchedEdges => _matchedEdges.ToArray();

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _matchedEdges.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge> augmentor = null;
            ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reverser = null;

            try
            {
                ThrowIfCancellationRequested();

                // Augmenting the graph
                augmentor = VisitedGraph.ComputeBipartiteToMaximumFlowGraphAugmentorAlgorithm(
                    SourceToVertices,
                    VerticesToSink,
                    VertexFactory,
                    EdgeFactory, this);

                ThrowIfCancellationRequested();

                // Adding reverse edges
                reverser = VisitedGraph.CreateReversedEdgeAugmentorAlgorithm(EdgeFactory);
                reverser.AddReversedEdges();

                ThrowIfCancellationRequested();

                // Compute maximum flow
                var flow = VisitedGraph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => 1.0, EdgeFactory, reverser, this);

                flow.Compute(augmentor.SuperSource, augmentor.SuperSink);

                ThrowIfCancellationRequested();

                foreach (TEdge edge in VisitedGraph.Edges)
                {
                    if (Math.Abs(flow.ResidualCapacities[edge]) < float.Epsilon)
                    {
                        if (VisitedGraph.AreVerticesEqual(edge.Source, augmentor.SuperSource)
                            || VisitedGraph.AreVerticesEqual(edge.Source, augmentor.SuperSink)
                            || VisitedGraph.AreVerticesEqual(edge.Target, augmentor.SuperSource)
                            || VisitedGraph.AreVerticesEqual(edge.Target, augmentor.SuperSink))
                        {
                            // Skip all edges that connect to SuperSource or SuperSink
                            continue;
                        }

                        _matchedEdges.Add(edge);
                    }
                }
            }
            finally
            {
                if (reverser != null && reverser.Augmented)
                {
                    reverser.RemoveReversedEdges();
                }
                if (augmentor != null && augmentor.Augmented)
                {
                    augmentor.Rollback();
                }
            }
        }

        #endregion
    }
}