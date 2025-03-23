using System;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// A mutable bidirectional directed graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IMutableBidirectionalGraph<TVertex, TEdge>
        : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        , IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Removes in-edges of the given <paramref name="vertex"/> that match
        /// predicate <paramref name="predicate"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">Edge predicate.</param>
        /// <returns>Number of edges removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        int RemoveInEdgeIf([NotNull] TVertex vertex, [NotNull, InstantHandle] Func<TEdge, bool> predicate);

        /// <summary>
        /// Clears in-edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void ClearInEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Clears in-edges and out-edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void ClearEdges([NotNull] TVertex vertex);
    }
}