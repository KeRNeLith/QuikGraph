using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <inheritdoc cref="CreateSourceFirstBidirectionalTopologicalSortAlgorithm"/>
    public static class SourceFirstBidirectionalTopologicalSortAlgorithm
    {
        /// <summary> Creates a new <see cref="SourceFirstBidirectionalTopologicalSortAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>
            CreateSourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            TopologicalSortDirection direction = TopologicalSortDirection.Forward,
            int capacity = -1) where TEdge : IEdge<TVertex>
            => new SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph,direction, capacity);

    }

    /// <summary> Topological sort algorithm (can be performed on an acyclic bidirectional graph). </summary>
    public sealed class SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly BinaryQueue<TVertex, int> _heap;

        private readonly TopologicalSortDirection _direction;

        [NotNull, ItemNotNull]
        private readonly IList<TVertex> _sortedVertices;

        /// <summary> Creates a new <see cref="SourceFirstBidirectionalTopologicalSortAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="direction">Sort direction.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public SourceFirstBidirectionalTopologicalSortAlgorithm(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            TopologicalSortDirection direction = TopologicalSortDirection.Forward,
            int capacity = -1) : base(visitedGraph)
        {
            _direction = direction;
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

                TVertex successor = _direction == TopologicalSortDirection.Forward
                    ? edge.Target
                    : edge.Source;

                ++InDegrees[successor];
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

                // Update the count of its successor vertices
                IEnumerable<TEdge> successorEdges = _direction == TopologicalSortDirection.Forward
                    ? VisitedGraph.OutEdges(vertex)
                    : VisitedGraph.InEdges(vertex);

                foreach (TEdge edge in successorEdges)
                {
                    TVertex successor = _direction == TopologicalSortDirection.Forward
                        ? edge.Target
                        : edge.Source;

                    --InDegrees[successor];

                    Debug.Assert(InDegrees[successor] >= 0);

                    _heap.Update(successor);
                }
            }

            SortedVertices = _sortedVertices.ToArray();
        }

        #endregion
    }
}