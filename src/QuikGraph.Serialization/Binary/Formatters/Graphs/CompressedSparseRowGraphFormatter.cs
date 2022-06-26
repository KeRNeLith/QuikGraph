#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Serialization.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="CompressedSparseRowGraph{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class CompressedSparseRowGraphFormatter<TVertex> : FormatterBase<CompressedSparseRowGraph<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            CompressedSparseRowGraph<TVertex> value,
            MessagePackSerializerOptions options)
        {
            IMessagePackFormatter<SerializableArray<TVertex>> outEdgesTargetsFormatter =
                options.Resolver.GetFormatterWithVerify<SerializableArray<TVertex>>();
            outEdgesTargetsFormatter.Serialize(ref writer, new SerializableArray<TVertex>(value.OutEdgesTargets), options);

            var outEdgesStartRanges = new SerializableDictionary<TVertex, KeyValuePair<int, int>>(value.VertexCount);
            foreach (TVertex vertex in value.Vertices)
            {
                outEdgesStartRanges.Add(vertex, value.GetRangeFor(vertex));
            }
            IMessagePackFormatter<SerializableDictionary<TVertex, KeyValuePair<int, int>>> outEdgesStartRangesFormatter =
                options.Resolver.GetFormatterWithVerify<SerializableDictionary<TVertex, KeyValuePair<int, int>>>();
            outEdgesStartRangesFormatter.Serialize(ref writer, outEdgesStartRanges, options);
        }

        /// <inheritdoc />
        protected override CompressedSparseRowGraph<TVertex> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            TVertex[] outEdgesTargets = null;
            SerializableDictionary<TVertex, KeyValuePair<int, int>> outEdgesStartRanges = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        IMessagePackFormatter<SerializableArray<TVertex>> outEdgesTargetsFormatter =
                            options.Resolver.GetFormatterWithVerify<SerializableArray<TVertex>>();
                        outEdgesTargets = outEdgesTargetsFormatter.Deserialize(ref reader, options).Array;
                        break;
                    case 1:
                        IMessagePackFormatter<SerializableDictionary<TVertex, KeyValuePair<int, int>>> outEdgesStartRangesFormatter =
                            options.Resolver.GetFormatterWithVerify<SerializableDictionary<TVertex, KeyValuePair<int, int>>>();
                        outEdgesStartRanges = outEdgesStartRangesFormatter.Deserialize(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new CompressedSparseRowGraph<TVertex>(
                outEdgesTargets ?? throw new InvalidOperationException("Deserialization process failed to get edges targets."),
                outEdgesStartRanges ?? throw new InvalidOperationException("Deserialization process failed to get edges ranges."));
        }
    }
}
#endif