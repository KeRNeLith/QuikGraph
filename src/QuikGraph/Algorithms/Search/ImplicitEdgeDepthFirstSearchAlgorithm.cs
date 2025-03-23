using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="CreateImplicitEdgeDepthFirstSearchAlgorithm"/>
    public static class ImplicitEdgeDepthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="ImplicitEdgeDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static ImplicitEdgeDepthFirstSearchAlgorithm<TVertex
            , TEdge> CreateImplicitEdgeDepthFirstSearchAlgorithm<TVertex, TEdge>(
            [NotNull] this IIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new ImplicitEdgeDepthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, host);
    }

    /// <summary> edge depth first search algorithm for implicit directed graphs.</summary>
    /// <remarks>
    /// This is a variant of the classic DFS where the edges are color marked.
    /// </remarks>
    public sealed class ImplicitEdgeDepthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IIncidenceGraph<TVertex, TEdge>>
        , IEdgeColorizerAlgorithm<TVertex, TEdge>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Creates a new <see cref="ImplicitEdgeDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal ImplicitEdgeDepthFirstSearchAlgorithm(
            [NotNull] IIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
        }

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
        /// Fired when an edge starts to be treated.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> StartEdge;

        private void OnStartEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            StartEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is discovered.
        /// </summary>
        public event EdgeEdgeAction<TVertex, TEdge> DiscoverTreeEdge;

        private void OnDiscoverTreeEdge([NotNull] TEdge edge, [NotNull] TEdge targetEdge)
        {
            Debug.Assert(edge != null);
            Debug.Assert(targetEdge != null);

            DiscoverTreeEdge?.Invoke(edge, targetEdge);
        }

        /// <summary>
        /// Invoked on each edge as it becomes a member of the edges that form
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
        /// Fired on an edge after all of its out edges have been added to
        /// the search tree and all of the adjacent vertices have been
        /// discovered (but before their out-edges have been examined).
        /// </summary>
        public event EdgeAction<TVertex, TEdge> FinishEdge;

        private void OnFinishEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            FinishEdge?.Invoke(edge);
        }

        #endregion

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            EdgesColors.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();

            // Start with root vertex
            OnStartVertex(root);

            // Process each out edge of the root one
            foreach (TEdge edge in VisitedGraph.OutEdges(root))
            {
                ThrowIfCancellationRequested();

                if (!EdgesColors.ContainsKey(edge))
                {
                    OnStartEdge(edge);
                    Visit(edge, 0);
                }
            }
        }

        #endregion

        #region IEdgeColorizerAlgorithm<TVertex,TEdge>

        /// <inheritdoc />
        public IDictionary<TEdge, GraphColor> EdgesColors { get; } = new Dictionary<TEdge, GraphColor>();

        #endregion

        private void Visit([NotNull] TEdge startingEdge, int depth)
        {
            Debug.Assert(startingEdge != null);
            Debug.Assert(depth >= 0);

            if (depth > MaxDepth)
                return;

            // Mark edge as gray
            EdgesColors[startingEdge] = GraphColor.Gray;
            // Add edge to the search tree
            OnTreeEdge(startingEdge);

            // Iterate over out-edges
            foreach (TEdge edge in VisitedGraph.OutEdges(startingEdge.Target))
            {
                ThrowIfCancellationRequested();

                // Check edge is not explored yet,
                // if not, explore it
                if (!EdgesColors.TryGetValue(edge, out GraphColor color))
                {
                    OnDiscoverTreeEdge(startingEdge, edge);
                    Visit(edge, depth + 1);
                }
                else
                {
                    if (color == GraphColor.Gray)
                    {
                        OnBackEdge(edge);
                    }
                    else
                    {
                        OnForwardOrCrossEdge(edge);
                    }
                }
            }

            // All out-edges have been explored
            EdgesColors[startingEdge] = GraphColor.Black;
            OnFinishEdge(startingEdge);
        }
    }
}