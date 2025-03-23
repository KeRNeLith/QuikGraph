using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;
using QuikGraph.Collections;


namespace QuikGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Tests for <see cref="UndirectedBreadthFirstSearchAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedBreadthFirstAlgorithmSearchTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunBFSAndCheck<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] TVertex sourceVertex)
            where TEdge : IEdge<TVertex>
        {
            var parents = new Dictionary<TVertex, TVertex>();
            var distances = new Dictionary<TVertex, int>();
            TVertex currentVertex = default;
            int currentDistance = 0;
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();

            algorithm.InitializeVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[vertex]);
            };

            algorithm.StartVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[vertex]);
            };

            algorithm.DiscoverVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Gray, algorithm.VerticesColors[vertex]);
                if (vertex.Equals(sourceVertex))
                {
                    currentVertex = sourceVertex;
                }
                else
                {
                    Assert.IsNotNull(currentVertex);
                    Assert.AreEqual(parents[vertex], currentVertex);
                    // ReSharper disable once AccessToModifiedClosure
                    Assert.AreEqual(distances[vertex], currentDistance + 1);
                    Assert.AreEqual(distances[vertex], distances[parents[vertex]] + 1);
                }
            };

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            algorithm.ExamineEdge += edge =>
            {
                Assert.IsTrue(edge.Source.Equals(currentVertex) || edge.Target.Equals(currentVertex));
            };

            algorithm.ExamineVertex += vertex =>
            {
                TVertex u = vertex;
                currentVertex = u;
                // Ensure that the distances monotonically increase.
                // ReSharper disable AccessToModifiedClosure
                Assert.IsTrue(distances[u] == currentDistance || distances[u] == currentDistance + 1);

                if (distances[u] == currentDistance + 1) // New level
                    ++currentDistance;
                // ReSharper restore AccessToModifiedClosure
            };

            algorithm.TreeEdge += (_, args) =>
            {
                TVertex u = args.Edge.Source;
                TVertex v = args.Edge.Target;
                if (algorithm.VerticesColors[v] == GraphColor.Gray)
                {
                    TVertex temp = u;
                    u = v;
                    v = temp;
                }

                Assert.AreEqual(GraphColor.White, algorithm.VerticesColors[v]);
                Assert.AreEqual(distances[u], currentDistance);
                parents[v] = u;
                distances[v] = distances[u] + 1;
            };

            algorithm.NonTreeEdge += (_, args) =>
            {
                TVertex u = args.Edge.Source;
                TVertex v = args.Edge.Target;
                if (algorithm.VerticesColors[v] != GraphColor.White)
                {
                    TVertex temp = u;
                    u = v;
                    v = temp;
                }

                Assert.IsFalse(algorithm.VerticesColors[v] == GraphColor.White);

                if (algorithm.VisitedGraph.IsDirected)
                {
                    // Cross or back edge
                    Assert.IsTrue(distances[v] <= distances[u] + 1);
                }
                else
                {
                    // Cross edge (or going backwards on a tree edge)
                    Assert.IsTrue(
                        distances[v] == distances[u]
                        || distances[v] == distances[u] + 1
                        || distances[v] == distances[u] - 1);
                }
            };

            algorithm.GrayTarget += (_, args) =>
            {
                Assert.AreEqual(GraphColor.Gray, algorithm.VerticesColors[args.Target]);
            };

            algorithm.BlackTarget += (_, args) =>
            {
                Assert.AreEqual(GraphColor.Black, algorithm.VerticesColors[args.Target]);

                foreach (var edge in algorithm.VisitedGraph.AdjacentEdges(args.Target))
                    Assert.IsFalse(algorithm.VerticesColors[edge.Target] == GraphColor.White);
            };

            algorithm.FinishVertex += vertex =>
            {
                Assert.AreEqual(GraphColor.Black, algorithm.VerticesColors[vertex]);
            };

            parents.Clear();
            distances.Clear();
            currentDistance = 0;

            foreach (TVertex vertex in graph.Vertices)
            {
                distances[vertex] = int.MaxValue;
                parents[vertex] = vertex;
            }

            distances[sourceVertex] = 0;

            var recorder = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (recorder.Attach(algorithm))
            {
                algorithm.Compute(sourceVertex);
            }

            // All white vertices should be unreachable from the source.
            foreach (TVertex vertex in graph.Vertices)
            {
                if (algorithm.VerticesColors[vertex] != GraphColor.White)
                {
                    continue;
                }

                // Check !IsReachable(sourceVertex, vertex, graph);
                var path = recorder.GetPath(vertex);
                if (path != null)
                {
                    foreach (TEdge edge in path)
                    {
                        Assert.AreNotEqual(sourceVertex, edge.Source);
                        Assert.AreNotEqual(sourceVertex, edge.Target);
                    }
                }
            }

            // The shortest path to a child should be one longer than
            // shortest path to the parent.
            foreach (TVertex vertex in graph.Vertices)
            {
                if (!parents[vertex].Equals(vertex)) // Not the root of the BFS tree
                    Assert.AreEqual(distances[vertex], distances[parents[vertex]] + 1);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            var verticesColors = new Dictionary<int, GraphColor>();
            var queue = new BinaryQueue<int, double>(_ => 1.0);
            algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, verticesColors);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, verticesColors, null);
            AssertAlgorithmProperties(algorithm, graph, verticesColors);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                UndirectedBreadthFirstSearchAlgorithm<TVertex, TEdge> algo,
                IUndirectedGraph<TVertex, TEdge> g,
                IDictionary<TVertex, GraphColor> vColors = null)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                if (vColors is null)
                    CollectionAssert.IsEmpty(algo.VerticesColors);
                else
                    Assert.AreSame(vColors, algo.VerticesColors);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            UndirectedGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            var verticesColors = new Dictionary<int, GraphColor>();
            var queue = new BinaryQueue<int, double>(_ => 1.0);

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm());

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, verticesColors));
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(null, verticesColors);
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(null, verticesColors));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, null));
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, verticesColors, null));
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(null, verticesColors, null);
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(null, verticesColors, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(queue, null, null));
            _ = graph.CreateUndirectedBreadthFirstSearchAlgorithm(null, null, null);
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateUndirectedBreadthFirstSearchAlgorithm(null, null, null)); 
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            ComputeWithoutRoot_Throws_Test(
                () => graph.CreateUndirectedBreadthFirstSearchAlgorithm());
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateUndirectedBreadthFirstSearchAlgorithm());
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();
            // Algorithm not run
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(algorithm.GetVertexColor(1));

            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_SlowTests), new object[] { 10 })]
        public void UndirectedBreadthFirstSearch(UndirectedGraph<string, IEdge<string>> graph)
        {
            foreach (string vertex in graph.Vertices)
                RunBFSAndCheck(graph, vertex);
        }

        [Pure]
        [NotNull]
        public static UndirectedBreadthFirstSearchAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new UndirectedGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            var algorithm = graph.CreateUndirectedBreadthFirstSearchAlgorithm();

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}