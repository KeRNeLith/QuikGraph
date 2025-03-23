using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.ConnectedComponents;

namespace QuikGraph.Tests.Algorithms.ConnectedComponents
{
    /// <summary>
    /// Tests for <see cref="WeaklyConnectedComponentsAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class WeaklyConnectedComponentsAlgorithmTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void RunWeaklyConnectedComponentsAndCheck<TVertex, TEdge>([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.ComputeWeaklyConnectedComponents();

            Assert.AreEqual(graph.VertexCount, algorithm.ComponentIndex.Count);
            if (graph.VertexCount == 0)
            {
                Assert.IsTrue(algorithm.ComponentCount == 0);
                return;
            }

            Assert.Positive(algorithm.ComponentCount);
            Assert.LessOrEqual(algorithm.ComponentCount, graph.VertexCount);
            foreach (KeyValuePair<TVertex, int> pair in algorithm.ComponentIndex)
            {
                Assert.GreaterOrEqual(pair.Value, 0);
                Assert.IsTrue(pair.Value < algorithm.ComponentCount, $"{pair.Value} < {algorithm.ComponentCount}");
            }

            foreach (TVertex vertex in graph.Vertices)
            {
                foreach (TEdge edge in graph.OutEdges(vertex))
                {
                    Assert.AreEqual(algorithm.ComponentIndex[edge.Source], algorithm.ComponentIndex[edge.Target]);
                }
            }
        }

        [Test]
        public void TestConstructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();
            var algorithm = graph.ComputeWeaklyConnectedComponents();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.ComputeWeaklyConnectedComponents(components);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.ComputeWeaklyConnectedComponents(components, null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                WeaklyConnectedComponentsAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g, QuikGraph.Algorithms.ComputationState.Finished);
                Assert.AreEqual(0, algo.ComponentCount);
                CollectionAssert.IsEmpty(algo.ComponentIndex);
                CollectionAssert.IsEmpty(algo.Graphs);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var components = new Dictionary<int, int>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.ComputeWeaklyConnectedComponents());

            _ = graph.ComputeWeaklyConnectedComponents(null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.ComputeWeaklyConnectedComponents(components));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.ComputeWeaklyConnectedComponents(null));

            _ = graph.ComputeWeaklyConnectedComponents(null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.ComputeWeaklyConnectedComponents(components, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.ComputeWeaklyConnectedComponents(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void OneComponent()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 3)
            );

            var algorithm = graph.ComputeWeaklyConnectedComponents();

            Assert.AreEqual(1, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(1, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3, 4 },
                algorithm.Graphs[0].Vertices);
        }

        [Test]
        public void TwoComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 3),

                Edge.Create(5, 6),
                Edge.Create(5, 7),
                Edge.Create(7, 6)
            );

            var algorithm = graph.ComputeWeaklyConnectedComponents();

            Assert.AreEqual(2, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0,
                    [5] = 1,
                    [6] = 1,
                    [7] = 1
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(2, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3, 4  },
                algorithm.Graphs[0].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 5, 6, 7 },
                algorithm.Graphs[1].Vertices);
        }

        [Test]
        public void MultipleComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 3),

                Edge.Create(5, 6),
                Edge.Create(5, 7),
                Edge.Create(7, 6),

                Edge.Create(8, 9)
            );
            graph.AddVertex(10);

            var algorithm = graph.ComputeWeaklyConnectedComponents();

            Assert.AreEqual(4, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0,
                    [5] = 1,
                    [6] = 1,
                    [7] = 1,
                    [8] = 2,
                    [9] = 2,
                    [10] = 3
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(4, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3, 4 },
                algorithm.Graphs[0].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 5, 6, 7 },
                algorithm.Graphs[1].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 8, 9 },
                algorithm.Graphs[2].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 10 },
                algorithm.Graphs[3].Vertices);
        }
    }
}