using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.TopologicalSort;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;


namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="UndirectedTopologicalSortAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedTopologicalSortAlgorithmTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void RunUndirectedTopologicalSortAndCheck<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.AllowCyclicGraph = true;

            algorithm.Compute();

            Assert.IsNotNull(algorithm.SortedVertices);
            Assert.AreEqual(graph.VertexCount, algorithm.SortedVertices.Length);
        }

        [Test]
        public void Constructor()
        {
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.AllowCyclicGraph = true;
            AssertAlgorithmProperties(algorithm, graph, true);

            algorithm = graph.CreateUndirectedTopologicalSortAlgorithm(0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateUndirectedTopologicalSortAlgorithm(10);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                UndirectedTopologicalSortAlgorithm<TVertex, TEdge> algo,
                IUndirectedGraph<TVertex, TEdge> g,
                bool allowCycles = false)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.SortedVertices);
                Assert.AreEqual(allowCycles, algo.AllowCyclicGraph);
            }

            #endregion

            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedTopologicalSortAlgorithm());
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

            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing,
            // here the order is more vertices ordered by depth
            CollectionAssert.AreEqual(
                new[] { 1, 2, 4, 5, 7, 8, 6, 3 },
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

            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing,
            // here the order is more vertices ordered by depth
            CollectionAssert.AreEqual(
                new[] { 0, 1, 3, 4, 2 },
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

            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.Compute();

            // Order in undirected graph is some strange thing,
            // here the order is more vertices ordered by depth
            CollectionAssert.AreEqual(
                new[] { 5, 6, 0, 1, 3, 4, 2 },
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

            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());

            algorithm = graph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.AllowCyclicGraph = true;
            algorithm.Compute();

            // Order in undirected graph is some strange thing,
            // here the order is more vertices ordered by depth
            CollectionAssert.AreEqual(
                new[] { 0, 1, 2, 3, 4 },
                algorithm.SortedVertices);
        }

        [Test]
        public void UndirectedTopologicalSort_DCT8()
        {
            var graph = TestGraphFactory.LoadUndirectedGraph(GetGraphFilePath("DCT8.graphml"));
            RunUndirectedTopologicalSortAndCheck(graph);
        }

        [Test]
        public void UndirectedTopologicalSort_Throws()
        {
            var cyclicGraph = new UndirectedGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4),
                Edge.Create(3, 1)
            );

            var algorithm = cyclicGraph.CreateUndirectedTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());

            algorithm = cyclicGraph.CreateUndirectedTopologicalSortAlgorithm();
            algorithm.AllowCyclicGraph = true;
            Assert.DoesNotThrow(() => algorithm.Compute());
        }
    }
}