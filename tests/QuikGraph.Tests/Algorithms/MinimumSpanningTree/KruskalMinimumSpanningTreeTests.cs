using System;
using NUnit.Framework;
using QuikGraph.Algorithms.MinimumSpanningTree;


namespace QuikGraph.Tests.Algorithms.MinimumSpanningTree
{
    /// <summary>
    /// Tests for <see cref="KruskalMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class KruskalMinimumSpanningTreeTests : MinimumSpanningTreeTestsBase
    {
        [Test]
        public void Constructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateKruskalMinimumSpanningTreeAlgorithm(_ => 1.0);
            algorithm.AssertAlgorithmState(graph);

            algorithm = graph.CreateKruskalMinimumSpanningTreeAlgorithm(_ => 1.0, null);
            algorithm.AssertAlgorithmState(graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateKruskalMinimumSpanningTreeAlgorithm(_ => 1.0));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateKruskalMinimumSpanningTreeAlgorithm(null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateKruskalMinimumSpanningTreeAlgorithm(null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateKruskalMinimumSpanningTreeAlgorithm(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateKruskalMinimumSpanningTreeAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateKruskalMinimumSpanningTreeAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Kruskal()
        {
            // Kruskal 10, 50, 100, 200, 300, 400
            UndirectedGraph<string, TaggedEdge<string, double>> graph = GetUndirectedCompleteGraph(10);
            KruskalSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(50);
            KruskalSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(100);
            KruskalSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(200);
            KruskalSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(300);
            KruskalSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(400);
            KruskalSpanningTree(graph, x => x.Tag);
        }
    }
}