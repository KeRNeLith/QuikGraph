using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.ShortestPath;

namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Base class for tests related to <see cref="FloydWarshallAllShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    internal class FloydWarshallTestsBase : RootedAlgorithmTestsBase
    {
        #region Test helpers

        [Pure]
        [NotNull]
        protected static AdjacencyGraph<char, IEdge<char>> CreateGraph([NotNull] Dictionary<IEdge<char>, double> distances)
        {
            var graph = new AdjacencyGraph<char, IEdge<char>>();

            const string vertices = "ABCDE";
            graph.AddVertexRange(vertices);
            AddEdge('A', 'C', 1);
            AddEdge('B', 'B', 2);
            AddEdge('B', 'D', 1);
            AddEdge('B', 'E', 2);
            AddEdge('C', 'B', 7);
            AddEdge('C', 'D', 3);
            AddEdge('D', 'E', 1);
            AddEdge('E', 'A', 1);
            AddEdge('E', 'B', 1);

            return graph;

            #region Local function

            void AddEdge(char source, char target, double weight)
            {
                var edge = Edge.Create(source, target);
                distances[edge] = weight;
                graph.AddEdge(edge);
            }

            #endregion
        }

        #endregion
    }
}