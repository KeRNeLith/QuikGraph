using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MaximumFlow;
using QuikGraph.Algorithms.RandomWalks;
using QuikGraph.Algorithms.TopologicalSort;
using QuikGraph.Collections;
using QuikGraph.Tests.Structures;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Extensions
{
    /// <summary>
    /// Tests related to <see cref="AlgorithmExtensions"/>.
    /// </summary>
    internal sealed class AlgorithmExtensionsTests : GraphTestsBase
    {
        [Test]
        public void GetIndexer()
        {
            var dictionary1 = new Dictionary<int, double>();
            Func<int, double> indexer1 = AlgorithmExtensions.GetIndexer(dictionary1);

            Assert.Throws<KeyNotFoundException>(() => indexer1(12));

            dictionary1[12] = 42.0;
            Assert.AreEqual(42.0, indexer1(12));

            var dictionary2 = new Dictionary<TestVertex, TestVertex>();
            Func<TestVertex, TestVertex> indexer2 = AlgorithmExtensions.GetIndexer(dictionary2);

            var key = new TestVertex("1");
            var keyBis = new TestVertex("1");
            Assert.Throws<KeyNotFoundException>(() => indexer2(key));

            var value = new TestVertex("2");
            dictionary2[key] = value;
            Assert.AreSame(value, indexer2(key));

            Assert.Throws<KeyNotFoundException>(() => indexer2(keyBis));
        }

        [Test]
        public void GetIndexer_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => AlgorithmExtensions.GetIndexer<int, double>(null));
        }

        [Test]
        public void GetVertexIdentity()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            VertexIdentity<int> vertexIdentity1 = graph1.GetVertexIdentity();

            Assert.AreEqual("12", vertexIdentity1(12));
            Assert.AreEqual("42", vertexIdentity1(42));
            // Check identity didn't change
            Assert.AreEqual("12", vertexIdentity1(12));
            Assert.AreEqual("42", vertexIdentity1(42));

            var graph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            VertexIdentity<TestVertex> vertexIdentity2 = graph2.GetVertexIdentity();

            var vertex1 = new TestVertex("12");
            var vertex2 = new TestVertex("42");
            Assert.AreEqual("0", vertexIdentity2(vertex1));
            Assert.AreEqual("1", vertexIdentity2(vertex2));
            // Check identity didn't change
            Assert.AreEqual("0", vertexIdentity2(vertex1));
            Assert.AreEqual("1", vertexIdentity2(vertex2));
        }

        [Test]
        public void GetVertexIdentity_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => AlgorithmExtensions.GetVertexIdentity<int>(null));
        }

        [Test]
        public void GetEdgeIdentity()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            EdgeIdentity<int, IEdge<int>> edgeIdentity1 = graph1.GetEdgeIdentity();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 3);
            var edge3 = Edge.Create(1, 2);
            Assert.AreEqual("0", edgeIdentity1(edge1));
            Assert.AreEqual("1", edgeIdentity1(edge2));
            Assert.AreEqual("2", edgeIdentity1(edge3));
            // Check identity didn't change
            Assert.AreEqual("0", edgeIdentity1(edge1));
            Assert.AreEqual("1", edgeIdentity1(edge2));
            Assert.AreEqual("2", edgeIdentity1(edge3));

            var graph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var edgeIdentity2 = graph2.GetEdgeIdentity();

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var vertex3 = new TestVertex("3");
            var edge4 = Edge.Create(vertex1, vertex2);
            var edge5 = Edge.Create(vertex2, vertex3);
            var edge6 = Edge.Create(vertex1, vertex2);
            Assert.AreEqual("0", edgeIdentity2(edge4));
            Assert.AreEqual("1", edgeIdentity2(edge5));
            Assert.AreEqual("2", edgeIdentity2(edge6));
            // Check identity didn't change
            Assert.AreEqual("0", edgeIdentity2(edge4));
            Assert.AreEqual("1", edgeIdentity2(edge5));
            Assert.AreEqual("2", edgeIdentity2(edge6));
        }

        [Test]
        public void GetEdgeIdentity_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => AlgorithmExtensions.GetEdgeIdentity<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(() => AlgorithmExtensions.GetEdgeIdentity<TestVertex, IEdge<TestVertex>>(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void TreeBreadthFirstSearch()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge35 = Edge.Create(3, 5);
            var edge36 = Edge.Create(3, 6);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge24, edge35, edge36
            );
            graph.AddVertex(7);

            var pathAccessor = graph.TreeBreadthFirstSearch(1);

            Assert.IsNull(pathAccessor(7));

            var path = pathAccessor(5);
            CollectionAssert.AreEqual(new[] { edge13, edge35 }, path);
        }

        [Test]
        public void TreeBreadthFirstSearch_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeBreadthFirstSearch(vertex));
            Assert.Throws<ArgumentNullException>(() => graph.TreeBreadthFirstSearch(null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeBreadthFirstSearch(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void TreeDepthFirstSearch()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge35 = Edge.Create(3, 5);
            var edge36 = Edge.Create(3, 6);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge24, edge35, edge36
            );
            graph.AddVertex(7);

            var pathAccessor = graph.TreeDepthFirstSearch(1);

            Assert.IsNull(pathAccessor(7));

            var path = pathAccessor(5);
            CollectionAssert.AreEqual(new[] { edge12, edge23, edge35 }, path);
        }

        [Test]
        public void TreeDepthFirstSearch_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeDepthFirstSearch(vertex));
            Assert.Throws<ArgumentNullException>(() => graph.TreeDepthFirstSearch(null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeDepthFirstSearch(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void TreeCyclePoppingRandom()
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 3);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 2);
            var edge7 = Edge.Create(3, 5);
            var edge8 = Edge.Create(3, 6);
            var edge9 = Edge.Create(4, 1);
            var edge10 = Edge.Create(4, 2);
            var edge11 = Edge.Create(4, 5);
            var edge12 = Edge.Create(4, 6);
            var edge13 = Edge.Create(5, 6);
            var edge14 = Edge.Create(6, 2);
            var edge15 = Edge.Create(6, 3);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge1, edge2, edge3, edge4, edge5, edge6,
                edge7, edge8, edge9, edge10, edge11,
                edge12, edge13, edge14, edge15
            );
            graph.AddVertex(7);

            var pathAccessor = graph.TreeCyclePoppingRandom(2);

            Assert.IsNull(pathAccessor(7));

            // Would require more tests...
        }

        [Test]
        public void TreeCyclePoppingRandom_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            var chain = new NormalizedMarkovEdgeChain<TestVertex, IEdge<TestVertex>>();
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(vertex));
            Assert.Throws<ArgumentNullException>(() => graph.TreeCyclePoppingRandom(null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(null));

            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(vertex, chain));
            Assert.Throws<ArgumentNullException>(() => graph.TreeCyclePoppingRandom(null, chain));
            Assert.Throws<ArgumentException>(() => graph.TreeCyclePoppingRandom(vertex, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(null, chain));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(vertex, null));
            Assert.Throws<ArgumentNullException>(() => graph.TreeCyclePoppingRandom(null, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).TreeCyclePoppingRandom(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #region Shortest paths

        [Test]
        public void ShortestPaths_Dijkstra_AStar_BellmanFord_Dag()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge18 = Edge.Create(1, 8);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge26 = Edge.Create(2, 6);
            var edge34 = Edge.Create(3, 4);
            var edge45 = Edge.Create(4, 5);
            var edge46 = Edge.Create(4, 6);
            var edge56 = Edge.Create(5, 6);
            var edge67 = Edge.Create(6, 7);
            var edge810 = Edge.Create(8, 10);
            var edge95 = Edge.Create(9, 5);
            var edge109 = Edge.Create(10, 9);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge18, edge24, edge25,
                edge26, edge34, edge45, edge46, edge56,
                edge67, edge810, edge95, edge109
            );

            Func<int, List<IEdge<int>>>[] algorithmResults =
            {
                graph.ShortestPathsDijkstra(_ => 1.0, 2),
                graph.ShortestPathsAStar(_ => 1.0, _ => 1.0, 2),
                graph.ShortestPathsBellmanFord(_ => 1.0, 2, out _),
                graph.ShortestPathsDag(_ => 1.0, 2)
            };

            foreach (Func<int, List<IEdge<int>>> result in algorithmResults)
            {
                CheckResult(result);
            }

            #region Local function

            void CheckResult(Func<int, List<IEdge<int>>> pathAccessor)
            {
                Assert.IsNotNull(pathAccessor);

                Assert.IsNull(pathAccessor(1));

                var path = pathAccessor(7);
                CollectionAssert.AreEqual(new[] { edge26, edge67 }, path);

                path = pathAccessor(4);
                CollectionAssert.AreEqual(new[] { edge24 }, path);
            }

            #endregion
        }

        [Test]
        public void ShortestPaths_BellmanFord_NegativeCycle()
        {
            var edge12 = Edge.Create(1, 2);
            var edge24 = Edge.Create(2, 4);
            var edge41 = Edge.Create(4, 1);
            
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge24, edge41
            );

            Func<int, List<IEdge<int>>> pathAccessor = graph.ShortestPathsBellmanFord(
                edge =>
                {
                    if (edge == edge12)
                        return 12.0;
                    if (edge == edge24)
                        return -42.0;
                    if (edge == edge41)
                        return 22.0;
                    return 1.0;
                },
                1,
                out bool foundNegativeCycle);
            Assert.IsNotNull(pathAccessor);
            Assert.IsTrue(foundNegativeCycle);

            // Path accessors is usable but will generate a stack overflow
            // if accessing path using edge in the negative cycle.
        }

        [Test]
        public void ShortestPathsDijkstra_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(_ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(null, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ShortestPathsAStar_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(_ => 1.0, _ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(null, _ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(_ => 1.0, null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(_ => 1.0, _ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(null, _ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(_ => 1.0, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(_ => 1.0, _ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(null, null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(null, _ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(_ => 1.0, null, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(null, null, vertex));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(null, _ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(_ => 1.0, null, null));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsAStar(null, null, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsAStar(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ShortestPathsBellmanFord_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsBellmanFord(_ => 1.0, vertex, out _));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsBellmanFord(null, vertex, out _));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsBellmanFord(_ => 1.0, null, out _));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsBellmanFord(null, vertex, out _));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsBellmanFord(null, null, out _));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsBellmanFord(_ => 1.0, null, out _));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsBellmanFord(null, null, out _));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ShortestPathsDag_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDag(_ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDag(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDag(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDag(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDag(null, null));
            Assert.Throws<ArgumentNullException>(() => 
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDag(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDag(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ShortestPaths_UndirectedDijkstra()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge18 = Edge.Create(1, 8);
            var edge45 = Edge.Create(4, 5);
            var edge46 = Edge.Create(4, 6);
            var edge56 = Edge.Create(5, 6);
            var edge67 = Edge.Create(6, 7);
            var edge810 = Edge.Create(8, 10);

            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge18, edge45,
                edge46, edge56, edge67, edge810
            );
            graph.AddVertex(9);

            Func<int, List<IEdge<int>>> pathAccessor = graph.ShortestPathsDijkstra(_ => 1.0, 2);
            Assert.IsNotNull(pathAccessor);

            Assert.IsNull(pathAccessor(9));

            var path = pathAccessor(8);
            CollectionAssert.AreEqual(new[] { edge12, edge18 }, path);

            path = pathAccessor(1);
            CollectionAssert.AreEqual(new[] { edge12 }, path);
        }

        [Test]
        public void ShortestPathsUndirectedDijkstra_Throws()
        {
            var graph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IUndirectedGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(_ => 1.0, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IUndirectedGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(null, vertex));
            Assert.Throws<ArgumentNullException>(() => graph.ShortestPathsDijkstra(null, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IUndirectedGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(() =>
                ((IUndirectedGraph<TestVertex, IEdge<TestVertex>>)null).ShortestPathsDijkstra(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region K-Shortest path

        [Test]
        public void RankedShortestPathHoffmanPavley()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge18 = Edge.Create(1, 8);
            var edge21 = Edge.Create(2, 1);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge26 = Edge.Create(2, 6);
            var edge33 = Edge.Create(3, 3);
            var edge34 = Edge.Create(3, 4);
            var edge45 = Edge.Create(4, 5);
            var edge46 = Edge.Create(4, 6);
            var edge56 = Edge.Create(5, 6);
            var edge67 = Edge.Create(6, 7);
            var edge810 = Edge.Create(8, 10);
            var edge95 = Edge.Create(9, 5);
            var edge109 = Edge.Create(10, 9);

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge18, edge21, edge24,
                edge25, edge26, edge33, edge34, edge45,
                edge46, edge56, edge67, edge810, edge95,
                edge109
            );

            IEnumerable<IEnumerable<IEdge<int>>> paths = graph.RankedShortestPathHoffmanPavley(_ => 1.0, 1, 5, 5);
            CollectionAssert.AreEqual(
                new[]
                {
                    new[] { edge12, edge25 },
                    new[] { edge13, edge34, edge45 },
                    new[] { edge12, edge24, edge45 },
                    new[] { edge18, edge810, edge109, edge95 }
                },
                paths);

            paths = graph.RankedShortestPathHoffmanPavley(_ => 1.0, 1, 5);
            CollectionAssert.AreEqual(
                new[]
                {
                    new[] { edge12, edge25 },
                    new[] { edge13, edge34, edge45 },
                    new[] { edge12, edge24, edge45 }
                },
                paths);
        }

        [Test]
        public void RankedShortestPathHoffmanPavley_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();

            var vertex = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(_ => 1.0, vertex, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(null, vertex, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(_ => 1.0, null, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(_ => 1.0, vertex, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(null, vertex, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(_ => 1.0, null, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(_ => 1.0, vertex, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(null, null, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(null, vertex, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(_ => 1.0, null, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(null, null, vertex, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(null, vertex, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => graph.RankedShortestPathHoffmanPavley(null, null, null, int.MaxValue));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<TestVertex, IEdge<TestVertex>>)null).RankedShortestPathHoffmanPavley(null, null, null, int.MaxValue));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentOutOfRangeException>(
                () => graph.RankedShortestPathHoffmanPavley(_ => 1.0, vertex, vertex, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => graph.RankedShortestPathHoffmanPavley(_ => 1.0, vertex, vertex, -1));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CreateSinksTestCases(
            [NotNull, InstantHandle] Func<IMutableVertexAndEdgeSet<int, IEdge<int>>> createGraph)
        {
            yield return new TestCaseData(createGraph(),Enumerable.Empty<int>());

            var edge12 = Edge.Create(1, 2);
            var edge14 = Edge.Create(1, 4);
            var edge22 = Edge.Create(2, 2);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge35 = Edge.Create(3, 5);
            var edge41 = Edge.Create(4, 1);
            var edge45 = Edge.Create(4, 5);
            var edge46 = Edge.Create(4, 6);

            IMutableVertexAndEdgeSet<int, IEdge<int>> cycleGraph = createGraph();
            cycleGraph.AddVerticesAndEdgeRange(edge12, edge24, edge41);
            yield return new TestCaseData(cycleGraph,Enumerable.Empty<int>());

            IMutableVertexAndEdgeSet<int, IEdge<int>> cycleGraph2 = createGraph();
            cycleGraph2.AddVerticesAndEdgeRange(
                edge12, edge24, edge25, edge35, edge41, edge22
            );
            yield return new TestCaseData(cycleGraph2, new[] { 5 });

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph1 = createGraph();
            graph1.AddVerticesAndEdgeRange(edge22);
            yield return new TestCaseData(graph1,Enumerable.Empty<int>());

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph2 = createGraph();
            graph2.AddVerticesAndEdgeRange(
                edge12, edge14, edge23, edge24, edge35, edge45
            );
            yield return new TestCaseData(graph2, new[] { 5 });

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph3 = createGraph();
            graph3.AddVerticesAndEdgeRange(
                edge12, edge14, edge24, edge35, edge45, edge46
            );
            yield return new TestCaseData(graph3, new[] { 5, 6 } );
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> SinksTestCases
        {
            [UsedImplicitly]
            get
            {
                var testCases = CreateSinksTestCases(() => new AdjacencyGraph<int, IEdge<int>>())
                    .Concat(CreateSinksTestCases(() => new BidirectionalGraph<int, IEdge<int>>()));
                foreach (TestCaseData testCase in testCases)
                {
                    yield return testCase;
                }
            }
        }

        [TestCaseSource(nameof(SinksTestCases))]
        public void Sinks(
            [NotNull] IVertexListGraph<int, IEdge<int>> graph,
            [NotNull] IEnumerable<int> expectedSinks)
        {
            CollectionAssert.AreEquivalent(expectedSinks, graph.Sinks());
        }

        [Test]
        public void Sinks_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<NullReferenceException>(
                () => ((IVertexListGraph<int, IEdge<int>>)null).Sinks().ToArray());
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CreateRootsTestCases(
            [NotNull, InstantHandle] Func<IMutableVertexAndEdgeSet<int, IEdge<int>>> createGraph)
        {
            yield return new TestCaseData(createGraph(), Enumerable.Empty<int>());

            var edge12 = Edge.Create(1, 2);
            var edge14 = Edge.Create(1, 4);
            var edge22 = Edge.Create(2, 2);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge35 = Edge.Create(3, 5);
            var edge41 = Edge.Create(4, 1);
            var edge45 = Edge.Create(4, 5);
            var edge46 = Edge.Create(4, 6);

            IMutableVertexAndEdgeSet<int, IEdge<int>> cycleGraph = createGraph();
            cycleGraph.AddVerticesAndEdgeRange(edge12, edge24, edge41);
            yield return new TestCaseData(cycleGraph, Enumerable.Empty<int>());

            IMutableVertexAndEdgeSet<int, IEdge<int>> cycleGraph2 = createGraph();
            cycleGraph2.AddVerticesAndEdgeRange(edge12, edge24, edge25, edge35, edge41, edge22);
            yield return new TestCaseData(cycleGraph2, new[] { 3 });

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph1 = createGraph();
            graph1.AddVerticesAndEdgeRange(edge22);
            yield return new TestCaseData(graph1, Enumerable.Empty<int>());

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph2 = createGraph();
            graph2.AddVerticesAndEdgeRange(edge12, edge14, edge23, edge24, edge35, edge45);
            yield return new TestCaseData(graph2, new[] { 1 });

            IMutableVertexAndEdgeSet<int, IEdge<int>> graph3 = createGraph();
            graph3.AddVerticesAndEdgeRange(edge12, edge14, edge24, edge35, edge45, edge46);
            yield return new TestCaseData(graph3, new[] {1, 3 });
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> RootsTestCases
        {
            [UsedImplicitly]
            get => CreateRootsTestCases(() => new AdjacencyGraph<int, IEdge<int>>());
        }

        [TestCaseSource(nameof(RootsTestCases))]
        public void Roots_NotBidirectional(
            [NotNull] IVertexListGraph<int, IEdge<int>> graph,
            [NotNull] IEnumerable<int> expectedRoots)
        {
            CollectionAssert.AreEquivalent(expectedRoots, graph.Roots());
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void AdjacencyGraphRoots<T>(IVertexAndEdgeListGraph<T, IEdge<T>> graph)
        {
                var roots = new HashSet<T>(graph.Roots());
                foreach (var edge in graph.Edges)
                    Assert.IsFalse(roots.Contains(edge.Target));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> BidirectionalRootsTestCases
        {
            [UsedImplicitly]
            get
            {
                return CreateRootsTestCases(() => new BidirectionalGraph<int, IEdge<int>>());
            }
        }

        [TestCaseSource(nameof(BidirectionalRootsTestCases))]
        public void Roots_Bidirectional(
            [NotNull] IBidirectionalGraph<int, IEdge<int>> graph,
            [NotNull] IEnumerable<int> expectedRoots)
        {
            CollectionAssert.AreEquivalent(expectedRoots, graph.Roots());
        }

        [Test]
        public void Roots_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<int, IEdge<int>>)null).Roots().ToArray());
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).Roots().ToArray());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> IsolatedVerticesTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new BidirectionalGraph<int, IEdge<int>>(),Enumerable.Empty<int>());

                var edge12 = Edge.Create(1, 2);
                var edge14 = Edge.Create(1, 4);
                var edge22 = Edge.Create(2, 2);
                var edge23 = Edge.Create(2, 3);
                var edge24 = Edge.Create(2, 4);
                var edge26 = Edge.Create(2, 6);
                var edge35 = Edge.Create(3, 5);
                var edge36 = Edge.Create(3, 6);
                var edge41 = Edge.Create(4, 1);
                var edge45 = Edge.Create(4, 5);
                var edge46 = Edge.Create(4, 6);

                var cycleGraph = new BidirectionalGraph<int, IEdge<int>>();
                cycleGraph.AddVerticesAndEdgeRange(edge12, edge24, edge41);
                yield return new TestCaseData(cycleGraph,Enumerable.Empty<int>());

                var cycleGraph2 = new BidirectionalGraph<int, IEdge<int>>();
                cycleGraph2.AddVerticesAndEdgeRange(edge12, edge24, edge41, edge22);
                yield return new TestCaseData(cycleGraph2,Enumerable.Empty<int>());

                var cycleGraph3 = new BidirectionalGraph<int, IEdge<int>>();
                cycleGraph3.AddVerticesAndEdgeRange(edge22);
                yield return new TestCaseData(cycleGraph3,Enumerable.Empty<int>());

                var cycleGraph4 = new BidirectionalGraph<int, IEdge<int>>();
                cycleGraph4.AddVerticesAndEdgeRange(edge12, edge22, edge24, edge41);
                cycleGraph4.AddVertex(5);
                yield return new TestCaseData(cycleGraph4, new[] { 5 } );

                var graph1 = new BidirectionalGraph<int, IEdge<int>>();
                graph1.AddVertexRange( 4, 5 );
                graph1.AddVerticesAndEdgeRange(edge12, edge23, edge26, edge36);
                yield return new TestCaseData(graph1, new[] { 4, 5 });

                var graph2 = new BidirectionalGraph<int, IEdge<int>>();
                graph2.AddVerticesAndEdgeRange(
                    edge12, edge14, edge23, edge24, edge26, edge35, edge45, edge46
                );
                yield return new TestCaseData(
                    graph2,
                    Enumerable.Empty<int>());
            }
        }

        [TestCaseSource(nameof(IsolatedVerticesTestCases))]
        public void IsolatedVertices(
            [NotNull] IBidirectionalGraph<int, IEdge<int>> graph,
            [NotNull] IEnumerable<int> expectedIsolatedVertices)
        {
            CollectionAssert.AreEquivalent(expectedIsolatedVertices, graph.IsolatedVertices());
        }

        [Test]
        public void IsolatedVertices_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((BidirectionalGraph<int, IEdge<int>>)null).IsolatedVertices());
        }

        #region Topological sort

        [Test]
        public void TopologicalSort()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(3, 5),
                Edge.Create(5, 7),
                Edge.Create(6, 3),
                Edge.Create(6, 7)
            );

            CollectionAssert.AreEqual(
                new[] { 6, 3, 5, 7, 1, 2, 4 },
                graph.TopologicalSort());
        }

        [Test]
        public void TopologicalSort_Undirected()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(3, 5),
                Edge.Create(5, 7),
                Edge.Create(6, 7)
            );

            CollectionAssert.AreEqual(
                new[] { 1, 3, 5, 7, 6, 2, 4 },
                graph.TopologicalSort());
        }

        [Test]
        public void TopologicalSort_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<int, IEdge<int>>) null).TopologicalSort());

            Assert.Throws<ArgumentNullException>(
                () => ((IUndirectedGraph<int, IEdge<int>>)null).TopologicalSort());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void SourceFirstTopologicalSort()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(3, 5),
                Edge.Create(5, 7),
                Edge.Create(6, 3),
                Edge.Create(6, 7)
            );

            CollectionAssert.AreEqual(
                new[] { 6, 3, 1, 5, 2, 7, 4 },
                graph.SourceFirstTopologicalSort());
        }

        [Test]
        public void SourceFirstTopologicalSort_Undirected()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(3, 5),
                Edge.Create(5, 7),
                Edge.Create(6, 7)
            );

            CollectionAssert.AreEqual(
                new[] { 4, 6, 2, 7, 1, 5, 3 },
                graph.SourceFirstTopologicalSort());
        }

        [Test]
        public void SourceFirstTopologicalSort_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexAndEdgeListGraph<int, IEdge<int>>)null).SourceFirstTopologicalSort());

            Assert.Throws<ArgumentNullException>(
                () => ((IUndirectedGraph<int, IEdge<int>>)null).SourceFirstTopologicalSort());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void SourceFirstBidirectionalTopologicalSort()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(3, 5),
                Edge.Create(5, 7),
                Edge.Create(6, 3),
                Edge.Create(6, 7)
            );

            CollectionAssert.AreEqual(
                new[] { 6, 3, 1, 5, 2, 7, 4 },
                graph.SourceFirstBidirectionalTopologicalSort());

            CollectionAssert.AreEqual(
                new[] { 6, 3, 1, 5, 2, 7, 4 },
                graph.SourceFirstBidirectionalTopologicalSort(TopologicalSortDirection.Forward));

            CollectionAssert.AreEqual(
                new[] { 4, 7, 2, 5, 1, 3, 6 },
                graph.SourceFirstBidirectionalTopologicalSort(TopologicalSortDirection.Backward));
        }

        [Test]
        public void SourceFirstBidirectionalTopologicalSort_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).SourceFirstBidirectionalTopologicalSort());

            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).SourceFirstBidirectionalTopologicalSort(TopologicalSortDirection.Forward));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).SourceFirstBidirectionalTopologicalSort(TopologicalSortDirection.Backward));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Connected components

        [Test]
        public void ConnectedComponents()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 4),
                Edge.Create(2, 3),
                Edge.Create(3, 1),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(6, 7),
                Edge.Create(7, 5),

                Edge.Create(8, 9)
            );

            var components = new Dictionary<int, int>();

            Assert.AreEqual(2, graph.ConnectedComponents(components));
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0,
                    [5] = 0,
                    [6] = 0,
                    [7] = 0,
                    [8] = 1,
                    [9] = 1
                },
                components);
        }

        [Test]
        public void ConnectedComponents_Throws()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            _ = graph.ConnectedComponents(null);
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ConnectedComponents<int, IEdge<int>>(null, components));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ConnectedComponents<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void IncrementalConnectedComponent()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 0, 1, 2, 3 );
            using (graph.IncrementalConnectedComponents(
                out Func<KeyValuePair<int, IDictionary<int, int>>> getComponents))
            {
                KeyValuePair<int, IDictionary<int, int>> current = getComponents();
                Assert.AreEqual(4, current.Key);

                graph.AddEdge(Edge.Create(0, 1));
                current = getComponents();
                Assert.AreEqual(3, current.Key);

                graph.AddEdge(Edge.Create(2, 3));
                current = getComponents();
                Assert.AreEqual(2, current.Key);

                graph.AddEdge(Edge.Create(1, 3));
                current = getComponents();
                Assert.AreEqual(1, current.Key);

                graph.AddVertex(4);
                current = getComponents();
                Assert.AreEqual(2, current.Key);
            }
        }

        [Test]
        public void IncrementalConnectedComponent_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.IncrementalConnectedComponents<int, IEdge<int>>(null, out _));
        }

        [Test]
        public void StronglyConnectedComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 4),
                Edge.Create(2, 3),
                Edge.Create(3, 1),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(6, 7),
                Edge.Create(7, 5)
            );

            var components = new Dictionary<int, int>();

            Assert.AreEqual(3, graph.StronglyConnectedComponentsCount(components));
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 2,
                    [2] = 2,
                    [3] = 2,
                    [4] = 1,
                    [5] = 0,
                    [6] = 0,
                    [7] = 0
                },
                components);
        }

        [Test]
        public void StronglyConnectedComponents_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.AreEqual(0, graph.StronglyConnectedComponentsCount(null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.StronglyConnectedComponentsCount<int, IEdge<int>>(null, components));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.StronglyConnectedComponentsCount<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void WeaklyConnectedComponents()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 4),
                Edge.Create(2, 3),
                Edge.Create(3, 1),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(6, 7),
                Edge.Create(7, 5),
                
                Edge.Create(8, 9)
            );

            var components = new Dictionary<int, int>();

            Assert.AreEqual(2, graph.WeaklyConnectedComponentsCount(components));
            CollectionAssert.AreEquivalent(
                new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0,
                    [5] = 0,
                    [6] = 0,
                    [7] = 0,
                    [8] = 1,
                    [9] = 1
                },
                components);
        }

        [Test]
        public void WeaklyConnectedComponents_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var components = new Dictionary<int, int>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.AreEqual(0, graph.WeaklyConnectedComponentsCount(null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.WeaklyConnectedComponentsCount<int, IEdge<int>>(null, components));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.WeaklyConnectedComponentsCount<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void StronglyCondensedGraph_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.CondensateStronglyConnected<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(null));
        }

        [Test]
        public void WeaklyCondensedGraph_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.CondensateWeaklyConnected<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(null));
        }

        [Test]
        public void EdgesCondensedGraph_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).CondensateEdges(_ => true));
            Assert.Throws<ArgumentNullException>(
                () => graph.CondensateEdges(null));
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, IEdge<int>>)null).CondensateEdges(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> OddVerticesTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new AdjacencyGraph<int, IEdge<int>>(),Enumerable.Empty<int>());

                var edge12 = Edge.Create(1, 2);
                var edge14 = Edge.Create(1, 4);
                var edge22 = Edge.Create(2, 2);
                var edge23 = Edge.Create(2, 3);
                var edge24 = Edge.Create(2, 4);
                var edge25 = Edge.Create(2, 5);
                var edge26 = Edge.Create(2, 6);
                var edge35 = Edge.Create(3, 5);
                var edge41 = Edge.Create(4, 1);
                var edge45 = Edge.Create(4, 5);
                var edge46 = Edge.Create(4, 6);

                var cycleGraph = new AdjacencyGraph<int, IEdge<int>>();
                cycleGraph.AddVerticesAndEdgeRange(edge12, edge24, edge41);
                yield return new TestCaseData(cycleGraph, Enumerable.Empty<int>());

                var cycleGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                cycleGraph2.AddVerticesAndEdgeRange(edge12, edge24, edge41, edge22);
                yield return new TestCaseData(cycleGraph2,Enumerable.Empty<int>());

                var cycleGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                cycleGraph3.AddVerticesAndEdgeRange(edge12, edge24, edge25, edge35, edge41, edge22);
                yield return new TestCaseData(cycleGraph3, new[] { 2, 3 });

                var cycleGraph4 = new AdjacencyGraph<int, IEdge<int>>();
                cycleGraph4.AddVerticesAndEdgeRange(edge12, edge22, edge24, edge25, edge35, edge41, edge45);
                yield return new TestCaseData(cycleGraph4, new[] { 2, 3, 4, 5 });

                var graph1 = new AdjacencyGraph<int, IEdge<int>>();
                graph1.AddVerticesAndEdgeRange(edge12, edge14, edge23, edge24, edge35, edge45);
                yield return new TestCaseData(graph1, new[] { 2, 4 });

                var graph2 = new AdjacencyGraph<int, IEdge<int>>();
                graph2.AddVerticesAndEdgeRange(edge12, edge14, edge23, edge24, edge26, edge35, edge45, edge46);
                yield return new TestCaseData(graph2,Enumerable.Empty<int>());
            }
        }

        [TestCaseSource(nameof(OddVerticesTestCases))]
        public void OddVertices(
            [NotNull] IVertexAndEdgeListGraph<int, IEdge<int>> graph,
            [NotNull] IEnumerable<int> expectedOddVertices)
        {
            CollectionAssert.AreEquivalent(expectedOddVertices, graph.OddVertices());
        }

        [Test]
        public void OddVertices_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<int, IEdge<int>>)null).OddVertices());
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CreateIsDirectedAcyclicGraphTestCases(
            [NotNull, InstantHandle] Func<IMutableVertexAndEdgeSet<int, IEdge<int>>> createGraph)
        {
            // Empty graph
            yield return new TestCaseData(createGraph())
            {
                ExpectedResult = true
            };

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge16 = Edge.Create(1, 6);
            var edge22 = Edge.Create(2, 2);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge31 = Edge.Create(3, 1);
            var edge34 = Edge.Create(3, 4);
            var edge35 = Edge.Create(3, 5);
            var edge41 = Edge.Create(4, 1);
            var edge44 = Edge.Create(4, 4);
            var edge52 = Edge.Create(5, 2);
            var edge56 = Edge.Create(5, 6);

            // Not empty acyclic
            var adjacencyGraph1 = createGraph();
            adjacencyGraph1.AddVertexRange( 1, 2, 3 );
            yield return new TestCaseData(adjacencyGraph1)
            {
                ExpectedResult = true
            };

            var adjacencyGraph2 = createGraph();
            adjacencyGraph2.AddVerticesAndEdgeRange(
                edge12, edge14, edge23, edge24
            );
            yield return new TestCaseData(adjacencyGraph2)
            {
                ExpectedResult = true
            };

            var adjacencyGraph3 = createGraph();
            adjacencyGraph3.AddVertex(0);
            adjacencyGraph3.AddVerticesAndEdgeRange(
                edge12, edge14, edge23, edge56
            );
            yield return new TestCaseData(adjacencyGraph3)
            {
                ExpectedResult = true
            };

            // Not acyclic
            var cyclicGraph1 = createGraph();
            cyclicGraph1.AddVerticesAndEdge(edge22);
            yield return new TestCaseData(cyclicGraph1)
            {
                ExpectedResult = false
            };

            var cyclicGraph2 = createGraph();
            cyclicGraph2.AddVerticesAndEdgeRange(
                edge12, edge14, edge22, edge23, edge24
            );
            yield return new TestCaseData(cyclicGraph2)
            {
                ExpectedResult = false
            };

            var cyclicGraph3 = createGraph();
            cyclicGraph3.AddVerticesAndEdgeRange(
                edge12, edge14, edge23, edge24, edge41
            );
            yield return new TestCaseData(cyclicGraph3)
            {
                ExpectedResult = false
            };

            var cyclicGraph4 = createGraph();
            cyclicGraph4.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge31, edge34, edge44
            );
            yield return new TestCaseData(cyclicGraph4)
            {
                ExpectedResult = false
            };

            var cyclicGraph5 = createGraph();
            cyclicGraph5.AddVertex(0);
            cyclicGraph5.AddVerticesAndEdgeRange(
                edge16, edge23, edge25, edge34, edge35, edge52
            );
            yield return new TestCaseData(cyclicGraph5)
            {
                ExpectedResult = false
            };
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> IsDirectedAcyclicGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                IEnumerable<TestCaseData> testCases = CreateIsDirectedAcyclicGraphTestCases(() => new AdjacencyGraph<int, IEdge<int>>())
                    .Concat(CreateIsDirectedAcyclicGraphTestCases(() => new BidirectionalGraph<int, IEdge<int>>()));
                foreach (TestCaseData testCase in testCases)
                {
                    yield return testCase;
                }
            }
        }

        [TestCaseSource(nameof(IsDirectedAcyclicGraphTestCases))]
        public bool IsDirectedAcyclicGraph([NotNull] IVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            return graph.IsDirectedAcyclicGraph();
        }

        [TestCaseSource(nameof(IsDirectedAcyclicGraphTestCases))]
        public bool IsDirectedAcyclicGraph_FromEdges([NotNull] IVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            return graph.Edges.IsDirectedAcyclicGraph<int, IEdge<int>>();
        }

        [Test]
        public void IsDirectedAcyclicGraph_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<int, IEdge<int>>)null).IsDirectedAcyclicGraph());

            Assert.Throws<ArgumentNullException>(
                () => ((IEnumerable<IEdge<int>>)null).IsDirectedAcyclicGraph<int, IEdge<int>>());
            var edges = new[] { Edge.Create(1, 2), null, Edge.Create(1, 3) };
            Assert.Throws<ArgumentNullException>(
                () => edges.IsDirectedAcyclicGraph<int, IEdge<int>>());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> IsUndirectedAcyclicGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                // Empty graph
                yield return new TestCaseData(new UndirectedGraph<int, IEdge<int>>())
                {
                    ExpectedResult = true
                };

                var edge12 = Edge.Create(1, 2);
                var edge14 = Edge.Create(1, 4);
                var edge16 = Edge.Create(1, 6);
                var edge22 = Edge.Create(2, 2);
                var edge23 = Edge.Create(2, 3);
                var edge24 = Edge.Create(2, 4);
                var edge25 = Edge.Create(2, 5);
                var edge35 = Edge.Create(3, 5);
                var edge56 = Edge.Create(5, 6);

                // Not empty acyclic
                var undirectedGraph1 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph1.AddVertexRange( 1, 2, 3 );
                yield return new TestCaseData(undirectedGraph1)
                {
                    ExpectedResult = true
                };

                var undirectedGraph2 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph2.AddVerticesAndEdgeRange(
                    edge12, edge23, edge24
                );
                yield return new TestCaseData(undirectedGraph2)
                {
                    ExpectedResult = true
                };

                var undirectedGraph3 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph3.AddVertex(0);
                undirectedGraph3.AddVerticesAndEdgeRange(
                    edge12, edge14, edge23, edge56
                );
                yield return new TestCaseData(undirectedGraph3)
                {
                    ExpectedResult = true
                };

                // Not acyclic
                var cyclicGraph1 = new UndirectedGraph<int, IEdge<int>>();
                cyclicGraph1.AddVerticesAndEdge(edge22);
                yield return new TestCaseData(cyclicGraph1)
                {
                    ExpectedResult = false
                };

                var cyclicGraph2 = new UndirectedGraph<int, IEdge<int>>();
                cyclicGraph2.AddVerticesAndEdgeRange(
                    edge12, edge14, edge22, edge23, edge24
                );
                yield return new TestCaseData(cyclicGraph2)
                {
                    ExpectedResult = false
                };

                var cyclicGraph3 = new UndirectedGraph<int, IEdge<int>>();
                cyclicGraph3.AddVerticesAndEdgeRange(
                    edge12, edge14, edge23, edge24
                );
                yield return new TestCaseData(cyclicGraph3)
                {
                    ExpectedResult = false
                };

                var cyclicGraph4 = new UndirectedGraph<int, IEdge<int>>();
                cyclicGraph4.AddVertex(0);
                cyclicGraph4.AddVerticesAndEdgeRange(
                    edge16, edge23, edge25, edge35
                );
                yield return new TestCaseData(cyclicGraph4)
                {
                    ExpectedResult = false
                };
            }
        }

        [TestCaseSource(nameof(IsUndirectedAcyclicGraphTestCases))]
        public bool IsUndirectedAcyclicGraph([NotNull] IUndirectedGraph<int, IEdge<int>> graph)
        {
            return graph.IsUndirectedAcyclicGraph();
        }

        [TestCaseSource(nameof(IsUndirectedAcyclicGraphTestCases))]
        public bool IsUndirectedAcyclicGraph_FromEdges([NotNull] IUndirectedGraph<int, IEdge<int>> graph)
        {
            return graph.Edges.IsUndirectedAcyclicGraph<int, IEdge<int>>();
        }

        [Test]
        public void IsUndirectedAcyclicGraph_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).IsUndirectedAcyclicGraph());

            Assert.Throws<ArgumentNullException>(
                () => ((IEnumerable<IEdge<int>>)null).IsUndirectedAcyclicGraph<int, IEdge<int>>());
            var edges = new[] { Edge.Create(1, 2), null, Edge.Create(1, 3) };
            Assert.Throws<ArgumentNullException>(
                () => edges.IsUndirectedAcyclicGraph<int, IEdge<int>>());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ComputePredecessorCost()
        {
            var predecessors = new Dictionary<int, IEdge<int>>();
            var edgeCosts = new Dictionary<IEdge<int>, double>();

            Assert.AreEqual(0, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 1));

            var edge12 = Edge.Create(1, 2);
            predecessors[2] = edge12;
            edgeCosts[edge12] = 12;
            Assert.AreEqual(0, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 1));
            Assert.AreEqual(12, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 2));

            var edge31 = Edge.Create(3, 1);
            predecessors[1] = edge31;
            edgeCosts[edge31] = -5;
            var edge34 = Edge.Create(3, 4);
            predecessors[4] = edge34;
            edgeCosts[edge34] = 42;

            Assert.AreEqual(-5, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 1));
            Assert.AreEqual(7, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 2));
            Assert.AreEqual(0, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 3));
            Assert.AreEqual(42, AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, 4));
        }

        [Test]
        public void ComputePredecessorCost_Throws()
        {
            var predecessors = new Dictionary<TestVertex, IEdge<TestVertex>>();
            var edgeCosts = new Dictionary<IEdge<TestVertex>, double>();
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost(null, edgeCosts, vertex1));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost(predecessors, null, vertex1));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, (TestVertex)null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost<TestVertex, IEdge<TestVertex>>(null, null, vertex1));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost(predecessors, null, null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.ComputePredecessorCost<TestVertex, IEdge<TestVertex>>(null, null, null));

            // Wrong usage
            predecessors[vertex2] = Edge.Create(vertex1, vertex2);
            Assert.Throws<KeyNotFoundException>(
                () => AlgorithmExtensions.ComputePredecessorCost(predecessors, edgeCosts, vertex2));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ComputeDisjointSet()
        {
            var emptyGraph = new UndirectedGraph<int, IEdge<int>>();
            IDisjointSet<int> disjointSet = emptyGraph.ComputeDisjointSet();
            Assert.AreEqual(0, disjointSet.ElementCount);
            Assert.AreEqual(0, disjointSet.SetCount);

            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2, 3, 4 );
            disjointSet = graph.ComputeDisjointSet();
            Assert.AreEqual(4, disjointSet.ElementCount);
            Assert.AreEqual(4, disjointSet.SetCount);

            graph.AddEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4)
            );
            graph.AddVertex(5);
            disjointSet = graph.ComputeDisjointSet();
            Assert.AreEqual(5, disjointSet.ElementCount);
            Assert.AreEqual(2, disjointSet.SetCount);
            Assert.IsTrue(disjointSet.AreInSameSet(1, 2));
            Assert.IsTrue(disjointSet.AreInSameSet(1, 3));
            Assert.IsTrue(disjointSet.AreInSameSet(1, 4));
            Assert.IsFalse(disjointSet.AreInSameSet(1, 5));
        }

        [Test]
        public void ComputeDisjointSet_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).ComputeDisjointSet());
        }

        [Test]
        public void MinimumSpanningTreePrim_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new UndirectedGraph<int, IEdge<int>>().MinimumSpanningTreePrim(null));
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).MinimumSpanningTreePrim(_ => 1.0));
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).MinimumSpanningTreePrim(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void MinimumSpanningTreeKruskal_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new UndirectedGraph<int, IEdge<int>>().MinimumSpanningTreeKruskal(null));
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).MinimumSpanningTreeKruskal(_ => 1.0));
            Assert.Throws<ArgumentNullException>(
                () => ((UndirectedGraph<int, IEdge<int>>)null).MinimumSpanningTreeKruskal(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void OfflineLeastCommonAncestor_Throws()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var graph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            graph1.AddVertexRange( vertex1, vertex2 );
            var pairs1 = new[] { new SEquatableEdge<TestVertex>(vertex1, vertex2) };
            
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<TestVertex, IEdge<TestVertex>>)null).OfflineLeastCommonAncestor(vertex1, pairs1));
            Assert.Throws<ArgumentNullException>(
                () => graph1.OfflineLeastCommonAncestor(null, pairs1));
            Assert.Throws<ArgumentNullException>(
                () => graph1.OfflineLeastCommonAncestor(vertex1, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<TestVertex, IEdge<TestVertex>>)null).OfflineLeastCommonAncestor(null, pairs1));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<TestVertex, IEdge<TestVertex>>)null).OfflineLeastCommonAncestor(vertex1, null));
            Assert.Throws<ArgumentNullException>(
                () => graph1.OfflineLeastCommonAncestor(null, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IVertexListGraph<TestVertex, IEdge<TestVertex>>)null).OfflineLeastCommonAncestor(null, null));

            var pairs2 = new[] { new SEquatableEdge<int>(1, 2) };
            var graph2 = new AdjacencyGraph<int, IEdge<int>>();
            Assert.Throws<ArgumentException>(
                () => graph2.OfflineLeastCommonAncestor(1, pairs2));

            var graph3 = new AdjacencyGraph<int, IEdge<int>>();
            graph3.AddVertex(1);
            Assert.Throws<ArgumentException>(
                () => graph3.OfflineLeastCommonAncestor(1, pairs2));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void MaximumFlow_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2 );
            Func<IEdge<int>, double> capacities = _ => 1.0;
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;
            var reverseEdgesAlgorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);

            Assert.Throws<ArgumentException>(
                () => graph.MaximumFlow(capacities, 1, 1, out _, edgeFactory, reverseEdgesAlgorithm));

            Assert.Throws<InvalidOperationException>(
                () => graph.MaximumFlow(capacities, 1, 2, out _, edgeFactory, reverseEdgesAlgorithm));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CloneTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new AdjacencyGraph<int, EquatableEdge<int>>());
                yield return new TestCaseData(new BidirectionalGraph<int, EquatableEdge<int>>());
            }
        }

        [TestCaseSource(nameof(CloneTestCases))]
        public void Clone([NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> cloned)
        {
            var emptyGraph1 = new AdjacencyGraph<int, EquatableEdge<int>>();
            emptyGraph1.Clone(v => v, (_, v1, v2) => new EquatableEdge<int>(v1, v2), cloned);
            AssertEmptyGraph(cloned);

            cloned.Clear();
            var notEmptyGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
            notEmptyGraph.AddVerticesAndEdgeRange(
                new EquatableEdge<int>(1, 2),
                new EquatableEdge<int>(2, 2),
                new EquatableEdge<int>(2, 3),
                new EquatableEdge<int>(3, 1)
            );
            notEmptyGraph.Clone(v => v, (_, v1, v2) => new EquatableEdge<int>(v1, v2), cloned);
            cloned.AssertHasVertices(1, 2, 3);
            cloned.AssertHasEdges(new EquatableEdge<int>[] {
                    new (1, 2),
                    new (2, 2),
                    new (2, 3),
                    new (3, 1)
                });

            // Clone is not empty
            cloned.Clear();
            cloned.AddVerticesAndEdge(new EquatableEdge<int>(1, 4));
            notEmptyGraph.Clone(v => v, (_, v1, v2) => new EquatableEdge<int>(v1, v2), cloned);
            // Clone has been cleaned and then re-filled
            cloned.AssertHasVertices(1, 2, 3 );
            cloned.AssertHasEdges(new EquatableEdge<int>[]
                {
                    new (1, 2),
                    new (2, 2),
                    new (2, 3),
                    new (3, 1)
                });
        }

        [Test]
        public void Clone_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var clone = new AdjacencyGraph<int, IEdge<int>>();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone(null, v => v, (e, _, _) => e, clone));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(null, (e, _, _) => e, clone));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(v => v, null, clone));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(v => v, (e, _, _) => e, null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone(null, null, (e, _, _) => e, clone));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone(null, v => v, null, clone));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone<int, IEdge<int>>(null, v => v, (e, _, _) => e, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(null, null, clone));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(null, (e, _, _) => e, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(v => v, null, null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone(null, null, null, clone));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone<int, IEdge<int>>(null, null, (e, _, _) => e, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.Clone(null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => AlgorithmExtensions.Clone<int, IEdge<int>>(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}