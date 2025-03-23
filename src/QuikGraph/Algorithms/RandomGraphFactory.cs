using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Helpers related to graphs and randomness.
    /// </summary>
    public static class RandomGraphFactory
    {
        /// <summary>
        /// Gets a random vertex within the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>Chosen vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static TVertex GetVertex<TVertex>(
            [NotNull] IVertexSet<TVertex> graph,
            [NotNull] Random rng)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return GetVertex(graph.Vertices, graph.VertexCount, rng);
        }

        /// <summary>
        /// Gets a random vertex within the given set of <paramref name="vertices"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="vertices">Set of vertices.</param>
        /// <param name="count">Number of vertices in the set.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>Chosen vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertices"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is negative or equal 0.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="count"/> is higher than <paramref name="vertices"/> count.</exception>
        [Pure]
        [NotNull]
        public static TVertex GetVertex<TVertex>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            int count,
            [NotNull] Random rng)
        {
            if (vertices is null)
                throw new ArgumentNullException(nameof(vertices));
            if (rng is null)
                throw new ArgumentNullException(nameof(rng));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Must be positive.");

            int i = rng.Next(count);
            foreach (TVertex vertex in vertices)
            {
                if (i == 0)
                    return vertex;
                --i;
            }

            // Failed
            throw new InvalidOperationException("Could not find vertex.");
        }

        /// <summary>
        /// Gets a random edge within the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>Chosen vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static TEdge GetEdge<TVertex, TEdge>(
            [NotNull] IEdgeSet<TVertex, TEdge> graph,
            [NotNull] Random rng)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return GetEdge<TVertex, TEdge>(graph.Edges, graph.EdgeCount, rng);
        }

        /// <summary>
        /// Gets a random edge within the given set of <paramref name="edges"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Set of edges.</param>
        /// <param name="count">Number of edges in the set.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>Chosen vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is negative or equal 0.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="count"/> is higher than <paramref name="edges"/> count.</exception>
        [Pure]
        [NotNull]
        public static TEdge GetEdge<TVertex, TEdge>(
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges,
            int count,
            [NotNull] Random rng)
            where TEdge : IEdge<TVertex>
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            if (rng is null)
                throw new ArgumentNullException(nameof(rng));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Must be positive.");

            int i = rng.Next(count);
            foreach (TEdge edge in edges)
            {
                if (i == 0)
                    return edge;
                --i;
            }

            // Failed
            throw new InvalidOperationException("Could not find edge.");
        }

        private static void CreateInternal<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> graph,
            [NotNull, InstantHandle] VertexFactory<TVertex> vertexFactory,
            [NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] Random rng,
            int vertexCount,
            int edgeCount,
            bool selfEdges)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (vertexFactory is null)
                throw new ArgumentNullException(nameof(vertexFactory));
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));
            if (rng is null)
                throw new ArgumentNullException(nameof(rng));
            if (vertexCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(vertexCount), "Must request at least one vertex.");
            if (edgeCount < 0)
                throw new ArgumentOutOfRangeException(nameof(edgeCount), "Must not be negative.");

            var vertices = new TVertex[vertexCount];
            for (int i = 0; i < vertexCount; ++i)
            {
                TVertex vertex = vertexFactory();
                vertices[i] = vertex;
                graph.AddVertex(vertex);
            }

            int j = 0;
            while (j < edgeCount)
            {
                TVertex a = vertices[rng.Next(vertexCount)];
                TVertex b;
                do
                {
                    b = vertices[rng.Next(vertexCount)];
                }
                while (!selfEdges && graph.AreVerticesEqual(a, b));

                if (graph.AddEdge(edgeFactory(a, b)))
                {
                    ++j;
                }
            }
        }

        /// <summary>
        /// Fills the given <paramref name="graph"/> with <paramref name="vertexCount"/> vertices
        /// and <paramref name="edgeCount"/> edges created randomly between vertices (directed graph).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to fill.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="rng">Random number generator.</param>
        /// <param name="vertexCount">Number of vertices to create.</param>
        /// <param name="edgeCount">Number of edges to create.</param>
        /// <param name="selfEdges">Indicates if self edge are allowed.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="vertexCount"/> is negative or equal 0.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="edgeCount"/> is negative.</exception>
        public static void Create<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] VertexFactory<TVertex> vertexFactory,
            [NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] Random rng,
            int vertexCount,
            int edgeCount,
            bool selfEdges)
            where TEdge : IEdge<TVertex>
        {
            CreateInternal(graph, vertexFactory, edgeFactory, rng, vertexCount, edgeCount, selfEdges);
        }

        /// <summary>
        /// Fills the given <paramref name="graph"/> with <paramref name="vertexCount"/> vertices
        /// and <paramref name="edgeCount"/> edges created randomly between vertices (undirected graph).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to fill.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="rng">Random number generator.</param>
        /// <param name="vertexCount">Number of vertices to create.</param>
        /// <param name="edgeCount">Number of edges to create.</param>
        /// <param name="selfEdges">Indicates if self edge are allowed.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rng"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="vertexCount"/> is negative or equal 0.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="edgeCount"/> is negative.</exception>
        public static void Create<TVertex, TEdge>(
            [NotNull] IMutableUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] VertexFactory<TVertex> vertexFactory,
            [NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] Random rng,
            int vertexCount,
            int edgeCount,
            bool selfEdges)
            where TEdge : IEdge<TVertex>
        {
            CreateInternal(graph, vertexFactory, edgeFactory, rng, vertexCount, edgeCount, selfEdges);
        }
    }
}