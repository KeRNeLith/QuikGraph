using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms.VertexColoring;


namespace QuikGraph.Tests.Algorithms.GraphColoring
{
    /// <summary> Tests for <see cref="VertexColoringAlgorithm{TVertex,TEdge}"/>. </summary>
    [TestFixture]
    public static class VertexColoringAlgorithmTests
    {
        [Test]
        public static void TestConstructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = new VertexColoringAlgorithm<int, IEdge<int>>(graph);
            algorithm.AssertAlgorithmState(graph);
            CollectionAssert.IsEmpty(algorithm.Colors);
        }

        [Test]
        public static void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new VertexColoringAlgorithm<int, IEdge<int>>(null));
        }

        [Test]
        public static void VertexColoringEmptyGraph()
        {
            var graph = new UndirectedGraph<char, Edge<char>>(true);
            var algorithm = graph.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Expecting to no get any color
            Assert.AreEqual(0, coloredVertices.Values.Count);
        }

        [Test]
        public static void VertexColoringNoEdge()
        {
            var graphDisconnected = CreateDisconnectedGraph();

            var algorithm = graphDisconnected.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Graph doesn't have first vertex color
            Assert.IsFalse(coloredVertices.Values.Contains(1));

            int?[] result = coloredVertices.Values.ToArray();

            // Expecting to get only 1 color
            Assert.AreEqual(1, result.Max() + 1);

            // Not equal to null 
            foreach (int? color in result)
            {
                Assert.AreNotEqual(null, color);
            }

            // and corresponding colors of vertices
            Assert.AreEqual(0, result[0]); // 0 vertex = 0 color
            Assert.AreEqual(0, result[1]); // 1 vertex = 0 color
            Assert.AreEqual(0, result[2]); // 2 vertex = 0 color
            Assert.AreEqual(0, result[3]); // 3 vertex = 0 color
            Assert.AreEqual(0, result[4]); // 4 vertex = 0 color

        }

        public static UndirectedGraph<char, IEdge<char>> CreateDisconnectedGraph(char first = '0', char last = '4')
        {
            var graphDisconnected = new UndirectedGraph<char, IEdge<char>>(true);
            AddVertices(graphDisconnected, first, last);
            return graphDisconnected;
        }

        /// <summary> Generate undirected simple graph </summary>
        /// <remarks>
        /// <pre>
        ///     (1)
        ///    / | \ 
        /// (0)  |  (3)-(4)
        ///    \ | /
        ///     (2)
        /// </pre>
        /// </remarks>
        [Test]
        public static void VertexColoringSimpleGraph()
        {
            var graph = CreateDisconnectedGraph();
            AddEdgesTo(graph);
            var algorithm = graph.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Graph doesn't have third vertex color
            Assert.IsFalse(coloredVertices.Values.Contains(3));

            int?[] result = coloredVertices.Values.ToArray();

            // Expecting to get 3 different colors
            Assert.AreEqual(3, result.Max() + 1);

            // Not equal to null 
            foreach (int? color in result)
            {
                Assert.AreNotEqual(null, color);
            }

            // and corresponding colors of vertices
            Assert.AreEqual(0, result[0]); // 0 vertex = 0 color
            Assert.AreEqual(1, result[1]); // 1 vertex = 1 color
            Assert.AreEqual(2, result[2]); // 2 vertex = 2 color
            Assert.AreEqual(0, result[3]); // 3 vertex = 0 color
            Assert.AreEqual(1, result[4]); // 4 vertex = 1 color
        }

        private static void AddEdgesTo(UndirectedGraph<char, IEdge<char>> g)
        {
            g.AddEdge(Edge.Create('0', '1'));
            g.AddEdge(Edge.Create('0', '2'));
            g.AddEdge(Edge.Create('1', '2'));
            g.AddEdge(Edge.Create('1', '3'));
            g.AddEdge(Edge.Create('2', '3'));
            g.AddEdge(Edge.Create('3', '4'));
        }

        [Test]
        public static void VertexColoringGraph()
        {
            var graph = CreateTestGraph07();
            var algorithm = graph.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Graph doesn't have third vertex color
            Assert.IsFalse(coloredVertices.Values.Contains(3));

            int?[] result = coloredVertices.Values.ToArray();

            // Expecting to get 3 different colors
            Assert.AreEqual(3, result.Max() + 1);

            // Not equal to null 
            foreach (int? color in result)
            {
                Assert.AreNotEqual(null, color);
            }

            // And corresponding colors of vertices
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
            Assert.AreEqual(1, result[2]);
            Assert.AreEqual(1, result[3]);
            Assert.AreEqual(2, result[4]);
            Assert.AreEqual(0, result[5]);
            Assert.AreEqual(0, result[6]);
            Assert.AreEqual(1, result[7]);
        }

        /// <summary> graph has a minimum number of vertex colors only if to swap (1) and (4) vertices </summary>
        /// <returns>
        /// <pre>
        ///    (2)      (7)-(5)
        ///   /   \     /
        /// (1)   (4)-(0)
        ///   \   /
        ///    (3)  (6)
        /// </pre>
        /// </returns>
        public static UndirectedGraph<char, IEdge<char>> CreateTestGraph07()
        {
            var g = new UndirectedGraph<char, IEdge<char>>(true);

            AddVertices(g, '0', '7');

            g.AddEdge(Edge.Create('0', '4'));
            g.AddEdge(Edge.Create('1', '2'));
            g.AddEdge(Edge.Create('1', '3'));
            g.AddEdge(Edge.Create('2', '4'));
            g.AddEdge(Edge.Create('3', '4'));
            g.AddEdge(Edge.Create('5', '7'));
            g.AddEdge(Edge.Create('7', '0'));

            return g;
        }

        [Test]
        public static void VertexColoringCompleteGraph()
        {
            var graph = CreateFullGraph05();
            var algorithm = graph.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Graph doesn't have sixth vertex color
            Assert.IsFalse(coloredVertices.Values.Contains(6));

            int?[] result = coloredVertices.Values.ToArray();

            // Expecting to get 6 different colors
            Assert.AreEqual(6, result.Max() + 1);

            // Not equal to null 
            foreach (int? color in result)
            {
                Assert.AreNotEqual(null, color);
            }

            // and corresponding colors of vertices
            Assert.AreEqual(0, result[0]); // 0 vertex = 0 color
            Assert.AreEqual(1, result[1]); // 1 vertex = 1 color
            Assert.AreEqual(2, result[2]); // 2 vertex = 2 color
            Assert.AreEqual(3, result[3]); // 3 vertex = 3 color
            Assert.AreEqual(4, result[4]); // 4 vertex = 4 color
            Assert.AreEqual(5, result[5]); // 5 vertex = 5 color
        }

        /// <summary> Generate undirected full graph </summary>
        /// <returns>
        /// A Full graph
        /// <pre>
        ///    _____(2)_____
        ///   /    / | \    \
        /// (0)-(1)--+--(4)-(5) 
        ///   \    \ | /    /
        ///    \____(3)____/
        ///  + edges: (0-4), (0-5) and (1-5)
        /// </pre>
        /// </returns> 
        public static UndirectedGraph<char, IEdge<char>> CreateFullGraph05()
        {
            var g = new UndirectedGraph<char, IEdge<char>>(true);
            AddVertices(g, '0', '5');

            g.AddEdge(Edge.Create('0', '1'));
            g.AddEdge(Edge.Create('0', '2'));
            g.AddEdge(Edge.Create('0', '3'));
            g.AddEdge(Edge.Create('0', '4'));
            g.AddEdge(Edge.Create('0', '5'));
            g.AddEdge(Edge.Create('1', '2'));
            g.AddEdge(Edge.Create('1', '3'));
            g.AddEdge(Edge.Create('1', '4'));
            g.AddEdge(Edge.Create('1', '5'));
            g.AddEdge(Edge.Create('2', '3'));
            g.AddEdge(Edge.Create('2', '4'));
            g.AddEdge(Edge.Create('2', '5'));
            g.AddEdge(Edge.Create('3', '4'));
            g.AddEdge(Edge.Create('3', '5'));
            g.AddEdge(Edge.Create('4', '5'));

            return g;
        }

        public static void AddVertices(this IMutableVertexSet<char> g, char first, char last)
        {
            for(char i = first; i <= last; i++)
            {
                g.AddVertex(i);
            }
        }

        [Test]
        public static void VertexColoringBipartiteGraph()
        {
            var graph = CreateBipartiteGraph();
            var algorithm = graph.ComputeVertexColoring();

            IDictionary<char, int?> coloredVertices = algorithm.Colors;

            // Graph doesn't have color 2
            Assert.IsFalse(coloredVertices.Values.Contains(2));

            int?[] colors = coloredVertices.Values.ToArray();

            // Expecting to get 2 different colors
            Assert.AreEqual(2, colors.Max() + 1);

            // Not equal to null 
            foreach (int? color in colors)
            {
                Assert.AreNotEqual(null, color);
            }

            // and corresponding colors of vertices
            Assert.AreEqual(0, coloredVertices['0']);
            Assert.AreEqual(0, coloredVertices['1']);
            Assert.AreEqual(0, coloredVertices['2']);
            Assert.AreEqual(1, coloredVertices['3']);
            Assert.AreEqual(1, coloredVertices['4']);
            Assert.AreEqual(1, coloredVertices['5']);
            Assert.AreEqual(1, coloredVertices['6']);
        }

        /// <summary>
        /// Generate undirected bipartite graph
        /// </summary>
        /// <returns>
        ///       (3)
        ///      / 
        /// +-(4)-(1)-+
        /// |  |   |  |
        /// | (0)-(5) |
        /// |    /    |
        /// +-(2)-(6)-+
        /// </returns>
        public static UndirectedGraph<char, IEdge<char>> CreateBipartiteGraph()
        {
            var g = new UndirectedGraph<char, IEdge<char>>(true);

            AddVertices(g, '0', '6');

            g.AddEdge(Edge.Create('0', '4'));
            g.AddEdge(Edge.Create('0', '5'));
            g.AddEdge(Edge.Create('1', '3'));
            g.AddEdge(Edge.Create('1', '4'));
            g.AddEdge(Edge.Create('1', '5'));
            g.AddEdge(Edge.Create('1', '6'));
            g.AddEdge(Edge.Create('2', '5'));
            g.AddEdge(Edge.Create('2', '6'));
            g.AddEdge(Edge.Create('2', '4'));

            return g;
        }
    }
}
