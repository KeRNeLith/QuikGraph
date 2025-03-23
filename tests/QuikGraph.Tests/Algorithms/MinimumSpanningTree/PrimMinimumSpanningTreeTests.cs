using System;
using NUnit.Framework;
using QuikGraph.Algorithms.MinimumSpanningTree;


namespace QuikGraph.Tests.Algorithms.MinimumSpanningTree
{
    /// <summary>
    /// Tests for <see cref="PrimMinimumSpanningTreeAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class PrimMinimumSpanningTreeTests : MinimumSpanningTreeTestsBase
    {
        [Test]
        public void Constructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreatePrimMinimumSpanningTreeAlgorithm(_ => 1.0);
            algorithm.AssertAlgorithmState(graph);

            algorithm = graph.CreatePrimMinimumSpanningTreeAlgorithm(_ => 1.0, null);
            algorithm.AssertAlgorithmState(graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreatePrimMinimumSpanningTreeAlgorithm(_ => 1.0));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreatePrimMinimumSpanningTreeAlgorithm(null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreatePrimMinimumSpanningTreeAlgorithm(null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreatePrimMinimumSpanningTreeAlgorithm(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreatePrimMinimumSpanningTreeAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreatePrimMinimumSpanningTreeAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Prim()
        {
            // Prim 10, 50, 100, 200, 300, 400
            UndirectedGraph<string, TaggedEdge<string, double>> graph = GetUndirectedCompleteGraph(10);
            PrimSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(50);
            PrimSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(100);
            PrimSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(200);
            PrimSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(300);
            PrimSpanningTree(graph, x => x.Tag);

            graph = GetUndirectedCompleteGraph(400);
            PrimSpanningTree(graph, x => x.Tag);
        }
    }
}