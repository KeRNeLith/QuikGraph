#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
#if SUPPORTS_SERIALIZATION
using System;
using System.Runtime.Serialization;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Serialization.Collections
{
    /// <summary>
    /// Implementation of a <see cref="T:System.Collections.Generic.Dictionary{TKey,TValue}"/> made for serialization.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TVertex,TEdge}"/> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="capacity">Dictionary capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

#if SUPPORTS_SERIALIZATION
        private SerializableDictionary([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
#endif