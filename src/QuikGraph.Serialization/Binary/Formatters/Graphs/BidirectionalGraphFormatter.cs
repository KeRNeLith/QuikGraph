#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class BidirectionalGraphFormatter<TVertex, TEdge> : FormatterBase<BidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 4;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            BidirectionalGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.AllowParallelEdges);
            writer.Write(value.EdgeCount);
            writer.Write(value.EdgeCapacity);

            IMessagePackFormatter<VertexEdgeDictionary<TVertex, TEdge>> formatter =
                options.Resolver.GetFormatterWithVerify<VertexEdgeDictionary<TVertex, TEdge>>();
            formatter.Serialize(ref writer, value.VertexOutEdges, options);
        }

        /// <inheritdoc />
        protected override BidirectionalGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            bool allowParallelEdges = false;
            int edgeCount = 0;
            int edgeCapacity = 0;
            VertexEdgeDictionary<TVertex, TEdge> vertexOutEdges = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        allowParallelEdges = reader.ReadBoolean();
                        break;
                    case 1:
                        edgeCount = reader.ReadInt32();
                        break;
                    case 2:
                        edgeCapacity = reader.ReadInt32();
                        break;
                    case 3:
                        IMessagePackFormatter<VertexEdgeDictionary<TVertex, TEdge>> formatter =
                            options.Resolver.GetFormatterWithVerify<VertexEdgeDictionary<TVertex, TEdge>>();
                        vertexOutEdges = formatter.Deserialize(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            if (edgeCount < 0)
                throw new InvalidOperationException("Deserialization process failed to get vertices and edges.");
            return new BidirectionalGraph<TVertex, TEdge>(
                allowParallelEdges,
                edgeCount,
                edgeCapacity,
                vertexOutEdges ?? throw new InvalidOperationException("Deserialization process failed to get vertices and edges."));
        }
    }
}
#endif