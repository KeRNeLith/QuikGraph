#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="UndirectedBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class UndirectedBidirectionalGraphFormatter<TVertex, TEdge> : FormatterBase<UndirectedBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 1;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            UndirectedBidirectionalGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            FormatterHelpers.SerializeData(ref writer, value.OriginalGraph, options);
        }

        /// <inheritdoc />
        protected override UndirectedBidirectionalGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            IBidirectionalGraph<TVertex, TEdge> originalGraph = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        originalGraph = FormatterHelpers.DeserializeData<IBidirectionalGraph<TVertex, TEdge>>(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new UndirectedBidirectionalGraph<TVertex, TEdge>(
                originalGraph ?? throw new InvalidOperationException("Deserialization process failed to get original graph."));
        }
    }
}
#endif