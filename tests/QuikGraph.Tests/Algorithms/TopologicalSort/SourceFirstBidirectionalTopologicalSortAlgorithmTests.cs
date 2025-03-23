using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.TopologicalSort;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="SourceFirstBidirectionalTopologicalSortAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class SourceFirstBidirectionalTopologicalSortAlgorithmTests
    {
        #region Test helpers

        private static void RunSourceFirstTopologicalSortAndCheck<TVertex, TEdge>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> graph,
            TopologicalSortDirection direction)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(graph, direction);
            algorithm.Compute();

            Assert.IsNotNull(algorithm.SortedVertices);
            Assert.AreEqual(graph.VertexCount, algorithm.SortedVertices.Length);
            Assert.IsNotNull(algorithm.InDegrees);
            Assert.AreEqual(graph.VertexCount, algorithm.InDegrees.Count);
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(capacity: -10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(capacity: 0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(capacity: 10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Forward);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Forward, -10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Forward, 0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Forward, 10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward, -10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward, 0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward, 10);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge> algo,
                IBidirectionalGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.SortedVertices);
                CollectionAssert.IsEmpty(algo.InDegrees);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(
                () => new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(null, TopologicalSortDirection.Forward));
            Assert.Throws<ArgumentNullException>(
                () => new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(null, TopologicalSortDirection.Backward));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 6),
                Edge.Create(2, 8),
                Edge.Create(4, 2),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(7, 5),
                Edge.Create(7, 8)
            );

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 1, 7, 4, 2, 5, 8, 3, 6 },
                algorithm.SortedVertices);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 3, 6, 8, 5, 2, 7, 1, 4 },
                algorithm.SortedVertices);
        }

        [Test]
        public void SimpleGraphOneToAnother()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 4)
            );

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 0, 1, 2, 3, 4 },
                algorithm.SortedVertices);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 4, 3, 2, 1, 0 },
                algorithm.SortedVertices);
        }

        [Test]
        public void ForestGraph()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 4),

                Edge.Create(5, 6)
            );

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 0, 5, 1, 6, 2, 3, 4 },
                algorithm.SortedVertices);

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward);
            algorithm.Compute();

            CollectionAssert.AreEqual(
                new[] { 4, 6, 3, 5, 2, 1, 0 },
                algorithm.SortedVertices);
        }

        [Test]
        public void GraphWithSelfEdge_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(2, 2),
                Edge.Create(3, 4)
            );

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(graph);
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());

            algorithm = graph.CreateSourceFirstBidirectionalTopologicalSortAlgorithm(TopologicalSortDirection.Backward);
            Assert.Throws<CyclicGraphException>(() => algorithm.Compute());
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public void SourceFirstBidirectionalTopologicalSort(BidirectionalGraph<string, IEdge<string>> graph)
        {
            RunSourceFirstTopologicalSortAndCheck(graph, TopologicalSortDirection.Forward);
            RunSourceFirstTopologicalSortAndCheck(graph, TopologicalSortDirection.Backward);
        }

        [Test]
        public void SourceFirstBidirectionalTopologicalSort_DCT8()
        {
            var graph = TestGraphFactory.LoadBidirectionalGraph(GetGraphFilePath("DCT8.graphml"));
            RunSourceFirstTopologicalSortAndCheck(graph, TopologicalSortDirection.Forward);
            RunSourceFirstTopologicalSortAndCheck(graph, TopologicalSortDirection.Backward);
        }

        [Test]
        public void SourceFirstBidirectionalTopologicalSort_Throws()
        {
            var cyclicGraph = new BidirectionalGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4),
                Edge.Create(3, 1)
            );

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, IEdge<int>>(cyclicGraph);
            Assert.Throws<CyclicGraphException>(algorithm.Compute);
        }
    }
}