#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="ReversedBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class ReversedBidirectionalGraphFormatter<TVertex, TEdge> : FormatterBase<ReversedBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 1;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            ReversedBidirectionalGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            SerializeData(ref writer, value.OriginalGraph, options);
        }

        /// <inheritdoc />
        protected override ReversedBidirectionalGraph<TVertex, TEdge> OnDeserialize(
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
                        originalGraph = DeserializeData<IBidirectionalGraph<TVertex, TEdge>>(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new ReversedBidirectionalGraph<TVertex, TEdge>(
                originalGraph ?? throw new InvalidOperationException("Deserialization process failed to get original graph."));
        }
    }
}
#endif