using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MaximumFlow;


namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdmondsKarpMaximumFlowAlgorithmTests
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
            var reversedEdgeAugmentorAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reversedEdgeAugmentorAlgorithm.AddReversedEdges();

            double flow = graph.MaximumFlow(
                _ => 1,
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
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            Func<IEdge<int>, double> capacities = _ => 1.0;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm);
            AssertAlgorithmProperties(algorithm, graph, capacities, edgeFactory);

            algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm);
            AssertAlgorithmProperties(algorithm, graph, capacities, edgeFactory);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge> algo,
                IMutableVertexAndEdgeListGraph<TVertex, TEdge> g,
                Func<TEdge, double> c,
                EdgeFactory<int, IEdge<int>> eFactory)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
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
            AdjacencyGraph<int, IEdge<int>> graphNull = null, graph1 = new();
            var graph2 = new AdjacencyGraph<int, IEdge<int>>();
            Func<IEdge<int>, double> capacities = _ => 1.0;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var reverseEdgesAlgorithm1 = graph1.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            var reverseEdgesAlgorithm2 = graph2.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, reverseEdgesAlgorithm1));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, null, null));
            Assert.Throws<ArgumentNullException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateEdmondsKarpMaximumFlowAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            Assert.Throws<ArgumentException>(() => graph1.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm2));
            Assert.Throws<ArgumentException>(() => graph2.CreateEdmondsKarpMaximumFlowAlgorithm(capacities, edgeFactory, reverseEdgesAlgorithm1));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void SimpleFlow()
        {
            const string source = "A";
            const string sink = "G";

            var graph = new AdjacencyGraph<string, EquatableTaggedEdge<string, double>>(true);
            graph.AddVertexRange( "A", "B", "C", "D", "E", "F", "G" );

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddEdgeRange(
                new EquatableTaggedEdge<string, double>[] {
                new ("A", "D", 3),
                new ("A", "B", 3),
                new ("B", "C", 4),
                new ("C", "A", 3),
                new ("C", "D", 1),
                new ("D", "E", 2),
                new ("D", "F", 6),
                new ("E", "B", 1),
                new ("C", "E", 2),
                new ("E", "G", 1),
                new ("F", "G", 9)
                });

            // edgeFactory will be used to create the reversed edges to store residual capacities using the ReversedEdgeAugmentorAlgorithm-class.
            // The edgeFactory assigns a capacity of 0.0 for the new edges because the initial (residual) capacity must be 0.0.
            EdgeFactory<string, EquatableTaggedEdge<string, double>> edgeFactory = (sourceNode, targetNode) => new EquatableTaggedEdge<string, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

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
            graph.AddVertexRange( "A", "B", "C", "D", "E", "F", "G" );

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddEdgeRange(new TaggedEdge<string, double>[]
            {
                new ("A", "D", 3),
                new ("A", "B", 3),
                new ("B", "C", 4),
                new ("C", "A", 3),
                new ("C", "D", 1),
                new ("D", "E", 2),
                new ("D", "F", 6),
                new ("E", "B", 1),
                new ("C", "E", 2)
            });

            // edgeFactory will be used to create the reversed edges to store residual capacities using the ReversedEdgeAugmentorAlgorithm-class.
            // The edgeFactory assigns a capacity of 0.0 for the new edges because the initial (residual) capacity must be 0.0.
            EdgeFactory<string, TaggedEdge<string, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<string, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

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
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVertex(3);

            Func<IEdge<int>, double> capacities = _ => 1.0;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm( capacities, edgeFactory, reverseEdgesAlgorithm);
            algorithm.Compute(1, 2);

            Assert.AreEqual(GraphColor.Black, algorithm.GetVertexColor(1));
            Assert.AreEqual(GraphColor.White, algorithm.GetVertexColor(2));
            Assert.AreEqual(GraphColor.White, algorithm.GetVertexColor(3));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { 100 })]
        [Category(TestCategories.LongRunning)]
        public void EdmondsKarpMaxFlow(AdjacencyGraph<string, IEdge<string>> graph)
        {
            if (graph.VertexCount > 1)
                EdmondsKarpMaxFlow(graph, Edge.Create);
        }

        [Test]
        public void EdmondsKarpMaxFlow_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, TaggedEdge<TestVertex, double>>();
            EdgeFactory<TestVertex, TaggedEdge<TestVertex, double>> edgeFactory = (source, target) => new TaggedEdge<TestVertex, double>(source, target, 0.0);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

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
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            Assert.Throws<InvalidOperationException>(() => algorithm.Compute());

            algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);
            algorithm.Source = vertex1;

            Assert.Throws<InvalidOperationException>(() => algorithm.Compute());

            algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);
            algorithm.Source = vertex1;
            algorithm.Sink = vertex2;
            Assert.Throws<Exception>(() => algorithm.Compute());

            algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);
            {
                algorithm.Source = vertex1;
                algorithm.Sink = vertex2;
            }
            graph.AddVertex(vertex1);
            Assert.Throws<Exception>(() => algorithm.Compute());
        }

        [Test]
        public void EdmondsKarpMaxFlow_NegativeCapacity_Throws()
        {
            const int source = 1;
            const int sink = 4;

            var graph = new AdjacencyGraph<int, TaggedEdge<int, double>>();
            
            // TaggedEdge.Tag is the capacity of the edge
            graph.AddVerticesAndEdgeRange(
                new TaggedEdge<int, double>(1, 2, 3),
                new TaggedEdge<int, double>(1, 4, 4),
                new TaggedEdge<int, double>(2, 3, -1),
                new TaggedEdge<int, double>(3, 4, 1)
            );

            EdgeFactory<int, TaggedEdge<int, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<int, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            Assert.Throws<NegativeCapacityException>(() => algorithm.Compute(source, sink));
        }

        [Test]
        public void EdmondsKarpMaxFlow_NotAugmented_Throws()
        {
            const int source = 1;
            const int sink = 4;

            var graph = new AdjacencyGraph<int, TaggedEdge<int, double>>();

            // TaggedEdge.Tag is the capacity of the edge
            graph.AddVerticesAndEdgeRange(
                new TaggedEdge<int, double>(1, 2, 3),
                new TaggedEdge<int, double>(1, 4, 4),
                new TaggedEdge<int, double>(2, 3, -1),
                new TaggedEdge<int, double>(3, 4, 1)
            );

            EdgeFactory<int, TaggedEdge<int, double>> edgeFactory = (sourceNode, targetNode) => new TaggedEdge<int, double>(sourceNode, targetNode, 0.0);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edge => edge.Tag, edgeFactory, reverseEdgesAlgorithm);

            Assert.Throws<InvalidOperationException>(() => algorithm.Compute(source, sink));
        }

        [Pure]
        [NotNull]
        public static EdmondsKarpMaximumFlowAlgorithm<T, IEdge<T>> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var graph = new AdjacencyGraph<T, IEdge<T>>();
            graph.AddVerticesAndEdgeRange(scenario.EdgesInGraph.Select(e => Edge.Create(e.Source, e.Target)));
            graph.AddVertexRange(scenario.SingleVerticesInGraph);

            double Capacities(IEdge<T> edge) => 1.0;
            IEdge<T> EdgeFactory(T source, T target) => Edge.Create(source, target);
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(EdgeFactory);
            reverseEdgesAlgorithm.AddReversedEdges();

            var algorithm = graph.CreateEdmondsKarpMaximumFlowAlgorithm(Capacities, EdgeFactory, reverseEdgesAlgorithm);

            if (scenario.DoComputation)
                algorithm.Compute(scenario.Root, scenario.AccessibleVerticesFromRoot.First());
            return algorithm;
        }
    }
}