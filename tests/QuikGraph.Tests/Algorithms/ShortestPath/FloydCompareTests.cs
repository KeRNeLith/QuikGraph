using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for comparing <see cref="FloydWarshallAllShortestPathAlgorithm{TVertex,TEdge}"/> and other shortest path finder algorithms.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.CISkip)]
    internal sealed class FloydCompareTests : FloydWarshallTestsBase
    {
        #region Test helpers

        private static void CheckPath<TVertex, TEdge>([NotNull] TVertex source, [NotNull] TVertex target, [NotNull, ItemNotNull] TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            Assert.AreEqual(source, edges[0].Source);
            for (int i = 0; i < edges.Length - 1; ++i)
                Assert.AreEqual(edges[i].Target, edges[i + 1].Source);
            Assert.AreEqual(target, edges[edges.Length - 1].Target);
        }

        /// <summary> Compares the <paramref name="shortestPathAlgorithmFactory"/> Results with the <see cref="FloydWarshallAllShortestPathAlgorithm{TVertex,TEdge}"/> </summary>
        private static void CompareAlgorithms<TVertex, TEdge, TGraph>(
            [NotNull] AdjacencyGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> getDistances,
            [NotNull, InstantHandle] Func<AdjacencyGraph<TVertex, TEdge>, Func<TEdge, double>, ShortestPathAlgorithmBase<TVertex, TEdge, TGraph>> shortestPathAlgorithmFactory)
            where TEdge : IEdge<TVertex>
            where TGraph : IVertexSet<TVertex>
        {
            // Compute all paths
            var algorithm = graph.CreateFloydWarshallAllShortestPathAlgorithm(getDistances);
            algorithm.Compute();

            TVertex[] vertices = graph.Vertices.ToArray();
            foreach (TVertex source in vertices)
            {
                var otherAlgorithm = shortestPathAlgorithmFactory(graph, getDistances);
                var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
                using (predecessors.Attach(otherAlgorithm))
                    otherAlgorithm.Compute(source);

                Func<TVertex, List<TEdge>> otherPaths = predecessors.GetPath;
                foreach (TVertex target in vertices)
                {
                    if (source.Equals(target))
                        continue;

                    IEnumerable<TEdge> floydPath = algorithm.GetPath(source, target);
                    List<TEdge> otherPath = otherPaths(target);
                    Assert.AreEqual(floydPath == null, otherPath == null);

                    if (floydPath == null)
                    {
                        continue;
                    }

                    TEdge[] floydEdges = floydPath.ToArray();
                    CheckPath(source, target, floydEdges);

                    TEdge[] otherEdges = otherPath.ToArray();
                    CheckPath(source, target, otherEdges);

                    // All distances are usually 1 in this test,
                    // so it should at least be the same number
                    if (otherEdges.Length != floydEdges.Length)
                    {
                        Assert.Fail("Path do not have the same length.");
                    }

                    // Check path length are the same
                    double floydLength = floydEdges.Sum(getDistances);
                    double otherLength = otherEdges.Sum(getDistances);
                    if (Math.Abs(floydLength - otherLength) > double.Epsilon)
                    {
                        Assert.Fail("Path do not have the same length.");
                    }
                }
            }
        }

        #endregion

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void FloydVsBellmannGraphML(AdjacencyGraph<string, IEdge<string>> graph)
        {
            CompareAlgorithms(graph, _ => 1.0, (g, d)
                => g.CreateBellmanFordShortestPathAlgorithm(d));
        }

        [Test]
        public void FloydVsDijkstra()
        {
            var distances = new Dictionary<IEdge<char>, double>();
            var graph = CreateGraph(distances);
            CompareAlgorithms(graph, e => distances[e], (g, d)
                => g.CreateDijkstraShortestPathAlgorithm(d));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void FloydVsDijkstraGraphML(AdjacencyGraph<string, IEdge<string>> graph)
        {
            CompareAlgorithms(graph, _ => 1, (g, d)
                => g.CreateDijkstraShortestPathAlgorithm(d));
        }
    }
}