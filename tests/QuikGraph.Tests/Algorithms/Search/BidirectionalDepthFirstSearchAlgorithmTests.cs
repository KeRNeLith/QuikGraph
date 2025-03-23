using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Search;


namespace QuikGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Tests for <see cref="BidirectionalDepthFirstSearchAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BidirectionalDepthFirstSearchAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunDFSAndCheck<TVertex, TEdge>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> graph,
            int maxDepth = int.MaxValue)
            where TEdge : IEdge<TVertex>
        {
            var discoverTimes = new Dictionary<TVertex, int>();
            var finishTimes = new Dictionary<TVertex, int>();
            int time = 0;
            var dfs = new BidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge>(graph)
            {
                MaxDepth = maxDepth
            };

            dfs.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, dfs.VerticesColors[vertex]);
            };

            dfs.StartVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, dfs.VerticesColors[vertex]);
            };

            dfs.DiscoverVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.VerticesColors[vertex]);
                discoverTimes[vertex] = time++;
            };

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            dfs.ExamineEdge += edge =>
            {
                // Depending if the edge was taken from in or out edges
                // Here we cannot determine in which case we are
                Assert.IsTrue(
                    dfs.VerticesColors[edge.Source] == GraphColor.Gray
                    ||
                    dfs.VerticesColors[edge.Target] == GraphColor.Gray);
            };

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            dfs.TreeEdge += edge =>
            {
                // Depending if the edge was taken from in or out edges
                // Here we cannot determine in which case we are
                Assert.IsTrue(
                    dfs.VerticesColors[edge.Source] == GraphColor.White
                    ||
                    dfs.VerticesColors[edge.Target] == GraphColor.White);
            };

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            dfs.BackEdge += edge =>
            {
                // Depending if the edge was taken from in or out edges
                // Here we cannot determine in which case we are
                Assert.IsTrue(
                    dfs.VerticesColors[edge.Source] == GraphColor.Gray
                    ||
                    dfs.VerticesColors[edge.Target] == GraphColor.Gray);
            };

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            dfs.ForwardOrCrossEdge += edge =>
            {
                // Depending if the edge was taken from in or out edges
                // Here we cannot determine in which case we are
                Assert.IsTrue(
                    dfs.VerticesColors[edge.Source] == GraphColor.Black
                    ||
                    dfs.VerticesColors[edge.Target] == GraphColor.Black);
            };

            dfs.FinishVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Black, dfs.VerticesColors[vertex]);
                finishTimes[vertex] = time++;
            };

            dfs.Compute();

            // Check
            // All vertices should be black
            foreach (TVertex vertex in graph.Vertices)
            {
                Assert.IsTrue(dfs.VerticesColors.ContainsKey(vertex));
                Assert.AreEqual(dfs.VerticesColors[vertex], GraphColor.Black);
            }

            foreach (TVertex u in graph.Vertices)
            {
                foreach (TVertex v in graph.Vertices)
                {
                    if (!u.Equals(v))
                    {
                        Assert.IsTrue(
                            finishTimes[u] < discoverTimes[v]
                            || finishTimes[v] < discoverTimes[u]
                            || (discoverTimes[v] < discoverTimes[u] && finishTimes[u] < finishTimes[v])
                            || (discoverTimes[u] < discoverTimes[v] && finishTimes[v] < finishTimes[u]));
                    }
                }
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            var verticesColors = new Dictionary<int, GraphColor>();
            algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph, verticesColors);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph, verticesColors, null);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm.MaxDepth = 12;
            AssertAlgorithmProperties(algorithm, graph, verticesColors, 12);

            algorithm.ProcessAllComponents = true;
            AssertAlgorithmProperties(algorithm, graph, verticesColors, 12, true);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                BidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge> algo,
                IBidirectionalGraph<TVertex, TEdge> g,
                IDictionary<TVertex, GraphColor> vColors = null,
                int maxDepth = int.MaxValue,
                bool processAllComponents = false)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                if (vColors is null)
                    CollectionAssert.IsEmpty(algo.VerticesColors);
                else
                    Assert.AreSame(vColors, algo.VerticesColors);
                Assert.AreEqual(maxDepth, algo.MaxDepth);
                Assert.AreEqual(processAllComponents, algo.ProcessAllComponents);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var verticesColors = new Dictionary<int, GraphColor>();

            Assert.Throws<ArgumentNullException>(
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(null));

            _ = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph, null);
            Assert.Throws<ArgumentNullException>(
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(null, verticesColors));
            Assert.Throws<ArgumentNullException>(
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(null, null));

            Assert.Throws<ArgumentNullException>(
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(null, verticesColors, null));
            _ = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph, null, null);
            Assert.Throws<ArgumentNullException>(
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement

            Assert.Throws<ArgumentOutOfRangeException>(() => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph).MaxDepth = -1);
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<TestVertex, IEdge<TestVertex>>(graph);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => new BidirectionalDepthFirstSearchAlgorithm<TestVertex, IEdge<TestVertex>>(graph));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
            // Algorithm not run
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(algorithm.GetVertexColor(1));

            algorithm.Compute();

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void DepthFirstSearch(BidirectionalGraph<string, IEdge<string>> graph)
        {
            RunDFSAndCheck(graph);
            RunDFSAndCheck(graph, 12);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ProcessAllComponents(bool processAll)
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 1),
                Edge.Create(2, 4),
                Edge.Create(2, 5),

                Edge.Create(6, 7),
                Edge.Create(6, 8),
                Edge.Create(8, 6)
            );

            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<int, IEdge<int>>(graph)
            {
                ProcessAllComponents = processAll
            };
            algorithm.Compute(1);

            if (processAll)
            {
                QuikGraphAssert.TrueForAll(algorithm.VerticesColors, pair => pair.Value == GraphColor.Black);
            }
            else
            {
                QuikGraphAssert.TrueForAll(
                    new[] { 1, 2, 3, 4, 5 },
                    vertex => algorithm.VerticesColors[vertex] == GraphColor.Black);
                QuikGraphAssert.TrueForAll(
                    new[] { 6, 7, 8 },
                    vertex => algorithm.VerticesColors[vertex] == GraphColor.White);
            }
        }

        [Pure]
        [NotNull]
        public static BidirectionalDepthFirstSearchAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new BidirectionalGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            var algorithm = new BidirectionalDepthFirstSearchAlgorithm<T, IEdge<T>>(graph);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}