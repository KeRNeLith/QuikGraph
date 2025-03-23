using System;
using NUnit.Framework;
using QuikGraph.Algorithms.Condensation;
using QuikGraph.Tests.Structures;

namespace QuikGraph.Tests.Algorithms.Condensation
{
    /// <summary>
    /// Tests for <see cref="CondensedEdge{TVertex,TEdge,TGraph}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class CondensedEdgeTests : EdgeTestsBase
    {
        [Test]
        public void TestConstruction()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            var graph2 = new AdjacencyGraph<int, IEdge<int>>();

            // Value type
            CheckEdge(
                new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph2),
                graph1,
                graph2);
            CheckEdge(
                new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph2, graph1),
                graph2,
                graph1);
            CheckEdge(
                new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph1),
                graph1,
                graph1);

            // Reference type
            var graph3 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph4 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            CheckEdge(
                new CondensedEdge<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(graph3, graph4),
                graph3,
                graph4);
            CheckEdge(
                new CondensedEdge<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(graph4, graph3),
                graph4,
                graph3);
            CheckEdge(
                new CondensedEdge<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(graph3, graph3),
                graph3,
                graph3);
        }

        [Test]
        public void Construction_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(null, graph));
            Assert.Throws<ArgumentNullException>(() => new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph, null));
            Assert.Throws<ArgumentNullException>(() => new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TestEdges()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            var graph2 = new AdjacencyGraph<int, IEdge<int>>();

            var edge = new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph2);
            CollectionAssert.IsEmpty(edge.Edges);

            var subEdge = Edge.Create(1, 2);
            edge.Edges.Add(subEdge);
            CollectionAssert.AreEqual(new[] { subEdge }, edge.Edges);

            edge.Edges.RemoveAt(0);
            CollectionAssert.IsEmpty(edge.Edges);
        }

        [Test]
        public void TestEquals()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            var graph2 = new AdjacencyGraph<int, IEdge<int>>();

            var edge1 = new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph2);
            var edge2 = new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph2);
            var edge3 = new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph2, graph1);
            var edge4 = new CondensedEdge<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(graph1, graph2);

            var subEdge = Edge.Create(1, 2);
            edge4.Edges.Add(subEdge);

            Assert.AreEqual(edge1, edge1);
            Assert.AreNotEqual(edge1, edge2);
            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge1, edge4);

            Assert.AreNotEqual(null, edge1);
        }
    }
}