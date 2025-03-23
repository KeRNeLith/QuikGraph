using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Search;

using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Tests for <see cref="DepthFirstSearchAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DepthFirstAlgorithmSearchTests : SearchAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunDFSAndCheck<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            int maxDepth = int.MaxValue)
            where TEdge : IEdge<TVertex>
        {
            var parents = new Dictionary<TVertex, TVertex>();
            var discoverTimes = new Dictionary<TVertex, int>();
            var finishTimes = new Dictionary<TVertex, int>();
            int time = 0;
            var dfs = graph.CreateDepthFirstSearchAlgorithm();
            dfs.MaxDepth = maxDepth;

            dfs.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, dfs.VerticesColors[vertex]);
            };

            dfs.StartVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, dfs.VerticesColors[vertex]);
                Assert.IsFalse(parents.ContainsKey(vertex));
                parents[vertex] = vertex;
            };

            dfs.DiscoverVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.VerticesColors[vertex]);
                Assert.AreEqual(GraphColor.Gray, dfs.VerticesColors[parents[vertex]]);

                discoverTimes[vertex] = time++;
            };

            dfs.ExamineEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.VerticesColors[edge.Source]);
            };

            dfs.TreeEdge += edge =>
            {
                Assert.AreEqual(GraphColor.White, dfs.VerticesColors[edge.Target]);
                parents[edge.Target] = edge.Source;
            };

            dfs.BackEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.VerticesColors[edge.Target]);
            };

            dfs.ForwardOrCrossEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Black, dfs.VerticesColors[edge.Target]);
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
                            || (discoverTimes[v] < discoverTimes[u] && finishTimes[u] < finishTimes[v] && IsDescendant(parents, u, v))
                            || (discoverTimes[u] < discoverTimes[v] && finishTimes[v] < finishTimes[u] && IsDescendant(parents, v, u)));
                    }
                }
            }
        }

        #endregion

        [Test]
        public void TestConstructor()
        {
            AdjacencyGraph<int, IEdge<int>> graph = new ();
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            var verticesColors = new Dictionary<int, GraphColor>();
            algorithm = graph.CreateDepthFirstSearchAlgorithm(verticesColors);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm = graph.CreateDepthFirstSearchAlgorithm(null);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateDepthFirstSearchAlgorithm(verticesColors);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm = graph.CreateDepthFirstSearchAlgorithm(verticesColors, edges => edges.Where(e => e != null));
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm.MaxDepth = 12;
            AssertAlgorithmProperties(algorithm, graph, verticesColors, 12);

            algorithm.ProcessAllComponents = true;
            AssertAlgorithmProperties(algorithm, graph, verticesColors, 12, true);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                DepthFirstSearchAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g,
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
                Assert.IsNotNull(algo.OutEdgesFilter);
            }

            #endregion
        }

        [Test]
        public void TestConstructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var verticesColors = new Dictionary<int, GraphColor>();
            IEnumerable<IEdge<int>> Filter(IEnumerable<IEdge<int>> edges) => edges.Where(e => e != null);

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm());

            _ = graph.CreateDepthFirstSearchAlgorithm(host: null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(verticesColors));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm());

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(verticesColors, null));
            _ = graph.CreateDepthFirstSearchAlgorithm(null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(verticesColors, Filter));
            _ = graph.CreateDepthFirstSearchAlgorithm(null, Filter, null);
            _ = graph.CreateDepthFirstSearchAlgorithm(verticesColors, null, null);
            _ = graph.CreateDepthFirstSearchAlgorithm(null, null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(null, Filter, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(verticesColors, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateDepthFirstSearchAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement

            Assert.Throws<ArgumentOutOfRangeException>(() => graph.CreateDepthFirstSearchAlgorithm().MaxDepth = -1);
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => graph.CreateDepthFirstSearchAlgorithm());
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateDepthFirstSearchAlgorithm());
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
            // Algorithm not run
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(algorithm.GetVertexColor(1));

            algorithm.Compute();

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        public void DepthFirstSearch(AdjacencyGraph<string, IEdge<string>> graph)
        {
            RunDFSAndCheck(graph);
            RunDFSAndCheck(graph, 12);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ProcessAllComponents(bool processAll)
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
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

            var algorithm = graph.CreateDepthFirstSearchAlgorithm();
                algorithm.ProcessAllComponents = processAll;
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
        public static DepthFirstSearchAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            var algorithm = graph.CreateDepthFirstSearchAlgorithm();

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}