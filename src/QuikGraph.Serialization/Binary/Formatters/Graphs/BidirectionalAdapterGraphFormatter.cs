#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="BidirectionalAdapterGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class BidirectionalAdapterGraphFormatter<TVertex, TEdge> : FormatterBase<BidirectionalAdapterGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 1;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            BidirectionalAdapterGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            SerializeData(ref writer, value.BaseGraph, options);
        }

        /// <inheritdoc />
        protected override BidirectionalAdapterGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        DeserializeNullableData(ref reader, options, out baseGraph);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new BidirectionalAdapterGraph<TVertex, TEdge>(
                baseGraph ?? throw new InvalidOperationException("Deserialization process failed to get base graph."));
        }
    }
}
#endif