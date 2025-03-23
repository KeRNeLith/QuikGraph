using System;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;
using QuikGraph.Utils;

namespace QuikGraph.Algorithms.VertexCover
{
    /// <inheritdoc cref="CreateMinimumVertexCoverApproximationAlgorithm"/>
    public static class MinimumVertexCoverApproximationAlgorithm
    {
        /// <summary> Creates a new <see cref="MinimumVertexCoverApproximationAlgorithm{TVertex,TEdge}"/>. </summary>
        public static MinimumVertexCoverApproximationAlgorithm<TVertex, TEdge>
            CreateMinimumVertexCoverApproximationAlgorithm<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [CanBeNull] Func<int, int> rng = null) where TEdge : IEdge<TVertex>
            => new MinimumVertexCoverApproximationAlgorithm<TVertex, TEdge>(graph, rng);

        /// <summary> Computes a new <see cref="MinimumVertexCoverApproximationAlgorithm{TVertex,TEdge}"/>. </summary>
        public static MinimumVertexCoverApproximationAlgorithm<int, IEdge<int>>
            ComputeMinimumVertexCoverApproximationAlgorithm(this UndirectedGraph<int, IEdge<int>> graph
            , Func<int, int> rng)
        {
            var algorithm = graph.CreateMinimumVertexCoverApproximationAlgorithm(rng);
            algorithm.Compute();
            return algorithm;
        }
    }

    /// <summary> Minimum vertices cover approximation algorithm for undirected graphs. </summary>
    /// <remarks>
    /// This is a modified version (by Batov Nikita)
    /// of the original Mihalis Yannakakis and Fanica Gavril algorithm.
    /// </remarks>
    public sealed class MinimumVertexCoverApproximationAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly VertexList<TVertex> _coverSet = new VertexList<TVertex>();

        [NotNull]
        private readonly Func<int, int> _rng;

        /// <summary> Creates a new <see cref="MinimumVertexCoverApproximationAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="graph">Graph to compute the cover.</param>
        /// <param name="rng">Random number generator.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        internal MinimumVertexCoverApproximationAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph,
            [CanBeNull] Func<int, int> rng = null)
            : base(graph)
        {
            _rng = rng ?? new CryptoRandom().Next;
        }

        /// <summary>
        /// Set of covering vertices.
        /// </summary>
        public VertexList<TVertex> CoverSet =>
            State == ComputationState.Finished
                ? _coverSet
                : null;

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            var graph = new UndirectedGraph<TVertex, TEdge>(
                VisitedGraph.AllowParallelEdges,
                VisitedGraph.EdgeEqualityComparer);
            graph.AddVerticesAndEdgeRange(VisitedGraph.Edges);

            while (!graph.IsEdgesEmpty)
            {
                TEdge[] graphEdges = graph.Edges.ToArray();

                // Get a random edge
                int randomEdgeIndex = _rng(graphEdges.Length - 1);
                TEdge randomEdge = graphEdges[randomEdgeIndex];

                TVertex source = randomEdge.Source;
                TVertex target = randomEdge.Target;

                if (graph.AdjacentDegree(randomEdge.Source) > 1 && !_coverSet.Contains(source))
                {
                    _coverSet.Add(source);
                }

                if (graph.AdjacentDegree(randomEdge.Target) > 1 && !_coverSet.Contains(target))
                {
                    _coverSet.Add(target);
                }

                if (graph.AdjacentDegree(randomEdge.Target) == 1
                    && graph.AdjacentDegree(randomEdge.Source) == 1)
                {
                    if (!_coverSet.Contains(source))
                    {
                        _coverSet.Add(source);
                    }

                    graph.RemoveEdges(
                        graph.AdjacentEdges(source).ToArray());
                }
                else
                {
                    TEdge[] edgesToRemove = graph.AdjacentEdges(target)
                        .Concat(graph.AdjacentEdges(source))
                        .ToArray();

                    graph.RemoveEdges(edgesToRemove);
                }
            }
        }

        #endregion
    }
}