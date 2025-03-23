using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;


namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for <see cref="BellmanFordShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BellmanFordShortestPathAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunBellmanFordAndCheck<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.OutDegree(edge.Source) + 1 ?? double.PositiveInfinity;

            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(e => distances[e]);

            algorithm.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[vertex]);
            };

            var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessors.Attach(algorithm))
                algorithm.Compute(root);

            Assert.AreEqual(graph.VertexCount, algorithm.VerticesColors.Count);
            foreach (TVertex vertex in graph.Vertices)
            {
                Assert.AreEqual(GraphColor.Black, algorithm.VerticesColors[vertex]);
            }

            Assert.IsFalse(algorithm.FoundNegativeCycle);
            CollectionAssert.IsNotEmpty(algorithm.GetDistances());
            Assert.AreEqual(graph.VertexCount, algorithm.GetDistances().Count());

            Verify(algorithm, predecessors);
        }

        private static void Verify<TVertex, TEdge>(
            [NotNull] BellmanFordShortestPathAlgorithm<TVertex, TEdge> algorithm,
            [NotNull] VertexPredecessorRecorderObserver<TVertex, TEdge> predecessors)
            where TEdge : IEdge<TVertex>
        {
            // Verify the result
            foreach (TVertex vertex in algorithm.VisitedGraph.Vertices)
            {
                if (!predecessors.VerticesPredecessors.TryGetValue(vertex, out TEdge predecessor))
                    continue;
                if (predecessor.Source.Equals(vertex))
                    continue;
                Assert.AreEqual(
                    algorithm.TryGetDistance(vertex, out double currentDistance),
                    algorithm.TryGetDistance(predecessor.Source, out double predecessorDistance));
                Assert.GreaterOrEqual(currentDistance, predecessorDistance);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            Func<IEdge<int>, double> Weights = _ => 1.0;

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(Weights);
            AssertAlgorithmProperties(algorithm, graph, Weights);

            algorithm = graph.CreateBellmanFordShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = graph.CreateBellmanFordShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                BellmanFordShortestPathAlgorithm<TVertex, TEdge> algo,
                IVertexAndEdgeListGraph<TVertex, TEdge> g,
                Func<TEdge, double> eWeights = null,
                IDistanceRelaxer relaxer = null)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.VerticesColors);
                Assert.IsFalse(algo.FoundNegativeCycle);
                if (eWeights is null)
                    Assert.IsNotNull(algo.Weights);
                else
                    Assert.AreSame(eWeights, algo.Weights);
                CollectionAssert.IsEmpty(algo.GetDistances());
                if (relaxer is null)
                    Assert.IsNotNull(algo.DistanceRelaxer);
                else
                    Assert.AreSame(relaxer, algo.DistanceRelaxer);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;

            Func<IEdge<int>, double> Weights = _ => 1.0;

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(Weights));
            Assert.Throws<ArgumentNullException>(() => graph.CreateBellmanFordShortestPathAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(null));

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(() => graph.CreateBellmanFordShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            _ = graph.CreateBellmanFordShortestPathAlgorithm(Weights, null);
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateBellmanFordShortestPathAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateBellmanFordShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            _ = graph.CreateBellmanFordShortestPathAlgorithm(Weights, null, null);
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateBellmanFordShortestPathAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateBellmanFordShortestPathAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_Throws_Test(
                () => graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(_ => 1.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void BellmanFord(AdjacencyGraph<string, IEdge<string>> graph)
        {
            foreach (string root in graph.Vertices)
                RunBellmanFordAndCheck(graph, root);
        }

        [Test]
        public void BellmanFord_NegativeCycle()
        {
            // Without negative cycle
            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            var edge34 = Edge.Create(3, 4);

            var negativeWeightGraph = new AdjacencyGraph<int, IEdge<int>>();
            negativeWeightGraph.AddVerticesAndEdgeRange(
                edge12, edge23, edge34
            );

            var algorithm = negativeWeightGraph.CreateBellmanFordShortestPathAlgorithm(
                e =>
                {
                    if (e == edge12)
                        return 12.0;
                    if (e == edge23)
                        return -23.0;
                    if (e == edge34)
                        return -34.0;
                    return 1.0;
                });
            Assert.DoesNotThrow(() => algorithm.Compute(1));
            Assert.IsFalse(algorithm.FoundNegativeCycle);

            // With negative cycle
            var edge41 = Edge.Create(4, 1);

            var negativeCycleGraph = new AdjacencyGraph<int, IEdge<int>>();
            negativeCycleGraph.AddVerticesAndEdgeRange(edge12, edge23, edge34, edge41);

            algorithm = negativeCycleGraph.CreateBellmanFordShortestPathAlgorithm(
                e =>
                {
                    if (e == edge12)
                        return 12.0;
                    if (e == edge23)
                        return -23.0;
                    if (e == edge34)
                        return -34.0;
                    if (e == edge41)
                        return 41.0;
                    return 1.0;
                });
            Assert.DoesNotThrow(() => algorithm.Compute(1));
            Assert.IsTrue(algorithm.FoundNegativeCycle);
        }

        [Pure]
        [NotNull]
        public static BellmanFordShortestPathAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Weights(IEdge<T> e) => 1.0;
            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(Weights);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}