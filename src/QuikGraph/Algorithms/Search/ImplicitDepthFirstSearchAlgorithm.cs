using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="CreateImplicitDepthFirstSearchAlgorithm"/>
    public static class ImplicitDepthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="ImplicitDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static ImplicitDepthFirstSearchAlgorithm<TVertex, TEdge>
            CreateImplicitDepthFirstSearchAlgorithm<TVertex, TEdge>(
            [NotNull] this IIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new ImplicitDepthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, host);
    }

    /// <summary> Depth first search algorithm for implicit directed graphs. </summary>
    public sealed class ImplicitDepthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IIncidenceGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IVertexTimeStamperAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Creates a new <see cref="ImplicitDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal ImplicitDepthFirstSearchAlgorithm(
            [NotNull] IIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host= null)
            : base(visitedGraph, host)
        {
        }

        /// <summary>
        /// Stores vertices associated to their colors (treatment state).
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, GraphColor> VerticesColors { get; } = new Dictionary<TVertex, GraphColor>();

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

        /// <summary>
        /// Fired on the root vertex once before the start of the search from it.
        /// </summary>
        public event VertexAction<TVertex> StartVertex;

        private void OnStartVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            StartVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Invoked when a vertex is encountered for the first time. 
        /// </summary>
        public event VertexAction<TVertex> DiscoverVertex;

        private void OnDiscoverVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DiscoverVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Invoked on every out-edge of each vertex after it is discovered. 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        private void OnExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired on each edge as it becomes a member of the edges that form
        /// the search tree. If you wish to record predecessors, do so at this
        /// event point.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired on the back edges in the graph.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BackEdge;

        private void OnBackEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            BackEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired on forward or cross edges in the graph.
        /// (In an undirected graph this method is never called.)
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

        private void OnForwardOrCrossEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ForwardOrCrossEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired on a vertex after all of its out edges have been added to
        /// the search tree and all of the adjacent vertices have been
        /// discovered (but before their out-edges have been examined).
        /// </summary>
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
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();
            OnStartVertex(root);
            Visit(root, 0);
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

                if (!VerticesColors.TryGetValue(v, out GraphColor vColor))
                {
                    OnTreeEdge(edge);
                    Visit(v, depth + 1);
                }
                else
                {
                    if (vColor == GraphColor.Gray)
                    {
                        OnBackEdge(edge);
                    }
                    else
                    {
                        OnForwardOrCrossEdge(edge);
                    }
                }
            }

            VerticesColors[u] = GraphColor.Black;
            OnVertexFinished(u);
        }
    }
}