using System;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms.ShortestPath;

namespace QuikGraph.Tests.Algorithms.ShortestPath
{
    /// <summary>
    /// Tests for <see cref="YenShortestPathsAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class YenShortestPathsAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            Func<EquatableTaggedEdge<int, double>, double> weights = edge => edge.Tag;

            var graph = new AdjacencyGraph<int, EquatableTaggedEdge<int, double>>();
            graph.AddVertexRange(new[] { 1, 2 });
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>(graph, 1, 2, int.MaxValue, weights));
            Assert.DoesNotThrow(() => new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>(graph, 1, 2, 10, weights));

            Assert.DoesNotThrow(() => new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>(graph, 1, 2, int.MaxValue, weights, paths => paths.Where(path => path.Count() > 2)));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Constructor_Throws()
        {
            Func<EquatableTaggedEdge<TestVertex, double>, double> weights = edge => edge.Tag;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");

            var graph = new AdjacencyGraph<TestVertex, EquatableTaggedEdge<TestVertex, double>>();
            Assert.Throws<ArgumentException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, vertex2, int.MaxValue, weights));

            graph = new AdjacencyGraph<TestVertex, EquatableTaggedEdge<TestVertex, double>>();
            graph.AddVertex(vertex1);
            Assert.Throws<ArgumentException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, vertex2, int.MaxValue, weights));

            graph = new AdjacencyGraph<TestVertex, EquatableTaggedEdge<TestVertex, double>>();
            graph.AddVertex(vertex2);
            Assert.Throws<ArgumentException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, vertex2, int.MaxValue, weights));

            graph = new AdjacencyGraph<TestVertex, EquatableTaggedEdge<TestVertex, double>>();
            graph.AddVertexRange(new[] { vertex1, vertex2 });

            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(null, vertex1, vertex2, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, null, vertex2, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, null, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(null, null, vertex2, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(null, vertex1, null, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, null, null, int.MaxValue, weights));
            Assert.Throws<ArgumentNullException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(null, null, null, int.MaxValue, weights));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, vertex2, 0, weights));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new YenShortestPathsAlgorithm<TestVertex, EquatableTaggedEdge<TestVertex, double>>(graph, vertex1, vertex2, -1, weights));
            // ReSharper restore ObjectCreationAsStatement
        }

        /// <summary>
        /// Attempt to use for simple graph.
        /// Expecting that Dijkstra’s algorithm couldn't find any ways.
        /// </summary>
        [Test]
        public void SimpleNoPathGraph()
        {
            var graph = new AdjacencyGraph<char, EquatableTaggedEdge<char, double>>(true);
            graph.AddVertex('1');

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Func<EquatableTaggedEdge<char, double>, double> weights = edge => edge.Tag;
            var algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '1', 10, weights);
            Assert.Throws<NoPathFoundException>(() => algorithm.Execute());

            graph.AddVertex('2');
            algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '2', 10, weights);
            Assert.Throws<NoPathFoundException>(() => algorithm.Execute());
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        /// <summary>
        /// Attempt to use for loop graph.
        /// Expecting that Dijkstra’s algorithm couldn't find any ways.
        /// </summary>
        [Test]
        public void LoopGraph()
        {
            var graph = new AdjacencyGraph<char, EquatableTaggedEdge<char, double>>(true);
            graph.AddVertexRange("1");

            Func<EquatableTaggedEdge<char, double>, double> weights = edge => edge.Tag;
            var algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '1', 10, weights);
            graph.AddEdge(new EquatableTaggedEdge<char, double>('1', '1', 7));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NoPathFoundException>(() => algorithm.Execute());
        }

        [Test]
        public void GraphWithCycle()
        {
            var graph = new AdjacencyGraph<char, EquatableTaggedEdge<char, double>>(true);
            graph.AddVertexRange("12345");
            var edges = new[]
            {
                new EquatableTaggedEdge<char, double>('1', '2', 1.0),
                new EquatableTaggedEdge<char, double>('1', '3', 12.0),
                new EquatableTaggedEdge<char, double>('1', '4', 0.5),
                new EquatableTaggedEdge<char, double>('2', '3', 3.0),
                new EquatableTaggedEdge<char, double>('2', '4', 2.0),
                new EquatableTaggedEdge<char, double>('3', '5', 1.0),
                new EquatableTaggedEdge<char, double>('5', '2', 6.0)
            };
            graph.AddEdgeRange(edges);

            Func<EquatableTaggedEdge<char, double>, double> weights = edge => edge.Tag;
            var algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '5', 10, weights);
            YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>.SortedPath[] paths = algorithm.Execute().ToArray();

            // Expecting to get 2 paths:
            // 1 => 1-2-3-5
            // 2 => 1-3-5
            // Consistently checking the result
            Assert.AreEqual(2, paths.Length);
            // 1
            EquatableTaggedEdge<char, double>[] path0 = paths[0].ToArray();
            Assert.AreEqual(path0[0], edges[0]);
            Assert.AreEqual(path0[1], edges[3]);
            Assert.AreEqual(path0[2], edges[5]);
            // 2
            EquatableTaggedEdge<char, double>[] path1 = paths[1].ToArray();
            Assert.AreEqual(path1[0], edges[1]);
            Assert.AreEqual(path1[1], edges[5]);
        }

        [Test]
        public void GraphWithMultiplePaths()
        {
            var graph = new AdjacencyGraph<string, EquatableTaggedEdge<string, double>>(false);
            graph.AddVertexRange(new[] { "A", "B", "C", "D" });
            var edges = new[]
            {
                new EquatableTaggedEdge<string, double>("A", "B", 5),
                new EquatableTaggedEdge<string, double>("A", "C", 6),
                new EquatableTaggedEdge<string, double>("B", "C", 7),
                new EquatableTaggedEdge<string, double>("B", "D", 8),
                new EquatableTaggedEdge<string, double>("C", "D", 9)
            };
            graph.AddEdgeRange(edges);

            Func<EquatableTaggedEdge<string, double>, double> weights = edge => edge.Tag;
            var algorithm = new YenShortestPathsAlgorithm<string, EquatableTaggedEdge<string, double>>(graph, "A", "D", 5, weights);
            YenShortestPathsAlgorithm<string, EquatableTaggedEdge<string, double>>.SortedPath[] paths = algorithm.Execute().ToArray();

            // Expecting to get 3 paths:
            // 1 => A-B-D
            // 2 => A-C-D
            // 3 => A-B-C-D
            // Consistently checking the result
            Assert.AreEqual(3, paths.Length);
            // 1
            EquatableTaggedEdge<string, double>[] path0 = paths[0].ToArray();
            Assert.AreEqual(path0[0], edges[0]);
            Assert.AreEqual(path0[1], edges[3]);
            // 2
            EquatableTaggedEdge<string, double>[] path1 = paths[1].ToArray();
            Assert.AreEqual(path1[0], edges[1]);
            Assert.AreEqual(path1[1], edges[4]);
            // 3
            EquatableTaggedEdge<string, double>[] path2 = paths[2].ToArray();
            Assert.AreEqual(path2[0], edges[0]);
            Assert.AreEqual(path2[1], edges[2]);
            Assert.AreEqual(path2[2], edges[4]);
        }

        [Test]
        public void GraphWithMultiplePaths_KShortest()
        {
            var graph = new AdjacencyGraph<char, EquatableTaggedEdge<char, double>>(false);
            graph.AddVertexRange("CDEFGH");
            var edges = new[]
            {
                new EquatableTaggedEdge<char, double>('C', 'D', 3),
                new EquatableTaggedEdge<char, double>('C', 'E', 2),
                new EquatableTaggedEdge<char, double>('D', 'F', 4),
                new EquatableTaggedEdge<char, double>('E', 'D', 1),
                new EquatableTaggedEdge<char, double>('E', 'F', 2),
                new EquatableTaggedEdge<char, double>('E', 'G', 3),
                new EquatableTaggedEdge<char, double>('F', 'G', 2),
                new EquatableTaggedEdge<char, double>('F', 'H', 1),
                new EquatableTaggedEdge<char, double>('G', 'H', 2)
            };
            graph.AddEdgeRange(edges);

            // K = 5
            Func<EquatableTaggedEdge<char, double>, double> weights = edge => edge.Tag;
            var algorithmK5 = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, 'C', 'H', 5, weights);
            YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>.SortedPath[] paths = algorithmK5.Execute().ToArray();

            // Expecting to get 5 paths:
            // 1 => C-E-F-H
            // 2 => C-E-G-H
            // 3 => C-E-F-G-H
            // 4 => C-E-D-F-H
            // 5 => C-D-F-H
            // Consistently checking the result
            Assert.AreEqual(5, paths.Length);
            CheckFiveFirstPaths(paths);


            // K = 50
            var algorithmK50 = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, 'C', 'H', 50, weights);
            paths = algorithmK50.Execute().ToArray();

            // Expecting to get 7 paths:
            // 1 => C-E-F-H
            // 2 => C-E-G-H
            // 3 => C-E-F-G-H
            // 4 => C-E-D-F-H
            // 5 => C-D-F-H
            // 6 => C-E-D-F-G-H
            // 7 => C-D-F-G-H
            // Consistently checking the result
            Assert.AreEqual(7, paths.Length);
            CheckFiveFirstPaths(paths);
            // 6
            EquatableTaggedEdge<char, double>[] path5 = paths[5].ToArray();
            Assert.AreEqual(path5[0], edges[1]);    // C-E
            Assert.AreEqual(path5[1], edges[3]);    // E-D
            Assert.AreEqual(path5[2], edges[2]);    // D-F
            Assert.AreEqual(path5[3], edges[6]);    // F-G
            Assert.AreEqual(path5[4], edges[8]);    // G-H
            // 7
            EquatableTaggedEdge<char, double>[] path6 = paths[6].ToArray();
            Assert.AreEqual(path6[0], edges[0]);    // C-D
            Assert.AreEqual(path6[1], edges[2]);    // D-F
            Assert.AreEqual(path6[2], edges[6]);    // F-G
            Assert.AreEqual(path6[3], edges[8]);    // G-H

            #region Local function

            void CheckFiveFirstPaths(YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>.SortedPath[] ps)
            {
                // 1
                EquatableTaggedEdge<char, double>[] path0 = ps[0].ToArray();
                Assert.AreEqual(path0[0], edges[1]);    // C-E
                Assert.AreEqual(path0[1], edges[4]);    // E-F
                Assert.AreEqual(path0[2], edges[7]);    // F-H
                // 2
                EquatableTaggedEdge<char, double>[] path1 = ps[1].ToArray();
                Assert.AreEqual(path1[0], edges[1]);    // C-E
                Assert.AreEqual(path1[1], edges[5]);    // E-G
                Assert.AreEqual(path1[2], edges[8]);    // G-H
                // 3
                EquatableTaggedEdge<char, double>[] path2 = ps[2].ToArray();
                Assert.AreEqual(path2[0], edges[1]);    // C-E
                Assert.AreEqual(path2[1], edges[4]);    // E-F
                Assert.AreEqual(path2[2], edges[6]);    // F-G
                Assert.AreEqual(path2[3], edges[8]);    // G-H
                // 4
                EquatableTaggedEdge<char, double>[] path3 = ps[3].ToArray();
                Assert.AreEqual(path3[0], edges[1]);    // C-E
                Assert.AreEqual(path3[1], edges[3]);    // E-D
                Assert.AreEqual(path3[2], edges[2]);    // D-F
                Assert.AreEqual(path3[3], edges[7]);    // F-H
                // 5
                EquatableTaggedEdge<char, double>[] path4 = ps[4].ToArray();
                Assert.AreEqual(path4[0], edges[0]);    // C-D
                Assert.AreEqual(path4[1], edges[2]);    // D-F
                Assert.AreEqual(path4[2], edges[7]);    // F-H
            }

            #endregion
        }

        [Test]
        public void MultipleRunMethods()
        {
            Func<EquatableTaggedEdge<char, double>, double> weights = edge => edge.Tag;
            AdjacencyGraph<char, EquatableTaggedEdge<char, double>> graph = GenerateGraph(
                out EquatableTaggedEdge<char, double>[] graphEdges);

            // Default filter function case
            var algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '5', 10, weights);
            RunYenAndCheck(algorithm);

            // Custom filter function case
            algorithm = new YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>(graph, '1', '5', 10, weights, e => e);
            RunYenAndCheck(algorithm);

            #region Local functions

            AdjacencyGraph<char, EquatableTaggedEdge<char, double>> GenerateGraph(
                out EquatableTaggedEdge<char, double>[] edges)
            {
                var g = new AdjacencyGraph<char, EquatableTaggedEdge<char, double>>(true);

                g.AddVertexRange("123456");
                edges = new[]
                {
                    new EquatableTaggedEdge<char, double>('1', '2', 7),
                    new EquatableTaggedEdge<char, double>('1', '3', 9),
                    new EquatableTaggedEdge<char, double>('1', '6', 14),
                    new EquatableTaggedEdge<char, double>('2', '3', 10),
                    new EquatableTaggedEdge<char, double>('2', '4', 15),
                    new EquatableTaggedEdge<char, double>('3', '4', 11),
                    new EquatableTaggedEdge<char, double>('3', '6', 2),
                    new EquatableTaggedEdge<char, double>('4', '5', 6),
                    new EquatableTaggedEdge<char, double>('5', '6', 9)
                };
                g.AddEdgeRange(edges);

                return g;
            }

            void RunYenAndCheck(YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>> yen)
            {
                // Generate simple graph
                // like this https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
                // but with directed edges input graph
                YenShortestPathsAlgorithm<char, EquatableTaggedEdge<char, double>>.SortedPath[] paths = yen.Execute().ToArray();

                // Expecting to get 3 paths:
                // 1 => 1-3-4-5
                // 2 => 1-2-4-5
                // 3 => 1-2-3-4-5
                // Consistently checking the result
                Assert.AreEqual(3, paths.Length);
                // 1
                EquatableTaggedEdge<char, double>[] path0 = paths[0].ToArray();
                Assert.AreEqual(path0[0], graphEdges[1]);
                Assert.AreEqual(path0[1], graphEdges[5]);
                Assert.AreEqual(path0[2], graphEdges[7]);
                // 2
                EquatableTaggedEdge<char, double>[] path1 = paths[1].ToArray();
                Assert.AreEqual(path1[0], graphEdges[0]);
                Assert.AreEqual(path1[1], graphEdges[4]);
                Assert.AreEqual(path1[2], graphEdges[7]);
                // 3
                EquatableTaggedEdge<char, double>[] path2 = paths[2].ToArray();
                Assert.AreEqual(path2[0], graphEdges[0]);
                Assert.AreEqual(path2[1], graphEdges[3]);
                Assert.AreEqual(path2[2], graphEdges[5]);
                Assert.AreEqual(path2[3], graphEdges[7]);
            }

            #endregion
        }

        [Test]
        public void SortedPathHashCode()
        {
            var edges = new[]
            {
                new EquatableTaggedEdge<int, double>(1, 2, 1.0),
                new EquatableTaggedEdge<int, double>(2, 3, 1.0),
                new EquatableTaggedEdge<int, double>(3, 4, 1.0)
            };
            var path1 = new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>.SortedPath(edges);
            var path2 = new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>.SortedPath(edges);

            var path3 = new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>.SortedPath(new[]
            {
                new EquatableTaggedEdge<int, double>(1, 2, 1.0),
                new EquatableTaggedEdge<int, double>(2, 3, 1.0),
                new EquatableTaggedEdge<int, double>(3, 4, 1.0)
            });

            Assert.AreEqual(path1.GetHashCode(), path1.GetHashCode());
            Assert.AreNotEqual(path1.GetHashCode(), path2.GetHashCode());
            Assert.AreNotEqual(path1.GetHashCode(), path3.GetHashCode());
            Assert.AreNotEqual(path2.GetHashCode(), path3.GetHashCode());
        }

        [Test]
        public void SortedPathEnumeration()
        {
            var edges = new[]
            {
                new EquatableTaggedEdge<int, double>(1, 2, 1.0),
                new EquatableTaggedEdge<int, double>(2, 3, 1.0),
                new EquatableTaggedEdge<int, double>(3, 4, 1.0)
            };

            var path = new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>.SortedPath(edges);
            CollectionAssert.AreEqual(edges, path);

            CollectionAssert.IsEmpty(
                new YenShortestPathsAlgorithm<int, EquatableTaggedEdge<int, double>>.SortedPath(
                    Enumerable.Empty<EquatableTaggedEdge<int, double>>()));
        }
    }
}