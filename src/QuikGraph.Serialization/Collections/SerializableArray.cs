#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Serialization.Collections
{
    /// <summary>
    /// Implementation of an array made for serialization.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public sealed class SerializableArray<T> : IEnumerable<T>
    {
        /// <summary>
        /// Array.
        /// </summary>
        [NotNull, ItemCanBeNull]
        public T[] Array { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableArray{T}"/> class.
        /// </summary>
        /// <param name="array">Array to serialize.</param>
        public SerializableArray([NotNull, ItemCanBeNull] T[] array)
        {
            Array = array;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T elem in Array)
                yield return elem;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif