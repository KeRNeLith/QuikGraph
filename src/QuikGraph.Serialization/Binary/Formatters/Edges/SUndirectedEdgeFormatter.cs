#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="SUndirectedEdge{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public class SUndirectedEdgeFormatter<TVertex> : FormatterBase<SUndirectedEdge<TVertex>>
    {
        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            SUndirectedEdge<TVertex> value,
            MessagePackSerializerOptions options)
        {
            SerializeEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override SUndirectedEdge<TVertex> OnDeserialize(
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

            return new SUndirectedEdge<TVertex>(source, target);
        }
    }
}
#endif