#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Diagnostics;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// Helpers for MessagePack formatters.
    /// </summary>
    [CLSCompliant(false)]
    public static class FormatterHelpers
    {
        /// <summary>
        /// Proceeds the serialization of a data of type <typeparamref name="TData"/> into given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="data">The data to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SerializeData<TData>(
            ref MessagePackWriter writer,
            [NotNull] TData data,
            [NotNull] MessagePackSerializerOptions options)
        {
            Debug.Assert(data != null);
            Debug.Assert(options != null);

            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            formatter.Serialize(ref writer, data, options);
        }

        /// <summary>
        /// Proceeds the serialization of a nullable data of type <typeparamref name="TData"/> into given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="data">The data to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SerializeNullableData<TData>(
            ref MessagePackWriter writer,
            [CanBeNull] TData data,
            [NotNull] MessagePackSerializerOptions options)
        {
            Debug.Assert(options != null);

            if (data == null)
            {
                writer.WriteNil();
            }
            else
            {
                IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
                formatter.Serialize(ref writer, data, options);
            }
        }

        /// <summary>
        /// Proceeds the serialization of an <see cref="IEdge{TVertex}"/> into given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="edge">The <see cref="IEdge{TVertex}"/> to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="IEdge{TVertex}"/>.
        /// </param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SerializeEdge<TVertex>(
            ref MessagePackWriter writer,
            [NotNull] IEdge<TVertex> edge,
            [NotNull] MessagePackSerializerOptions options)
        {
            Debug.Assert(edge != null);
            Debug.Assert(options != null);

            IMessagePackFormatter<object> vertexFormatter = TypelessFormatter.Instance;
            vertexFormatter.Serialize(ref writer, edge.Source, options);
            vertexFormatter.Serialize(ref writer, edge.Target, options);
        }

        /// <summary>
        /// Proceeds the serialization of a tagged <see cref="IEdge{TVertex}"/> into given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TTag">Tag type.</typeparam>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="edge">The <see cref="IEdge{TVertex}"/> to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="IEdge{TVertex}"/>.
        /// </param>
        public static void SerializeTaggedEdge<TVertex, TEdge, TTag>(
            ref MessagePackWriter writer,
            [NotNull] TEdge edge,
            [NotNull] MessagePackSerializerOptions options)
            where TEdge : IEdge<TVertex>, ITagged<TTag>
        {
            Debug.Assert(edge != null);
            Debug.Assert(options != null);

            SerializeEdge(ref writer, edge, options);
            SerializeNullableData(ref writer, edge.Tag, options);
        }

        /// <summary>
        /// Proceeds the serialization of an <see cref="ITermEdge{TVertex}"/> into given <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="writer">The writer to use when serializing the value.</param>
        /// <param name="edge">The <see cref="ITermEdge{TVertex}"/> to be serialized.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="ITermEdge{TVertex}"/>.
        /// </param>
        public static void SerializeTermEdge<TVertex>(
            ref MessagePackWriter writer,
            [NotNull] ITermEdge<TVertex> edge,
            [NotNull] MessagePackSerializerOptions options)
        {
            Debug.Assert(edge != null);
            Debug.Assert(options != null);

            SerializeEdge(ref writer, edge, options);
            writer.Write(edge.SourceTerminal);
            writer.Write(edge.TargetTerminal);
        }

        /// <summary>
        /// Proceeds the deserialization of a data of type <typeparamref name="TData"/> from given <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
        /// <param name="data">Deserialized data.</param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void DeserializeNullableData<TData>(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            [CanBeNull] out TData data)
        {
            Debug.Assert(options != null);

            if (reader.TryReadNil())
            {
                data = default(TData);
                return;
            }

            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            data = (TData)formatter.Deserialize(ref reader, options);
        }

        /// <summary>
        /// Proceeds the deserialization of a data of type <typeparamref name="TData"/> from given <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <typeparamref name="TData"/>.
        /// </param>
        /// <returns>Deserialized data.</returns>
        [NotNull]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TData DeserializeData<TData>(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options)
        {
            Debug.Assert(options != null);

            IMessagePackFormatter<object> formatter = TypelessFormatter.Instance;
            return (TData)formatter.Deserialize(ref reader, options);
        }

        /// <summary>
        /// Proceeds the deserialization of an <see cref="IEdge{TVertex}"/> data from given <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="IEdge{TVertex}"/>.
        /// </param>
        /// <param name="source">Deserialized source vertex.</param>
        /// <param name="target">Deserialized target vertex.</param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void DeserializeEdge<TVertex>(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            [NotNull] out TVertex source,
            [NotNull] out TVertex target)
        {
            Debug.Assert(options != null);

            IMessagePackFormatter<object> vertexFormatter = TypelessFormatter.Instance;
            source = (TVertex)vertexFormatter.Deserialize(ref reader, options);
            target = (TVertex)vertexFormatter.Deserialize(ref reader, options);
        }

        /// <summary>
        /// Proceeds the deserialization of a tagged <see cref="IEdge{TVertex}"/> data from given <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TTag">Tag type.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="IEdge{TVertex}"/>.
        /// </param>
        /// <param name="source">Deserialized source vertex.</param>
        /// <param name="target">Deserialized target vertex.</param>
        /// <param name="tag">Deserialized edge tag.</param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void DeserializeTaggedEdge<TVertex, TTag>(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            [NotNull] out TVertex source,
            [NotNull] out TVertex target,
            [CanBeNull] out TTag tag)
        {
            Debug.Assert(options != null);

            DeserializeEdge(ref reader, options, out source, out target);
            DeserializeNullableData(ref reader, options, out tag);
        }

        /// <summary>
        /// Proceeds the deserialization of an <see cref="ITermEdge{TVertex}"/> data from given <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">
        /// The serialization settings to use, including the resolver to use to obtain formatters
        /// for types that make up the <see cref="ITermEdge{TVertex}"/>.
        /// </param>
        /// <param name="source">Deserialized source vertex.</param>
        /// <param name="target">Deserialized target vertex.</param>
        /// <param name="sourceTerminal">Deserialized source terminal.</param>
        /// <param name="targetTerminal">Deserialized target terminal.</param>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void DeserializeTermEdge<TVertex>(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            [NotNull] out TVertex source,
            [NotNull] out TVertex target,
            out int sourceTerminal,
            out int targetTerminal)
        {
            Debug.Assert(options != null);

            DeserializeEdge(ref reader, options, out source, out target);
            sourceTerminal = reader.ReadInt32();
            targetTerminal = reader.ReadInt32();
        }

        /// <summary>
        /// Skips <paramref name="n"/> data from given <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="n">Number of data to skip.</param>
        public static void SkipData(ref MessagePackReader reader, int n)
        {
            for (int i = 0; i < n; ++i)
            {
                reader.Skip();
            }
        }
    }
}
#endif