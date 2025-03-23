#if SUPPORTS_SERIALIZATION || SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
#endif
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Stores associations of vertices to their edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class VertexEdgeDictionary<TVertex, TEdge>
        : Dictionary<TVertex, IEdgeList<TEdge>>
        , IVertexEdgeDictionary<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="VertexEdgeDictionary{TVertex,TEdge}"/> class.
        /// </summary>
        public VertexEdgeDictionary()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="VertexEdgeDictionary{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="capacity">Dictionary capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public VertexEdgeDictionary(int capacity)
            : base(capacity)
        {
        }

#if SUPPORTS_SERIALIZATION
        private VertexEdgeDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        #region ICloneable

        /// <summary>
        /// Clones this vertex/edges dictionary.
        /// </summary>
        /// <returns>Cloned dictionary.</returns>
        [NotNull]
        public VertexEdgeDictionary<TVertex, TEdge> Clone()
        {
            var clone = new VertexEdgeDictionary<TVertex, TEdge>(Count);
            foreach (KeyValuePair<TVertex, IEdgeList<TEdge>> pair in this)
                clone.Add(pair.Key, pair.Value.Clone());
            return clone;
        }

        /// <inheritdoc />
        IVertexEdgeDictionary<TVertex, TEdge> IVertexEdgeDictionary<TVertex, TEdge>.Clone()
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

        #endregion
    }
}