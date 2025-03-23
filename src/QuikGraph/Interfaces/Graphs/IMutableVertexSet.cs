using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Extension Methods for <see cref="IMutableVertexSet{TVertex}"/> </summary>
    public static class MutableVertexSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int AddVertexRange<TVertex>(this IMutableVertexSet<TVertex> set
            , [NotNull, ItemNotNull] params TVertex[] vertices) => set.AddVertexRange(vertices.AsEnumerable());

    }

    /// <summary> Represents a mutable set of vertices.</summary>
    public interface IMutableVertexSet<TVertex> : IVertexSet<TVertex>
    {
        /// <summary>
        /// Fired when a vertex is added to this set.
        /// </summary>
        event VertexAction<TVertex> VertexAdded;

        /// <summary>
        /// Adds a vertex to this set.
        /// </summary>
        /// <param name="vertex">Vertex to add.</param>
        /// <returns>True if the vertex was added, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        bool AddVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Adds given vertices to this set.
        /// </summary>
        /// <param name="vertices">Vertices to add.</param>
        /// <returns>The number of vertex added.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="vertices"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        int AddVertexRange([NotNull, ItemNotNull] IEnumerable<TVertex> vertices);

        /// <summary>
        /// Fired when a vertex is removed from this set.
        /// </summary>
        event VertexAction<TVertex> VertexRemoved;

        /// <summary>
        /// Removes the given vertex from this set.
        /// </summary>
        /// <param name="vertex">Vertex to remove.</param>
        /// <returns>True if the vertex was removed, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        bool RemoveVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Removes all vertices matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Predicate to check on each vertex.</param>
        /// <returns>The number of vertex removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        int RemoveVertexIf([NotNull, InstantHandle] Func<TVertex, bool> predicate);
    }
}