using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.ConnectedComponents;


namespace QuikGraph.Tests.Algorithms.ConnectedComponents
{
    /// <summary>
    /// Tests for <see cref="ConnectedComponentsAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ConnectedComponentsAlgorithmTests
    {
        #region Test helpers

        private static void RunConnectedComponentsAndCheck<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new ConnectedComponentsAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();

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
                foreach (TEdge edge in graph.AdjacentEdges(vertex))
                {
                    Assert.AreEqual(algorithm.ComponentIndex[edge.Source], algorithm.ComponentIndex[edge.Target]);
                }
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();
            var algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph, components);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph, components, null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                ConnectedComponentsAlgorithm<TVertex, TEdge> algo,
                IUndirectedGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.AreEqual(0, algo.ComponentCount);
                CollectionAssert.IsEmpty(algo.ComponentIndex);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new ConnectedComponentsAlgorithm<int, IEdge<int>>(null));

            _ = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph, null);
            Assert.Throws<ArgumentNullException>(
                () => new ConnectedComponentsAlgorithm<int, IEdge<int>>(null, components));
            Assert.Throws<ArgumentNullException>(
                () => new ConnectedComponentsAlgorithm<int, IEdge<int>>(null, null));

            _ = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph, null, null);
            Assert.Throws<ArgumentNullException>(
                () => new ConnectedComponentsAlgorithm<int, IEdge<int>>(null, components, null));
            Assert.Throws<ArgumentNullException>(
                () => new ConnectedComponentsAlgorithm<int, IEdge<int>>(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void OneComponent()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 3)
            );

            var algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

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
        }

        [Test]
        public void TwoComponents()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
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

            var algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

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
        }

        [Test]
        public void MultipleComponents()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
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

            var algorithm = new ConnectedComponentsAlgorithm<int, IEdge<int>>(graph);
            algorithm.Compute();

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
        }

        [Category(TestCategories.LongRunning)]
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_SlowTests), new object[] { 10 })]
        public void ConnectedComponents(UndirectedGraph<string, IEdge<string>> graph)
        {
            while (graph.EdgeCount > 0)
            {
                RunConnectedComponentsAndCheck(graph);
                graph.RemoveEdge(graph.Edges.First());
            }
        }
    }
}