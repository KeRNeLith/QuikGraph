using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.TopologicalSort;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary> Tests for <see cref="TopologicalSortAlgorithm{TVertex,TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class TopologicalSortAlgorithmTests
    {

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public static void RunTopologicalSortAndCheck<TVertex, TEdge>([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.ComputeTopologicalSortAlgorithm();

            Assert.IsNotNull(algorithm.SortedVertices);
            Assert.AreEqual(graph.VertexCount, algorithm.SortedVertices.Length);
        }

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTopologicalSortAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateTopologicalSortAlgorithm(-10);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateTopologicalSortAlgorithm(0);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateTopologicalSortAlgorithm(10);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                TopologicalSortAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                Assert.IsNull(algo.SortedVertices);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => TopologicalSortAlgorithm.CreateTopologicalSortAlgorithm<int, IEdge<int>>(null));
        }

        [Test]
        public void OneTwo()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddEdge(Edge.Create(1, 2));

            var algorithm = graph.ComputeTopologicalSortAlgorithm(graph.VertexCount);

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                algorithm.SortedVertices);
        }

        // Trying to see if order of vertices affects the topological sort order
        [Test]
        public void TwoOne()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            // Deliberately adding 1 and then 2, before adding edge (2, 1).
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddEdge(Edge.Create(2, 1));

            var algorithm = graph.ComputeTopologicalSortAlgorithm(graph.VertexCount);

            CollectionAssert.AreEqual(
                new[] { 2, 1 },
                algorithm.SortedVertices);
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 6),
                Edge.Create(2, 8),
                Edge.Create(4, 2),
                Edge.Create(4, 5),
                Edge.Create(5, 6),
                Edge.Create(7, 5),
                Edge.Create(7, 8)
            );

            var algorithm = graph.ComputeTopologicalSortAlgorithm();

            CollectionAssert.AreEqual(
                new[] { 7, 4, 5, 1, 2, 8, 6, 3 },
                algorithm.SortedVertices);
        }

        [Test]
        public void ForestGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 4),

                Edge.Create(5, 6)
            );

            var algorithm = graph.ComputeTopologicalSortAlgorithm();

            CollectionAssert.AreEqual(
                new[] { 5, 6, 0, 1, 2, 3, 4 },
                algorithm.SortedVertices);
        }

        [Test]
        public void GraphWithSelfEdge_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(0, 1),
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(2, 2),
                Edge.Create(3, 4)
            );

            var algorithm = graph.CreateTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(algorithm.Compute);
        }

        [Test]
        public void TopologicalSort_DCT8()
        {
            var graph = TestGraphFactory.LoadGraph(GetGraphFilePath("DCT8.graphml"));
            RunTopologicalSortAndCheck(graph);
        }

        [Test]
        public void TopologicalSort_Throws()
        {
            var cyclicGraph = new AdjacencyGraph<int, IEdge<int>>();
            cyclicGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(1, 4),
                Edge.Create(3, 1)
            );

            var algorithm = cyclicGraph.CreateTopologicalSortAlgorithm();
            Assert.Throws<CyclicGraphException>(algorithm.Compute);
        }

        #region Test classes

        private class Letter
        {
            private readonly char _char;

            public Letter(char letter)
            {
                _char = letter;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return _char.ToString();
            }
        }

        #endregion

        [Test]
        public void FacebookSeattleWordPuzzle()
        {
            /* A puzzle from Facebook Seattle opening party:
            http://www.facebook.com/note.php?note_id=146727365346299
            You are given a list of relationships between the letters in a single word, all of which are in the form: 
            "The first occurrence of A comes before N occurrences of B." 
            You can safely assume that you have all such relationships except for any in which N would be 0. 
            Determine the original word, then go to http://www.facebook.com/seattle/[insert-word-here] to find the second part of the puzzle.

            The first occurrence of 'e' comes before 1 occurrence of 's'.
            The first occurrence of 'i' comes before 1 occurrence of 'n'.
            The first occurrence of 'i' comes before 1 occurrence of 'i'.
            The first occurrence of 'n' comes before 2 occurrences of 'e'.
            The first occurrence of 'e' comes before 1 occurrence of 'e'.
            The first occurrence of 'i' comes before 1 occurrence of 'v'.
            The first occurrence of 'n' comes before 1 occurrence of 'i'.
            The first occurrence of 'n' comes before 1 occurrence of 'v'.
            The first occurrence of 'i' comes before 1 occurrence of 's'.
            The first occurrence of 't' comes before 1 occurrence of 's'.
            The first occurrence of 'v' comes before 1 occurrence of 's'.
            The first occurrence of 'v' comes before 2 occurrences of 'e'.
            The first occurrence of 't' comes before 2 occurrences of 'e'.
            The first occurrence of 'i' comes before 2 occurrences of 'e'.
            The first occurrence of 'v' comes before 1 occurrence of 't'.
            The first occurrence of 'n' comes before 1 occurrence of 't'.
            The first occurrence of 'v' comes before 1 occurrence of 'i'.
            The first occurrence of 'i' comes before 1 occurrence of 't'.
            The first occurrence of 'n' comes before 1 occurrence of 's'.
            */

            var graph = new AdjacencyGraph<Letter, IEdge<Letter>>();

            // A more generalized algorithm would handle duplicate letters automatically.
            // This is the quick and dirty solution.
            var i1 = new Letter('i');
            var i2 = new Letter('i');
            var e1 = new Letter('e');
            var e2 = new Letter('e');

            var s = new Letter('s');
            var n = new Letter('n');
            var t = new Letter('t');
            var v = new Letter('v');

            graph.AddVertexRange(e1, e2, s, i1, i2, n, t, v );

            graph.AddEdge(Edge.Create(e1, s));
            graph.AddEdge(Edge.Create(i1, n));
            graph.AddEdge(Edge.Create(i1, i2));
            graph.AddEdge(Edge.Create(n, e1));
            graph.AddEdge(Edge.Create(n, e2));
            graph.AddEdge(Edge.Create(e1, e2));
            graph.AddEdge(Edge.Create(i1, v));
            graph.AddEdge(Edge.Create(n, e1));
            graph.AddEdge(Edge.Create(n, v));
            graph.AddEdge(Edge.Create(i1, s));
            graph.AddEdge(Edge.Create(t, s));
            graph.AddEdge(Edge.Create(v, s));
            graph.AddEdge(Edge.Create(v, e1));
            graph.AddEdge(Edge.Create(v, e2));
            graph.AddEdge(Edge.Create(t, e1));
            graph.AddEdge(Edge.Create(t, e2));
            graph.AddEdge(Edge.Create(i1, e1));
            graph.AddEdge(Edge.Create(i1, e2));
            graph.AddEdge(Edge.Create(v, t));
            graph.AddEdge(Edge.Create(n, t));
            graph.AddEdge(Edge.Create(v, i2));
            graph.AddEdge(Edge.Create(i1, t));
            graph.AddEdge(Edge.Create(n, s));

            var sort = graph.ComputeTopologicalSortAlgorithm();

            var builder = new StringBuilder();
            foreach (Letter item in sort.SortedVertices)
            {
                builder.Append(item);
            }
            string word = builder.ToString();

            Assert.AreEqual("invitees", word);
        }
    }
}