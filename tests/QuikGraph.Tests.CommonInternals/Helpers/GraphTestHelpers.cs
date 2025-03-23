using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests
{
    /// <summary>
    /// Test helpers for graphs.
    /// </summary>
    internal static class GraphTestHelpers
    {
        public static void AssertVertexCountEqual<TVertex>(
            [NotNull] this IVertexSet<TVertex> left,
            [NotNull] IVertexSet<TVertex> right)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.AreEqual(left.VertexCount, right.VertexCount);
        }

        public static void AssertEdgeCountEqual<TVertex, TEdge>(
            [NotNull] this IEdgeSet<TVertex, TEdge> left,
            [NotNull] IEdgeSet<TVertex, TEdge> right)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.AreEqual(left.EdgeCount, right.EdgeCount);
        }

        public static bool InVertexSet<TVertex>(
            [NotNull] IVertexSet<TVertex> graph,
            [NotNull] TVertex vertex)
        {
            return graph.ContainsVertex(vertex);
        }

        public static bool InVertexSet<TVertex, TEdge>(
            [NotNull] IEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            return InVertexSet(graph, edge.Source)
                   && InVertexSet(graph, edge.Target);
        }

        public static bool InEdgeSet<TVertex, TEdge>(
            IEdgeListGraph<TVertex, TEdge> graph,
            TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            return InVertexSet(graph, edge) && graph.ContainsEdge(edge);
        }

        [Pure]
        public static bool IsDescendant<TValue>(
            [NotNull] Dictionary<TValue, TValue> parents,
            [NotNull] TValue u,
            [NotNull] TValue v)
        {
            TValue t;
            TValue current = u;
            do
            {
                t = current;
                current = parents[t];
                if (current.Equals(v))
                    return true;
            }
            while (!t.Equals(current));

            return false;
        }

        #region Vertices helpers

        public static void AssertNoVertex<TVertex>([NotNull] IVertexSet<TVertex> graph)
        {
            Assert.IsTrue(graph.IsVerticesEmpty);
            Assert.AreEqual(0, graph.VertexCount);
            CollectionAssert.IsEmpty(graph.Vertices);
        }

        public static void AssertHasVertices<TVertex>(
            [NotNull] this IVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] params TVertex[] vertices)
        {
            graph.AssertHasVertices(vertices.AsEnumerable());
        }

        public static void AssertHasVertices<TVertex>(
            [NotNull] this IVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            TVertex[] vertexArray = vertices.ToArray();
            CollectionAssert.IsNotEmpty(vertexArray);

            Assert.IsFalse(graph.IsVerticesEmpty);
            Assert.AreEqual(vertexArray.Length, graph.VertexCount);
            CollectionAssert.AreEquivalent(vertexArray, graph.Vertices);
        }

        public static void AssertNoVertices<TVertex>(
            [NotNull] this IImplicitVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            AssertImplicitHasVertices(graph, vertices, false);
        }

        public static void AssertHasVertices<TVertex>(
            [NotNull] this IImplicitVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            AssertImplicitHasVertices(graph, vertices, true);
        }

        private static void AssertImplicitHasVertices<TVertex>(
            [NotNull] this IImplicitVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            bool expectedContains)
        {
            TVertex[] vertexArray = vertices.ToArray();
            CollectionAssert.IsNotEmpty(vertexArray);

            foreach (TVertex vertex in vertexArray)
            {
                Assert.AreEqual(expectedContains, graph.ContainsVertex(vertex));
            }
        }

        #endregion

        #region Edges helpers

        public static void AssertNoEdge<TVertex, TEdge>([NotNull] this IEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.IsEdgesEmpty);
            Assert.AreEqual(0, graph.EdgeCount);
            CollectionAssert.IsEmpty(graph.Edges);
        }

        public static void AssertHasEdges<TVertex, TEdge>(
            [NotNull] this IEdgeSet<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            graph.AssertHasEdges(edges.AsEnumerable());
        }

        public static void AssertHasEdges<TVertex, TEdge>(
            [NotNull] this IEdgeSet<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsEdgesEmpty);
            Assert.AreEqual(edgeArray.Length, graph.EdgeCount);
            CollectionAssert.AreEquivalent(edgeArray, graph.Edges);
        }

        public static void AssertHasEdges<TVertex, TEdge>(
            [NotNull] this IEdgeSet<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            graph.AssertHasEdges(edges.AsEnumerable());
        }

        public static void AssertHasEdges<TVertex, TEdge>(
            [NotNull] this IEdgeSet<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            AssertHasEdges(
                graph,
                edges.Select(edge => new SReversedEdge<TVertex, TEdge>(edge)));
        }

        public static void AssertSameReversedEdge(
            [NotNull] IEdge<int> edge,
            SReversedEdge<int, IEdge<int>> reversedEdge)
        {
            Assert.AreEqual(new SReversedEdge<int, IEdge<int>>(edge), reversedEdge);
            Assert.AreSame(edge, reversedEdge.OriginalEdge);
        }

        public static void AssertSameReversedEdges(
            [NotNull, ItemNotNull] IEnumerable<IEdge<int>> edges,
            [NotNull] IEnumerable<SReversedEdge<int, IEdge<int>>> reversedEdges)
        {
            var edgesArray = edges.ToArray();
            var reversedEdgesArray = reversedEdges.ToArray();
            Assert.AreEqual(edgesArray.Length, reversedEdgesArray.Length);
            for (int i = 0; i < edgesArray.Length; ++i)
                AssertSameReversedEdge(edgesArray[i], reversedEdgesArray[i]);
        }

        #endregion

        #region Graph helpers

        public static void AssertEmptyGraph<TVertex, TEdge>(
            [NotNull] IEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            AssertNoVertex(graph);
            graph.AssertNoEdge();
        }

        public static void AssertEmptyGraph<TVertex>(
            [NotNull] CompressedSparseRowGraph<TVertex> graph)
        {
            AssertNoVertex(graph);
            graph.AssertNoEdge();
        }

        public static void AssertNoInEdge<TVertex, TEdge>([NotNull] IBidirectionalIncidenceGraph<TVertex, TEdge> graph, [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.IsInEdgesEmpty(vertex));
            Assert.AreEqual(0, graph.InDegree(vertex));
            CollectionAssert.IsEmpty(graph.InEdges(vertex));
        }

        public static void AssertHasInEdges<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            AssertHasInEdges(graph, vertex, edges.AsEnumerable());
        }

        public static void AssertHasInEdges<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsInEdgesEmpty(vertex));
            Assert.AreEqual(edgeArray.Length, graph.InDegree(vertex));
            CollectionAssert.AreEquivalent(edgeArray, graph.InEdges(vertex));
        }

        public static void AssertHasReversedInEdges<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            AssertHasReversedInEdges(graph, vertex, edges.AsEnumerable());
        }

        public static void AssertHasReversedInEdges<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsInEdgesEmpty(vertex));
            Assert.AreEqual(edgeArray.Length, graph.InDegree(vertex));
            CollectionAssert.AreEquivalent(
                edgeArray.Select(edge => new SReversedEdge<TVertex, TEdge>(edge)),
                graph.InEdges(vertex));
        }

        public static void AssertNoOutEdge<TVertex, TEdge>([NotNull] IImplicitGraph<TVertex, TEdge> graph, [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.IsOutEdgesEmpty(vertex));
            Assert.AreEqual(0, graph.OutDegree(vertex));
            CollectionAssert.IsEmpty(graph.OutEdges(vertex));
        }

        public static void AssertHasOutEdges<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            AssertHasOutEdges(graph, vertex, edges.AsEnumerable());
        }

        public static void AssertHasOutEdges<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsOutEdgesEmpty(vertex));
            Assert.AreEqual(edgeArray.Length, graph.OutDegree(vertex));
            CollectionAssert.AreEquivalent(edgeArray, graph.OutEdges(vertex));
        }

        public static void AssertHasReversedOutEdges<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params TEdge[] edges)
            where TEdge : IEdge<TVertex>
        {
            AssertHasReversedOutEdges(graph, vertex, edges.AsEnumerable());
        }

        public static void AssertHasReversedOutEdges<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, SReversedEdge<TVertex, TEdge>> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsOutEdgesEmpty(vertex));
            Assert.AreEqual(edgeArray.Length, graph.OutDegree(vertex));
            CollectionAssert.AreEquivalent(
                edgeArray.Select(edge => new SReversedEdge<TVertex, TEdge>(edge)),
                graph.OutEdges(vertex));
        }



        public static void AssertNoAdjacentEdge<TVertex, TEdge>([NotNull] IImplicitUndirectedGraph<TVertex, TEdge> graph, [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.IsAdjacentEdgesEmpty(vertex));
            Assert.AreEqual(0, graph.AdjacentDegree(vertex));
            CollectionAssert.IsEmpty(graph.AdjacentEdges(vertex));
        }

        public static void AssertHasAdjacentEdges<TVertex, TEdge>(
            [NotNull] this IImplicitUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params TEdge[] edges)    // If not set => equals the count of edges
            where TEdge : IEdge<TVertex>
        {
            graph.AssertHasAdjacentEdges(vertex, edges.AsEnumerable());
        }

        public static void AssertHasAdjacentEdges<TVertex, TEdge>(
            [NotNull] this IImplicitUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges,
            int degree = -1)    // If not set => equals the count of edges
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsAdjacentEdgesEmpty(vertex));
            Assert.AreEqual(
                degree < 0 ? edgeArray.Length : degree,
                graph.AdjacentDegree(vertex));
            CollectionAssert.AreEquivalent(edgeArray, graph.AdjacentEdges(vertex));
        }



        public static void AssertEquivalentGraphs<TVertex, TEdge>(
            [NotNull] IEdgeListGraph<TVertex, TEdge> expected,
            [NotNull] IEdgeListGraph<TVertex, TEdge> actual)
            where TEdge : IEdge<TVertex>
        {
            Assert.AreEqual(expected.IsDirected, actual.IsDirected);
            Assert.AreEqual(expected.AllowParallelEdges, actual.AllowParallelEdges);

            if (expected.IsVerticesEmpty)
            {
                AssertNoVertex(actual);
            }
            else
            {
                AssertHasVertices(actual, expected.Vertices);
            }

            if (expected.IsEdgesEmpty)
            {
                AssertNoEdge(actual);
            }
            else
            {
                AssertHasEdges(actual, expected.Edges);
            }
        }

        #endregion
    }
}