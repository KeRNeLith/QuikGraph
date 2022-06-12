#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Serialization.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="SerializableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    [CLSCompliant(false)]
    public sealed class SerializableDictionaryFormatter<TKey, TValue>
        : DictionaryFormatterBase<
            TKey,
            TValue,
            SerializableDictionary<TKey, TValue>>
    {
        /// <inheritdoc />
        protected override int GetCount(SerializableDictionary<TKey, TValue> dictionary)
        {
            return dictionary.Count;
        }

        /// <inheritdoc />
        protected override SerializableDictionary<TKey, TValue> Create(int count, MessagePackSerializerOptions options)
        {
            return new SerializableDictionary<TKey, TValue>(count);
        }

        /// <inheritdoc />
        protected override void Add(
            SerializableDictionary<TKey, TValue> dictionary,
            int index,
            TKey key,
            TValue value,
            MessagePackSerializerOptions options)
        {
            dictionary.Add(key, value);
        }
    }
}
#endif