#if SUPPORTS_SERIALIZATION || SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary> Stores a list of edges and implements <see cref="IReadOnlyList{T}"/>. </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class EdgeList<TEdge> : List<TEdge>, IEdgeList<TEdge>
    {
        /// <summary> Initializes a new <see cref="EdgeList{TEdge}"/> class. </summary>
        public EdgeList()
        {
        }

        /// <summary> Initializes a new <see cref="EdgeList{TEdge}"/> class. </summary>
        /// <param name="capacity">List capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public EdgeList(int capacity)
            : base(capacity)
        {
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public EdgeList([NotNull] EdgeList<TEdge> other)
            : base(other)
        {
        }

        /// <inheritdoc />
        public EdgeList([CanBeNull] IEnumerable<TEdge> items)
            : base(items ?? Enumerable.Empty<TEdge>())
        {
        }

        /// <summary> Clones this edge list. </summary>
        [NotNull]
        public EdgeList<TEdge> Clone() => new EdgeList<TEdge>(this);

        /// <inheritdoc />
        IEdgeList<TEdge> IEdgeList<TEdge>.Clone() => Clone();

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone() => Clone();
#endif
    }
}