#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="UndirectedEdge{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class UndirectedEdgeFormatter<TVertex> : FormatterBase<UndirectedEdge<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            UndirectedEdge<TVertex> value,
            MessagePackSerializerOptions options)
        {
            SerializeEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override UndirectedEdge<TVertex> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeEdge(
                ref reader,
                options,
                out TVertex source,
                out TVertex target);
            readCount += 2;

            return new UndirectedEdge<TVertex>(source, target);
        }
    }
}
#endif