using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;


namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="MaximumBipartiteMatchingAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class MaximumBipartiteMatchingAlgorithmTests
    {
        #region Test helpers

        [NotNull]
        private readonly EdgeFactory<string, IEdge<string>> _edgeFactory = Edge.Create;

        private static void AssertThatMaxMatchEdgesAreValid<TVertex, TEdge>(
            [NotNull, ItemNotNull] TVertex[] vertexSetA,
            [NotNull, ItemNotNull] TVertex[] vertexSetB,
            [NotNull] MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> maxMatch)
            where TEdge : IEdge<TVertex>
        {
            foreach (TEdge edge in maxMatch.MatchedEdges)
            {
                bool isValidEdge = vertexSetA.Contains(edge.Source) && vertexSetB.Contains(edge.Target)
                                   ||
                                   vertexSetB.Contains(edge.Source) && vertexSetA.Contains(edge.Target);
                Assert.IsTrue(isValidEdge, "Match contains invalid edges.");
            }
        }

        private static void MaxBipartiteMatch<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] TVertex[] vertexSetA,
            [NotNull, ItemNotNull] TVertex[] vertexSetB,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            int expectedMatchSize)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.VertexCount > 0);
            DateTime startTime = DateTime.Now;

            MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> maxMatch = graph.ComputeMaximumBipartiteMatching(vertexSetA, vertexSetB, vertexFactory, edgeFactory);

            TimeSpan computeTime = DateTime.Now - startTime;

            Assert.IsTrue(computeTime < TimeSpan.FromMinutes(5));

            AssertThatMaxMatchEdgesAreValid(vertexSetA, vertexSetB, maxMatch);

            Assert.AreEqual(expectedMatchSize, maxMatch.MatchedEdges.Length);
        }

        private void RunBipartiteMatchAndCheck(
            [NotNull, ItemNotNull] IEnumerable<IEdge<string>> edges,
            [NotNull, ItemNotNull] IEnumerable<string> setA,
            [NotNull, ItemNotNull] IEnumerable<string> setB,
            int expectedMatchSize)
        {
            var graph = edges.ToAdjacencyGraph<string, IEdge<string>>();

            var vertexFactory = new StringVertexFactory();

            if (graph.VertexCount > 0)
            {
                MaxBipartiteMatch(
                    graph,
                    setA.ToArray(),
                    setB.ToArray(),
                    vertexFactory.CreateVertex,
                    _edgeFactory,
                    expectedMatchSize);
            }
        }

        #endregion

        #region Test classes

        private sealed class StringVertexFactory
        {
            private int _id;

            [NotNull]
            private readonly string _prefix;

            public StringVertexFactory()
                : this("Super")
            {
            }

            private StringVertexFactory([NotNull] string prefix)
            {
                _prefix = prefix;
            }

            [NotNull]
            public string CreateVertex()
            {
                return $"{_prefix}{++_id}";
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            int[] sourceToVertices = { 1, 2 };
            int[] verticesToSink = { 1, 2 };

            var algorithm = graph.CreateMaximumBipartiteMatching(
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
                MaximumBipartiteMatchingAlgorithm<TVertex, TEdge> algo,
                IMutableVertexAndEdgeListGraph<TVertex, TEdge> g,
                IEnumerable<TVertex> soToV,
                IEnumerable<TVertex> vToSi,
                VertexFactory<int> vFactory,
                EdgeFactory<int, IEdge<int>> eFactory)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.AreSame(vFactory, algo.VertexFactory);
                Assert.AreSame(eFactory, algo.EdgeFactory);
                Assert.AreSame(soToV, algo.SourceToVertices);
                Assert.AreSame(vToSi, algo.VerticesToSink);
                CollectionAssert.IsEmpty(algo.MatchedEdges);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            AdjacencyGraph<int, IEdge<int>> nullGraph = null;
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            VertexFactory<int> vertexFactory = () => 1;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            int[] sourceToVertices = { 1, 2 };
            int[] verticesToSink = { 1, 2 };


            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, verticesToSink, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, null, vertexFactory, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, verticesToSink, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, verticesToSink, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, null, null, edgeFactory));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, null, vertexFactory, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, verticesToSink, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(sourceToVertices, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.CreateMaximumBipartiteMatching(null, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateMaximumBipartiteMatching(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void BipartiteMaxMatchSimple()
        {
            int[] integers = Enumerable.Range(0, 100).ToArray();
            string[] even = integers.Where(n => n % 2 == 0).Select(n => n.ToString()).ToArray();
            string[] odd = integers.Where(n => n % 2 != 0).Select(n => n.ToString()).ToArray();

            // Create the edges from even to odd
            var edges = TestHelpers.CreateAllPairwiseEdges(even, odd, _edgeFactory);

            int expectedMatchSize = Math.Min(even.Length, odd.Length);
            RunBipartiteMatchAndCheck(edges, even, odd, expectedMatchSize);
        }

        [Test]
        public void BipartiteMaxMatchSimpleReversedEdges()
        {
            int[] integers = Enumerable.Range(0, 100).ToArray();
            string[] even = integers.Where(n => n % 2 == 0).Select(n => n.ToString()).ToArray();
            string[] odd = integers.Where(n => n % 2 != 0).Select(n => n.ToString()).ToArray();

            // Create the edges from odd to even
            var edges = TestHelpers.CreateAllPairwiseEdges(odd, even, _edgeFactory);

            int expectedMatchSize = Math.Min(even.Length, odd.Length);
            RunBipartiteMatchAndCheck(edges, even, odd, expectedMatchSize);
        }

        [Test]
        public void BipartiteMaxMatchTwoFullyConnectedSets()
        {
            var setA = new List<string>();
            var setB = new List<string>();

            const int nodesInSet1 = 100;
            const int nodesInSet2 = 10;

            // Create a set of vertices in each set which all match each other
            int[] integers = Enumerable.Range(0, nodesInSet1).ToArray();
            string[] even = integers.Where(n => n % 2 == 0).Select(n => n.ToString()).ToArray();
            string[] odd = integers.Where(n => n % 2 != 0).Select(n => n.ToString()).ToArray();
            var edges = TestHelpers.CreateAllPairwiseEdges(even, odd, _edgeFactory).ToList();

            setA.AddRange(even);
            setB.AddRange(odd);

            // Create another set of vertices in each set which all match each other
            integers = Enumerable.Range(nodesInSet1 + 1, nodesInSet2).ToArray();
            even = integers.Where(n => n % 2 == 0).Select(n => n.ToString()).ToArray();
            odd = integers.Where(n => n % 2 != 0).Select(n => n.ToString()).ToArray();
            edges.AddRange(TestHelpers.CreateAllPairwiseEdges(even, odd, _edgeFactory));

            setA.AddRange(even);
            setB.AddRange(odd);

            int expectedMatchSize = Math.Min(setA.Count, setB.Count);
            RunBipartiteMatchAndCheck(edges, setA, setB, expectedMatchSize);
        }

        [Test]
        public void BipartiteMaxMatchUnequalPartitionsTest()
        {
            var setA = new List<string>();
            var setB = new List<string>();

            // Create a bipartite graph with small and large vertex partitions
            const int smallerSetSize = 1;
            const int largerSetSize = 1000;

            // Create a set of vertices in each set which all match each other
            string[] leftNodes = Enumerable.Range(0, smallerSetSize).Select(n => $"L{n}").ToArray();
            string[] rightNodes = Enumerable.Range(0, largerSetSize).Select(n => $"R{n}").ToArray();
            var edges = TestHelpers.CreateAllPairwiseEdges(leftNodes, rightNodes, _edgeFactory);

            setA.AddRange(leftNodes);
            setB.AddRange(rightNodes);

            int expectedMatchSize = Math.Min(setA.Count, setB.Count);
            RunBipartiteMatchAndCheck(edges, setA, setB, expectedMatchSize);
        }
    }
}