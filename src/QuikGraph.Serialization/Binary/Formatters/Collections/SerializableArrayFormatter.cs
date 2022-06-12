#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Serialization.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="SerializableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    [CLSCompliant(false)]
    public sealed class SerializableArrayFormatter<T> : CollectionFormatterBase<T, T[], SerializableArray<T>>
    {
        /// <inheritdoc />
        protected override int GetCount(SerializableArray<T> collection)
        {
            return collection.Array.Length;
        }

        /// <inheritdoc />
        protected override T[] Create(int count, MessagePackSerializerOptions options)
        {
            if (count == 0)
                return Array.Empty<T>();
            return new T[count];
        }

        /// <inheritdoc />
        protected override void Add(T[] collection, int index, T value, MessagePackSerializerOptions options)
        {
            collection[index] = value;
        }

        /// <inheritdoc />
        protected override SerializableArray<T> Complete(T[] intermediateCollection)
        {
            return new SerializableArray<T>(intermediateCollection);
        }
    }
}
#endif