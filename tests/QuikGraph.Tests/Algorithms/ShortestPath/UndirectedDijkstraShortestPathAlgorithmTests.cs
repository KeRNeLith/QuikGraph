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
    /// Tests for <see cref="UndirectedDijkstraShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedDijkstraShortestPathAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunUndirectedDijkstraAndCheck<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph, [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.AdjacentDegree(edge.Source) + 1 ?? double.PositiveInfinity;

            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(e => distances[e]);
            var predecessors = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessors.Attach(algorithm))
                algorithm.Compute(root);

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

            CollectionAssert.IsNotEmpty(algorithm.GetDistances());
            Assert.AreEqual(graph.VertexCount, algorithm.GetDistances().Count());

            Verify(algorithm, predecessors);
        }

        private static void Verify<TVertex, TEdge>(
            [NotNull] UndirectedDijkstraShortestPathAlgorithm<TVertex, TEdge> algorithm,
            [NotNull] UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge> predecessors)
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

            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights);
            AssertAlgorithmProperties(algorithm, graph, Weights);

            algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                UndirectedDijkstraShortestPathAlgorithm<TVertex, TEdge> algo,
                IUndirectedGraph<TVertex, TEdge> g,
                Func<TEdge, double> eWeights = null,
                IDistanceRelaxer relaxer = null)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.VerticesColors);
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
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;

            Func<IEdge<int>, double> Weights = _ => 1.0;

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            _ = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            _ = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedDijkstraShortestPathAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [Category(TestCategories.LongRunning)]
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_SlowTests), new object[] { 20 })]
        public void UndirectedDijkstra(UndirectedGraph<string, IEdge<string>> graph)
        {
            int cut = 0;
            foreach (string root in graph.Vertices)
            {
                if (cut++ > 10)
                    break;
                RunUndirectedDijkstraAndCheck(graph, root);
            }
        }

        [Test]
        public void UndirectedDijkstraSimpleGraph()
        {
            var undirectedGraph = new UndirectedGraph<object, IEdge<object>>(true);
            object v1 = "vertex1";
            object v2 = "vertex2";
            object v3 = "vertex3";
            var e1 = Edge.Create(v1, v2);
            var e2 = Edge.Create(v2, v3);
            var e3 = Edge.Create(v3, v1);
            undirectedGraph.AddVertex(v1);
            undirectedGraph.AddVertex(v2);
            undirectedGraph.AddVertex(v3);
            undirectedGraph.AddEdge(e1);
            undirectedGraph.AddEdge(e2);
            undirectedGraph.AddEdge(e3);

            var algorithm = undirectedGraph.CreateUndirectedDijkstraShortestPathAlgorithm(_ => 1.0);
            var observer = new UndirectedVertexPredecessorRecorderObserver<object, IEdge<object>>();
            using (observer.Attach(algorithm))
                algorithm.Compute(v1);

            Assert.IsNotNull(observer.GetPath(v3));
        }

        [Pure]
        [NotNull]
        public static UndirectedDijkstraShortestPathAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new UndirectedGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Weights(IEdge<T> e) => 1.0;
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(Weights);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}