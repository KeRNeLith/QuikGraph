using System;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// A mutable incidence graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IMutableIncidenceGraph<TVertex, TEdge> : IMutableGraph<TVertex, TEdge>, IIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Removes all out-edges of the <paramref name="vertex"/>
        /// where the <paramref name="predicate"/> is evaluated to true.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">Predicate to remove edges.</param>
        /// <returns>The number of removed edges.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        int RemoveOutEdgeIf([NotNull] TVertex vertex, [NotNull, InstantHandle] Func<TEdge, bool> predicate);

        /// <summary>
        /// Trims the out-edges of the given <paramref name="vertex"/>
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void ClearOutEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Trims excess storage allocated for edges.
        /// </summary>
        void TrimEdgeExcess();
    }
}