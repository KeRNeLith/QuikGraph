using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Graphs
{
    /// <summary> Test the degree of graphs. </summary>
    [TestFixture]
    internal sealed class DegreeTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public static void AssertDegreeSumEqualsTwiceEdgeCount<TVertex, TEdge>([NotNull] IBidirectionalGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            int totalDegree = 0;
            foreach (TVertex vertex in graph.Vertices)
                totalDegree += graph.Degree(vertex)!.Value;

            Assert.AreEqual(graph.EdgeCount * 2, totalDegree);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public static void AssertInDegreeSumEqualsEdgeCount<TVertex, TEdge>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            int totalInDegree = 0;
            foreach (TVertex vertex in graph.Vertices)
                totalInDegree += graph.InDegree(vertex)!.Value;

            Assert.AreEqual(graph.EdgeCount, totalInDegree);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public static void OutDegreeSumEqualsEdgeCount<TVertex, TEdge>([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            int totalOutDegree = 0;
            foreach (TVertex vertex in graph.Vertices)
                totalOutDegree += graph.OutDegree(vertex)!.Value;

            Assert.AreEqual(graph.EdgeCount, totalOutDegree);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void AssertAdjacentDegreeSumEqualsTwiceEdgeCount<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            int totalAdjacentDegree = 0;
            foreach (TVertex vertex in graph.Vertices)
                totalAdjacentDegree += graph.AdjacentDegree(vertex)!.Value;

            Assert.AreEqual(graph.EdgeCount * 2, totalAdjacentDegree);
        }

    }
}