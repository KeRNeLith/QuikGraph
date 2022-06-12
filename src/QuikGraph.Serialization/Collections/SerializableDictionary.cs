#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.Collections.Generic;

namespace QuikGraph.Serialization.Collections
{
    /// <summary>
    /// Implementation of a <see cref="Dictionary{TKey,TValue}"/> made for serialization.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
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
    }
}
#endif