using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.TopologicalSort;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;


namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="UndirectedFirstTopologicalSortAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedFirstTopologicalSortAlgorithmTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void RunUndirectedFirstTopologicalSortAndCheck<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
            => RunUndirectedFirstTopologicalSortAndCheck(graph, true);

        public static void RunUndirectedFirstTopologicalSortAndCheck<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph, bool allowCycles)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(allowCycles);
            algorithm.Compute();

            Assert.IsNotNull(algorithm.SortedVertices);
            Assert.AreEqual(graph.VertexCount, algorithm.SortedVertices.Length);
            Assert.IsNotNull(algorithm.Degrees);
            Assert.AreEqual(graph.VertexCount, algorithm.Degrees.Count);
        }

        [Test]
        public void Constructor()
        {
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(true);
            AssertAlgorithmProperties(algorithm, graph, true);

            algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(capacity: 0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(capacity: 10);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge> algo,
                IUndirectedGraph<TVertex, TEdge> g,
                bool allowCycles = false)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.SortedVertices);
                CollectionAssert.IsEmpty(algo.Degrees);
                Assert.AreEqual(allowCycles, algo.AllowCyclicGraph);
            }

            #endregion

            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateUndirectedFirstTopologicalSortAlgorithm());
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(7, 5),
                Edge.Create(7, 8)
            );

            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing, here the order
            // is more vertices ordered from lower to higher adjacent vertices
            CollectionAssert.AreEqual(
                new[] { 1, 8, 3, 7, 2, 6, 4, 5 },
                algorithm.SortedVertices);
        }

        [Test]
        public void SimpleGraphOneToAnother()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(3, 4)
            );

            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing, here the order
            // is more vertices ordered from lower to higher adjacent vertices
            CollectionAssert.AreEqual(
                new[] { 0, 4, 2, 3, 1 },
                algorithm.SortedVertices);
        }

        [Test]
        public void ForestGraph()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(3, 4),

                Edge.Create(5, 6)
            );

            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing, here the order
            // is more vertices ordered from lower to higher adjacent vertices
            CollectionAssert.AreEqual(
                new[] { 0, 6, 5, 4, 2, 3, 1 },
                algorithm.SortedVertices);
        }

        [Test]
        public void GraphWithSelfEdge()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(2, 2),
                Edge.Create(3, 4)
            );

            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());

            algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(true);
            algorithm.Compute();

            // Order in undirected graph is some strange thing, here the order
            // is more vertices ordered from lower to higher adjacent vertices
            CollectionAssert.AreEqual(
                new[] { 0, 4, 1, 3, 2 },
                algorithm.SortedVertices);
        }

        [Test]
        public void UndirectedFirstTopologicalSort_DCT8()
        {
            var graph = TestGraphFactory.LoadUndirectedGraph(GetGraphFilePath("DCT8.graphml"));
            RunUndirectedFirstTopologicalSortAndCheck(graph, true);
        }

        [Test]
        public void UndirectedFirstTopologicalSort_Throws()
        {
            var cyclicGraph = new UndirectedGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4),
                Edge.Create(3, 1)
            );

            var algorithm = cyclicGraph.CreateUndirectedFirstTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());

            algorithm = cyclicGraph.CreateUndirectedFirstTopologicalSortAlgorithm(true);
            Assert.DoesNotThrow(() => algorithm.Compute());
        }
    }
}