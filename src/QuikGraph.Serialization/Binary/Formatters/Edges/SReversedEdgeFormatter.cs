#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="SReversedEdge{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Original edge type.</typeparam>
    [CLSCompliant(false)]
    public class SReversedEdgeFormatter<TVertex, TEdge> : FormatterBase<SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 1;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            SReversedEdge<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            SerializeData(ref writer, value.OriginalEdge, options);
        }

        /// <inheritdoc />
        protected override SReversedEdge<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            TEdge originalEdge = DeserializeData<TEdge>(ref reader, options);
            ++readCount;

            return new SReversedEdge<TVertex, TEdge>(originalEdge);
        }
    }
}
#endif