using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Extension Methods for <see cref="IMutableEdgeListGraph{TVertex, TEdge}"/> </summary>
    public static class MutableEdgeListGraph
    {
        /// <summary> Adds the <paramref name="edges"/> to the <paramref name="graph"/>. </summary>
        public static int AddEdgeRange<TVertex, TEdge>(this IMutableEdgeListGraph<TVertex, TEdge> graph
            , [NotNull, ItemNotNull] params TEdge[] edges) where TEdge : IEdge<TVertex>
            => graph.AddEdgeRange(edges.AsEnumerable());

    }

    /// <summary>
    /// A mutable edge list graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IMutableEdgeListGraph<TVertex, TEdge> : IMutableGraph<TVertex, TEdge>, IEdgeListGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Adds the <paramref name="edge"/> to this graph. </summary>
        /// <returns>True if the edge was added, false when the edge already exists and no multiple Edges are allowed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Also adds the Vertices if missing.
        /// It does not improve Quality much to detect unknown Vertices and Edges.
        /// </remarks>
        bool AddEdge([NotNull] TEdge edge);

        /// <summary>
        /// Fired when an edge is added to this graph.
        /// </summary>
        event EdgeAction<TVertex, TEdge> EdgeAdded;

        /// <summary> Adds the <paramref name="edges"/> to this graph. </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges successfully added to this graph.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        int AddEdgeRange([NotNull, ItemNotNull] IEnumerable<TEdge> edges);

        /// <summary>
        /// Removes the <paramref name="edge"/> from this graph.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        /// <returns>True if the <paramref name="edge"/> was successfully removed, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        bool RemoveEdge([NotNull] TEdge edge);

        /// <summary>
        /// Fired when an edge has been removed from this graph.
        /// </summary>
        event EdgeAction<TVertex, TEdge> EdgeRemoved;

        /// <summary>
        /// Removes all edges that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Predicate to check if an edge should be removed.</param>
        /// <returns>The number of edges removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        int RemoveEdgeIf([NotNull, InstantHandle] Func<TEdge, bool> predicate);
    }
}