using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="CreateAStarShortestPathAlgorithm"/>
    public static class AStarShortestPathAlgorithm
    {
        /// <summary> Creates an AStarShortestPathAlgorithm </summary>
        public static AStarShortestPathAlgorithm<TVertex, TEdge> CreateAStarShortestPathAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] Func<TVertex, double> costHeuristic,
            [CanBeNull] IAlgorithmComponent host = null,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null) where TEdge : IEdge<TVertex>
            => new AStarShortestPathAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, costHeuristic, host, distanceRelaxer);
    }

    /// <summary> A* single source shortest-path algorithm for directed graph with positive distances. </summary>
    /// <remarks>
    /// The A* algorithm is a generalization of Dijkstra's shortest path algorithm
    /// that uses a heuristic to optimize the search process.
    /// </remarks>
    public sealed class AStarShortestPathAlgorithm<TVertex, TEdge>
        : ShortestPathAlgorithmBase<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        private FibonacciQueue<TVertex, double> _vertexQueue;

        private Dictionary<TVertex, double> _costs;

        /// <summary>
        /// Initializes a new <see cref="AStarShortestPathAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="costHeuristic">Function that computes a cost for a given vertex.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="costHeuristic"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        public AStarShortestPathAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] Func<TVertex, double> costHeuristic,
            [CanBeNull] IAlgorithmComponent host = null,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null)
            : base(visitedGraph, edgeWeights, host, distanceRelaxer)
        {
            CostHeuristic = costHeuristic ?? throw new ArgumentNullException(nameof(costHeuristic));
        }

        /// <summary>
        /// Function that computes a cost for a given vertex.
        /// </summary>
        [NotNull]
        public Func<TVertex, double> CostHeuristic { get; }

        #region Events

        /// <inheritdoc />
        public event VertexAction<TVertex> InitializeVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> DiscoverVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> StartVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        /// <summary>
        /// Fired when a vertex is going to be analyzed.
        /// </summary>
        public event VertexAction<TVertex> ExamineVertex;

        /// <summary>
        /// Fired when an edge is going to be analyzed.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        /// <summary>
        /// Fired when relax of an edge does not decrease distance.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeNotRelaxed;

        private void OnEdgeNotRelaxed([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeNotRelaxed?.Invoke(edge);
        }

        private void OnExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            if (Weights(edge) < 0)
                throw new NegativeWeightException();
        }

        private void OnAStarTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            bool decreased = Relax(edge);
            if (decreased)
            {
                TVertex target = edge.Target;
                double distance = GetVertexDistance(target);

                _costs[target] = DistanceRelaxer.Combine(distance, CostHeuristic(target));
                OnTreeEdge(edge);
            }
            else
            {
                OnEdgeNotRelaxed(edge);
            }
        }

        private void OnGrayTarget([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            bool decreased = Relax(edge);
            if (decreased)
            {
                TVertex target = edge.Target;
                double distance = GetVertexDistance(target);

                _costs[target] = DistanceRelaxer.Combine(distance, CostHeuristic(target));
                _vertexQueue.Update(target);
                OnTreeEdge(edge);
            }
            else
            {
                OnEdgeNotRelaxed(edge);
            }
        }

        private void OnBlackTarget([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            bool decreased = Relax(edge);
            if (decreased)
            {
                TVertex target = edge.Target;
                double distance = GetVertexDistance(target);

                OnTreeEdge(edge);
                _costs[target] = DistanceRelaxer.Combine(distance, CostHeuristic(target));
                _vertexQueue.Enqueue(target);
                VerticesColors[target] = GraphColor.Gray;
            }
            else
            {
                OnEdgeNotRelaxed(edge);
            }
        }

        #endregion

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            VerticesColors.Clear();
            _costs = new Dictionary<TVertex, double>(VisitedGraph.VertexCount);

            // Initialize colors and distances
            double initialDistance = DistanceRelaxer.InitialDistance;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors.Add(vertex, GraphColor.White);
                SetVertexDistance(vertex, initialDistance);
                _costs.Add(vertex, initialDistance);
            }

            _vertexQueue = new FibonacciQueue<TVertex, double>(_costs, DistanceRelaxer.Compare);
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (TryGetRootVertex(out TVertex root))
            {
                RootShouldBeInGraph(root);
                ComputeFromRoot(root);
            }
            else
            {
                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    if (VerticesColors[vertex] == GraphColor.White)
                    {
                        ComputeFromRoot(vertex);
                    }
                }
            }
        }

        #endregion

        private void ComputeFromRoot([NotNull] TVertex rootVertex)
        {
            Debug.Assert(rootVertex != null);
            Debug.Assert(VisitedGraph.ContainsVertex(rootVertex));
            Debug.Assert(VerticesColors[rootVertex] == GraphColor.White);

            VerticesColors[rootVertex] = GraphColor.Gray;
            SetVertexDistance(rootVertex, 0);
            ComputeNoInit(rootVertex);
        }

        private void ComputeNoInit([NotNull] TVertex root)
        {
            BreadthFirstSearchAlgorithm<TVertex, TEdge> bfs = null;

            try
            {
                bfs = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(
                    VisitedGraph,
                    _vertexQueue,
                    VerticesColors,
                    host: this);

                bfs.InitializeVertex += InitializeVertex;
                bfs.DiscoverVertex += DiscoverVertex;
                bfs.StartVertex += StartVertex;
                bfs.ExamineEdge += ExamineEdge;
                bfs.ExamineVertex += ExamineVertex;
                bfs.FinishVertex += FinishVertex;

                bfs.ExamineEdge += OnExamineEdge;
                bfs.TreeEdge += OnAStarTreeEdge;
                bfs.GrayTarget += OnGrayTarget;
                bfs.BlackTarget += OnBlackTarget;

                bfs.Visit(root);
            }
            finally
            {
                if (bfs != null)
                {
                    bfs.InitializeVertex -= InitializeVertex;
                    bfs.DiscoverVertex -= DiscoverVertex;
                    bfs.StartVertex -= StartVertex;
                    bfs.ExamineEdge -= ExamineEdge;
                    bfs.ExamineVertex -= ExamineVertex;
                    bfs.FinishVertex -= FinishVertex;

                    bfs.ExamineEdge -= OnExamineEdge;
                    bfs.TreeEdge -= OnAStarTreeEdge;
                    bfs.GrayTarget -= OnGrayTarget;
                    bfs.BlackTarget -= OnBlackTarget;
                }
            }
        }
    }
}