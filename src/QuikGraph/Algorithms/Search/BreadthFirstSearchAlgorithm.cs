using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="CreateBreadthFirstSearchAlgorithm"/>
    public static class BreadthFirstSearchAlgorithm
    {
        /// <summary>Creates a new <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.</summary>
        public static BreadthFirstSearchAlgorithm<TVertex, TEdge> CreateBreadthFirstSearchAlgorithm<TVertex
            , TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IQueue<TVertex> vertexQueue = null,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgesFilter = null,
            [CanBeNull] IAlgorithmComponent host = null)
            where TEdge : IEdge<TVertex>
            => new BreadthFirstSearchAlgorithm<TVertex, TEdge>(
                visitedGraph,
                vertexQueue,
                verticesColors,
                outEdgesFilter,
                host);
    }

    /// <summary>
    /// A breath first search algorithm for directed graphs.
    /// </summary>
    /// <remarks>
    /// This is a modified version of the classic DFS algorithm
    /// where the search is performed both in depth and height.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class BreadthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        [NotNull]
        private readonly IQueue<TVertex> _vertexQueue;

        /// <summary>
        /// Initializes a new <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexQueue">Queue of vertices to treat.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        /// <param name="outEdgesFilter">Function that is used filter out-edges of a vertex.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexQueue"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesColors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="outEdgesFilter"/> is <see langword="null"/>.</exception>
        public BreadthFirstSearchAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IQueue<TVertex> vertexQueue = null,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgesFilter = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            VerticesColors = verticesColors ?? new Dictionary<TVertex, GraphColor>(visitedGraph.VertexCount);
            _vertexQueue = vertexQueue ?? new Collections.Queue<TVertex>();
            OutEdgesFilter = outEdgesFilter ?? (edges => edges);
        }

        /// <summary>
        /// Filter of edges.
        /// </summary>
        [NotNull]
        public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> OutEdgesFilter { get; }

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
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a gray vertex.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> NonTreeEdge;

        private void OnNonTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            NonTreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as gray.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> GrayTarget;

        private void OnGrayTarget([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            GrayTarget?.Invoke(edge);
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as black.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BlackTarget;

        private void OnBlackTarget([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            BlackTarget?.Invoke(edge);
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

            ThrowIfCancellationRequested();

            // Initialize vertices
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors[vertex] = GraphColor.White;
                OnVertexInitialized(vertex);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            if (TryGetRootVertex(out TVertex rootVertex))
            {
                RootShouldBeInGraph(rootVertex);

                // Enqueue select root only
                EnqueueRoot(rootVertex);
            }
            else
            {
                // Enqueue roots
                foreach (TVertex root in VisitedGraph.Roots())
                {
                    EnqueueRoot(root);
                }
            }

            FlushVisitQueue();
        }

        #endregion

        /// <summary>
        /// Stores vertices associated to their colors (treatment state).
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, GraphColor> VerticesColors { get; }

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
            Debug.Assert(root!= null);

            EnqueueRoot(root);
            FlushVisitQueue();
        }

        private void EnqueueRoot([NotNull] TVertex vertex)
        {
            OnStartVertex(vertex);

            VerticesColors[vertex] = GraphColor.Gray;

            OnDiscoverVertex(vertex);
            _vertexQueue.Enqueue(vertex);
        }

        private void FlushVisitQueue()
        {
            while (_vertexQueue.Count > 0)
            {
                ThrowIfCancellationRequested();

                TVertex u = _vertexQueue.Dequeue();
                OnExamineVertex(u);
                foreach (TEdge edge in OutEdgesFilter(VisitedGraph.OutEdges(u)))
                {
                    TVertex v = edge.Target;
                    OnExamineEdge(edge);

                    GraphColor vColor = VerticesColors[v];
                    if (vColor == GraphColor.White)
                    {
                        OnTreeEdge(edge);
                        VerticesColors[v] = GraphColor.Gray;
                        OnDiscoverVertex(v);
                        _vertexQueue.Enqueue(v);
                    }
                    else
                    {
                        OnNonTreeEdge(edge);
                        if (vColor == GraphColor.Gray)
                        {
                            OnGrayTarget(edge);
                        }
                        else
                        {
                            OnBlackTarget(edge);
                        }
                    }
                }

                VerticesColors[u] = GraphColor.Black;
                OnVertexFinished(u);
            }
        }
    }
}