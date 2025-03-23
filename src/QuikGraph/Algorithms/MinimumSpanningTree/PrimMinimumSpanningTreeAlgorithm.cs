using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.MinimumSpanningTree
{
    /// <inheritdoc cref="CreatePrimMinimumSpanningTreeAlgorithm"/>
    public static class PrimMinimumSpanningTreeAlgorithm
    {
        /// <summary> Creates a new <see cref="PrimMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static PrimMinimumSpanningTreeAlgorithm<TVertex, TEdge>
            CreatePrimMinimumSpanningTreeAlgorithm<TVertex, TEdge> (
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new PrimMinimumSpanningTreeAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, host);
    }

    /// <summary> Prim minimum spanning tree algorithm implementation. </summary>
    public sealed class PrimMinimumSpanningTreeAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
            , IMinimumSpanningTreeAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        [NotNull]
        private readonly Func<TEdge, double> _edgeWeights;

        /// <summary> Creates a new <see cref="PrimMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        internal PrimMinimumSpanningTreeAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            _edgeWeights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
        }

        /// <summary>
        /// Fired when an edge is going to be analyzed.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        private void OnExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(edge);
        }

        #region ITreeBuilderAlgorithm<TVertex,TEdge>

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        #endregion

        #region AlgorithmBase<TGraph>

        private Dictionary<TVertex, HashSet<TEdge>> _verticesEdges;
        private HashSet<TVertex> _visitedVertices;
        private ForestDisjointSet<TVertex> _sets;

        private HashSet<TEdge> _edges;
        private BinaryQueue<TEdge, double> _queue;

        private void InitializeVerticesToEdges()
        {
            _verticesEdges = new Dictionary<TVertex, HashSet<TEdge>>();
            _visitedVertices = new HashSet<TVertex>();
            _sets = new ForestDisjointSet<TVertex>(VisitedGraph.VertexCount);

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                if (_visitedVertices.Count == 0)
                {
                    _visitedVertices.Add(vertex);
                }

                _sets.MakeSet(vertex);
                _verticesEdges.Add(vertex, new HashSet<TEdge>());
            }

            foreach (TEdge edge in VisitedGraph.Edges)
            {
                _verticesEdges[edge.Source].Add(edge);
                _verticesEdges[edge.Target].Add(edge);
            }
        }

        private void InitializeQueue()
        {
            _edges = new HashSet<TEdge>();
            _queue = new BinaryQueue<TEdge, double>(_edgeWeights);
            TVertex lastVertex = _visitedVertices.First();
            foreach (TEdge edge in _verticesEdges[lastVertex])
            {
                if (!_edges.Contains(edge))
                {
                    _edges.Add(edge);
                    _queue.Enqueue(edge);
                }
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            InitializeVerticesToEdges();

            ThrowIfCancellationRequested();

            InitializeQueue();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            ThrowIfCancellationRequested();

            while (_edges.Count > 0 && _visitedVertices.Count < VisitedGraph.VertexCount)
            {
                TEdge minEdge = _queue.Dequeue();
                OnExamineEdge(minEdge);

                if (!_sets.AreInSameSet(minEdge.Source, minEdge.Target))
                {
                    OnTreeEdge(minEdge);
                    _sets.Union(minEdge.Source, minEdge.Target);

                    TVertex lastVertex;
                    if (_visitedVertices.Contains(minEdge.Source))
                    {
                        lastVertex = minEdge.Target;
                        _visitedVertices.Add(minEdge.Target);
                    }
                    else
                    {
                        lastVertex = minEdge.Source;
                        _visitedVertices.Add(minEdge.Source);
                    }

                    EnqueueEdgesFrom(lastVertex);
                }
            }
        }

        private void EnqueueEdgesFrom([NotNull] TVertex vertex)
        {
            foreach (TEdge edge in _verticesEdges[vertex])
            {
                if (_edges.Contains(edge))
                    continue;

                _edges.Add(edge);
                _queue.Enqueue(edge);
            }
        }

        #endregion
    }
}