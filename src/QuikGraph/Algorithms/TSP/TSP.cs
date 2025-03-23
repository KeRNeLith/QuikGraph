using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.ShortestPath;

namespace QuikGraph.Algorithms.TSP
{
    /// <summary> TSP (Travelling Salesman Problem); Finding a path that best link all vertices. </summary>
    // ReSharper disable once InconsistentNaming
    public class TSP<TVertex, TEdge, TGraph>
        : ShortestPathAlgorithmBase<TVertex, TEdge, TGraph>
        where TEdge : EquatableEdge<TVertex>
        where TGraph : BidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly TasksManager<TVertex, TEdge> _taskManager = new TasksManager<TVertex, TEdge>();

        /// <summary>
        /// Shortest path found, otherwise <see langword="null"/>.
        /// </summary>
        [CanBeNull]
        public BidirectionalGraph<TVertex, TEdge> ResultPath { get; private set; }

        /// <summary>
        /// Best cost found to answer the problem.
        /// </summary>
        public double BestCost { get; private set; } = double.PositiveInfinity;

        /// <summary>
        /// Initializes a new <see cref="TSP{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public TSP(
            [NotNull] TGraph visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights)
            : base(visitedGraph, edgeWeights, null)
        {
        }

        [Pure]
        [NotNull]
        private static Dictionary<EquatableEdge<TVertex>, double> BuildWeightsDictionary(
            [NotNull] TGraph visitedGraph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights)
        {
            var weights = new Dictionary<EquatableEdge<TVertex>, double>();
            foreach (TEdge edge in visitedGraph.Edges)
            {
                weights[edge] = edgeWeights(edge);
            }

            return weights;
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            var path = new BidirectionalGraph<TVertex, TEdge>();
            path.AddVertexRange(VisitedGraph.Vertices);

            _taskManager.AddTask(
                new Task<TVertex, TEdge>(
                    VisitedGraph,
                    BuildWeightsDictionary(VisitedGraph, Weights),
                    path,
                    0));
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            while (_taskManager.HasTasks())
            {
                ThrowIfCancellationRequested();

                Task<TVertex, TEdge> task = _taskManager.GetTask();

                if (task.ConnectsAllVertices())
                {
                    BestCost = task.MinCost;
                    ResultPath = task.Path;
                    return;
                }

                if (task.Split(
                    out Task<TVertex, TEdge> task1,
                    out Task<TVertex, TEdge> task2))
                {
                    _taskManager.AddTask(task1);
                    _taskManager.AddTask(task2);
                }
            }
        }

        #endregion
    }
}