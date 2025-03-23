using System;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.MaximumFlow;


namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="MultiSourceSinkGraphAugmentorAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class MultiSourceSinkGraphAugmentorAlgorithmTests : GraphAugmentorAlgorithmTestsBase
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public static void RunAugmentationAndCheck(
            [NotNull] IMutableBidirectionalGraph<string, IEdge<string>> graph)
        {
            int vertexCount = graph.VertexCount;
            int edgeCount = graph.EdgeCount;
            int vertexId = graph.VertexCount + 1;

            string[] noInEdgesVertices = graph.Vertices.Where(v => graph.IsInEdgesEmpty(v) ?? true).ToArray();
            string[] noOutEdgesVertices = graph.Vertices.Where(v => graph.IsOutEdgesEmpty(v) ?? true).ToArray();

            using (var augmentor = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(()
                => (vertexId++).ToString(), Edge.Create))
            {
                bool added = false;
                augmentor.EdgeAdded += _ => { added = true; };

                augmentor.Compute();
                Assert.IsTrue(added);
                VerifyVertexCount(graph, augmentor, vertexCount);
                VerifySourceConnector(graph, augmentor, noInEdgesVertices);
                VerifySinkConnector(graph, augmentor, noOutEdgesVertices);
            }

            Assert.AreEqual(graph.VertexCount, vertexCount);
            Assert.AreEqual(graph.EdgeCount, edgeCount);
        }

        private static void VerifyVertexCount<TVertex, TEdge>(
            [NotNull] IVertexSet<TVertex> graph,
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            [NotNull] MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge> augmentor,
            int vertexCount)
            where TEdge : IEdge<TVertex>
        {
            Assert.AreEqual(vertexCount + 2 /* Source + Sink */, graph.VertexCount);
            Assert.IsTrue(graph.ContainsVertex(augmentor.SuperSource));
            Assert.IsTrue(graph.ContainsVertex(augmentor.SuperSink));
        }

        private static void VerifySourceConnector<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            [NotNull] MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge> augmentor,
            [NotNull, ItemNotNull] TVertex[] noInEdgesVertices)
            where TEdge : IEdge<TVertex>
        {
            foreach (TVertex vertex in noInEdgesVertices)
            {
                Assert.IsTrue(graph.ContainsEdge(augmentor.SuperSource, vertex));
            }

            foreach (TVertex vertex in graph.Vertices.Except(noInEdgesVertices))
            {
                if (vertex.Equals(augmentor.SuperSource))
                    continue;
                if (vertex.Equals(augmentor.SuperSink))
                    continue;
                Assert.IsFalse(graph.ContainsEdge(augmentor.SuperSource, vertex));
            }
        }

        private static void VerifySinkConnector<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            [NotNull] MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge> augmentor,
            [NotNull, ItemNotNull] TVertex[] noOutEdgesVertices)
            where TEdge : IEdge<TVertex>
        {
            foreach (TVertex vertex in noOutEdgesVertices)
            {
                Assert.IsTrue(graph.ContainsEdge(vertex, augmentor.SuperSink));
            }

            foreach (TVertex vertex in graph.Vertices.Except(noOutEdgesVertices))
            {
                if (vertex.Equals(augmentor.SuperSource))
                    continue;
                if (vertex.Equals(augmentor.SuperSink))
                    continue;
                Assert.IsFalse(graph.ContainsEdge(vertex, augmentor.SuperSink));
            }
        }

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory);
            AssertAlgorithmProperties(algorithm, graph, vertexFactory, edgeFactory);

            algorithm = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory, null);
            AssertAlgorithmProperties(algorithm, graph, vertexFactory, edgeFactory);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                MultiSourceSinkGraphAugmentorAlgorithm<TVertex, TEdge> algo,
                IMutableBidirectionalGraph<TVertex, TEdge> g,
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
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            BidirectionalGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, edgeFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMultiSourceSinkGraphAugmentorAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Graph augmentor

        [Test]
        public void CreateAndSetSuperSource()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var algorithm = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory);

            CreateAndSetSuperSource_Test(algorithm);
        }

        [Test]
        public void CreateAndSetSuperSink()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var algorithm = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory);

            CreateAndSetSuperSink_Test(algorithm);
        }

        [Test]
        public void RunAugmentation()
        {
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            RunAugmentation_Test(
                graph => graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory));
        }

        [Test]
        public void RunAugmentation_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            int vertexID = 0;
            VertexFactory<int> vertexFactory = () => ++vertexID;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var algorithm = graph.CreateMultiSourceSinkGraphAugmentorAlgorithm(vertexFactory, edgeFactory);

            RunAugmentation_Throws_Test(algorithm);
        }

        #endregion

    }
}