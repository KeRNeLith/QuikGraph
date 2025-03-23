using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MinimumSpanningTree;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Collections;
using QuikGraph.Serialization;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms.MinimumSpanningTree
{
    /// <summary> Base class for minimum spanning tree tests. </summary>
    internal abstract class MinimumSpanningTreeTestsBase
    {
        #region Test helpers

        [Pure]
        [NotNull]
        protected static UndirectedGraph<string, TaggedEdge<string, double>> GetUndirectedCompleteGraph(int vertex)
        {
            var random = new Random();
            var graph = new UndirectedGraph<string, TaggedEdge<string, double>>();
            var trueGraph = new UndirectedGraph<string, TaggedEdge<string, double>>();
            var sets = new ForestDisjointSet<string>(vertex);
            for (int i = 0; i < vertex; ++i)
            {
                graph.AddVertex(i.ToString());
                trueGraph.AddVertex(i.ToString());
                sets.MakeSet(i.ToString());
            }

            for (int i = 0; i < vertex; ++i)
            {
                for (int j = i + 1; j < vertex; ++j)
                {
                    graph.AddEdge(
                        new TaggedEdge<string, double>(
                            i.ToString(),
                            j.ToString(),
                            random.Next(100)));
                }
            }

            return graph;
        }

        private static double CompareRoot<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.AdjacentDegree(edge.Source) + 1 ?? Double.PositiveInfinity;

            TEdge[] prim = graph.MinimumSpanningTreePrim(e => distances[e]).ToArray();
            TEdge[] kruskal = graph.MinimumSpanningTreeKruskal(e => distances[e]).ToArray();

            double primCost = prim.Sum(e => distances[e]);
            double kruskalCost = kruskal.Sum(e => distances[e]);
            if (Math.Abs(primCost - kruskalCost) > double.Epsilon)
                Assert.Fail("Cost do not match.");

            return kruskalCost;
        }

        private static void AssertSpanningTree<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IEnumerable<TEdge> tree)
            where TEdge : IEdge<TVertex>
        {
            var spanned = new Dictionary<TVertex, TEdge>();
            foreach (TEdge edge in tree)
            {
                spanned[edge.Source] = spanned[edge.Target] = default;
            }

            // Find vertices that are connected to some edge
            var treeable = new Dictionary<TVertex, TEdge>();
            foreach (TEdge edge in graph.Edges)
                treeable[edge.Source] = treeable[edge.Target] = edge;

            // Ensure they are in the tree
            foreach (TVertex vertex in treeable.Keys)
                Assert.IsTrue(spanned.ContainsKey(vertex), $"{vertex} not in tree.");
        }

        private static void AssertMinimumSpanningTree<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] IMinimumSpanningTreeAlgorithm<TVertex, TEdge> algorithm)
            where TEdge : IEdge<TVertex>
        {
            var edgeRecorder = new EdgeRecorderObserver<TVertex, TEdge>();
            using (edgeRecorder.Attach(algorithm))
                algorithm.Compute();

            AssertSpanningTree(graph, edgeRecorder.Edges);
        }

        protected static void PrimSpanningTree<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph, [NotNull] Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = edgeWeights(edge);

            var prim = graph.CreatePrimMinimumSpanningTreeAlgorithm(e => distances[e]);
            AssertMinimumSpanningTree(graph, prim);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void Prim<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.AdjacentDegree(edge.Source) + 1 ?? Double.PositiveInfinity;

            IEnumerable<TEdge> edges = graph.MinimumSpanningTreePrim(e => distances[e]);
            AssertSpanningTree(graph, edges);
        }

        protected static void KruskalSpanningTree<TVertex, TEdge>(IUndirectedGraph<TVertex, TEdge> graph, Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = edgeWeights(edge);

            var kruskal = new KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>(graph, e => distances[e]);
            AssertMinimumSpanningTree(graph, kruskal);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void Kruskal<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>();
            foreach (TEdge edge in graph.Edges)
                distances[edge] = graph.AdjacentDegree(edge.Source) + 1 ?? Double.PositiveInfinity;

            IEnumerable<TEdge> edges = graph.MinimumSpanningTreeKruskal(e => distances[e]);
            AssertSpanningTree(graph, edges);
        }

        #endregion

        [Test]
        public void SimpleComparePrimKruskal()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVerticesAndEdge(Edge.Create(3, 2));
            graph.AddVerticesAndEdge(Edge.Create(3, 4));
            graph.AddVerticesAndEdge(Edge.Create(1, 4));

            double cost = CompareRoot(graph);
            Assert.AreEqual(9, cost);
        }

        [Test]
        public void DelegateComparePrimKruskal()
        {
            int[] vertices = { 1, 2, 3, 4 };
            var graph = vertices.ToDelegateUndirectedGraph(
                vertex => vertex switch
                {
                    1 => new[] { new EquatableEdge<int>(1, 2), new EquatableEdge<int>(1, 4) },
                    2 => new[] { new EquatableEdge<int>(1, 2), new EquatableEdge<int>(3, 1) },
                    3 => new[] { new EquatableEdge<int>(3, 2), new EquatableEdge<int>(3, 4) },
                    4 => new[] { new EquatableEdge<int>(1, 4), new EquatableEdge<int>(3, 4) },
                    _ => (IEnumerable<EquatableEdge<int>>)null
                });

            double cost = CompareRoot(graph);
            Assert.AreEqual(9, cost);
        }

        [Test]
        public void TestGraph()
        {
            UndirectedGraph<string, TaggedEdge<string, double>> undirectedGraph = XmlReader
                .Create(GetGraphFilePath("testGraph.xml"))
                .DeserializeFromXml(
                    "graph",
                    "node",
                    "edge",
                    "",
                    _ => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                    reader => reader.GetAttribute("id"),
                    reader => new TaggedEdge<string, double>(
                        reader.GetAttribute("source") ?? throw new AssertionException("Must have source attribute"),
                        reader.GetAttribute("target") ?? throw new AssertionException("Must have target attribute"),
                        int.Parse(reader.GetAttribute("weight") ?? throw new AssertionException("Must have weight attribute"))));

            TaggedEdge<string, double>[] prim = undirectedGraph.MinimumSpanningTreePrim(e => e.Tag).ToArray();
            double primCost = prim.Sum(e => e.Tag);

            TaggedEdge<string, double>[] kruskal = undirectedGraph.MinimumSpanningTreeKruskal(e => e.Tag).ToArray();
            double kruskalCost = kruskal.Sum(e => e.Tag);

            Assert.AreEqual(63, primCost);
            Assert.AreEqual(primCost, kruskalCost);
        }
    }
}