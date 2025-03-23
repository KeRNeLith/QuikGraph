using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> A directed graph that efficiently traverses both <see cref="InEdges"/> and <see cref="IImplicitGraph{TVertex,TEdge}.OutEdges"/>. </summary>
    /// <remarks>
    /// Adds Methods to efficiently query both <see cref="InEdges"/> and <see cref="IImplicitGraph{TVertex,TEdge}.OutEdges"/>.
    /// </remarks>
    public interface IBidirectionalIncidenceGraph<TVertex, TEdge> : IIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> AKA FanIn; Gets the number of in-edges pointing towards <paramref name="vertex"/>. </summary>
        /// <returns><see langword="null"/> when the <paramref name="vertex"/> is unknown.</returns>
        [Pure]
        int? InDegree([NotNull] TVertex vertex);

        /// <summary> Returns the collection of in-edges of <paramref name="target"/>. </summary>
        /// <returns> <see langword="null"/> when <paramref name="target"/> is not part of this Graph.</returns>
        [Pure]
        [ItemNotNull] [CanBeNull]
        IEnumerable<TEdge> InEdges([NotNull] TVertex target);

        /// <summary> Gets the in-edge at location <paramref name="index"/>. </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="index">The index.</param>
        /// <returns>The in-edge at position <paramref name="index"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">No vertex at <paramref name="index"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [CanBeNull]
        TEdge InEdge([NotNull] TVertex vertex, int index);

        /// <summary>
        /// Gets the degree of <paramref name="vertex"/>, i.e.
        /// the sum of the out-degree and in-degree of <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>The sum of OutDegree and InDegree of <paramref name="vertex"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        //[Obsolete("Make this an Extension Method")]
        int? Degree([NotNull] TVertex vertex);
    }
}