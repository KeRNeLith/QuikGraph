#if SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Stores a list of edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class EdgeList<TVertex, TEdge> : List<TEdge>, IEdgeList<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeList{TVertex,TEdge}"/> class.
        /// </summary>
        public EdgeList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeList{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="capacity">List capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public EdgeList(int capacity)
            : base(capacity)
        {
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public EdgeList([NotNull, ItemNotNull] IEnumerable<TEdge> other)
            : base(other)
        {
        }

        /// <summary>
        /// Clones this edge list.
        /// </summary>
        /// <returns>Cloned list.</returns>
        [NotNull]
        public EdgeList<TVertex, TEdge> Clone()
        {
            return new EdgeList<TVertex, TEdge>(this);
        }

        /// <inheritdoc />
        IEdgeList<TVertex, TEdge> IEdgeList<TVertex, TEdge>.Clone()
        {
            return Clone();
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
    }
}