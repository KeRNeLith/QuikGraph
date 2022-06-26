#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="TermEdge{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class TermEdgeFormatter<TVertex> : FormatterBase<TermEdge<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 4;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            TermEdge<TVertex> value,
            MessagePackSerializerOptions options)
        {
            SerializeTermEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override TermEdge<TVertex> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeTermEdge(
                ref reader,
                options,
                out TVertex source,
                out TVertex target,
                out int sourceTerminal,
                out int targetTerminal);
            readCount += 4;

            return new TermEdge<TVertex>(source, target, sourceTerminal, targetTerminal);
        }
    }
}
#endif