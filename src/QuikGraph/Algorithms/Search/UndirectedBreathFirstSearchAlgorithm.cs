using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="UndirectedBreadthFirstSearchAlgorithm"/>
    public static class UndirectedBreadthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="UndirectedBreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static UndirectedBreadthFirstSearchAlgorithm<TVertex
            , TEdge> CreateUndirectedBreadthFirstSearchAlgorithm<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IQueue<TVertex> vertexQueue = null,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new UndirectedBreadthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, vertexQueue, verticesColors, host);
    }

    /// <summary> breath first search algorithm for undirected graphs. </summary>
    public sealed class UndirectedBreadthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>
        , IUndirectedVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly IQueue<TVertex> _vertexQueue;

        /// <summary> Initializes a new <see cref="UndirectedBreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexQueue">Queue of vertices to treat.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexQueue"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesColors"/> is <see langword="null"/>.</exception>
        internal UndirectedBreadthFirstSearchAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IQueue<TVertex> vertexQueue = null,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            VerticesColors = verticesColors ?? new Dictionary<TVertex, GraphColor>(visitedGraph.VertexCount);
            _vertexQueue = vertexQueue ?? new Collections.Queue<TVertex>(); //visitedGraph.VertexCount);
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
        /// Fired when a vertex is discovered and under treatment.
        /// </summary>
        public event VertexAction<TVertex> DiscoverVertex;

        private void OnDiscoverVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DiscoverVertex?.Invoke(vertex);
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
        /// Fired when an edge is going to be treated when coming from a white vertex.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a gray vertex.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> NonTreeEdge;

        private void OnNonTreeEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            NonTreeEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as gray.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> GrayTarget;

        private void OnGrayTarget([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            GrayTarget?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as black.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> BlackTarget;

        private void OnBlackTarget([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            BlackTarget?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        private void OnVertexFinished([NotNull] TVertex vertex)
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

            // Initialize vertices
            ThrowIfCancellationRequested();

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors[vertex] = GraphColor.White;
                OnVertexInitialized(vertex);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();
            EnqueueRoot(root);
            FlushVisitQueue();
        }

        #endregion

        /// <summary>
        /// Stores vertices associated to their colors (treatment state).
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, GraphColor> VerticesColors { get; }

        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        #region IVertexColorizerAlgorithm<TVertex>

        /// <inheritdoc />
        public GraphColor? GetVertexColor(TVertex vertex)
        {
            if (VerticesColors.TryGetValue(vertex, out GraphColor color))
                return color;
            return null;
        }

        #endregion

        internal void Visit([NotNull] TVertex root)
        {
            Debug.Assert(root != null);

            EnqueueRoot(root);
            FlushVisitQueue();
        }

        private void EnqueueRoot([NotNull] TVertex root)
        {
            OnStartVertex(root);

            VerticesColors[root] = GraphColor.Gray;

            OnDiscoverVertex(root);
            _vertexQueue.Enqueue(root);
        }

        private void FlushVisitQueue()
        {
            while (_vertexQueue.Count > 0)
            {
                ThrowIfCancellationRequested();

                TVertex u = _vertexQueue.Dequeue();

                OnExamineVertex(u);

                ExploreAdjacentEdges(u);

                VerticesColors[u] = GraphColor.Black;
                OnVertexFinished(u);
            }
        }

        private void ExploreAdjacentEdges([NotNull] TVertex u)
        {
            foreach (TEdge edge in VisitedGraph.AdjacentEdges(u))
            {
                bool reversed = VisitedGraph.AreVerticesEqual(edge.Target, u);
                TVertex v = reversed ? edge.Source : edge.Target;
                OnExamineEdge(edge);

                GraphColor vColor = VerticesColors[v];
                if (vColor == GraphColor.White)
                {
                    OnTreeEdge(edge, reversed);
                    VerticesColors[v] = GraphColor.Gray;
                    OnDiscoverVertex(v);
                    _vertexQueue.Enqueue(v);
                }
                else
                {
                    OnNonTreeEdge(edge, reversed);
                    if (vColor == GraphColor.Gray)
                    {
                        OnGrayTarget(edge, reversed);
                    }
                    else
                    {
                        OnBlackTarget(edge, reversed);
                    }
                }
            }
        }
    }
}