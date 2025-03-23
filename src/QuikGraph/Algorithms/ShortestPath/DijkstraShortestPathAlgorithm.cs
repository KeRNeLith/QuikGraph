using System;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="CreateDijkstraShortestPathAlgorithm"/>
    public static class DijkstraShortestPathAlgorithm
    {
        /// <summary> Creates a new <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/> instance. </summary>
        public static DijkstraShortestPathAlgorithm<TVertex, TEdge> CreateDijkstraShortestPathAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new DijkstraShortestPathAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, distanceRelaxer, host);
    }

    /// <summary>
    /// Dijkstra single source shortest path algorithm for directed graph
    /// with positive distance.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class DijkstraShortestPathAlgorithm<TVertex, TEdge>
        : ShortestPathAlgorithmBase<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        private FibonacciQueue<TVertex, double> _vertexQueue;

        /// <summary>
        /// Initializes a new <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        internal DijkstraShortestPathAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, edgeWeights, host, distanceRelaxer ?? DistanceRelaxers.ShortestDistance)
        {
        }

        [Conditional("DEBUG")]
        private void AssertHeap()
        {
            if (_vertexQueue.Count == 0)
                return;

            TVertex top = _vertexQueue.Peek();
            TVertex[] vertices = _vertexQueue.ToArray();
            for (int i = 1; i < vertices.Length; ++i)
            {
                if (GetVertexDistance(top)> GetVertexDistance(vertices[i]))
                    Debug.Assert(false);
            }
        }

        #region Events

        /// <inheritdoc />
        public event VertexAction<TVertex> InitializeVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> DiscoverVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> StartVertex;

        /// <summary>
        /// Fired when a vertex is going to be analyzed.
        /// </summary>
        public event VertexAction<TVertex> ExamineVertex;

        /// <summary>
        /// Fired when an edge is going to be analyzed.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        /// <summary>
        /// Fired when relax of an edge does not decrease distance.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeNotRelaxed;

        private void OnEdgeNotRelaxed([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeNotRelaxed?.Invoke(edge);
        }

        private void InternalExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            if (Weights(edge) < 0)
                throw new NegativeWeightException();
        }

        private void OnDijkstraTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            bool decreased = Relax(edge);
            if (decreased)
            {
                OnTreeEdge(edge);
                AssertHeap();
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
                _vertexQueue.Update(edge.Target);
                AssertHeap();
                OnTreeEdge(edge);
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

            // Initialize colors and distances
            double initialDistance = DistanceRelaxer.InitialDistance;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors.Add(vertex, GraphColor.White);
                SetVertexDistance(vertex, initialDistance);
            }

            _vertexQueue = new FibonacciQueue<TVertex, double>(DistancesIndexGetter());
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (TryGetRootVertex(out TVertex rootVertex))
            {
                RootShouldBeInGraph(rootVertex);
                ComputeFromRoot(rootVertex);
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
#if DEBUG
                bfs.ExamineEdge += edge => AssertHeap();
#endif
                bfs.ExamineVertex += ExamineVertex;
                bfs.FinishVertex += FinishVertex;

                bfs.ExamineEdge += InternalExamineEdge;
                bfs.TreeEdge += OnDijkstraTreeEdge;
                bfs.GrayTarget += OnGrayTarget;

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

                    bfs.ExamineEdge -= InternalExamineEdge;
                    bfs.TreeEdge -= OnDijkstraTreeEdge;
                    bfs.GrayTarget -= OnGrayTarget;
                }
            }
        }
    }
}