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
    /// Tests for <see cref="AStarShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class AStartShortestPathAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunAStarAndCheck<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.OutDegree(edge.Source) + 1 ?? double.PositiveInfinity;

            var algorithm = graph.CreateAStarShortestPathAlgorithm(
                e => distances[e],
                _ => 0.0);

            algorithm.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[vertex]);
            };

            algorithm.DiscoverVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Gray, algorithm.VerticesColors[vertex]);
            };

            algorithm.FinishVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Black, algorithm.VerticesColors[vertex]);
            };

            var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessors.Attach(algorithm))
                algorithm.Compute(root);

            CollectionAssert.IsNotEmpty(algorithm.GetDistances());
            Assert.AreEqual(graph.VertexCount, algorithm.GetDistances().Count());

            Verify(algorithm, predecessors);
        }

        private static void Verify<TVertex, TEdge>(
            [NotNull] AStarShortestPathAlgorithm<TVertex, TEdge> algorithm,
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
            Func<int, double> Heuristic = _ => 1.0;
            Func<IEdge<int>, double> Weights = _ => 1.0;

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateAStarShortestPathAlgorithm(Weights, Heuristic);
            AssertAlgorithmProperties(algorithm, graph, Heuristic, Weights);

            algorithm = graph.CreateAStarShortestPathAlgorithm(Weights, Heuristic, null, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Heuristic, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = graph.CreateAStarShortestPathAlgorithm(Weights, Heuristic, null, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Heuristic, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                AStarShortestPathAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g,
                Func<TVertex, double> heuristic = null,
                Func<TEdge, double> eWeights = null,
                IDistanceRelaxer relaxer = null)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.VerticesColors);
                if (heuristic is null)
                    Assert.IsNotNull(algo.CostHeuristic);
                else
                    Assert.AreSame(heuristic, algo.CostHeuristic);
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

            Func<int, double> Heuristic = _ => 1.0;
            Func<IEdge<int>, double> Weights = _ => 1.0;

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, Heuristic));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(null, Heuristic));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, Heuristic));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, Heuristic));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(null, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(Weights, null, null, DistanceRelaxers.CriticalDistance));
            _ = graph.CreateAStarShortestPathAlgorithm(Weights, Heuristic, null);
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(null, Heuristic, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateAStarShortestPathAlgorithm(null, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, Heuristic, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, Heuristic, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateAStarShortestPathAlgorithm(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, Weights, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, null, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, Weights, null, null, DistanceRelaxers.CriticalDistance));
            _ = new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, Weights, Heuristic, null, null);
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, Weights, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, null, Heuristic, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(graph, null, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, Weights, Heuristic, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, Weights, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, null, Heuristic, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, Weights, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, null, Heuristic, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, null, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new AStarShortestPathAlgorithm<int, IEdge<int>>(null, null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateAStarShortestPathAlgorithm(_ => 1.0, _ => 0.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void AStar(AdjacencyGraph<string, IEdge<string>> graph)
        {
            foreach (string root in graph.Vertices)
                RunAStarAndCheck(graph, root);
        }

        [Test]
        public void AStar_Throws()
        {
            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            var edge34 = Edge.Create(3, 4);

            var negativeWeightGraph = new AdjacencyGraph<int, IEdge<int>>();
            negativeWeightGraph.AddVerticesAndEdgeRange(edge12, edge23, edge34);

            var algorithm = negativeWeightGraph.CreateAStarShortestPathAlgorithm(
                e =>
                {
                    if (e == edge12)
                        return 12.0;
                    if (e == edge23)
                        return -23.0;
                    if (e == edge34)
                        return 34.0;
                    return 1.0;
                },
                _ => 0.0);
            Assert.Throws<NegativeWeightException>(() => algorithm.Compute(1));
        }

        [Test]
        public void AStar_HeuristicCalls()
        {
            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge14 = Edge.Create(1, 4);
            var edge23 = Edge.Create(2, 3);
            var edge34 = Edge.Create(3, 4);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge01,
                edge02,
                edge03,
                edge23,
                edge14,
                edge34
            );

            const int root = 0;

            var colorUpdates = new HashSet<GraphColor>
            {
                GraphColor.White, GraphColor.Gray, GraphColor.Black
            };

            int heuristicCalls = 0;
            AStarShortestPathAlgorithm<int, IEdge<int>> algorithm = null;
            Func<int, double> heuristic = v =>
            {
                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AccessToModifiedClosure
                colorUpdates.Remove(algorithm.GetVertexColor(v)!.Value);
                ++heuristicCalls;
                return 10.0 / heuristicCalls;
            };

            algorithm = graph.CreateAStarShortestPathAlgorithm(
                e =>
                {
                    if (e == edge01)
                        return 8.0;
                    if (e == edge02)
                        return 6.0;
                    if (e == edge03)
                        return 20.0;
                    if (e == edge34)
                        return 5.0;
                    return 1.0;
                },
                heuristic);

            algorithm.Compute(root);

            CollectionAssert.IsEmpty(colorUpdates);
        }

        [Test]
        public void AStar_HeuristicCallCount()
        {
            var lineGraph = new AdjacencyGraph<int, IEdge<int>>();
            lineGraph.AddVerticesAndEdgeRange(
                Edge.Create(2, 3),
                Edge.Create(3, 4),
                Edge.Create(2, 1),
                Edge.Create(1, 0)
            );

            const int root = 2;

            var heuristicCalls = new List<int>();
            var algorithm = lineGraph.CreateAStarShortestPathAlgorithm(
                _ => 1.0,
                v =>
                {
                    // Goal is 2, h(v) = v
                    heuristicCalls.Add(v);
                    return v;
                });

            algorithm.Compute(root);

            // Heuristic function must be called at least 4 times
            Assert.GreaterOrEqual(4, heuristicCalls.Count);

            // 0 must be expanded before 4
            Assert.Contains(0, heuristicCalls);
            Assert.Contains(4, heuristicCalls);
            Assert.Less(heuristicCalls.IndexOf(0), heuristicCalls.IndexOf(4));
        }

        [Pure]
        [NotNull]
        public static AStarShortestPathAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Heuristic(T v) => 1.0;
            double Weights(IEdge<T> e) => 1.0;
            var algorithm = graph.CreateAStarShortestPathAlgorithm(Weights, Heuristic);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}