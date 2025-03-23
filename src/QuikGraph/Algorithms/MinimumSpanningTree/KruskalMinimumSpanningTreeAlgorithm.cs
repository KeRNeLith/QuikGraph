using System;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.MinimumSpanningTree
{
    /// <inheritdoc cref="CreateKruskalMinimumSpanningTreeAlgorithm"/>
    public static class KruskalMinimumSpanningTreeAlgorithm
    {
        /// <summary> Creates a new <see cref="KruskalMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>
            CreateKruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, host);
    }

    /// <summary> Kruskal minimum spanning tree algorithm implementation. </summary>
    public sealed class KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        , IMinimumSpanningTreeAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        [NotNull]
        private readonly Func<TEdge, double> _edgeWeights;

        /// <summary> Creates a new <see cref="KruskalMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public KruskalMinimumSpanningTreeAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            _edgeWeights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
        }

        /// <summary> Fired when an edge is going to be analyzed. </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        /// <inheritdoc cref="ExamineEdge"/>
        private void OnExamineEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(edge);
        }

        #region ITreeBuilderAlgorithm<TVertex,TEdge>

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        /// <inheritdoc cref="TreeEdge"/>
        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        #endregion

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            var sets = new ForestDisjointSet<TVertex>(VisitedGraph.VertexCount);
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                sets.MakeSet(vertex);
            }

            ThrowIfCancellationRequested();

            var queue = new BinaryQueue<TEdge, double>(_edgeWeights);
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                queue.Enqueue(edge);
            }

            ThrowIfCancellationRequested();

            while (queue.Count > 0)
            {
                TEdge edge = queue.Dequeue();
                OnExamineEdge(edge);

                if (!sets.AreInSameSet(edge.Source, edge.Target))
                {
                    OnTreeEdge(edge);
                    sets.Union(edge.Source, edge.Target);
                }
            }
        }

        #endregion
    }
}