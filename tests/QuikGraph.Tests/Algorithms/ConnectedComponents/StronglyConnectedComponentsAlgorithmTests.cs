using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.ConnectedComponents;

namespace QuikGraph.Tests.Algorithms.ConnectedComponents
{
    /// <summary> Tests for <see cref="StronglyConnectedComponentsAlgorithm{TVertex,TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class StronglyConnectedComponentsAlgorithmTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void RunStronglyConnectedComponentsAndCheck<TVertex, TEdge>([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.ComputeStronglyConnectedComponents();

            Assert.AreEqual(graph.VertexCount, algorithm.ComponentIndex.Count);
            Assert.AreEqual(graph.VertexCount, algorithm.Roots.Count);
            Assert.AreEqual(graph.VertexCount, algorithm.DiscoverTimes.Count);
            if (graph.VertexCount == 0)
            {
                Assert.AreEqual(0, algorithm.Steps);
                Assert.IsTrue(algorithm.ComponentCount == 0);
                return;
            }

            Assert.Positive(algorithm.ComponentCount);
            Assert.LessOrEqual(algorithm.ComponentCount, graph.VertexCount);
            foreach (TVertex vertex in algorithm.VisitedGraph.Vertices)
            {
                Assert.IsTrue(algorithm.ComponentIndex.ContainsKey(vertex));
                Assert.IsTrue(algorithm.DiscoverTimes.ContainsKey(vertex));
            }

            Assert.Positive(algorithm.Steps);
            AssertStepsProperties();
            foreach (KeyValuePair<TVertex, int> pair in algorithm.ComponentIndex)
            {
                Assert.GreaterOrEqual(pair.Value, 0);
                Assert.IsTrue(pair.Value < algorithm.ComponentCount, $"{pair.Value} < {algorithm.ComponentCount}");
            }

            foreach (KeyValuePair<TVertex, int> time in algorithm.DiscoverTimes)
            {
                Assert.IsNotNull(time.Key);
            }

            foreach (TVertex vertex in graph.Vertices)
            {
                Assert.GreaterOrEqual(algorithm.ComponentIndex[vertex], 0);
            }

            #region Local function

            void AssertStepsProperties()
            {
                Assert.GreaterOrEqual(algorithm.Steps, 0);
                Assert.AreEqual(algorithm.Steps, algorithm.VerticesPerStep.Count);
                Assert.AreEqual(algorithm.Steps, algorithm.ComponentsPerStep.Count);
            }

            #endregion
        }

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();
            var algorithm = graph.CreateStronglyConnectedComponentsAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateStronglyConnectedComponentsAlgorithm(components);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateStronglyConnectedComponentsAlgorithm(components, null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                StronglyConnectedComponentsAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.AreEqual(0, algo.ComponentCount);
                CollectionAssert.IsEmpty(algo.ComponentIndex);
                CollectionAssert.IsEmpty(algo.Graphs);
                CollectionAssert.IsEmpty(algo.Roots);

                Assert.AreEqual(0, algo.Steps);
                Assert.IsNull(algo.VerticesPerStep);
                Assert.IsNull(algo.ComponentsPerStep);
                CollectionAssert.IsEmpty(algo.DiscoverTimes);
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
                () => nullGraph.CreateStronglyConnectedComponentsAlgorithm());

            _ = graph.CreateStronglyConnectedComponentsAlgorithm(null);

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateStronglyConnectedComponentsAlgorithm(components));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateStronglyConnectedComponentsAlgorithm(null));

            _ = graph.CreateStronglyConnectedComponentsAlgorithm(null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateStronglyConnectedComponentsAlgorithm(components, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateStronglyConnectedComponentsAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TestOneComponent()
        {
            var cyclicGraph = new AdjacencyGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(3, 1)
            );

            var algorithm = cyclicGraph.ComputeStronglyConnectedComponents();
            algorithm.Compute();

            Assert.AreEqual(1, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(1, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                algorithm.Graphs[0].Vertices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Components of a 3-Component Graph:
        ///
        /// 1 --> 2 --> 3 --> 1
        ///       2 --> 4 --> 5
        /// </remarks>
        [Test]
        public void TestThreeComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(4, 5)
            );

            var algorithm = graph.ComputeStronglyConnectedComponents();

            Assert.AreEqual(3, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 2,
                    [2] = 2,
                    [3] = 2,
                    [4] = 1,
                    [5] = 0
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(3, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 5 },
                algorithm.Graphs[0].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 4 },
                algorithm.Graphs[1].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                algorithm.Graphs[2].Vertices);
        }

        [Test]
        public void MultipleComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 4),
                Edge.Create(2, 5),
                Edge.Create(3, 1),
                Edge.Create(3, 4),
                Edge.Create(4, 6),
                Edge.Create(5, 6),
                Edge.Create(5, 7),
                Edge.Create(6, 4),
                Edge.Create(7, 5),
                Edge.Create(7, 8),
                Edge.Create(8, 6),
                Edge.Create(8, 7)
            );
            graph.AddVertex(10);

            var algorithm = graph.ComputeStronglyConnectedComponents();

            Assert.AreEqual(4, algorithm.ComponentCount);
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 2,
                    [2] = 2,
                    [3] = 2,
                    [4] = 0,
                    [5] = 1,
                    [6] = 0,
                    [7] = 1,
                    [8] = 1,
                    [10] = 3
                },
                algorithm.ComponentIndex);
            Assert.AreEqual(4, algorithm.Graphs.Length);
            CollectionAssert.AreEquivalent(
                new[] { 4, 6 },
                algorithm.Graphs[0].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 5, 7, 8 },
                algorithm.Graphs[1].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                algorithm.Graphs[2].Vertices);
            CollectionAssert.AreEquivalent(
                new[] { 10 },
                algorithm.Graphs[3].Vertices);
        }

    }
}