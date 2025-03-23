using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Search;

using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Tests for <see cref="EdgeDepthFirstSearchAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdgeDepthFirstAlgorithmSearchTests : SearchAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunEdgeDFSAndCheck<TVertex, TEdge>(
            [NotNull] IEdgeListAndIncidenceGraph<TVertex, TEdge> graph,
            int maxDepth = int.MaxValue)
            where TEdge : IEdge<TVertex>
        {
            var parents = new Dictionary<TEdge, TEdge>();
            var discoverTimes = new Dictionary<TEdge, int>();
            var finishTimes = new Dictionary<TEdge, int>();
            int time = 0;
            var dfs = graph.CreateEdgeDepthFirstSearchAlgorithm();
            dfs.MaxDepth = maxDepth;

            dfs.InitializeEdge += edge =>
            {
                Assert.AreEqual(GraphColor.White, dfs.EdgesColors[edge]);
            };

            dfs.StartEdge += edge =>
            {
                Assert.AreEqual(GraphColor.White, dfs.EdgesColors[edge]);
                Assert.IsFalse(parents.ContainsKey(edge));
                parents[edge] = edge;
                discoverTimes[edge] = time++;
            };

            dfs.DiscoverTreeEdge += (edge, targetEdge) =>
            {
                parents[targetEdge] = edge;

                Assert.AreEqual(GraphColor.White, dfs.EdgesColors[targetEdge]);
                Assert.AreEqual(GraphColor.Gray, dfs.EdgesColors[parents[targetEdge]]);

                discoverTimes[targetEdge] = time++;
            };

            dfs.TreeEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.EdgesColors[edge]);
            };

            dfs.BackEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Gray, dfs.EdgesColors[edge]);
            };

            dfs.ForwardOrCrossEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Black, dfs.EdgesColors[edge]);
            };

            dfs.FinishEdge += edge =>
            {
                Assert.AreEqual(GraphColor.Black, dfs.EdgesColors[edge]);
                finishTimes[edge] = time++;
            };

            dfs.Compute();

            // Check
            // All vertices should be black
            foreach (TEdge edge in graph.Edges)
            {
                Assert.IsTrue(dfs.EdgesColors.ContainsKey(edge));
                Assert.AreEqual(dfs.EdgesColors[edge], GraphColor.Black);
            }

            foreach (TEdge e1 in graph.Edges)
            {
                foreach (TEdge e2 in graph.Edges)
                {
                    if (!e1.Equals(e2))
                    {
                        Assert.IsTrue(
                            finishTimes[e1] < discoverTimes[e2]
                            || finishTimes[e2] < discoverTimes[e1]
                            || (discoverTimes[e2] < discoverTimes[e1] && finishTimes[e1] < finishTimes[e2] && IsDescendant(parents, e1, e2))
                            || (discoverTimes[e1] < discoverTimes[e2] && finishTimes[e2] < finishTimes[e1] && IsDescendant(parents, e2, e1)));
                    }
                }
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            var edgesColors = new Dictionary<IEdge<int>, GraphColor>();
            algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm(edgesColors);
            AssertAlgorithmProperties(algorithm, graph, edgesColors);

            algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm(edgesColors, null);
            AssertAlgorithmProperties(algorithm, graph, edgesColors);

            algorithm.MaxDepth = 12;
            AssertAlgorithmProperties(algorithm, graph, edgesColors, 12);

            algorithm.ProcessAllComponents = true;
            AssertAlgorithmProperties(algorithm, graph, edgesColors, 12, true);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                EdgeDepthFirstSearchAlgorithm<TVertex, TEdge> algo,
                IEdgeListAndIncidenceGraph<TVertex, TEdge> g,
                IDictionary<TEdge, GraphColor> vColors = null,
                int maxDepth = int.MaxValue,
                bool processAllComponents = false)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                if (vColors is null)
                    CollectionAssert.IsEmpty(algo.EdgesColors);
                else
                    Assert.AreSame(vColors, algo.EdgesColors);
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
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var edgesColors = new Dictionary<IEdge<int>, GraphColor>();

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEdgeDepthFirstSearchAlgorithm());

            _ = graph.CreateEdgeDepthFirstSearchAlgorithm(null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEdgeDepthFirstSearchAlgorithm(edgesColors));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEdgeDepthFirstSearchAlgorithm(null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEdgeDepthFirstSearchAlgorithm(edgesColors, null));
            _ = graph.CreateEdgeDepthFirstSearchAlgorithm(null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEdgeDepthFirstSearchAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement

            Assert.Throws<ArgumentOutOfRangeException>(() => graph.CreateEdgeDepthFirstSearchAlgorithm().MaxDepth = -1);
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => graph.CreateEdgeDepthFirstSearchAlgorithm());
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateEdgeDepthFirstSearchAlgorithm());
        }

        #endregion

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void EdgeDepthFirstSearch(AdjacencyGraph<string, IEdge<string>> graph)
        {
            RunEdgeDFSAndCheck(graph);
            RunEdgeDFSAndCheck(graph, 12);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ProcessAllComponents(bool processAll)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge21 = Edge.Create(2, 1);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);

            var edge67 = Edge.Create(6, 7);
            var edge68 = Edge.Create(6, 8);
            var edge86 = Edge.Create(8, 6);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge21, edge24, edge25,
                edge67, edge68, edge86
            );

            var algorithm = graph.CreateEdgeDepthFirstSearchAlgorithm();
            algorithm.ProcessAllComponents = processAll;
            algorithm.Compute(1);

            if (processAll)
            {
                QuikGraphAssert.TrueForAll(algorithm.EdgesColors, pair => pair.Value == GraphColor.Black);
            }
            else
            {
                QuikGraphAssert.TrueForAll(
                    new[] { edge12, edge13, edge24, edge25 },
                    edge => algorithm.EdgesColors[edge] == GraphColor.Black);
                QuikGraphAssert.TrueForAll(
                    new[] { edge67, edge68, edge86 },
                    edge => algorithm.EdgesColors[edge] == GraphColor.White);
            }
        }
    }
}