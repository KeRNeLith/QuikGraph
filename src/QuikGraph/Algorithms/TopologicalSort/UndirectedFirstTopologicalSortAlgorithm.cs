using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <inheritdoc cref="CreateUndirectedFirstTopologicalSortAlgorithm"/>
    public static class UndirectedFirstTopologicalSortAlgorithm
    {
        /// <summary> Creates a new <see cref="UndirectedFirstTopologicalSortAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge>
            CreateUndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge> (
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph, bool allowCyclicGraph = false,
            int capacity = -1) where TEdge : IEdge<TVertex>
            => new UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge>(visitedGraph, allowCyclicGraph, capacity);
    }
    /// <summary> Undirected topological sort algorithm. </summary>
    public sealed class UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly BinaryQueue<TVertex, int> _heap;

        [NotNull, ItemNotNull]
        private readonly IList<TVertex> _sortedVertices;

        /// <summary> Creates a new <see cref="UndirectedFirstTopologicalSortAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="allowCyclicGraph"></param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal UndirectedFirstTopologicalSortAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph, bool allowCyclicGraph = false, int capacity = -1)
            : base(visitedGraph)
        {
            _heap = new BinaryQueue<TVertex, int>(vertex => Degrees[vertex]);
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
            AllowCyclicGraph = allowCyclicGraph;
        }

        /// <summary>
        /// Sorted vertices.
        /// </summary>
        [ItemNotNull]
        public TVertex[] SortedVertices { get; private set; }

        /// <summary>
        /// Vertices degrees.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, int> Degrees { get; } = new Dictionary<TVertex, int>();

        /// <summary>
        /// Gets or sets the flag that indicates if cyclic graph are supported or not.
        /// </summary>
        public bool AllowCyclicGraph { get; }

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
            if (!AllowCyclicGraph && VisitedGraph.Edges.Any(edge => edge.IsSelfEdge(VisitedGraph.AreVerticesEqual)))
                throw new CyclicGraphException();

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Degrees.Add(vertex, VisitedGraph.AdjacentDegree(vertex) ?? 0);
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
            Degrees.Clear();

            InitializeInDegrees();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            while (_heap.Count != 0)
            {
                ThrowIfCancellationRequested();

                TVertex vertex = _heap.Dequeue();
                int degree = Degrees[vertex];
                // 0 => isolated vertex
                // 1 => single adjacent edge
                if (degree != 0 && degree != 1 && !AllowCyclicGraph)
                    throw new CyclicGraphException();

                _sortedVertices.Add(vertex);
                OnVertexAdded(vertex);

                // Update the count of its adjacent vertices
                UpdateAdjacentDegree(vertex);
            }

            SortedVertices = _sortedVertices.ToArray();

            #region Local function

            void UpdateAdjacentDegree(TVertex vertex)
            {
                foreach (TEdge edge in VisitedGraph.AdjacentEdges(vertex).Where(e => !e.IsSelfEdge(VisitedGraph.AreVerticesEqual)))
                {
                    TVertex other = edge.GetOtherVertex(vertex, VisitedGraph.AreVerticesEqual);
                    --Degrees[other];

                    if (Degrees[other] < 0 && !AllowCyclicGraph)
                        throw new InvalidOperationException("Degree is negative, and cannot be.");

                    if (_heap.Contains(other))
                    {
                        _heap.Update(other);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}