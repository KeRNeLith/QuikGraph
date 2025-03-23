using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Search
{
    /// <inheritdoc cref="CreateEdgeDepthFirstSearchAlgorithm"/>
    public static class EdgeDepthFirstSearchAlgorithm
    {
        /// <summary> Creates a new <see cref="EdgeDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static EdgeDepthFirstSearchAlgorithm<TVertex, TEdge> CreateEdgeDepthFirstSearchAlgorithm<TVertex, TEdge>(
            [NotNull] this IEdgeListAndIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TEdge, GraphColor> edgesColors = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new EdgeDepthFirstSearchAlgorithm<TVertex, TEdge>(visitedGraph, edgesColors, host);
    }

    /// <summary> Edge depth first search algorithm for directed graphs. </summary>
    /// <remarks>
    /// This is a variant of the classic DFS algorithm where the
    /// edges are color marked instead of the vertices.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class EdgeDepthFirstSearchAlgorithm<TVertex, TEdge> 
        : RootedAlgorithmBase<TVertex, IEdgeListAndIncidenceGraph<TVertex, TEdge>>
        , IEdgeColorizerAlgorithm<TVertex, TEdge>
        , IEdgePredecessorRecorderAlgorithm<TVertex, TEdge>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Creates a new <see cref="EdgeDepthFirstSearchAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgesColors">Edges associated to their colors (treatment states).</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgesColors"/> is <see langword="null"/>.</exception>
        internal EdgeDepthFirstSearchAlgorithm(
            [NotNull] IEdgeListAndIncidenceGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TEdge, GraphColor> edgesColors = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            EdgesColors = edgesColors ?? new Dictionary<TEdge, GraphColor>();
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

        /// <summary>
        /// Fired when an edge is initialized.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> InitializeEdge;

        private void OnEdgeInitialized([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            InitializeEdge?.Invoke(edge);
        }

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

        /// <inheritdoc />
        public event EdgeEdgeAction<TVertex, TEdge> DiscoverTreeEdge;

        private void OnDiscoverTreeEdge([NotNull] TEdge edge, [NotNull] TEdge targetEdge)
        {
            Debug.Assert(edge != null);
            Debug.Assert(targetEdge != null);

            DiscoverTreeEdge?.Invoke(edge, targetEdge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a white edge.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a gray edge.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BackEdge;

        private void OnBackEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            BackEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a black edge.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

        private void OnForwardOrCrossEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ForwardOrCrossEdge?.Invoke(edge);
        }

        /// <inheritdoc />
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
            // Put all edges to white
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                ThrowIfCancellationRequested();

                EdgesColors[edge] = GraphColor.White;
                OnEdgeInitialized(edge);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            ThrowIfCancellationRequested();

            // Start with root vertex
            if (TryGetRootVertex(out TVertex root))
            {
                RootShouldBeInGraph(root);

                OnStartVertex(root);

                // Process each out edge of the root one
                VisitAllWhiteEdgesSet(VisitedGraph.OutEdges(root));

                // Process the rest of the graph edges
                if (ProcessAllComponents)
                {
                    VisitAllWhiteEdges();
                }
            }
            else
            {
                VisitAllWhiteEdges();
            }

            #region Local functions

            void VisitAllWhiteEdgesSet(IEnumerable<TEdge> edges)
            {
                foreach (TEdge edge in edges)
                {
                    ThrowIfCancellationRequested();

                    if (EdgesColors[edge] == GraphColor.White)
                    {
                        OnStartEdge(edge);
                        Visit(edge, 0);
                    }
                }
            }

            void VisitAllWhiteEdges()
            {
                VisitAllWhiteEdgesSet(VisitedGraph.Edges);
            }

            #endregion
        }

        #endregion

        #region IEdgeColorizerAlgorithm<TVertex,TEdge>

        /// <inheritdoc />
        public IDictionary<TEdge, GraphColor> EdgesColors { get; }

        #endregion

        private void Visit([NotNull] TEdge rootEdge, int depth)
        {
            Debug.Assert(rootEdge != null);

            // Mark edge as gray
            EdgesColors[rootEdge] = GraphColor.Gray;
            // Add edge to the search tree
            OnTreeEdge(rootEdge);

            // Iterate over out-edges
            foreach (TEdge edge in VisitedGraph.OutEdges(rootEdge.Target))
            {
                ThrowIfCancellationRequested();

                // Check edge is not explored yet,
                // If not, explore it
                if (EdgesColors[edge] == GraphColor.White)
                {
                    OnDiscoverTreeEdge(rootEdge, edge);

                    int newDepth = depth + 1;
                    if (newDepth <= MaxDepth)
                    {
                        Visit(edge, newDepth);
                    }
                    else
                    {
                        EdgesColors[edge] = GraphColor.Black;
                        OnFinishEdge(edge);
                    }
                }
                else if (EdgesColors[edge] == GraphColor.Gray)
                {
                    // Edge is being explored
                    OnBackEdge(edge);
                }
                else
                {
                    // Edge is black
                    OnForwardOrCrossEdge(edge);
                }
            }

            // All out-edges have been explored
            EdgesColors[rootEdge] = GraphColor.Black;
            OnFinishEdge(rootEdge);
        }
    }
}