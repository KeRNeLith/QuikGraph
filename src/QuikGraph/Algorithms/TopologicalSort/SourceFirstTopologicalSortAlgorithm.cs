using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <inheritdoc cref="CreateSourceFirstTopologicalSortAlgorithm"/>
    public static class SourceFirstTopologicalSortAlgorithm
    {
        /// <summary> Creates a new <see cref="SourceFirstTopologicalSortAlgorithm{TVertex,TEdge}"/>. </summary>
        public static SourceFirstTopologicalSortAlgorithm<TVertex, TEdge>

            CreateSourceFirstTopologicalSortAlgorithm<TVertex, TEdge> (
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1) where TEdge : IEdge<TVertex>
            => new SourceFirstTopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph, capacity);

        /// <summary> Computes a new <see cref="SourceFirstTopologicalSortAlgorithm{TVertex,TEdge}"/>. </summary>
        public static SourceFirstTopologicalSortAlgorithm<TVertex, TEdge> ComputeSourceFirstTopologicalSort<TVertex, TEdge>(
            this IVertexAndEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm();
            algorithm.Compute();
            return algorithm;
        }

    }

    /// <summary> Topological sort algorithm for acyclic graphs. </summary>
    public sealed class SourceFirstTopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly BinaryQueue<TVertex, int> _heap;

        [NotNull, ItemNotNull]
        private readonly IList<TVertex> _sortedVertices;

        /// <summary> Creates a new <see cref="SourceFirstTopologicalSortAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal SourceFirstTopologicalSortAlgorithm(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1)
            : base(visitedGraph)
        {
            _heap = new BinaryQueue<TVertex, int>(vertex => InDegrees[vertex]);
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
        }

        /// <summary>
        /// Sorted vertices.
        /// </summary>
        [ItemNotNull]
        public TVertex[] SortedVertices { get; private set; }

        /// <summary>
        /// Vertices in-degrees.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, int> InDegrees { get; } = new Dictionary<TVertex, int>();

        /// <summary>
        /// Fired when a vertex is added to the set of sorted vertices.
        /// </summary>
        public event VertexAction<TVertex> VertexAdded;

        private void OnVertexAdded([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexAdded?.Invoke(vertex);
        }

        private void InitializeInDegrees()
        {
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                InDegrees.Add(vertex, 0);
            }

            foreach (TEdge edge in VisitedGraph.Edges)
            {
                if (edge.IsSelfEdge(VisitedGraph.AreVerticesEqual))
                    throw new CyclicGraphException();

                ++InDegrees[edge.Target];
            }

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                _heap.Enqueue(vertex);
            }
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            SortedVertices = null;
            _sortedVertices.Clear();
            InDegrees.Clear();

            InitializeInDegrees();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            while (_heap.Count != 0)
            {
                ThrowIfCancellationRequested();

                TVertex vertex = _heap.Dequeue();
                if (InDegrees[vertex] != 0)
                    throw new CyclicGraphException();

                _sortedVertices.Add(vertex);
                OnVertexAdded(vertex);

                // Update the count of its adjacent vertices
                foreach (TVertex target in VisitedGraph.OutEdges(vertex).Select(outEdge => outEdge.Target))
                {
                    --InDegrees[target];

                    Debug.Assert(InDegrees[target] >= 0);

                    _heap.Update(target);
                }
            }

            SortedVertices = _sortedVertices.ToArray();
        }

        #endregion
    }
}