using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.RandomWalks
{
    /// <summary>
    /// Base class for Markov chain with weight.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public abstract class WeightedMarkovEdgeChainBase<TVertex, TEdge> : MarkovEdgeChainBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="WeightedMarkovEdgeChainBase{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Map that contains edge weights.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        protected WeightedMarkovEdgeChainBase([NotNull] IDictionary<TEdge, double> edgeWeights)
        {
            Weights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
        }

        /// <summary>
        /// Map of edge weights.
        /// </summary>
        [NotNull]
        public IDictionary<TEdge, double> Weights { get; }

        /// <summary>
        /// Gets the weight of the given <paramref name="vertex"/> out edges.
        /// </summary>
        /// <param name="graph">Graph to consider.</param>
        /// <param name="vertex">Vertex to get out weight.</param>
        /// <returns>Out weight.</returns>
        protected double GetOutWeight([NotNull] IImplicitGraph<TVertex, TEdge> graph, [NotNull] TVertex vertex)
        {
            IEnumerable<TEdge> edges = graph.OutEdges(vertex);
            return GetWeights(edges);
        }

        /// <summary>
        /// Gets the weight corresponding to all given <paramref name="edges"/>.
        /// </summary>
        /// <param name="edges">Edges to get total weight.</param>
        /// <returns>Edges weight.</returns>
        protected double GetWeights([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            return edges.Sum(edge => Weights[edge]);
        }

        /// <summary>
        /// Tries to get the successor of the given <paramref name="vertex"/> in the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The graph to search in.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="position">The position.</param>
        /// <param name="successor">Found successor, otherwise <see langword="null"/>.</param>
        /// <returns>True if a successor was found, false otherwise.</returns>
        protected bool TryGetSuccessor([NotNull] IImplicitGraph<TVertex, TEdge> graph, [NotNull] TVertex vertex, double position, out TEdge successor)
        {
            Debug.Assert(graph != null);
            Debug.Assert(vertex != null);

            IEnumerable<TEdge> edges = graph.OutEdges(vertex);
            return TryGetSuccessor(edges, position, out successor);
        }

        /// <summary>
        /// Tries to get the successor at the given <paramref name="position"/> in the given set of <paramref name="edges"/>.
        /// </summary>
        /// <param name="edges">Edge set in which searching.</param>
        /// <param name="position">The position.</param>
        /// <param name="successor">Found successor, otherwise <see langword="null"/>.</param>
        /// <returns>True if a successor was found, false otherwise.</returns>
        protected bool TryGetSuccessor([NotNull, ItemNotNull] IEnumerable<TEdge> edges, double position, out TEdge successor)
        {
            Debug.Assert(edges != null);

            double pos = 0;
            foreach (TEdge edge in edges)
            {
                double nextPos = pos + Weights[edge];
                if (position >= pos && position <= nextPos)
                {
                    successor = edge;
                    return true;
                }

                pos = nextPos;
            }

            successor = default(TEdge);
            return false;
        }
    }
}