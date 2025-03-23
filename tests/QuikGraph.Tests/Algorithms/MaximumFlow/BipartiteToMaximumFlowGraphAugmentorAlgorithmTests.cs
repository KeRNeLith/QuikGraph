using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.MaximumFlow;


namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="BipartiteToMaximumFlowGraphAugmentorAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BipartiteToMaximumFlowGraphAugmentorAlgorithmTests : GraphAugmentorAlgorithmTestsBase
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            int[] sourceToVertices = { 1, 2 };
            int[] verticesToSink = { 1, 2 };

            var algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(
                sourceToVertices,
                verticesToSink,
                vertexFactory,
                edgeFactory);
            AssertAlgorithmProperties(
                algorithm,
                graph,
                sourceToVertices,
                verticesToSink,
                vertexFactory,
                edgeFactory);

            algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(
                sourceToVertices,
                verticesToSink,
                vertexFactory,
                edgeFactory);
            AssertAlgorithmProperties(
                algorithm,
                graph,
                sourceToVertices,
                verticesToSink,
                vertexFactory,
                edgeFactory);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                BipartiteToMaximumFlowGraphAugmentorAlgorithm<TVertex, TEdge> algo,
                IMutableVertexAndEdgeSet<TVertex, TEdge> g,
                IEnumerable<TVertex> soToV,
                IEnumerable<TVertex> vToSi,
                VertexFactory<int> vFactory,
                EdgeFactory<int, IEdge<int>> eFactory)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsFalse(algo.Augmented);
                CollectionAssert.IsEmpty(algo.AugmentedEdges);
                Assert.AreSame(vFactory, algo.VertexFactory);
                Assert.AreSame(eFactory, algo.EdgeFactory);
                Assert.AreEqual(default(TVertex), algo.SuperSource);
                Assert.AreEqual(default(TVertex), algo.SuperSink);
                Assert.AreSame(soToV, algo.SourceToVertices);
                Assert.AreSame(vToSi, algo.VerticesToSink);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            AdjacencyGraph<int, IEdge<int>> nullGraph, graph = new();
            nullGraph = null;
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            int[] sourceToVertices = { 1, 2 };
            int[] verticesToSink = { 1, 2 };

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm( null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm( null, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Graph augmentor

        [Test]
        public void CreateAndSetSuperSource()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 3, 4, 5 );
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            int[] sourceToVertices = { 3, 4 };
            int[] verticesToSink = { 3, 5 };

            var algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory);

            CreateAndSetSuperSource_Test(algorithm);
        }

        [Test]
        public void CreateAndSetSuperSink()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 3, 4, 5 );
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            int[] sourceToVertices = { 3, 4 };
            int[] verticesToSink = { 3, 5 };

            var algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory);

            CreateAndSetSuperSink_Test(algorithm);
        }

        [Test]
        public void CreateAndSetSuperSourceOrSink_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            int[] sourceToVertices = { 3, 4 };
            int[] verticesToSink = { 3, 5 };

            var algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory);
            algorithm.Compute();
        }

        [Test]
        public void RunAugmentation()
        {
            int nextVertexId = 1;
            VertexFactory<int> vertexFactory = () =>
            {
                if (nextVertexId == 1)
                {
                    nextVertexId = 2;
                    return 1;
                }
                if (nextVertexId == 2)
                {
                    nextVertexId = 1;
                    return 2;
                }
                Assert.Fail("Should not arrive.");
                return 0;
            };
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            int[] sourceToVertices = { };
            int[] verticesToSink = { 4 };

            RunAugmentation_Test(
                graph => graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory),
                graph => graph.AddVertex(4));
        }

        [Test]
        public void RunAugmentation_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 3, 4 );
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            int[] sourceToVertices = { 3, 4 };
            int[] verticesToSink = { };

            var algorithm = graph.CreateBipartiteToMaximumFlowGraphAugmentorAlgorithm(sourceToVertices, verticesToSink, vertexFactory, edgeFactory);

            RunAugmentation_Throws_Test(algorithm);
        }

        #endregion
    }
}