using System;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;


namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for <see cref="DagShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DagShortestPathAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void DagAlgorithm_Test<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IDistanceRelaxer relaxer)
            where TEdge : IEdge<TVertex>
        {
            // Is this a dag?
            bool isDag = graph.IsDirectedAcyclicGraph();

            TVertex[] vertices = graph.Vertices.ToArray();
            foreach (TVertex root in vertices)
            {
                if (isDag)
                {
                    RunDagShortestPathAndCheck(graph, root, relaxer);
                }
                else
                {
                    Assert.Throws<CyclicGraphException>(() => RunDagShortestPathAndCheck(graph, root, relaxer));
                }
            }
        }

        private static void DagShortestPath_Test<TVertex, TEdge>([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            DagAlgorithm_Test(graph, DistanceRelaxers.ShortestDistance);
        }

        private static void DagCriticalPath_Test<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            DagAlgorithm_Test(graph, DistanceRelaxers.CriticalDistance);
        }

        private static void RunDagShortestPathAndCheck<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] IDistanceRelaxer relaxer)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0, relaxer);

            algorithm.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[vertex]);
            };

            algorithm.DiscoverVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Gray, algorithm.VerticesColors[vertex]);
            };

            algorithm.StartVertex += vertex =>
            {
                Assert.AreNotEqual(GraphColor.Black, algorithm.VerticesColors[vertex]);
            };

            algorithm.ExamineVertex += vertex =>
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

            Assert.AreEqual(graph.VertexCount, algorithm.VerticesColors.Count);
            foreach (TVertex vertex in graph.Vertices)
            {
                Assert.AreEqual(GraphColor.Black, algorithm.VerticesColors[vertex]);
            }

            CollectionAssert.IsNotEmpty(algorithm.GetDistances());
            Assert.AreEqual(graph.VertexCount, algorithm.GetDistances().Count());

            Verify(algorithm, predecessors);
        }

        private static void Verify<TVertex, TEdge>(
            [NotNull] DagShortestPathAlgorithm<TVertex, TEdge> algorithm,
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
                Assert.AreEqual(predecessorDistance + 1, currentDistance);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            Func<IEdge<int>, double> Weights = _ => 1.0;

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDagShortestPathAlgorithm(Weights);
            AssertAlgorithmProperties(algorithm, graph, Weights);

            algorithm = graph.CreateDagShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = graph.CreateDagShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                DagShortestPathAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g,
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
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;

            Func<IEdge<int>, double> Weights = _ => 1.0;

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(Weights));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateDagShortestPathAlgorithm(null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateDagShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            _ = graph.CreateDagShortestPathAlgorithm(Weights, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateDagShortestPathAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateDagShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            _ = graph.CreateDagShortestPathAlgorithm(Weights, null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateDagShortestPathAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDagShortestPathAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_Throws_Test(
                () => graph.CreateDagShortestPathAlgorithm(_ => 1.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateDagShortestPathAlgorithm(_ => 1.0));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateDagShortestPathAlgorithm(_ => 1.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { 50 })]
        [Category(TestCategories.LongRunning)]
        public void DagShortestPath(AdjacencyGraph<string, IEdge<string>> graph)
        {
            DagShortestPath_Test(graph);
            DagCriticalPath_Test(graph);
        }

        [Pure]
        [NotNull]
        public static DagShortestPathAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Weights(IEdge<T> e) => 1.0;
            var algorithm = graph.CreateDagShortestPathAlgorithm(Weights);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}