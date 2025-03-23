using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.MaximumFlow;

namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="GraphBalancerAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class GraphBalancingAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2 );
            graph.AddVerticesAndEdge(Edge.Create(1, 3));
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var capacities = new Dictionary<IEdge<int>, double>();

            var algorithm = graph.CreateGraphBalancerAlgorithm(1, 2, vertexFactory, edgeFactory);
            Assert.AreSame(graph, algorithm.VisitedGraph);
            Assert.AreSame(vertexFactory, algorithm.VertexFactory);
            Assert.AreSame(edgeFactory, algorithm.EdgeFactory);
            Assert.IsFalse(algorithm.Balanced);
            Assert.AreEqual(1, algorithm.Source);
            Assert.AreEqual(2, algorithm.Sink);
            Assert.IsNotNull(algorithm.Capacities);
            Assert.AreEqual(graph.EdgeCount, algorithm.Capacities.Count);
            CollectionAssert.IsEmpty(algorithm.SurplusVertices);
            CollectionAssert.IsEmpty(algorithm.SurplusEdges);
            CollectionAssert.IsEmpty(algorithm.DeficientVertices);
            CollectionAssert.IsEmpty(algorithm.DeficientEdges);
            Assert.AreEqual(default(int), algorithm.BalancingSource);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSourceEdge);
            Assert.AreEqual(default(int), algorithm.BalancingSink);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSinkEdge);

            algorithm = graph.CreateGraphBalancerAlgorithm(1, 2, vertexFactory, edgeFactory, capacities);
            Assert.AreSame(graph, algorithm.VisitedGraph);
            Assert.AreSame(vertexFactory, algorithm.VertexFactory);
            Assert.AreSame(edgeFactory, algorithm.EdgeFactory);
            Assert.IsFalse(algorithm.Balanced);
            Assert.AreEqual(1, algorithm.Source);
            Assert.AreEqual(2, algorithm.Sink);
            Assert.AreSame(capacities, algorithm.Capacities);
            CollectionAssert.IsEmpty(algorithm.SurplusVertices);
            CollectionAssert.IsEmpty(algorithm.SurplusEdges);
            CollectionAssert.IsEmpty(algorithm.DeficientVertices);
            CollectionAssert.IsEmpty(algorithm.DeficientEdges);
            Assert.AreEqual(default(int), algorithm.BalancingSource);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSourceEdge);
            Assert.AreEqual(default(int), algorithm.BalancingSink);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSinkEdge);
        }

        [Test]
        public void Constructor_Throws()
        {
            TestVertex vertex1 = new ("1");
            TestVertex vertex2 = new ("2");

            BidirectionalGraph<TestVertex, IEdge<TestVertex>> graph = new (), nullGraph = null;
            var graphWithVertex1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            graphWithVertex1.AddVertex(vertex1);
            VertexFactory<TestVertex> TestVertexFactory = () => new TestVertex();
            EdgeFactory<TestVertex, IEdge<TestVertex>> edgeFactory = Edge.Create;
            var capacities = new Dictionary<IEdge<TestVertex>, double>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, null));

            Assert.Throws<ArgumentException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory));
            Assert.Throws<ArgumentException>(() => graphWithVertex1.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory));

            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, vertex2, null, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, TestVertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, vertex2, null, null, capacities));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(vertex1, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateGraphBalancerAlgorithm(null, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateGraphBalancerAlgorithm(null, null, null, null, null));

            Assert.Throws<ArgumentException>(() => graph.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory, capacities));
            Assert.Throws<ArgumentException>(() => graphWithVertex1.CreateGraphBalancerAlgorithm(vertex1, vertex2, TestVertexFactory, edgeFactory, capacities));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Balance()
        {
            const int source = 1;
            const int sink = 3;
            var edge12 = new EquatableEdge<int>(1, 2);
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge23 = new EquatableEdge<int>(2, 3);
            var edge32 = new EquatableEdge<int>(3, 2);
            var edge34 = new EquatableEdge<int>(3, 4);
            var edge35 = new EquatableEdge<int>(3, 5);
            var edge42 = new EquatableEdge<int>(4, 2);
            var edge55 = new EquatableEdge<int>(5, 5);
            var edge67 = new EquatableEdge<int>(6, 7);
            var edge78 = new EquatableEdge<int>(7, 8);

            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge32, edge34,
                edge35, edge42, edge55, edge67, edge78
            );
            int vertexID = 9;
            VertexFactory<int> vertexFactory = () => vertexID++;
            EdgeFactory<int, EquatableEdge<int>> edgeFactory = (s, t) => new EquatableEdge<int>(s, t);

            var algorithm = graph.CreateGraphBalancerAlgorithm(source, sink, vertexFactory, edgeFactory);
            algorithm.BalancingSourceAdded += vertex => Assert.AreEqual(source, vertex);
            algorithm.BalancingSinkAdded += vertex => Assert.AreEqual(sink, vertex);
            var surplusSet = new HashSet<int> { 2, 5, 8 };
            algorithm.SurplusVertexAdded += vertex => Assert.IsTrue(surplusSet.Remove(vertex));
            var deficitSet = new HashSet<int> { 6 };
            algorithm.DeficientVertexAdded += vertex => Assert.IsTrue(deficitSet.Remove(vertex));

            algorithm.Balance();

            Assert.IsTrue(algorithm.Balanced);
            Assert.AreEqual(source, algorithm.Source);
            Assert.AreEqual(sink, algorithm.Sink);
            CollectionAssert.IsEmpty(surplusSet);
            CollectionAssert.IsEmpty(deficitSet);
            CollectionAssert.AreEquivalent(new[] { 2, 5, 8 },algorithm.SurplusVertices);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new EquatableEdge<int>(algorithm.BalancingSource, 2),
                    new EquatableEdge<int>(algorithm.BalancingSource, 5),
                    new EquatableEdge<int>(algorithm.BalancingSource, 8)
                },
                algorithm.SurplusEdges);
            CollectionAssert.AreEquivalent(new[] { 6 }, algorithm.DeficientVertices);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new EquatableEdge<int>(6, algorithm.BalancingSink)
                },
                algorithm.DeficientEdges);
            Assert.AreEqual(9, algorithm.BalancingSource);
            Assert.AreEqual(new EquatableEdge<int>(algorithm.BalancingSource, source), algorithm.BalancingSourceEdge);
            Assert.AreEqual(10, algorithm.BalancingSink);
            Assert.AreEqual(new EquatableEdge<int>(sink, algorithm.BalancingSink), algorithm.BalancingSinkEdge);
        }

        [Test]
        public void Balance_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2 );
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateGraphBalancerAlgorithm(1, 2, vertexFactory, edgeFactory);

            Assert.DoesNotThrow(algorithm.Balance);
            Assert.Throws<InvalidOperationException>(algorithm.Balance);
        }

        [Test]
        public void UnBalance()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge32 = Edge.Create(3, 2);
            var edge34 = Edge.Create(3, 4);
            var edge56 = Edge.Create(5, 6);

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge32, edge34, edge56
            );
            int vertexID = 6;
            VertexFactory<int> vertexFactory = () => vertexID++;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateGraphBalancerAlgorithm(1, 3, vertexFactory, edgeFactory);
            algorithm.Balance();

            Assert.IsTrue(algorithm.Balanced);

            algorithm.UnBalance();

            Assert.IsFalse(algorithm.Balanced);
            Assert.AreEqual(1, algorithm.Source);
            Assert.AreEqual(3, algorithm.Sink);
            CollectionAssert.IsEmpty(algorithm.SurplusVertices);
            CollectionAssert.IsEmpty(algorithm.SurplusEdges);
            CollectionAssert.IsEmpty(algorithm.DeficientVertices);
            CollectionAssert.IsEmpty(algorithm.DeficientEdges);
            Assert.AreEqual(default(int), algorithm.BalancingSource);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSourceEdge);
            Assert.AreEqual(default(int), algorithm.BalancingSink);
            Assert.AreEqual(default(Edge<int>), algorithm.BalancingSinkEdge);
        }

        [Test]
        public void UnBalance_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2 );
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateGraphBalancerAlgorithm(1, 2, vertexFactory, edgeFactory);

            Assert.Throws<InvalidOperationException>(algorithm.UnBalance);
        }

        [Test]
        public void GetBalancingIndex_Throws()
        {
            var source = new TestVertex("1");
            var sink = new TestVertex("2");
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            graph.AddVertexRange( source, sink );
            VertexFactory<TestVertex> vertexFactory = () => new TestVertex();
            EdgeFactory<TestVertex, IEdge<TestVertex>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateGraphBalancerAlgorithm(source, sink, vertexFactory, edgeFactory);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.GetBalancingIndex(null));
        }
    }
}