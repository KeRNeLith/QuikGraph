using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <summary>
    /// Undirected topological sort algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge> : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly BinaryQueue<TVertex, int> _heap;

        [NotNull, ItemNotNull]
        private readonly List<TVertex> _sortedVertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndirectedFirstTopologicalSortAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public UndirectedFirstTopologicalSortAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1)
            : base(visitedGraph)
        {
            _heap = new BinaryQueue<TVertex, int>(vertex => Degrees[vertex]);
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
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
        public bool AllowCyclicGraph { get; set; }

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
            if (!AllowCyclicGraph && VisitedGraph.Edges.Any(edge => edge.IsSelfEdge()))
                throw new NonAcyclicGraphException();

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Degrees.Add(vertex, VisitedGraph.AdjacentDegree(vertex));
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
                    throw new NonAcyclicGraphException();

                _sortedVertices.Add(vertex);
                OnVertexAdded(vertex);

                // Update the count of its adjacent vertices
                UpdateAdjacentDegree(vertex);
            }

            SortedVertices = _sortedVertices.ToArray();

            #region Local function

            void UpdateAdjacentDegree(TVertex vertex)
            {
                foreach (TEdge edge in VisitedGraph.AdjacentEdges(vertex).Where(e => !e.IsSelfEdge()))
                {
                    TVertex other = edge.GetOtherVertex(vertex);
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