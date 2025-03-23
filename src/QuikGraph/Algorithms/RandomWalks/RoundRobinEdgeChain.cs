using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.RandomWalks
{
    /// <summary>
    /// Round Robin chain implementation.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class RoundRobinEdgeChain<TVertex, TEdge> : IEdgeChain<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Dictionary<TVertex, int> _outEdgeIndices = new Dictionary<TVertex, int>();

        /// <inheritdoc />
        public bool TryGetSuccessor(IImplicitGraph<TVertex, TEdge> graph, TVertex vertex, out TEdge successor)
        {
            int? outDegree = graph.OutDegree(vertex);
            if (!(outDegree > 0))
            {
                successor = default(TEdge);
                return false;
            }

            if (!_outEdgeIndices.TryGetValue(vertex, out int index))
            {
                index = 0;
                _outEdgeIndices.Add(vertex, index);
            }

            TEdge edge = graph.OutEdge(vertex, index);
            _outEdgeIndices[vertex] = ++index % outDegree.Value;

            successor = edge;
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex vertex, out TEdge successor)
        {
            TEdge[] edgeArray = edges.ToArray();
            if (edgeArray.Length > 0)
            {
                if (!_outEdgeIndices.TryGetValue(vertex, out int index))
                {
                    index = 0;
                    _outEdgeIndices.Add(vertex, index);
                }

                TEdge edge = edgeArray[index];
                _outEdgeIndices[vertex] = ++index % edgeArray.Length;

                successor = edge;
                return true;
            }

            successor = default(TEdge);
            return false;
        }
    }
}