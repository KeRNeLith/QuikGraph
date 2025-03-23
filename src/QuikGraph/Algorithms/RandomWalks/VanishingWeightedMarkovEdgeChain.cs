using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.RandomWalks
{
    /// <summary>
    /// Markov chain with weight vanishing based on a factor.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class VanishingWeightedMarkovEdgeChain<TVertex, TEdge> : WeightedMarkovEdgeChainBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="VanishingWeightedMarkovEdgeChain{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Map that contains edge weights.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public VanishingWeightedMarkovEdgeChain([NotNull] IDictionary<TEdge, double> edgeWeights)
            : this(edgeWeights, 0.2)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="VanishingWeightedMarkovEdgeChain{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Map that contains edge weights.</param>
        /// <param name="factor">Vanishing factor.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public VanishingWeightedMarkovEdgeChain([NotNull] IDictionary<TEdge, double> edgeWeights, double factor)
            : base(edgeWeights)
        {
            Factor = factor;
        }

        /// <summary>
        /// Vanishing factor.
        /// </summary>
        public double Factor { get; set; }

        /// <inheritdoc />
        public override bool TryGetSuccessor(IImplicitGraph<TVertex, TEdge> graph, TVertex vertex, out TEdge successor)
        {
            bool? empty = graph.IsOutEdgesEmpty(vertex);
            if (empty == false)
            {
                // Get out weight
                double outWeight = GetOutWeight(graph, vertex);

                // Get successor
                if (TryGetSuccessor(graph, vertex, Rand.NextDouble() * outWeight, out successor))
                {
                    // Update probabilities
                    Weights[successor] *= Factor;

                    // Normalize
                    foreach (TEdge edge in graph.OutEdges(vertex))
                    {
                        Weights[edge] /= outWeight;
                    }

                    return true;
                }
            }

            successor = default(TEdge);
            return empty ?? false;
        }

        /// <inheritdoc />
        public override bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex vertex, out TEdge successor)
        {
            // Get out weight
            TEdge[] edgeArray = edges.ToArray();
            double outWeight = GetWeights(edgeArray);

            // Get successor
            if (TryGetSuccessor(edgeArray, Rand.NextDouble() * outWeight, out successor))
            {
                // Update probabilities
                Weights[successor] *= Factor;

                // Normalize
                foreach (TEdge edge in edgeArray)
                {
                    Weights[edge] /= outWeight;
                }

                return true;
            }

            successor = default(TEdge);
            return false;
        }
    }
}