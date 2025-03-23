using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DijkstraShortestPathAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunDijkstraAndCheck<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>(graph.EdgeCount);
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.OutDegree(edge.Source) + 1 ?? Double.PositiveInfinity;

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(e => distances[e]);

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
            [NotNull] DijkstraShortestPathAlgorithm<TVertex, TEdge> algorithm,
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
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(Weights);
            AssertAlgorithmProperties(algorithm, graph, Weights);

            algorithm = graph.CreateDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = graph.CreateDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                DijkstraShortestPathAlgorithm<TVertex, TEdge> algo,
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

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(Weights));
            Assert.Throws<ArgumentNullException>(() => graph.CreateDijkstraShortestPathAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(null));

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(() => graph.CreateDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            _ = graph.CreateDijkstraShortestPathAlgorithm(Weights, null);
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(Weights, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateDijkstraShortestPathAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(Weights, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            _ = graph.CreateDijkstraShortestPathAlgorithm(Weights, null, null);
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(null, DistanceRelaxers.CriticalDistance, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(Weights, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateDijkstraShortestPathAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateDijkstraShortestPathAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(
                graph,
                () => graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0));
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        [Category(TestCategories.LongRunning)]
        public void Dijkstra(AdjacencyGraph<string, IEdge<string>> graph)
        {
            foreach (string root in graph.Vertices)
                RunDijkstraAndCheck(graph, root);
        }

        [Test]
        public void Dijkstra_Throws()
        {
            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            var edge34 = Edge.Create(3, 4);

            var negativeWeightGraph = new AdjacencyGraph<int, IEdge<int>>();
            negativeWeightGraph.AddVerticesAndEdgeRange(
                edge12, edge23, edge34
            );

            var algorithm = negativeWeightGraph.CreateDijkstraShortestPathAlgorithm(
                e =>
                {
                    if (e == edge12)
                        return 12.0;
                    if (e == edge23)
                        return -23.0;
                    if (e == edge34)
                        return 34.0;
                    return 1.0;
                });
            Assert.Throws<NegativeWeightException>(() => algorithm.Compute(1));
        }

        [Test]
        public void DijkstraSimpleGraph()
        {
            var graph = new AdjacencyGraph<string, IEdge<string>>(true);

            // Add some vertices to the graph
            graph.AddVertex("A");
            graph.AddVertex("B");
            graph.AddVertex("D");
            graph.AddVertex("C");
            graph.AddVertex("E");

            // Create the edges
            // ReSharper disable InconsistentNaming
            var a_b = Edge.Create("A", "B");
            var a_c = Edge.Create("A", "C");
            var b_e = Edge.Create("B", "E");
            var c_d = Edge.Create("C", "D");
            var d_e = Edge.Create("D", "E");
            // ReSharper restore InconsistentNaming

            // Add edges to the graph
            graph.AddEdge(a_b);
            graph.AddEdge(a_c);
            graph.AddEdge(c_d);
            graph.AddEdge(d_e);
            graph.AddEdge(b_e);

            // Define some weights to the edges
            var weight = new Dictionary<IEdge<string>, double>(graph.EdgeCount)
            {
                [a_b] = 30,
                [a_c] = 30,
                [b_e] = 60,
                [c_d] = 40,
                [d_e] = 4
            };

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(e => weight[e]);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            var predecessorObserver = new VertexPredecessorRecorderObserver<string, IEdge<string>>();
            using (predecessorObserver.Attach(algorithm))
                // Run the algorithm with A set to be the source
                algorithm.Compute("A");

            Assert.AreEqual(74, algorithm.GetDistance("E"), double.Epsilon);
        }

        [Test]
        public void DijkstraSimpleGraph2()
        {
            var graph = new AdjacencyGraph<char, IEdge<char>>();
            var distances = new Dictionary<IEdge<char>, double>();

            graph.AddVertexRange("ABCDE");
            AddEdge('A', 'C', 1);
            AddEdge('B', 'B', 2);
            AddEdge('B', 'D', 1);
            AddEdge('B', 'E', 2);
            AddEdge('C', 'B', 7);
            AddEdge('C', 'D', 3);
            AddEdge('D', 'E', 1);
            AddEdge('E', 'A', 1);
            AddEdge('E', 'B', 1);

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(AlgorithmExtensions.GetIndexer(distances));
            var predecessors = new VertexPredecessorRecorderObserver<char, IEdge<char>>();
            using (predecessors.Attach(algorithm))
                algorithm.Compute('A');

            Assert.AreEqual(0, algorithm.GetDistance('A'));
            Assert.AreEqual(6, algorithm.GetDistance('B'));
            Assert.AreEqual(1, algorithm.GetDistance('C'));
            Assert.AreEqual(4, algorithm.GetDistance('D'));
            Assert.AreEqual(5, algorithm.GetDistance('E'));

            #region Local function

            void AddEdge(char source, char target, double weight)
            {
                var edge = Edge.Create(source, target);
                distances[edge] = weight;
                graph.AddEdge(edge);
            }

            #endregion
        }

        /// <summary> This Test is very slow (18.2s) </summary>
        [Test]
        [Category(TestCategories.CISkip)]
        public void DijkstraRepro12359()
        {
            var graph = TestGraphFactory.LoadGraph(GetGraphFilePath("repro12359.graphml"));
            int cut = 0;
            foreach (string root in graph.Vertices)
            {
                if (cut++ > 5)
                    break;
                RunDijkstraAndCheck(graph, root);
            }
        }

        [Test]
        public void LineGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            graph.AddEdge(Edge.Create(1, 2));
            graph.AddEdge(Edge.Create(2, 3));

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1);
            algorithm.Compute(1);

            Assert.AreEqual(0d, algorithm.GetDistance(1));
            Assert.AreEqual(2d, algorithm.GetDistance(3));
            Assert.AreEqual(1d, algorithm.GetDistance(2));
        }

        [Test]
        public void PredecessorsLineGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = Edge.Create(1, 2); graph.AddEdge(e12);
            var e23 = Edge.Create(2, 3); graph.AddEdge(e23);

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1);
            var vis = new VertexPredecessorRecorderObserver<int, IEdge<int>>();
            using (vis.Attach(algorithm))
                algorithm.Compute(1);

            var path = vis.GetPath(2);
            IEdge<int>[] pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);

            path = vis.GetPath(3);
            pathArray = path.ToArray();
            Assert.AreEqual(2, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);
            Assert.AreEqual(e23, pathArray[1]);
        }

        [Test]
        public void DoubleLineGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = Edge.Create(1, 2); graph.AddEdge(e12);
            var e23 = Edge.Create(2, 3); graph.AddEdge(e23);
            var e13 = Edge.Create(1, 3); graph.AddEdge(e13);

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1);
            algorithm.Compute(1);

            Assert.AreEqual(0.0, algorithm.GetDistance(1));
            Assert.AreEqual(1.0, algorithm.GetDistance(2));
            Assert.AreEqual(1.0, algorithm.GetDistance(3));
        }

        [Test]
        public void PredecessorsDoubleLineGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = Edge.Create(1, 2); graph.AddEdge(e12);
            var e23 = Edge.Create(2, 3); graph.AddEdge(e23);
            var e13 = Edge.Create(1, 3); graph.AddEdge(e13);

            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_ => 1);
            var vis = new VertexPredecessorRecorderObserver<int, IEdge<int>>();
            using (vis.Attach(algorithm))
                algorithm.Compute(1);

            var path = vis.GetPath(2);
            IEdge<int>[] pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);

            path = vis.GetPath(3);
            pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e13, pathArray[0]);
        }

        [Test]
        [Category(TestCategories.VerboseTest)]
        public void Scenario()
        {
            Assert.DoesNotThrow(() =>
            {
                var graph = CreateGraph(out var edgeCosts);

                // Run Dijkstra on this graph
                var dijkstra = graph.CreateDijkstraShortestPathAlgorithm(e => edgeCosts[e]);

                // Attach a Vertex Predecessor Recorder Observer to give us the paths
                var predecessorObserver = new VertexPredecessorRecorderObserver<string, IEdge<string>>();
                using (predecessorObserver.Attach(dijkstra))
                {
                    // Run the algorithm with A as source
                    dijkstra.Compute("A");
                }

                foreach (KeyValuePair<string, IEdge<string>> pair in predecessorObserver.VerticesPredecessors)
                {
                    Console.WriteLine($"If you want to get to {pair.Key} you have to enter through the in edge {pair.Value}.");
                }

                foreach (string vertex in graph.Vertices)
                {
                    double distance = AlgorithmExtensions.ComputePredecessorCost(
                        predecessorObserver.VerticesPredecessors,
                        edgeCosts,
                        vertex);
                    Console.WriteLine($"A -> {vertex}: {distance}");
                }
            });

            #region Local function

            AdjacencyGraph<string, IEdge<string>> CreateGraph(out Dictionary<IEdge<string>, double> costs)
            {
                var g = new AdjacencyGraph<string, IEdge<string>>(true);

                // Add some vertices to the graph
                g.AddVertex("A");
                g.AddVertex("B");
                g.AddVertex("C");
                g.AddVertex("D");
                g.AddVertex("E");
                g.AddVertex("F");
                g.AddVertex("G");
                g.AddVertex("H");
                g.AddVertex("I");
                g.AddVertex("J");

                // Create the edges
                // ReSharper disable InconsistentNaming
                var a_b = Edge.Create("A", "B");
                var a_d = Edge.Create("A", "D");
                var b_a = Edge.Create("B", "A");
                var b_c = Edge.Create("B", "C");
                var b_e = Edge.Create("B", "E");
                var c_b = Edge.Create("C", "B");
                var c_f = Edge.Create("C", "F");
                var c_j = Edge.Create("C", "J");
                var d_e = Edge.Create("D", "E");
                var d_g = Edge.Create("D", "G");
                var e_d = Edge.Create("E", "D");
                var e_f = Edge.Create("E", "F");
                var e_h = Edge.Create("E", "H");
                var f_i = Edge.Create("F", "I");
                var f_j = Edge.Create("F", "J");
                var g_d = Edge.Create("G", "D");
                var g_h = Edge.Create("G", "H");
                var h_g = Edge.Create("H", "G");
                var h_i = Edge.Create("H", "I");
                var i_f = Edge.Create("I", "F");
                var i_j = Edge.Create("I", "J");
                var i_h = Edge.Create("I", "H");
                var j_f = Edge.Create("J", "F");
                // ReSharper restore InconsistentNaming

                // Add the edges
                g.AddEdge(a_b);
                g.AddEdge(a_d);
                g.AddEdge(b_a);
                g.AddEdge(b_c);
                g.AddEdge(b_e);
                g.AddEdge(c_b);
                g.AddEdge(c_f);
                g.AddEdge(c_j);
                g.AddEdge(d_e);
                g.AddEdge(d_g);
                g.AddEdge(e_d);
                g.AddEdge(e_f);
                g.AddEdge(e_h);
                g.AddEdge(f_i);
                g.AddEdge(f_j);
                g.AddEdge(g_d);
                g.AddEdge(g_h);
                g.AddEdge(h_g);
                g.AddEdge(h_i);
                g.AddEdge(i_f);
                g.AddEdge(i_h);
                g.AddEdge(i_j);
                g.AddEdge(j_f);

                // Define some weights to the edges
                costs = new Dictionary<IEdge<string>, double>(g.EdgeCount)
                {
                    [a_b] = 4,
                    [a_d] = 1,
                    [b_a] = 74,
                    [b_c] = 2,
                    [b_e] = 12,
                    [c_b] = 12,
                    [c_f] = 74,
                    [c_j] = 12,
                    [d_e] = 32,
                    [d_g] = 22,
                    [e_d] = 66,
                    [e_f] = 76,
                    [e_h] = 33,
                    [f_i] = 11,
                    [f_j] = 21,
                    [g_d] = 12,
                    [g_h] = 10,
                    [h_g] = 2,
                    [h_i] = 72,
                    [i_f] = 31,
                    [i_h] = 18,
                    [i_j] = 7,
                    [j_f] = 8
                };

                return g;
            }

            #endregion
        }

        [Pure]
        [NotNull]
        public static DijkstraShortestPathAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(Edge.Create));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Weights(IEdge<T> e) => 1.0;
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(Weights);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}