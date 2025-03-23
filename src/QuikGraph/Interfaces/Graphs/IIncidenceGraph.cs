using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> A directed graph with <see cref="IImplicitGraph{TVertex,TEdge}.OutEdges"/> and Methods to efficiently query them.
    /// and vertices of type <typeparamref name="TVertex"/>
    /// edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IIncidenceGraph<TVertex, TEdge> : IImplicitGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Checks if this graph contains an edge that link
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>True if an edge exists, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        bool ContainsEdge([NotNull] TVertex source, [NotNull] TVertex target);

        /// <summary>
        /// Tries to get the edge that link
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="edge">Edge found, otherwise <see langword="null"/>.</param>
        /// <returns>True if an edge was found, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        [ContractAnnotation("=> true, edge:notnull;=> false, edge:null")]
        //[Obsolete("Not CoVariant => With [CanBeNull] Support use " + nameof(OutEdges))]
        bool TryGetEdge([NotNull] TVertex source, [NotNull] TVertex target, out TEdge edge);

        /// <summary> Get all edges from <paramref name="source"/> to <paramref name="target"/> vertices. </summary>
        [ItemNotNull]
        IEnumerable<TEdge> GetEdges([NotNull] TVertex source, [NotNull] TVertex target);
    }
}