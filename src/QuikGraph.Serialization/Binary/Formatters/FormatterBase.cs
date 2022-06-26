#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// Base class for a MessagePack formatter for <typeparamref name="TData"/>.
    /// </summary>
    /// <typeparam name="TData">Data type.</typeparam>
    [CLSCompliant(false)]
    public abstract class FormatterBase<TData> : IMessagePackFormatter<TData>
    {
        /// <summary>
        /// Number of data composing the <typeparamref name="TData"/> type.
        /// </summary>
        protected abstract int DataCount { get; }

        /// <inheritdoc />
        public void Serialize(ref MessagePackWriter writer, TData value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(DataCount);
            OnSerialize(ref writer, value, options);
        }

        /// <summary>
        /// Serializes the needed data associated to <paramref name="value"/>.
        /// </summary>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="value">The <typeparamref name="TData"/> object to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
        protected abstract void OnSerialize(
            ref MessagePackWriter writer,
            [NotNull] TData value,
            [NotNull] MessagePackSerializerOptions options);

        /// <inheritdoc />
        public TData Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return default(TData);

            options.Security.DepthStep(ref reader);

            // We should loop over all array elements independently of how many we expect,
            // since if we're serializing an older/newer version of an object it might
            // vary in number of elements that were serialized, but the contract of the formatter
            // is that exactly one data structure must be read, regardless.
            int dataCount = reader.ReadArrayHeader();
            int readCount = 0;
            TData deserializedObject = OnDeserialize(ref reader, dataCount, ref readCount, options);
            SkipData(ref reader, dataCount - readCount);

            --reader.Depth;
            return deserializedObject;
        }

        /// <summary>
        /// Deserializes data associated to an object of type <typeparamref name="TData"/>.</summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="dataCount">Number of data stored in <paramref name="reader"/>.</param>
        /// <param name="readCount">Number of data read from <paramref name="reader"/>.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
        /// <returns>The deserialized value.</returns>
        [CanBeNull]
        protected abstract TData OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            [NotNull] MessagePackSerializerOptions options);
    }
}
#endif