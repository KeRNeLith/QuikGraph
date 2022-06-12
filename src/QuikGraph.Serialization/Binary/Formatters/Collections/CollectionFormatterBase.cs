#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <typeparamref name="TCollection"/>.
    /// </summary>
    /// <typeparam name="TElement">Collection element type.</typeparam>
    /// <typeparam name="TIntermediate">Intermediate collection type.</typeparam>
    /// <typeparam name="TCollection">Formatter target collection type.</typeparam>
    [CLSCompliant(false)]
    public abstract class CollectionFormatterBase<TElement, TIntermediate, TCollection> : IMessagePackFormatter<TCollection>
        where TCollection : IEnumerable<TElement>
    {
        /// <inheritdoc />
        public void Serialize(ref MessagePackWriter writer, TCollection value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(GetCount(value));

            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            foreach (TElement item in value)
            {
                writer.CancellationToken.ThrowIfCancellationRequested();

                formatter.Serialize(ref writer, item, options);
            }
        }

        /// <inheritdoc />
        public TCollection Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return default(TCollection);

            options.Security.DepthStep(ref reader);

            int count = reader.ReadArrayHeader();

            TIntermediate list = this.Create(count, options);
            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            for (int i = 0; i < count; ++i)
            {
                reader.CancellationToken.ThrowIfCancellationRequested();

                var element = (TElement)formatter.Deserialize(ref reader, options);

                Add(list, i, element, options);
            }

            --reader.Depth;
            return Complete(list);
        }

        /// <summary>
        /// Gets the element count of the <paramref name="collection"/>.
        /// </summary>
        protected abstract int GetCount([NotNull] TCollection collection);

        /// <summary>
        /// Creates an instance of intermediate collection.
        /// </summary>
        protected abstract TIntermediate Create(int count, [NotNull] MessagePackSerializerOptions options);

        /// <summary>
        /// Adds an element to the intermediate collection.
        /// </summary>
        protected abstract void Add([NotNull] TIntermediate collection, int index, [CanBeNull] TElement value, [NotNull] MessagePackSerializerOptions options);

        /// <summary>
        /// Create the final collection type.
        /// </summary>
        protected abstract TCollection Complete([NotNull] TIntermediate intermediateCollection);
    }

    /// <summary>
    /// MessagePack formatter for <typeparamref name="TCollection"/>.
    /// </summary>
    /// <typeparam name="TElement">Collection element type.</typeparam>
    /// <typeparam name="TCollection">Formatter target collection type.</typeparam>
    [CLSCompliant(false)]
    public abstract class CollectionFormatterBase<TElement, TCollection> : CollectionFormatterBase<TElement, TCollection, TCollection>
        where TCollection : IEnumerable<TElement>
    {
        /// <inheritdoc />
        protected sealed override TCollection Complete(TCollection intermediateCollection)
        {
            return intermediateCollection;
        }
    }
}
#endif