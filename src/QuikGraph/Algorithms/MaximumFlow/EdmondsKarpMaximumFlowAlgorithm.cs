using System;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
using QuikGraph.Predicates;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateEdmondsKarpMaximumFlowAlgorithm{TVertex,TEdge}"/>
    public static class EdmondsKarpMaximumFlowAlgorithm
    {
        /// <summary> Creates a new <see cref="EdmondsKarpMaximumFlowAlgorithm"/> </summary>
        public static EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge> CreateEdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> capacities,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reverseEdgesAugmentorAlgorithm,
            [CanBeNull] IAlgorithmComponent host = null, TVertex source = default(TVertex)) where TEdge : IEdge<TVertex>
            => new EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge>(visitedGraph, capacities, edgeFactory, reverseEdgesAugmentorAlgorithm, host)
            {
                Source = source
            };
    }

    /// <summary> Edmond and Karp maximum flow algorithm for directed graph with positive capacities and flows. </summary>
    /// <remarks>
    /// The Edmonds-Karp Algorithm is an implementation of the Ford-Fulkerson method for computing the maximum flow in a flow network.
    /// The Ford-Fulkerson method relies on finding augmenting paths in the residual graph until no more augmenting paths are available.
    /// The Edmonds-Karp algorithm specifies that these augmenting paths be found using Breadth-First Search (BFS),
    /// which ensures that the shortest path (in terms of the number of edges) is found in each iteration.
    /// This specific choice of BFS results in a predictable runtime complexity.
    /// </remarks>
    public sealed class EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge> : MaximumFlowAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> _reverserAlgorithm;

        /// <summary>
        /// Initializes a new <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacities">Function that given an edge return the capacity of this edge.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="reverseEdgesAugmentorAlgorithm">Algorithm that is in of charge augmenting the graph (creating missing reversed edges).</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="capacities"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reverseEdgesAugmentorAlgorithm"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="reverseEdgesAugmentorAlgorithm"/> targets a graph different from <paramref name="visitedGraph"/>.</exception>
        public EdmondsKarpMaximumFlowAlgorithm([NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> capacities,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reverseEdgesAugmentorAlgorithm,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, capacities, edgeFactory, host)
        {
            if (reverseEdgesAugmentorAlgorithm is null)
                throw new ArgumentNullException(nameof(reverseEdgesAugmentorAlgorithm));
            if (!ReferenceEquals(visitedGraph, reverseEdgesAugmentorAlgorithm.VisitedGraph))
                throw new ArgumentException("Must target the same graph.", nameof(reverseEdgesAugmentorAlgorithm));

            _reverserAlgorithm = reverseEdgesAugmentorAlgorithm;
            ReversedEdges = reverseEdgesAugmentorAlgorithm.ReversedEdges;
        }

        [NotNull]
        private IVertexListGraph<TVertex, TEdge> ResidualGraph
            => VisitedGraph.FilterBy<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>(vertex
                => true, new ResidualEdgePredicate<TEdge>(ResidualCapacities).Test);

        private void Augment([NotNull] TVertex source, [NotNull] TVertex sink)
        {
            Debug.Assert(source != null);
            Debug.Assert(sink != null);

            // Find minimum residual capacity along the augmenting path
            double delta = double.MaxValue;
            TVertex u = sink;
            TEdge e;
            do
            {
                e = Predecessors[u];
                delta = Math.Min(delta, ResidualCapacities[e]);
                u = e.Source;
            } while (!VisitedGraph.AreVerticesEqual(u, source));

            // Push delta units of flow along the augmenting path
            u = sink;
            do
            {
                e = Predecessors[u];
                ResidualCapacities[e] -= delta;
                if (ReversedEdges != null && ReversedEdges.ContainsKey(e))
                {
                    ResidualCapacities[ReversedEdges[e]] += delta;
                }
                u = e.Source;
            } while (!VisitedGraph.AreVerticesEqual(u, source));
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            if (!_reverserAlgorithm.Augmented)
            {
                throw new InvalidOperationException(
                    $"The graph has not been augmented yet.{Environment.NewLine}" +
                    $"Call {nameof(ReversedEdgeAugmentorAlgorithm<int, IEdge<int>>)}.{nameof(ReversedEdgeAugmentorAlgorithm<int, IEdge<int>>.AddReversedEdges)}() before running this algorithm.");
            }

            if (Source == null)
                throw new InvalidOperationException("Source is not specified.");
            if (Sink == null)
                throw new InvalidOperationException("Sink is not specified.");
            if (!VisitedGraph.ContainsVertex(Source))
                throw new Exception("Source vertex is not part of the graph.");
            if (!VisitedGraph.ContainsVertex(Sink))
                throw new Exception("Sink vertex is not part of the graph.");
        }

        /// <summary>
        /// Computes the maximum flow between <see cref="MaximumFlowAlgorithm{TVertex,TEdge}.Source"/>
        /// and <see cref="MaximumFlowAlgorithm{TVertex,TEdge}.Sink"/>.
        /// </summary>
        protected override void InternalCompute()
        {
            ThrowIfCancellationRequested();

            IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph = VisitedGraph;
            foreach (TVertex vertex in graph.Vertices)
            {
                foreach (TEdge edge in graph.OutEdges(vertex))
                {
                    double capacity = Capacities(edge);
                    if (capacity < 0)
                        throw new NegativeCapacityException();
                    ResidualCapacities[edge] = capacity;
                }
            }

            VerticesColors[Sink] = GraphColor.Gray;
            while (VerticesColors[Sink] != GraphColor.White)
            {
                var verticesPredecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>(Predecessors, VisitedGraph.AreVerticesEqual);
                var queue = new Collections.Queue<TVertex>();
                var bfs = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(
                    ResidualGraph,
                    queue,
                    VerticesColors);

                using (verticesPredecessors.Attach(bfs))
                    bfs.Compute(Source);

                if (VerticesColors[Sink] != GraphColor.White)
                {
                    Augment(Source, Sink);
                }
            }

            MaxFlow = 0;
            foreach (TEdge edge in graph.OutEdges(Source))
            {
                MaxFlow += Capacities(edge) - ResidualCapacities[edge];
            }
        }

        #endregion
    }
}