#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="Edge{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class EdgeFormatter<TVertex> : FormatterBase<Edge<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            Edge<TVertex> value,
            MessagePackSerializerOptions options)
        {
            SerializeEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override Edge<TVertex> OnDeserialize(
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

            return new Edge<TVertex>(source, target);
        }
    }
}
#endif