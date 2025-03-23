using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <inheritdoc cref="CreateUndirectedTopologicalSortAlgorithm"/>
    public static class UndirectedTopologicalSortAlgorithm
    {
        /// <summary>Creates a new <see cref="UndirectedTopologicalSortAlgorithm{TVertex,TEdge}"/> class.</summary>
        public static UndirectedTopologicalSortAlgorithm<TVertex, TEdge>
            CreateUndirectedTopologicalSortAlgorithm<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1) where TEdge : IEdge<TVertex>
            => new UndirectedTopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph, capacity);

    }
    /// <summary> Undirected topological sort algorithm. </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class UndirectedTopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull, ItemNotNull]
        private readonly IList<TVertex> _sortedVertices;

        /// <summary>Initializes a new <see cref="UndirectedTopologicalSortAlgorithm{TVertex,TEdge}"/> class.</summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal UndirectedTopologicalSortAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1)
            : base(visitedGraph)
        {
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
        }

        /// <summary>
        /// Sorted vertices.
        /// </summary>
        [ItemNotNull]
        public TVertex[] SortedVertices { get; private set; }

        /// <summary>
        /// Gets or sets the flag that indicates if cyclic graph are supported or not.
        /// </summary>
        public bool AllowCyclicGraph { get; set; }

        private void BackEdge([NotNull] object sender, [NotNull] UndirectedEdgeEventArgs<TVertex, TEdge> args)
        {
            if (!AllowCyclicGraph)
                throw new CyclicGraphException();
        }

        private void OnVertexFinished([NotNull] TVertex vertex)
        {
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
            UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = new UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(
                    VisitedGraph,
                    host: this);
                dfs.BackEdge += BackEdge;
                dfs.FinishVertex += OnVertexFinished;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.BackEdge -= BackEdge;
                    dfs.FinishVertex -= OnVertexFinished;

                    SortedVertices = _sortedVertices.Reverse().ToArray();
                }
            }
        }

        #endregion
    }
}