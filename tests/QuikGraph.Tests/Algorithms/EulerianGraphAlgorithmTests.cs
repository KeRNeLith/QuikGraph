using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.TestHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary> Tests for <see cref="IsEulerianGraphAlgorithm{TVertex,TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class EulerianGraphAlgorithmTests
    {
        private static void AssertIsEulerian([NotNull] IUndirectedGraph<int, IUndirectedEdge<int>> graph,
            bool expectedEulerian)
        {
            IsEulerianGraphAlgorithm<int, IUndirectedEdge<int>> algorithm = graph.CreateEulerianGraphAlgorithm();
            Assert.AreEqual(expectedEulerian, algorithm.IsEulerian());
            Assert.AreEqual(expectedEulerian, graph.IsEulerian());
        }

        [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, new[] { 2, 1, 3 }, new[] { 2, 3, 1 }, new[] { 3, 2, 1 }, new[] { 3, 1, 2 })]
        public static void TestPermutations([NotNull, ItemNotNull] IList<int> vertices
            , params IList<int>[] items)
        {
            CollectionAssert.AreEqual(vertices.GetPermutations(), items);
        }
        [Test]
        public void IsEulerianEmpty()
        {
            var graph = CreateUndirectedGraph();
            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerianOneVertex()
        {
            var graph = CreateUndirectedGraph();
            graph.AddVertex(42);

            AssertIsEulerian(graph, true);
        }

        [Test]
        public void IsEulerianOneComponentTrue()
        {
            // Eulerian
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(1, 3));

            AssertIsEulerian(graph, true);
        }

        [Test]
        public void IsEulerianOneComponentFalse()
        {
            // Not Eulerian
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(3, 4),
                Edge.CreateUndirected(1, 4),
                Edge.CreateUndirected(1, 3));

            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerianManyComponentsTrue()
        {
            // Eulerian
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(1, 3));

            graph.AddVertex(4);
            graph.AddVertex(5);

            AssertIsEulerian(graph, true);
        }

        [Test]
        public void IsEulerianManyComponentsFalse()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(1, 3),
                Edge.CreateUndirected(4, 5),
                Edge.CreateUndirected(5, 6),
                Edge.CreateUndirected(4, 6));

            graph.AddVertex(7);

            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerianOneVertexWithLoop()
        {
            var graph = CreateUndirectedGraph(Edge.CreateUndirected(1, 1));

            AssertIsEulerian(graph, true);
        }

        [Test]
        public void IsEulerianOneVertexWithTwoLoops()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 1),
                Edge.CreateUndirected(1, 1));

            AssertIsEulerian(graph, true);
        }

        [Test]
        public void IsEulerianTwoVertices()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 2));

            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerianTwoVerticesWithLoops()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 1),
                Edge.CreateUndirected(2, 2)
            );

            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerianTwoVerticesOneEdge()
        {
            var graph = CreateUndirectedGraph(Edge.CreateUndirected(1, 2));

            AssertIsEulerian(graph, false);
        }

        [Test]
        public void IsEulerian_Throws()
        {
            IUndirectedGraph<int, UndirectedEdge<int>> graph = null;
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => graph.IsEulerian());

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateEulerianGraphAlgorithm());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}