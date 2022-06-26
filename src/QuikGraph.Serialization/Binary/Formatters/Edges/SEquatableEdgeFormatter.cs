#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="SEquatableEdge{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class SEquatableEdgeFormatter<TVertex> : FormatterBase<SEquatableEdge<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            SEquatableEdge<TVertex> value,
            MessagePackSerializerOptions options)
        {
            SerializeEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override SEquatableEdge<TVertex> OnDeserialize(
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

            return new SEquatableEdge<TVertex>(source, target);
        }
    }
}
#endif