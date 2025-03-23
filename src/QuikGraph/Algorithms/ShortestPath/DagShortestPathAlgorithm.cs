using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="CreateDagShortestPathAlgorithm"/>
    public static class DagShortestPathAlgorithm
    {
        /// <summary> Creates a new <see cref="DagShortestPathAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static DagShortestPathAlgorithm<TVertex
            , TEdge> CreateDagShortestPathAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new DagShortestPathAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, distanceRelaxer, host);
    }
    /// <summary> single source shortest path algorithm for directed acyclic graphs. </summary>
    public sealed class DagShortestPathAlgorithm<TVertex, TEdge>
        : ShortestPathAlgorithmBase<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Initializes a new <see cref="DagShortestPathAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        internal DagShortestPathAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, edgeWeights, host, distanceRelaxer ?? DistanceRelaxers.ShortestDistance)
        {
        }

        #region Events

        /// <inheritdoc />
        public event VertexAction<TVertex> InitializeVertex;

        private void OnVertexInitialized([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            InitializeVertex?.Invoke(vertex);
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> StartVertex;

        private void OnStartVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            StartVertex?.Invoke(vertex);
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> DiscoverVertex;

        private void OnDiscoverVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DiscoverVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when a vertex is going to be analyzed.
        /// </summary>
        public event VertexAction<TVertex> ExamineVertex;

        private void OnExamineVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            ExamineVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when an edge is going to be analyzed.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        private void OnExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when relax of an edge does not decrease distance.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeNotRelaxed;

        private void OnEdgeNotRelaxed([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeNotRelaxed?.Invoke(edge);
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        private void OnFinishVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            FinishVertex?.Invoke(vertex);
        }

        #endregion

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            // Initialize colors and distances
            double initialDistance = DistanceRelaxer.InitialDistance;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors[vertex] = GraphColor.White;
                SetVertexDistance(vertex, initialDistance);
                OnVertexInitialized(vertex);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();
            VerticesColors[root] = GraphColor.Gray;
            SetVertexDistance(root, 0);
            ComputeNoInit(root);
        }

        #endregion

        private void ComputeNoInit([NotNull] TVertex root)
        {
            IEnumerable<TVertex> orderedVertices = VisitedGraph.TopologicalSort();

            OnDiscoverVertex(root);
            foreach (TVertex vertex in orderedVertices)
            {
                OnStartVertex(vertex);

                VerticesColors[vertex] = GraphColor.Gray;
                OnExamineVertex(vertex);

                foreach (TEdge edge in VisitedGraph.OutEdges(vertex))
                {
                    VerticesColors[edge.Target] = GraphColor.Gray;
                    OnExamineEdge(edge);
                    OnDiscoverVertex(edge.Target);

                    bool decreased = Relax(edge);
                    if (decreased)
                    {
                        OnTreeEdge(edge);
                    }
                    else
                    {
                        OnEdgeNotRelaxed(edge);
                    }
                }

                VerticesColors[vertex] = GraphColor.Black;
                OnFinishVertex(vertex);
            }
        }
    }
}