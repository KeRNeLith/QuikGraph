using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ConnectedComponents;


namespace QuikGraph.Tests.Algorithms.ConnectedComponents
{
    /// <summary>
    /// Tests for <see cref="IncrementalConnectedComponentsAlgorithm{TVertex,TEdge}"/>
    /// </summary>
    [TestFixture]
    internal sealed class IncrementalConnectedComponentsAlgorithmTests
    {
        static AdjacencyGraph<int, IEdge<int>> graph = new(), nullGraph = null;
        [Test]
        public void Constructor()
        {
            var algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();
            algorithm.AssertAlgorithmState(graph);

            algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm(null);
            algorithm.AssertAlgorithmState(graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateIncrementalConnectedComponentsAlgorithm());

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateIncrementalConnectedComponentsAlgorithm(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void InvalidUse()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();

            Assert.Throws<InvalidOperationException>(() => { int _ = algorithm.ComponentCount; });
            Assert.Throws<InvalidOperationException>(() => algorithm.GetComponents());
        }

        [Test]
        public void IncrementalConnectedComponent()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 0, 1, 2, 3 );

            var algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();
            algorithm.Compute();

            Assert.AreEqual(4, algorithm.ComponentCount);
            Assert.AreEqual(4, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 1,
                    [2] = 2,
                    [3] = 3
                },
                algorithm.GetComponents().Value);

            graph.AddEdge(Edge.Create(0, 1));
            Assert.AreEqual(3, algorithm.ComponentCount);
            Assert.AreEqual(3, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 0,
                    [2] = 1,
                    [3] = 2
                },
                algorithm.GetComponents().Value);

            graph.AddEdge(Edge.Create(2, 3));
            Assert.AreEqual(2, algorithm.ComponentCount);
            Assert.AreEqual(2, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 0,
                    [2] = 1,
                    [3] = 1
                },
                algorithm.GetComponents().Value);

            graph.AddEdge(Edge.Create(1, 3));
            Assert.AreEqual(1, algorithm.ComponentCount);
            Assert.AreEqual(1, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 0,
                    [2] = 0,
                    [3] = 0
                },
                algorithm.GetComponents().Value);

            graph.AddVerticesAndEdge(Edge.Create(4, 5));
            Assert.AreEqual(2, algorithm.ComponentCount);
            Assert.AreEqual(2, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 1,
                    [5] = 1
                },
                algorithm.GetComponents().Value);

            graph.AddVertex(6);
            Assert.AreEqual(3, algorithm.ComponentCount);
            Assert.AreEqual(3, algorithm.GetComponents().Key);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [0] = 0,
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 1,
                    [5] = 1,
                    [6] = 2
                },
                algorithm.GetComponents().Value);
        }

        [Test]
        public void IncrementalConnectedComponent_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var edge13 = Edge.Create(1, 3);
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                edge13,
                Edge.Create(4, 5)
            );
            graph.AddVertex(6);

            using (graph.IncrementalConnectedComponents(out Func<KeyValuePair<int, IDictionary<int, int>>> _))
            {
                Assert.Throws<InvalidOperationException>(() => graph.RemoveVertex(6));
                Assert.Throws<InvalidOperationException>(() => graph.RemoveEdge(edge13));
            }
        }

        [Test]
        public void IncrementalConnectedComponentMultiRun()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();
            Assert.DoesNotThrow(() =>
            {
                algorithm.Compute();
                algorithm.Compute();
            });
        }

        [Test]
        public void Dispose()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();
            Assert.DoesNotThrow(() => algorithm.Dispose());

            algorithm = graph.CreateIncrementalConnectedComponentsAlgorithm();
            algorithm.Compute();
            Assert.DoesNotThrow(() => algorithm.Dispose());
        }
    }
}