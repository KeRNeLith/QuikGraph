using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="BidirectionalDepthFirstSearchAlgorithm"/>
    public static class BidirectionalDepthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="BidirectionalDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static BidirectionalDepthFirstSearchAlgorithm<TVertex
            , TEdge> CreateBidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new BidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, verticesColors, host);

    }
    /// <summary> A depth and height first search algorithm for directed graphs. </summary>
    /// <remarks>
    /// This is a modified version of the classic DFS algorithm
    /// where the search is performed in both depth and height.
    /// </remarks>
    public sealed class BidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IBidirectionalGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Creates a new <see cref="BidirectionalDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesColors"/> is <see langword="null"/>.</exception>
        public BidirectionalDepthFirstSearchAlgorithm(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            VerticesColors = verticesColors ?? new Dictionary<TVertex, GraphColor>(visitedGraph.VertexCount);
        }

        /// <summary>
        /// In case a root vertex has been set, indicates if the algorithm should
        /// walk through graph parts of other components than the root component.
        /// </summary>
        public bool ProcessAllComponents { get; set; }

        private int _maxDepth = int.MaxValue;

        /// <summary>
        /// Gets or sets the maximum exploration depth, from the start vertex.
        /// </summary>
        /// <remarks>
        /// Defaulted to <see cref="F:int.MaxValue"/>.
        /// </remarks>
        /// <value>
        /// Maximum exploration depth.
        /// </value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative or equal to 0.</exception>
        public int MaxDepth
        {
            get => _maxDepth;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be positive.");
                _maxDepth = value;
            }
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
        public event EdgeAction<TVertex, TEdge> BackEdge;

        private void OnBackEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            BackEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a black vertex.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

        private void OnForwardOrCrossEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ForwardOrCrossEdge?.Invoke(edge);
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

            // Put all vertex to white
            VerticesColors.Clear();
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors[vertex] = GraphColor.White;
                OnVertexInitialized(vertex);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // If there is a starting vertex, start with it
            if (TryGetRootVertex(out TVertex root))
            {
                RootShouldBeInGraph(root);

                OnStartVertex(root);
                Visit(root, 0);

                if (ProcessAllComponents)
                {
                    VisitAllWhiteVertices(); // All remaining vertices (because there are not white marked)
                }
            }
            else
            {
                VisitAllWhiteVertices();
            }

            #region Local function

            void VisitAllWhiteVertices()
            {
                // Process each vertex
                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    ThrowIfCancellationRequested();

                    if (VerticesColors[vertex] == GraphColor.White)
                    {
                        OnStartVertex(vertex);
                        Visit(vertex, 0);
                    }
                }
            }

            #endregion
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

        private void Visit([NotNull] TVertex u, int depth)
        {
            Debug.Assert(u != null);

            if (depth > MaxDepth)
                return;

            VerticesColors[u] = GraphColor.Gray;
            OnDiscoverVertex(u);

            foreach (TEdge edge in VisitedGraph.OutEdges(u))
            {
                ThrowIfCancellationRequested();

                OnExamineEdge(edge);
                TVertex v = edge.Target;
                ProcessEdge(depth, v, edge);
            }

            foreach (TEdge edge in VisitedGraph.InEdges(u))
            {
                ThrowIfCancellationRequested();

                OnExamineEdge(edge);
                TVertex v = edge.Source;
                ProcessEdge(depth, v, edge);
            }

            VerticesColors[u] = GraphColor.Black;
            OnVertexFinished(u);
        }

        private void ProcessEdge(int depth, [NotNull] TVertex vertex, [NotNull] TEdge edge)
        {
            GraphColor color = VerticesColors[vertex];
            if (color == GraphColor.White)
            {
                OnTreeEdge(edge);
                Visit(vertex, depth + 1);
            }
            else if (color == GraphColor.Gray)
            {
                OnBackEdge(edge);
            }
            else
            {
                OnForwardOrCrossEdge(edge);
            }
        }
    }
}