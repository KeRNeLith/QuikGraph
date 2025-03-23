using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> An implicit graph with <see cref="OutEdges"/> of type <typeparamref name="TEdge"/>
    /// and vertices of type <typeparamref name="TVertex"/>. </summary>
    /// <remarks>
    /// An implicit graph the vertices and edges are not explicitly stored or predefined,
    /// but are instead generated or computed on-the-fly.
    /// </remarks>
    public interface IImplicitGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
         where TEdge : IEdge<TVertex>
    {
        /// <summary> AKA FanOut; Gets the count of out-edges of <paramref name="vertex"/>. </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>The count of out-edges of <paramref name="vertex"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        int? OutDegree([NotNull] TVertex vertex);

        /// <summary> Gets the out-edges of <paramref name="vertex"/>. </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns> <see langword="null"/> when <paramref name="vertex"/> is unknown to the Graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [ItemNotNull] [CanBeNull]
        IEnumerable<TEdge> OutEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the out-edge of <paramref name="vertex"/> at position <paramref name="index"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="index">The index.</param>
        /// <returns>The out-edge at position <paramref name="index"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">No vertex at <paramref name="index"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [CanBeNull]
        TEdge OutEdge([NotNull] TVertex vertex, int index);
    }
}