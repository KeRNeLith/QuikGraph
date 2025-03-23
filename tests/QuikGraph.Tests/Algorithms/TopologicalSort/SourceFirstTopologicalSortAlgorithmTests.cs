using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.TopologicalSort;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary> Tests for <see cref="SourceFirstTopologicalSortAlgorithm{TVertex,TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class SourceFirstTopologicalSortAlgorithmTests
    {

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public static void RunSourceFirstTopologicalSortAndCheck<TVertex, TEdge>([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.ComputeSourceFirstTopologicalSort();

            Assert.IsNotNull(algorithm.SortedVertices);
            Assert.AreEqual(graph.VertexCount, algorithm.SortedVertices.Length);
            Assert.IsNotNull(algorithm.InDegrees);
            Assert.AreEqual(graph.VertexCount, algorithm.InDegrees.Count);
        }

        [Test]
        public void Constructor()
        {
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm(-10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm(0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm(10);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                SourceFirstTopologicalSortAlgorithm<TVertex, TEdge> algo,
                IVertexAndEdgeListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.SortedVertices);
                CollectionAssert.IsEmpty(algo.InDegrees);
            }

            #endregion

            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateSourceFirstTopologicalSortAlgorithm());
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
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

            var algorithm = graph.ComputeSourceFirstTopologicalSort();

            CollectionAssert.AreEqual(
                new[] { 1, 7, 4, 2, 5, 8, 3, 6 },
                algorithm.SortedVertices);
        }

        [Test]
        public void SimpleGraphOneToAnother()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 4)
            );

            var algorithm = graph.ComputeSourceFirstTopologicalSort();

            CollectionAssert.AreEqual(
                new[] { 0, 1, 2, 3, 4 },
                algorithm.SortedVertices);
        }

        [Test]
        public void ForestGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 4),

                Edge.Create(5, 6)
            );

            var algorithm = graph.ComputeSourceFirstTopologicalSort();

            CollectionAssert.AreEqual(
                new[] { 0, 5, 1, 6, 2, 3, 4 },
                algorithm.SortedVertices);
        }

        [Test]
        public void GraphWithSelfEdge_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(2, 2),
                Edge.Create(3, 4)
            );

            var algorithm = graph.CreateSourceFirstTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(algorithm.Compute);
        }

        [Test]
        public void SourceFirstTopologicalSort_DCT8()
        {
            var graph = TestGraphFactory.LoadGraph(GetGraphFilePath("DCT8.graphml"));
            RunSourceFirstTopologicalSortAndCheck(graph);
        }

        [Test]
        public void SourceFirstTopologicalSort_Throws()
        {
            var cyclicGraph = new AdjacencyGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4),
                Edge.Create(3, 1)
            );

            var algorithm = cyclicGraph.CreateSourceFirstTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(algorithm.Compute);
        }
    }
}