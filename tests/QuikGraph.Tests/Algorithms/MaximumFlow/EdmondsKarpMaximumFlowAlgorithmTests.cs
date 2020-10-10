using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MaximumFlow;
using static QuikGraph.Tests.Algorithms.AlgorithmTestHelpers;

namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class EdmondsKarpMaximumFlowAlgorithmTests
    {
        #region Test helpers

        private static void EdmondsKarpMaxFlow<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.VertexCount > 0);

            foreach (TVertex source in graph.Vertices)
            {
                foreach (TVertex sink in graph.Vertices)
                {
                    if (source.Equals(sink))
                        continue;

                    Assert.Positive(RunMaxFlowAlgorithmAndCheck(graph, edgeFactory, source, sink));
                }
            }
        }

        private static double RunMaxFlowAlgorithmAndCheck<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] TVertex source,
            [NotNull] TVertex sink)
            where TEdge : IEdge<TVertex>
        {
            var reversedEdgeAugmentorAlgorithm = new ReversedEdgeAugmentorAlgorithm<TVertex, TEdge>(graph, edgeFactory);
            reversedEdgeAugmentorAlgorithm.AddReversedEdges();

            double flow = graph.MaximumFlow(
                edge => 1,
                source, sink,
                out _,
                edgeFactory,
                reversedEdgeAugmentorAlgorithm);

            reversedEdgeAugmentorAlgorithm.RemoveReversedEdges();

            return flow;
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            Func<Edge<int>, double> capacities = edge => 1.0;
            EdgeFactory<int, Edge<int>> edgeFactory = (source, target) => new Edge<int>(source, target);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<int, Edge<int>>(graph, edgeFactory);

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(
                graph,
                capacities,
                edgeFactory,
                reverseEdgesAlgorithm);
            AssertAlgorithmProperties(
                algorithm,
                graph,
                capacities,
                edgeFactory);

            algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(
                null,
                graph,
                capacities,
                edgeFactory,
                reverseEdgesAlgorithm);
            AssertAlgorithmProperties(
                algorithm,
                graph,
                capacities,
                edgeFactory);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge> algo,
                IMutableVertexAndEdgeListGraph<TVertex, TEdge> g,
                Func<TEdge, double> c,
                EdgeFactory<int, Edge<int>> eFactory)
                where TEdge : IEdge<TVertex>
            {
                AssertAlgorithmState(algo, g);
                CollectionAssert.IsEmpty(algo.Predecessors);
                Assert.AreSame(c, algo.Capacities);
                CollectionAssert.IsEmpty(algo.ResidualCapacities);
                if (eFactory is null)
                    Assert.IsNotNull(algo.EdgeFactory);
                else
                    Assert.AreSame(eFactory, algo.EdgeFactory);
                CollectionAssert.IsEmpty(algo.ReversedEdges);
                Assert.AreEqual(default(TVertex), algo.Source);
                Assert.AreEqual(default(TVertex), algo.Sink);
                Assert.AreEqual(0.0, algo.MaxFlow);
                CollectionAssert.IsEmpty(algo.VerticesColors);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph1 = new AdjacencyGraph<int, Edge<int>>();
            var graph2 = new AdjacencyGraph<int, Edge<int>>();
            Func<Edge<int>, double> capacities = edge => 1.0;
            EdgeFactory<int, Edge<int>> edgeFactory = (source, target) => new Edge<int>(source, target);
            var reverseEdgesAlgorithm1 = new ReversedEdgeAugmentorAlgorithm<int, Edge<int>>(graph1, edgeFactory);
            var reverseEdgesAlgorithm2 = new ReversedEdgeAugmentorAlgorithm<int, Edge<int>>(graph2, edgeFactory);

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, capacities, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, capacities, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, capacities, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, capacities, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, capacities, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, capacities, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, graph1, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(null, null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph1, capacities, edgeFactory, reverseEdgesAlgorithm2));
            Assert.Throws<ArgumentException>(
                () => new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph2, capacities, edgeFactory, reverseEdgesAlgorithm1));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void SimpleFlow()
        {
            const string source = "A";
            const string sink = "G";

            var graph = new AdjacencyGraph<string, EquatableTaggedEdge<string, double>>(true);
            graph.AddVertexRange(new[] { "A", "B", "C", "D", "E", "F", "G" });

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddEdgeRange(new[]
            {
                new EquatableTaggedEdge<string, double>("A", "D", 3),
                new EquatableTaggedEdge<string, double>("A", "B", 3),
                new EquatableTaggedEdge<string, double>("B", "C", 4),
                new EquatableTaggedEdge<string, double>("C", "A", 3),
                new EquatableTaggedEdge<string, double>("C", "D", 1),
                new EquatableTaggedEdge<string, double>("D", "E", 2),
                new EquatableTaggedEdge<string, double>("D", "F", 6),
                new EquatableTaggedEdge<string, double>("E", "B", 1),
                new EquatableTaggedEdge<string, double>("C", "E", 2),
                new EquatableTaggedEdge<string, double>("E", "G", 1),
                new EquatableTaggedEdge<string, double>("F", "G", 9)
            });

            // edgeFactory will be used to create the reversed edges to store residual capacities using the ReversedEdgeAugmentorAlgorithm-class.
            // The edgeFactory assigns a capacity of 0.0 for the new edges because the initial (residual) capacity must be 0.0.
            EdgeFactory<string, EquatableTaggedEdge<string, double>> edgeFactory = (sourceNode, targetNode) => new EquatableTaggedEdge<string, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<string, EquatableTaggedEdge<string, double>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<string, EquatableTaggedEdge<string, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            algorithm.Compute(source, sink);

            Assert.AreEqual(source, algorithm.Source);
            Assert.AreEqual(sink, algorithm.Sink);
            Assert.AreEqual(5, algorithm.MaxFlow);
            CheckReversedEdges();
            CheckPredecessors();
            CheckResidualCapacities();

            #region Local function

            void CheckReversedEdges()
            {
                Assert.IsTrue(algorithm.ReversedEdges.Count % 2 == 0);
                foreach (var pair in algorithm.ReversedEdges)
                {
                    Assert.AreEqual(pair.Key.Source, pair.Value.Target);
                    Assert.AreEqual(pair.Key.Target, pair.Value.Source);
                }
            }

            void CheckPredecessors()
            {
                Assert.AreEqual(graph.VertexCount - 1, algorithm.Predecessors.Count);
                CollectionAssert.AreEquivalent(
                    new Dictionary<string, EquatableTaggedEdge<string, double>>
                    {
                        ["B"] = new EquatableTaggedEdge<string, double>("A", "B", 3),
                        ["C"] = new EquatableTaggedEdge<string, double>("B", "C", 4),
                        ["D"] = new EquatableTaggedEdge<string, double>("E", "D", 0),
                        ["E"] = new EquatableTaggedEdge<string, double>("C", "E", 2),
                        ["F"] = new EquatableTaggedEdge<string, double>("D", "F", 6),
                        ["G"] = new EquatableTaggedEdge<string, double>("F", "G", 9),
                    },
                    algorithm.Predecessors);
            }

            void CheckResidualCapacities()
            {
                Assert.AreEqual(graph.EdgeCount, algorithm.ResidualCapacities.Count);
                CollectionAssert.AreEquivalent(
                    new Dictionary<EquatableTaggedEdge<string, double>, double>
                    {
                        [new EquatableTaggedEdge<string, double>("A", "B", 3)] = 1,
                        [new EquatableTaggedEdge<string, double>("A", "C", 0)] = 0,
                        [new EquatableTaggedEdge<string, double>("A", "D", 3)] = 0,
                        [new EquatableTaggedEdge<string, double>("B", "A", 0)] = 2,
                        [new EquatableTaggedEdge<string, double>("B", "C", 4)] = 2,
                        [new EquatableTaggedEdge<string, double>("B", "E", 0)] = 0,
                        [new EquatableTaggedEdge<string, double>("C", "A", 3)] = 3,
                        [new EquatableTaggedEdge<string, double>("C", "B", 0)] = 2,
                        [new EquatableTaggedEdge<string, double>("C", "D", 1)] = 0,
                        [new EquatableTaggedEdge<string, double>("C", "E", 2)] = 1,
                        [new EquatableTaggedEdge<string, double>("D", "A", 0)] = 3,
                        [new EquatableTaggedEdge<string, double>("D", "C", 0)] = 1,
                        [new EquatableTaggedEdge<string, double>("D", "E", 2)] = 2,
                        [new EquatableTaggedEdge<string, double>("D", "F", 6)] = 2,
                        [new EquatableTaggedEdge<string, double>("E", "B", 1)] = 1,
                        [new EquatableTaggedEdge<string, double>("E", "C", 0)] = 1,
                        [new EquatableTaggedEdge<string, double>("E", "D", 0)] = 0,
                        [new EquatableTaggedEdge<string, double>("E", "G", 1)] = 0,
                        [new EquatableTaggedEdge<string, double>("F", "D", 0)] = 4,
                        [new EquatableTaggedEdge<string, double>("F", "G", 9)] = 5,
                        [new EquatableTaggedEdge<string, double>("G", "E", 0)] = 1,
                        [new EquatableTaggedEdge<string, double>("G", "F", 0)] = 4,
                    },
                    algorithm.ResidualCapacities);
            }

            #endregion
        }

        [Test]
        public void NotReachableSink()
        {
            const string source = "A";
            const string sink = "G";

            var graph = new AdjacencyGraph<string, TaggedEdge<string, double>>(true);
            graph.AddVertexRange(new[] { "A", "B", "C", "D", "E", "F", "G" });

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddEdgeRange(new[]
            {
                new TaggedEdge<string, double>("A", "D", 3),
                new TaggedEdge<string, double>("A", "B", 3),
                new TaggedEdge<string, double>("B", "C", 4),
                new TaggedEdge<string, double>("C", "A", 3),
                new TaggedEdge<string, double>("C", "D", 1),
                new TaggedEdge<string, double>("D", "E", 2),
                new TaggedEdge<string, double>("D", "F", 6),
                new TaggedEdge<string, double>("E", "B", 1),
                new TaggedEdge<string, double>("C", "E", 2)
            });

            // edgeFactory will be used to create the reversed edges to store residual capacities using the ReversedEdgeAugmentorAlgorithm-class.
            // The edgeFactory assigns a capacity of 0.0 for the new edges because the initial (residual) capacity must be 0.0.
            EdgeFactory<string, TaggedEdge<string, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<string, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<string, TaggedEdge<string, double>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<string, TaggedEdge<string, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            algorithm.Compute(source, sink);

            Assert.AreEqual(source, algorithm.Source);
            Assert.AreEqual(sink, algorithm.Sink);
            Assert.AreEqual(graph.VertexCount, algorithm.VerticesColors.Count);
            foreach (KeyValuePair<string, GraphColor> pair in algorithm.VerticesColors)
            {
                Assert.AreEqual(
                    pair.Key == sink ? GraphColor.White : GraphColor.Black,
                    pair.Value);
            }
            Assert.AreEqual(0, algorithm.MaxFlow);
        }

        [Test]
        public void GetVertexColor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdge(new Edge<int>(1, 2));
            graph.AddVertex(3);

            Func<Edge<int>, double> capacities = edge => 1.0;
            EdgeFactory<int, Edge<int>> edgeFactory = (source, target) => new Edge<int>(source, target);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<int, Edge<int>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph, capacities, edgeFactory, reverseEdgesAlgorithm);
            algorithm.Compute(1, 2);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.White, algorithm.GetVertexColor(2));
            Assert.AreEqual(GraphColor.White, algorithm.GetVertexColor(3));
        }

        [Test]
        [Category(TestCategories.LongRunning)]
        public void EdmondsKarpMaxFlow()
        {
            foreach (AdjacencyGraph<string, Edge<string>> graph in TestGraphFactory.GetAdjacencyGraphs_SlowTests(100))
            {
                if (graph.VertexCount > 1)
                    EdmondsKarpMaxFlow(graph, (source, target) => new Edge<string>(source, target));
            }
        }

        [Test]
        public void EdmondsKarpMaxFlow_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, TaggedEdge<TestVertex, double>>();
            EdgeFactory<TestVertex, TaggedEdge<TestVertex, double>> edgeFactory = (source, target) => new TaggedEdge<TestVertex, double>(source, target, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            var vertex = new TestVertex("1");
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, vertex));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(vertex, null));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void EdmondsKarpMaxFlow_WrongVertices_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, TaggedEdge<TestVertex, double>>();
            EdgeFactory<TestVertex, TaggedEdge<TestVertex, double>> edgeFactory = (source, target) => new TaggedEdge<TestVertex, double>(source, target, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            Assert.Throws<InvalidOperationException>(() => algorithm.Compute());

            algorithm = new EdmondsKarpMaximumFlowAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(
                graph,
                edge => edge.Tag,
                edgeFactory,
                reverseEdgesAlgorithm)
            {
                Source = vertex1
            };
            Assert.Throws<InvalidOperationException>(() => algorithm.Compute());

            algorithm = new EdmondsKarpMaximumFlowAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(
                graph,
                edge => edge.Tag,
                edgeFactory,
                reverseEdgesAlgorithm)
            {
                Source = vertex1,
                Sink = vertex2
            };
            Assert.Throws<VertexNotFoundException>(() => algorithm.Compute());

            algorithm = new EdmondsKarpMaximumFlowAlgorithm<TestVertex, TaggedEdge<TestVertex, double>>(
                graph,
                edge => edge.Tag,
                edgeFactory,
                reverseEdgesAlgorithm)
            {
                Source = vertex1,
                Sink = vertex2
            };
            graph.AddVertex(vertex1);
            Assert.Throws<VertexNotFoundException>(() => algorithm.Compute());
        }

        [Test]
        public void EdmondsKarpMaxFlow_NegativeCapacity_Throws()
        {
            const int source = 1;
            const int sink = 4;

            var graph = new AdjacencyGraph<int, TaggedEdge<int, double>>();
            
            // TaggedEdge.Tag is the capacity of the edge
            graph.AddVerticesAndEdgeRange(new[]
            {
                new TaggedEdge<int, double>(1, 2, 3),
                new TaggedEdge<int, double>(1, 4, 4),
                new TaggedEdge<int, double>(2, 3, -1),
                new TaggedEdge<int, double>(3, 4, 1)
            });

            EdgeFactory<int, TaggedEdge<int, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<int, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<int, TaggedEdge<int, double>>(graph, edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, TaggedEdge<int, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            Assert.Throws<NegativeCapacityException>(() => algorithm.Compute(source, sink));
        }

        [Test]
        public void EdmondsKarpMaxFlow_NotAugmented_Throws()
        {
            const int source = 1;
            const int sink = 4;

            var graph = new AdjacencyGraph<int, TaggedEdge<int, double>>();

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddVerticesAndEdgeRange(new[]
            {
                new TaggedEdge<int, double>(1, 2, 3),
                new TaggedEdge<int, double>(1, 4, 4),
                new TaggedEdge<int, double>(2, 3, -1),
                new TaggedEdge<int, double>(3, 4, 1)
            });

            EdgeFactory<int, TaggedEdge<int, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<int, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<int, TaggedEdge<int, double>>(graph, edgeFactory);
            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, TaggedEdge<int, double>>(graph, edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            Assert.Throws<InvalidOperationException>(() => algorithm.Compute(source, sink));
        }

        [Pure]
        [NotNull]
        public static EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>> CreateAlgorithmAndMaybeDoComputation(
            [NotNull] ContractScenario scenario)
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(e => new Edge<int>(e.Source, e.Target)));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Capacities(Edge<int> edge) => 1.0;
            Edge<int> EdgeFactory(int source, int target) => new Edge<int>(source, target);
            var reverseEdgesAlgorithm = new ReversedEdgeAugmentorAlgorithm<int, Edge<int>>(graph, EdgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(graph, Capacities, EdgeFactory, reverseEdgesAlgorithm);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root, scenario.AccessibleVerticesFromRoot.First());
            return algorithm;
        }
    }
}