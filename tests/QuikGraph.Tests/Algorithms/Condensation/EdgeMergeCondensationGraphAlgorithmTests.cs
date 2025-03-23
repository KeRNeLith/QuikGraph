using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Condensation;


namespace QuikGraph.Tests.Algorithms.Condensation
{
    /// <summary> Tests for <see cref="EdgeMergeCondensationGraphAlgorithm{TVertex, TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class EdgeMergeCondensationGraphAlgorithmTests
    {

        private static void RunEdgesCondensationAndCheck<TVertex, TEdge>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] Func<TVertex, bool> predicate)
            where TEdge : IEdge<TVertex>
        {
            IMutableBidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>> condensedGraph =
                graph.CondensateEdges(predicate);

            Assert.IsNotNull(condensedGraph);
            Assert.LessOrEqual(condensedGraph.VertexCount, graph.VertexCount);

            TVertex[] vertices = condensedGraph.Vertices.ToArray();
            foreach (MergedEdge<TVertex, TEdge> edge in condensedGraph.Edges)
            {
                Assert.Contains(edge.Source, vertices);
                Assert.Contains(edge.Target, vertices);

                Assert.Positive(edge.Edges.Count);
                Assert.Contains(edge.Edges.First().Source, vertices);
                Assert.Contains(edge.Edges.Last().Target, vertices);
            }
        }

        [Test]
        public void Constructor()
        {
            Func<int, bool> vertexPredicate = _ => true;
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var condensedGraph = new BidirectionalGraph<int, MergedEdge<int, IEdge<int>>>();
            var algorithm = new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(graph, condensedGraph, vertexPredicate);
            AssertAlgorithmProperties(algorithm, graph, condensedGraph, vertexPredicate);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                EdgeMergeCondensationGraphAlgorithm<TVertex, TEdge> algo,
                IBidirectionalGraph<TVertex, TEdge> g,
                IMutableBidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>> cg,
                Func<TVertex, bool> predicate)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.AreSame(predicate, algo.VertexPredicate);
                Assert.AreSame(cg, algo.CondensedGraph);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            Func<int, bool> vertexPredicate = _ => true;
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var condensedGraph = new BidirectionalGraph<int, MergedEdge<int, IEdge<int>>>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(graph, condensedGraph, null));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(graph, null, vertexPredicate));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(null, condensedGraph, vertexPredicate));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(graph, null, null));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(null, condensedGraph, null));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(null, null, vertexPredicate));
            Assert.Throws<ArgumentNullException>(() => new EdgeMergeCondensationGraphAlgorithm<int, IEdge<int>>(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>2 Graphs:
        ///
        /// _8 --> _9
        ///
        /// _1 --> _2 --> _3
        /// _4 --> _2
        ///
        /// _5 --> _7 --> _6
        /// _5 --> _6
        ///
        /// and the singly connected 
        ///
        /// _4 --> _5 --> _7 --> _1 --> _2 --> _3
        ///                      _8 --> _2
        ///                      _8 --> _9
        ///        _5 --> _6
        ///               _7 --> _6
        /// 
        /// </returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> EdgeCondensationAllVerticesTestCases()
        {
            var edges = new List<IEdge<int>>
            {
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(4, 2),
                Edge.Create(4, 3),
                Edge.Create(5, 6),
                Edge.Create(5, 7),
                Edge.Create(7, 6),
                Edge.Create(8, 9)
            };
            var graph2 = new BidirectionalGraph<int, IEdge<int>>();
            graph2.AddVerticesAndEdgeRange(edges);
            yield return new TestCaseData(graph2);


            var graph1 = new BidirectionalGraph<int, IEdge<int>>();
            graph1.AddVerticesAndEdgeRange(
                Edge.Create(4, 5),
                Edge.Create(7, 1),
                Edge.Create(8, 2)
            );
            yield return new TestCaseData(graph1);

        }

        [TestCaseSource(nameof(EdgeCondensationAllVerticesTestCases))]
        public void EdgeCondensationAllVertices([NotNull] IBidirectionalGraph<int, IEdge<int>> graph)
        {
            IMutableBidirectionalGraph<int, MergedEdge<int, IEdge<int>>> condensedGraph =
                graph.CondensateEdges(_ => true);

            Assert.IsNotNull(condensedGraph);
            Assert.AreEqual(graph.VertexCount, condensedGraph.VertexCount);
            Assert.AreEqual(graph.EdgeCount, condensedGraph.EdgeCount);

            CollectionAssert.AreEquivalent(graph.Vertices, condensedGraph.Vertices);

            var allEdges = condensedGraph.Edges.SelectMany(e => e.Edges).ToArray();
            CollectionAssert.AreEquivalent(graph.Edges, allEdges);
        }

        [Test]
        public void EdgeCondensationSomeVertices()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge38 = Edge.Create(3, 8);
            var edge42 = Edge.Create(4, 2);
            var edge43 = Edge.Create(4, 3);
            var edge44 = Edge.Create(4, 4);

            var edge45 = Edge.Create(4, 5);

            var edge56 = Edge.Create(5, 6);
            var edge57 = Edge.Create(5, 7);
            var edge76 = Edge.Create(7, 6);

            var edge71 = Edge.Create(7, 1);

            var edge89 = Edge.Create(8, 9);

            var edge82 = Edge.Create(8, 2);

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge38, edge42, edge43, edge44,
                edge45, edge56, edge57, edge76, edge71, edge89, edge82
            );

            IMutableBidirectionalGraph<int, MergedEdge<int, IEdge<int>>> condensedGraph =
                graph.CondensateEdges(v => v == 4 || v == 8);

            Assert.IsNotNull(condensedGraph);
            Assert.AreEqual(2, condensedGraph.VertexCount);
            Assert.AreEqual(6, condensedGraph.EdgeCount);
            CollectionAssert.AreEquivalent(new[] { 4, 8 }, condensedGraph.Vertices);
            CollectionAssert.AreEquivalent(new[] { edge82, edge23, edge38 }, condensedGraph.Edges.ElementAt(0).Edges);
            CollectionAssert.AreEquivalent(new[] { edge44 }, condensedGraph.Edges.ElementAt(1).Edges);
            CollectionAssert.AreEquivalent(new[] { edge43, edge38 }, condensedGraph.Edges.ElementAt(2).Edges);
            CollectionAssert.AreEquivalent(new[] { edge42, edge23, edge38 }, condensedGraph.Edges.ElementAt(3).Edges);
            CollectionAssert.AreEquivalent(new[] { edge45, edge57, edge71, edge13, edge38 }, condensedGraph.Edges.ElementAt(4).Edges);
            CollectionAssert.AreEquivalent(new[] { edge45, edge57, edge71, edge12, edge23, edge38 }, condensedGraph.Edges.ElementAt(5).Edges);
        }

        [Test]
        [Category(TestCategories.LongRunning)]
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_SlowTests), new object[] { -1 })]
        public void EdgeCondensation(BidirectionalGraph<string, IEdge<string>> graph)
        {
            var rand = new Random(123456);
            RunEdgesCondensationAndCheck(graph, _ => true);
            RunEdgesCondensationAndCheck(graph, _ => rand.Next(0, 1) == 1);
        }
    }
}