using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="CreateUndirectedDepthFirstSearchAlgorithm"/>
    public static class UndirectedDepthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="BidirectionalDepthFirstSearchAlgorithm{TVertex,TEdge}"/> </summary>
        public static UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge> CreateUndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(
                    [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
                    [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
                    [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> adjacentEdgesFilter = null,
                    [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(
                    visitedGraph, verticesColors, adjacentEdgesFilter, host);
    }

    /// <summary>
    /// A depth first search algorithm for undirected graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        , IUndirectedVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IVertexTimeStamperAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Creates a new <see cref="BidirectionalDepthFirstSearchAlgorithm{TVertex,TEdge}"/> </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        /// <param name="adjacentEdgesFilter">
        /// Delegate that takes the enumeration of out-edges and filters/reorders
        /// them. All vertices passed to the method should be enumerated once and only once.
        /// </param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesColors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="adjacentEdgesFilter"/> is <see langword="null"/>.</exception>
        public UndirectedDepthFirstSearchAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, GraphColor> verticesColors = null,
            [CanBeNull] Func<IEnumerable<TEdge>, IEnumerable<TEdge>> adjacentEdgesFilter = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            VerticesColors = verticesColors ?? new Dictionary<TVertex, GraphColor>(visitedGraph.VertexCount);
            AdjacentEdgesFilter = adjacentEdgesFilter ?? (edges => edges);
        }

        /// <summary>
        /// Filter of adjacent edges.
        /// </summary>
        [NotNull]
        public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> AdjacentEdgesFilter { get; }

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
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be positive");
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
        /// Fired when the maximal authorized depth is reached.
        /// </summary>
        public event VertexAction<TVertex> VertexMaxDepthReached;

        private void OnVertexMaxDepthReached([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexMaxDepthReached?.Invoke(vertex);
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
        public event UndirectedEdgeAction<TVertex, TEdge> ExamineEdge;

        private void OnExamineEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
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
        public event UndirectedEdgeAction<TVertex, TEdge> BackEdge;

        private void OnBackEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            BackEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a black vertex.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

        private void OnForwardOrCrossEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            ForwardOrCrossEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
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
                Debug.Assert(depth >= 0);

                Vertex = vertex;
                Edges = edges;
                Depth = depth;
            }
        }

        private void Visit([NotNull] TVertex root)
        {
            Debug.Assert(root != null);

            var todoStack = new Stack<SearchFrame>();
            var visitedEdges = new Dictionary<TEdge, int>(VisitedGraph.EdgeCount);

            VerticesColors[root] = GraphColor.Gray;
            OnDiscoverVertex(root);

            IEnumerable<TEdge> enumerable = AdjacentEdgesFilter(VisitedGraph.AdjacentEdges(root));
            todoStack.Push(new SearchFrame(root, enumerable.GetEnumerator(), 0));

            while (todoStack.Count > 0)
            {
                ThrowIfCancellationRequested();

                SearchFrame frame = todoStack.Pop();
                TVertex u = frame.Vertex;
                int depth = frame.Depth;

                if (depth > MaxDepth)
                {
                    OnVertexMaxDepthReached(u);
                    VerticesColors[u] = GraphColor.Black;
                    OnVertexFinished(u);
                    continue;
                }

                IEnumerator<TEdge> edges = frame.Edges;
                while (edges.MoveNext())
                {
                    ThrowIfCancellationRequested();

                    TEdge edge = edges.Current;

                    // ReSharper disable once AssignNullToNotNullAttribute
                    // Justification: Enumerable items are not null so if the MoveNext succeed it can't be null
                    if (visitedEdges.ContainsKey(edge))
                        continue; // Edge already visited

                    visitedEdges.Add(edge, 0);
                    bool reversed = VisitedGraph.AreVerticesEqual(edge.Target, u);
                    OnExamineEdge(edge, reversed);
                    TVertex v = reversed ? edge.Source : edge.Target;
                    GraphColor vColor = VerticesColors[v];
                    switch (vColor)
                    {
                        case GraphColor.White:
                            OnTreeEdge(edge, reversed);
                            todoStack.Push(new SearchFrame(u, edges, frame.Depth + 1));
                            u = v;
                            edges = AdjacentEdgesFilter(VisitedGraph.AdjacentEdges(u)).GetEnumerator();
                            VerticesColors[u] = GraphColor.Gray;
                            OnDiscoverVertex(u);
                            break;

                        case GraphColor.Gray:
                            OnBackEdge(edge, reversed);
                            break;

                        case GraphColor.Black:
                            OnForwardOrCrossEdge(edge, reversed);
                            break;
                    }
                }

                VerticesColors[u] = GraphColor.Black;
                OnVertexFinished(u);
            }
        }
    }
}