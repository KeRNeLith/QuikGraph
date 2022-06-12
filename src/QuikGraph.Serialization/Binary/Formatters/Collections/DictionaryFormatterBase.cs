#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <typeparamref name="TDictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key type.</typeparam>
    /// <typeparam name="TValue">Dictionary value type.</typeparam>
    /// <typeparam name="TIntermediate">Intermediate dictionary type.</typeparam>
    /// <typeparam name="TDictionary">Formatter target dictionary type.</typeparam>
    [CLSCompliant(false)]
    public abstract class DictionaryFormatterBase<TKey, TValue, TIntermediate, TDictionary> : IMessagePackFormatter<TDictionary>
        where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <inheritdoc />
        public void Serialize(ref MessagePackWriter writer, TDictionary value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteMapHeader(GetCount(value));

            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            foreach (KeyValuePair<TKey, TValue> pair in value)
            {
                writer.CancellationToken.ThrowIfCancellationRequested();

                formatter.Serialize(ref writer, pair.Key, options);
                formatter.Serialize(ref writer, pair.Value, options);
            }
        }

        /// <inheritdoc />
        public TDictionary Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return default(TDictionary);

            options.Security.DepthStep(ref reader);

            int count = reader.ReadMapHeader();

            TIntermediate dictionary = Create(count, options);
            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            for (int i = 0; i < count; ++i)
            {
                reader.CancellationToken.ThrowIfCancellationRequested();

                var key = (TKey)formatter.Deserialize(ref reader, options);
                var value = (TValue)formatter.Deserialize(ref reader, options);

                Add(dictionary, i, key, value, options);
            }

            --reader.Depth;
            return Complete(dictionary);
        }

        /// <summary>
        /// Gets the element count of the <paramref name="dictionary"/>.
        /// </summary>
        protected abstract int GetCount([NotNull] TDictionary dictionary);

        /// <summary>
        /// Creates an instance of intermediate collection.
        /// </summary>
        protected abstract TIntermediate Create(int count, [NotNull] MessagePackSerializerOptions options);

        /// <summary>
        /// Adds an element to the intermediate collection.
        /// </summary>
        protected abstract void Add(
            [NotNull] TIntermediate dictionary,
            int index,
            [NotNull] TKey key,
            [CanBeNull] TValue value,
            [NotNull] MessagePackSerializerOptions options);

        /// <summary>
        /// Create the final collection type.
        /// </summary>
        protected abstract TDictionary Complete([NotNull] TIntermediate intermediateDictionary);
    }

    /// <summary>
    /// MessagePack formatter for <typeparamref name="TDictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key type.</typeparam>
    /// <typeparam name="TValue">Dictionary value type.</typeparam>
    /// <typeparam name="TDictionary">Formatter target dictionary type.</typeparam>
    [CLSCompliant(false)]
    public abstract class DictionaryFormatterBase<TKey, TValue, TDictionary> : DictionaryFormatterBase<TKey, TValue, TDictionary, TDictionary>
        where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <inheritdoc />
        protected override TDictionary Complete(TDictionary intermediateDictionary)
        {
            return intermediateDictionary;
        }
    }
}
#endif