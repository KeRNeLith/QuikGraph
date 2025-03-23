using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.RandomWalks
{
    /// <summary>
    /// Markov chain with weight.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class WeightedMarkovEdgeChain<TVertex, TEdge> : WeightedMarkovEdgeChainBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="WeightedMarkovEdgeChainBase{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Map that contains edge weights.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public WeightedMarkovEdgeChain([NotNull] IDictionary<TEdge, double> edgeWeights)
            : base(edgeWeights)
        {
        }

        /// <inheritdoc />
        public override bool TryGetSuccessor(IImplicitGraph<TVertex, TEdge> graph, TVertex vertex, out TEdge successor)
        {
            bool? empty = graph.IsOutEdgesEmpty(vertex);
            if (empty == false)
            {
                // Compute out-edge su
                double outWeight = GetOutWeight(graph, vertex);
                // Scale and get next edge
                double random = Rand.NextDouble() * outWeight;
                return TryGetSuccessor(graph, vertex, random, out successor);
            }

            successor = default(TEdge);
            return empty ?? false;
        }

        /// <inheritdoc />
        public override bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex vertex, out TEdge successor)
        {
            TEdge[] edgeArray = edges.ToArray();
            // Compute out-edge su
            double outWeight = GetWeights(edgeArray);
            // Scale and get next edge
            double random = Rand.NextDouble() * outWeight;
            return TryGetSuccessor(edgeArray, random, out successor);
        }
    }
}