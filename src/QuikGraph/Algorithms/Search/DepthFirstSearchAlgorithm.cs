using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="DepthFirstSearchAlgorithm{TVertex, TEdge}"/>
    public static class DepthFirstSearchAlgorithm
    {
        /// <inheritdoc cref="DepthFirstSearchAlgorithm{TVertex, TEdge}"/>
        public static DepthFirstSearchAlgorithm<TVertex, TEdge> CreateDepthFirstSearchAlgorithm<TVertex, TEdge>(
            this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgesFilter = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new DepthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, verticesColors, outEdgesFilter, host);
    }

    /// <summary> Depth first search algorithm for directed graphs </summary>
    public sealed class DepthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IVertexTimeStamperAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Initializes a new <see cref="DepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="verticesColors"> optional Vertices associated to their colors (treatment states).</param>
        /// <param name="outEdgesFilter">
        /// optional Delegate that takes the enumeration of out-edges and filters/reorders them.
        /// All vertices passed to the method should be enumerated once and only once.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesColors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="outEdgesFilter"/> is <see langword="null"/>.</exception>
        internal DepthFirstSearchAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors,
            [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgesFilter,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            VerticesColors = verticesColors ?? new Dictionary<TVertex, GraphColor>(visitedGraph.VertexCount);
            OutEdgesFilter = outEdgesFilter ?? (edges => edges);
        }

        /// <summary>
        /// Filter of edges.
        /// </summary>
        [NotNull]
        public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> OutEdgesFilter { get; }

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

        /// <inheritdoc cref="IVertexTimeStamperAlgorithm{TVertex}" />
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
                Visit(root);

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
                        Visit(vertex);
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

        private struct SearchFrame
        {
            [NotNull]
            public TVertex Vertex { get; }

            [NotNull]
            public IEnumerator<TEdge> Edges { get; }

            public int Depth { get; }

            public SearchFrame([NotNull] TVertex vertex, [NotNull] IEnumerator<TEdge> edges, int depth)
            {
                Debug.Assert(vertex != null);
                Debug.Assert(edges != null);
                Debug.Assert(depth >= 0, "Must be positive.");

                Vertex = vertex;
                Edges = edges;
                Depth = depth;
            }
        }

        private void Visit([NotNull] TVertex root)
        {
            Debug.Assert(root != null);

            var todoStack = new Stack<SearchFrame>();
            VerticesColors[root] = GraphColor.Gray;
            OnDiscoverVertex(root);

            IEnumerable<TEdge> enumerable = OutEdgesFilter(VisitedGraph.OutEdges(root));
            todoStack.Push(new SearchFrame(root, enumerable.GetEnumerator(), 0));

            while (todoStack.Count > 0)
            {
                ThrowIfCancellationRequested();

                SearchFrame frame = todoStack.Pop();
                TVertex u = frame.Vertex;
                int depth = frame.Depth;
                IEnumerator<TEdge> edges = frame.Edges;

                if (depth > MaxDepth)
                {
                    edges.Dispose();
                    VerticesColors[u] = GraphColor.Black;
                    OnVertexFinished(u);
                    continue;
                }

                while (edges.MoveNext())
                {
                    ThrowIfCancellationRequested();

                    TEdge edge = edges.Current;

                    // ReSharper disable once AssignNullToNotNullAttribute
                    // Justification: Enumerable items are not null so if the MoveNext succeed it can't be null
                    OnExamineEdge(edge);
                    TVertex v = edge.Target;
                    GraphColor vColor = VerticesColors[v];
                    switch (vColor)
                    {
                        case GraphColor.White:
                            OnTreeEdge(edge);
                            todoStack.Push(new SearchFrame(u, edges, depth));
                            u = v;
                            edges = OutEdgesFilter(VisitedGraph.OutEdges(u)).GetEnumerator();
                            ++depth;
                            VerticesColors[u] = GraphColor.Gray;
                            OnDiscoverVertex(u);
                            break;

                        case GraphColor.Gray:
                            OnBackEdge(edge);
                            break;

                        case GraphColor.Black:
                            OnForwardOrCrossEdge(edge);
                            break;
                    }
                }

                edges.Dispose();

                VerticesColors[u] = GraphColor.Black;
                OnVertexFinished(u);
            }
        }
    }
}