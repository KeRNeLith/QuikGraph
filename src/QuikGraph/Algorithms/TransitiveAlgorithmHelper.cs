#if !NET35
using System;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Helper for transitive algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    internal sealed class TransitiveAlgorithmHelper<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly IMutableBidirectionalGraph<TVertex, TEdge> _graph;

        internal TransitiveAlgorithmHelper([NotNull] BidirectionalGraph<TVertex, TEdge> initialGraph)
        {
            Debug.Assert(initialGraph != null);

            _graph = initialGraph;
        }

#if NET35
        internal delegate void Action<in T1, in T2, in T3, in T4, in T5>(
            T1 param1,
            T2 param2,
            T3 param3,
            T4 param4,
            T5 param5);
#endif

        /// <summary>
        /// Runs through the graph and calls <paramref name="action"/>
        /// for each couple of indirect ancestor vertex of a given vertex.
        /// </summary>
        public void InternalCompute([NotNull, InstantHandle]
            Action<IMutableBidirectionalGraph<TVertex, TEdge>, TVertex, TVertex, bool, TEdge> action)
        {
            // Iterate in topological order, track indirect ancestors and remove edges from them to the visited vertex
            var verticesAncestors = new Dictionary<TVertex, HashSet<TVertex>>();
            foreach (TVertex vertexId in _graph.TopologicalSort().ToArray()) // Making sure we do not mess enumerator or something
            {
                var vertexPredecessors = new List<TVertex>();
                var vertexAncestors = new HashSet<TVertex>();
                verticesAncestors[vertexId] = vertexAncestors;

                // Get indirect ancestors
                foreach (TVertex predecessor in _graph.InEdges(vertexId).Select(inEdge => inEdge.Source))
                {
                    vertexPredecessors.Add(predecessor);

                    // Add all the ancestors of the predecessors
                    vertexAncestors.UnionWith(verticesAncestors[predecessor]);
                }

                // Add indirect edges
                foreach (TVertex indirectAncestor in vertexAncestors)
                {
                    bool found = _graph.TryGetEdge(
                        indirectAncestor, 
                        vertexId, 
                        out TEdge foundIndirectEdge);

                    action(_graph, indirectAncestor, vertexId, found, foundIndirectEdge);
                }

                // Add predecessors to ancestors list
                vertexAncestors.UnionWith(vertexPredecessors);
            }
        }
    }
}