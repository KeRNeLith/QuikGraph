using System;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Represents an implicit (i.e. dynamically/ad hoc-tested) set of vertices.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public interface IImplicitVertexSet<TVertex>
    {
        /// <summary> Determines whether this set contains the specified <paramref name="vertex"/>. </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the specified <paramref name="vertex"/> is contained in this set, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        bool ContainsVertex([NotNull] TVertex vertex);

        /// <summary> Optional Function to check for Equality, falls back to <see cref="System.Collections.Generic.EqualityComparer{T}.Default"/> </summary>
        Func<TVertex, TVertex, bool> AreVerticesEqual { get; }
    }
}