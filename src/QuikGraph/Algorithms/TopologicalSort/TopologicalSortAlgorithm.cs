using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <inheritdoc cref="CreateTopologicalSortAlgorithm"/>
    public static class TopologicalSortAlgorithm
    {
        /// <summary> Creates a new topological sort algorithm for the <paramref name="visitedGraph"/>. </summary>
        public static TopologicalSortAlgorithm<TVertex, TEdge> CreateTopologicalSortAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1) where TEdge : IEdge<TVertex>
            => new TopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph, capacity);

        /// <summary> Computes a new topological sort algorithm for the <paramref name="visitedGraph"/>. </summary>
        public static TopologicalSortAlgorithm<TVertex, TEdge> ComputeTopologicalSortAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1) where TEdge : IEdge<TVertex>
        {
            var ret = new TopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph, capacity);
            ret.Compute();
            return ret;
        }
    }

    /// <summary> Topological sort algorithm (can only be performed on an acyclic graph). </summary>
    public sealed class TopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        , IVertexTimeStamperAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        [NotNull, ItemNotNull]
        private readonly IList<TVertex> _sortedVertices;

        /// <summary>
        /// Initializes a new <see cref="TopologicalSortAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal TopologicalSortAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1)
            : base(visitedGraph)
        {
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
        }

        /// <summary>
        /// Sorted vertices.
        /// </summary>
        /// <remarks>It is <see langword="null"/> if the algorithm has not been run yet.</remarks>
        [ItemNotNull]
        public TVertex[] SortedVertices { get; private set; }

        private static void OnBackEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            throw new CyclicGraphException();
        }

        private void OnVertexFinished([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            _sortedVertices.Add(vertex);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            SortedVertices = null;
            _sortedVertices.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = VisitedGraph.CreateDepthFirstSearchAlgorithm(null, null, this);
                dfs.BackEdge += OnBackEdge;
                dfs.FinishVertex += OnVertexFinished;
                dfs.DiscoverVertex += DiscoverVertex;
                dfs.FinishVertex += FinishVertex;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.BackEdge -= OnBackEdge;
                    dfs.FinishVertex -= OnVertexFinished;
                    dfs.DiscoverVertex -= DiscoverVertex;
                    dfs.FinishVertex -= FinishVertex;

                    SortedVertices = _sortedVertices.Reverse().ToArray();
                }
            }
        }

        #endregion

        #region IVertexTimeStamperAlgorithm<TVertex>

        /// <inheritdoc />
        public event VertexAction<TVertex> DiscoverVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        #endregion
    }
}