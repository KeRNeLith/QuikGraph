using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;
using static QuikGraph.Tests.Algorithms.AlgorithmTestHelpers;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class DijkstraShortestPathAlgorithmTests : ShortestPathAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunDijkstraAndCheck<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>(graph.EdgeCount);
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.OutDegree(edge.Source) + 1;

            var algorithm = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(
                graph,
                e => distances[e]);

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

            Assert.IsNotNull(algorithm.Distances);
            Assert.AreEqual(graph.VertexCount, algorithm.Distances.Count);

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
            Func<Edge<int>, double> Weights = e => 1.0;

            var graph = new AdjacencyGraph<int, Edge<int>>();
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, Weights);
            AssertAlgorithmProperties(algorithm, graph, Weights);

            algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, graph, Weights, DistanceRelaxers.CriticalDistance);
            AssertAlgorithmProperties(algorithm, graph, Weights, DistanceRelaxers.CriticalDistance);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                DijkstraShortestPathAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g,
                Func<TEdge, double> eWeights = null,
                IDistanceRelaxer relaxer = null)
                where TEdge : IEdge<TVertex>
            {
                AssertAlgorithmState(algo, g);
                Assert.IsNull(algo.VerticesColors);
                if (eWeights is null)
                    Assert.IsNotNull(algo.Weights);
                else
                    Assert.AreSame(eWeights, algo.Weights);
                Assert.IsNull(algo.Distances);
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
            var graph = new AdjacencyGraph<int, Edge<int>>();

            Func<Edge<int>, double> Weights = e => 1.0;

            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, Weights));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null));

            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, Weights, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, graph, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, graph, Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, null, DistanceRelaxers.CriticalDistance));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, Weights, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, graph, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new DijkstraShortestPathAlgorithm<TestVertex, Edge<TestVertex>>(graph, edge => 1.0);
            SetRootVertex_Throws_Test(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            ClearRootVertex_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            ComputeWithoutRoot_NoThrows_Test(
                graph,
                () => new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVertex(0);
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            ComputeWithRoot_Throws_Test(
                () => new DijkstraShortestPathAlgorithm<TestVertex, Edge<TestVertex>>(graph, edge => 1.0));
        }

        #endregion

        #region Shortest path algorithm

        [Test]
        public void TryGetDistance()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVertex(1);
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            TryGetDistance_Test(algorithm);
        }

        [Test]
        public void TryGetDistance_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new DijkstraShortestPathAlgorithm<TestVertex, Edge<TestVertex>>(graph, edge => 1.0);
            TryGetDistance_Throws_Test(algorithm);
        }

        #endregion

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdge(new Edge<int>(1, 2));

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, edge => 1.0);
            algorithm.Compute(1);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(2));
        }

        [Test]
        [Category(TestCategories.LongRunning)]
        public void Dijkstra()
        {
            foreach (AdjacencyGraph<string, Edge<string>> graph in TestGraphFactory.GetAdjacencyGraphs_SlowTests())
            {
                foreach (string root in graph.Vertices)
                    RunDijkstraAndCheck(graph, root);
            }
        }

        [Test]
        public void Dijkstra_Throws()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge23 = new Edge<int>(2, 3);
            var edge34 = new Edge<int>(3, 4);

            var negativeWeightGraph = new AdjacencyGraph<int, Edge<int>>();
            negativeWeightGraph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge23, edge34
            });

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(
                negativeWeightGraph,
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
            var graph = new AdjacencyGraph<string, Edge<string>>(true);

            // Add some vertices to the graph
            graph.AddVertex("A");
            graph.AddVertex("B");
            graph.AddVertex("D");
            graph.AddVertex("C");
            graph.AddVertex("E");

            // Create the edges
            // ReSharper disable InconsistentNaming
            var a_b = new Edge<string>("A", "B");
            var a_c = new Edge<string>("A", "C");
            var b_e = new Edge<string>("B", "E");
            var c_d = new Edge<string>("C", "D");
            var d_e = new Edge<string>("D", "E");
            // ReSharper restore InconsistentNaming

            // Add edges to the graph
            graph.AddEdge(a_b);
            graph.AddEdge(a_c);
            graph.AddEdge(c_d);
            graph.AddEdge(d_e);
            graph.AddEdge(b_e);

            // Define some weights to the edges
            var weight = new Dictionary<Edge<string>, double>(graph.EdgeCount)
            {
                [a_b] = 30,
                [a_c] = 30,
                [b_e] = 60,
                [c_d] = 40,
                [d_e] = 4
            };

            var algorithm = new DijkstraShortestPathAlgorithm<string, Edge<string>>(graph, e => weight[e]);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            var predecessorObserver = new VertexPredecessorRecorderObserver<string, Edge<string>>();
            using (predecessorObserver.Attach(algorithm))
                // Run the algorithm with A set to be the source
                algorithm.Compute("A");

            Assert.AreEqual(74, algorithm.Distances["E"], double.Epsilon);
        }

        [Test]
        public void DijkstraSimpleGraph2()
        {
            var graph = new AdjacencyGraph<char, Edge<char>>();
            var distances = new Dictionary<Edge<char>, double>();

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

            var algorithm = new DijkstraShortestPathAlgorithm<char, Edge<char>>(graph, AlgorithmExtensions.GetIndexer(distances));
            var predecessors = new VertexPredecessorRecorderObserver<char, Edge<char>>();
            using (predecessors.Attach(algorithm))
                algorithm.Compute('A');

            Assert.AreEqual(0, algorithm.Distances['A']);
            Assert.AreEqual(6, algorithm.Distances['B']);
            Assert.AreEqual(1, algorithm.Distances['C']);
            Assert.AreEqual(4, algorithm.Distances['D']);
            Assert.AreEqual(5, algorithm.Distances['E']);

            #region Local function

            void AddEdge(char source, char target, double weight)
            {
                var edge = new Edge<char>(source, target);
                distances[edge] = weight;
                graph.AddEdge(edge);
            }

            #endregion
        }

        [Test]
        [Category(TestCategories.CISkip)]
        public void DijkstraRepro12359()
        {
            AdjacencyGraph<string, Edge<string>> graph = TestGraphFactory.LoadGraph(GetGraphFilePath("repro12359.graphml"));
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
            var graph = new AdjacencyGraph<int, Edge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            graph.AddEdge(new Edge<int>(1, 2));
            graph.AddEdge(new Edge<int>(2, 3));

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, e => 1);
            algorithm.Compute(1);

            Assert.AreEqual(0d, algorithm.Distances[1]);
            Assert.AreEqual(2d, algorithm.Distances[3]);
            Assert.AreEqual(1d, algorithm.Distances[2]);
        }

        [Test]
        public void PredecessorsLineGraph()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = new Edge<int>(1, 2); graph.AddEdge(e12);
            var e23 = new Edge<int>(2, 3); graph.AddEdge(e23);

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, e => 1);
            var vis = new VertexPredecessorRecorderObserver<int, Edge<int>>();
            using (vis.Attach(algorithm))
                algorithm.Compute(1);

            Assert.IsTrue(vis.TryGetPath(2, out IEnumerable<Edge<int>> path));
            Edge<int>[] pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);

            Assert.IsTrue(vis.TryGetPath(3, out path));
            pathArray = path.ToArray();
            Assert.AreEqual(2, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);
            Assert.AreEqual(e23, pathArray[1]);
        }

        [Test]
        public void DoubleLineGraph()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = new Edge<int>(1, 2); graph.AddEdge(e12);
            var e23 = new Edge<int>(2, 3); graph.AddEdge(e23);
            var e13 = new Edge<int>(1, 3); graph.AddEdge(e13);

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, e => 1);
            algorithm.Compute(1);

            Assert.AreEqual(0.0, algorithm.Distances[1]);
            Assert.AreEqual(1.0, algorithm.Distances[2]);
            Assert.AreEqual(1.0, algorithm.Distances[3]);
        }

        [Test]
        public void PredecessorsDoubleLineGraph()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>(true);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            var e12 = new Edge<int>(1, 2); graph.AddEdge(e12);
            var e23 = new Edge<int>(2, 3); graph.AddEdge(e23);
            var e13 = new Edge<int>(1, 3); graph.AddEdge(e13);

            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, e => 1);
            var vis = new VertexPredecessorRecorderObserver<int, Edge<int>>();
            using (vis.Attach(algorithm))
                algorithm.Compute(1);

            Assert.IsTrue(vis.TryGetPath(2, out IEnumerable<Edge<int>> path));
            Edge<int>[] pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e12, pathArray[0]);

            Assert.IsTrue(vis.TryGetPath(3, out path));
            pathArray = path.ToArray();
            Assert.AreEqual(1, pathArray.Length);
            Assert.AreEqual(e13, pathArray[0]);
        }

        [Test]
        [Category(TestCategories.Verbose)]
        public void Scenario()
        {
            AdjacencyGraph<string, Edge<string>> graph = CreateGraph(out Dictionary<Edge<string>, double> edgeCosts);

            // Run Dijkstra on this graph
            var dijkstra = new DijkstraShortestPathAlgorithm<string, Edge<string>>(graph, e => edgeCosts[e]);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            var predecessorObserver = new VertexPredecessorRecorderObserver<string, Edge<string>>();
            using (predecessorObserver.Attach(dijkstra))
            {
                // Run the algorithm with A as source
                dijkstra.Compute("A");
            }

            foreach (KeyValuePair<string, Edge<string>> pair in predecessorObserver.VerticesPredecessors)
                Console.WriteLine($"If you want to get to {pair.Key} you have to enter through the in edge {pair.Value}.");

            foreach (string vertex in graph.Vertices)
            {
                double distance = AlgorithmExtensions.ComputePredecessorCost(
                    predecessorObserver.VerticesPredecessors,
                    edgeCosts,
                    vertex);
                Console.WriteLine($"A -> {vertex}: {distance}");
            }

            #region Local function

            AdjacencyGraph<string, Edge<string>> CreateGraph(out Dictionary<Edge<string>, double> costs)
            {
                var g = new AdjacencyGraph<string, Edge<string>>(true);

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
                var a_b = new Edge<string>("A", "B");
                var a_d = new Edge<string>("A", "D");
                var b_a = new Edge<string>("B", "A");
                var b_c = new Edge<string>("B", "C");
                var b_e = new Edge<string>("B", "E");
                var c_b = new Edge<string>("C", "B");
                var c_f = new Edge<string>("C", "F");
                var c_j = new Edge<string>("C", "J");
                var d_e = new Edge<string>("D", "E");
                var d_g = new Edge<string>("D", "G");
                var e_d = new Edge<string>("E", "D");
                var e_f = new Edge<string>("E", "F");
                var e_h = new Edge<string>("E", "H");
                var f_i = new Edge<string>("F", "I");
                var f_j = new Edge<string>("F", "J");
                var g_d = new Edge<string>("G", "D");
                var g_h = new Edge<string>("G", "H");
                var h_g = new Edge<string>("H", "G");
                var h_i = new Edge<string>("H", "I");
                var i_f = new Edge<string>("I", "F");
                var i_j = new Edge<string>("I", "J");
                var i_h = new Edge<string>("I", "H");
                var j_f = new Edge<string>("J", "F");
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
                costs = new Dictionary<Edge<string>, double>(g.EdgeCount)
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
        public static DijkstraShortestPathAlgorithm<int, Edge<int>> CreateAlgorithmAndMaybeDoComputation(
            [NotNull] ContractScenario scenario)
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(e => new Edge<int>(e.Source, e.Target)));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Weights(Edge<int> e) => 1.0;
            var algorithm = new DijkstraShortestPathAlgorithm<int, Edge<int>>(graph, Weights);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root);
            return algorithm;
        }
    }
}