#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="EdgeListGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class EdgeListGraphFormatter<TVertex, TEdge> : FormatterBase<EdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 3;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EdgeListGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.IsDirected);
            writer.Write(value.AllowParallelEdges);

            IMessagePackFormatter<EdgeEdgeDictionary<TVertex, TEdge>> formatter =
                options.Resolver.GetFormatterWithVerify<EdgeEdgeDictionary<TVertex, TEdge>>();
            formatter.Serialize(ref writer, value.VertexEdges, options);
        }

        /// <inheritdoc />
        protected override EdgeListGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            bool isDirected = false;
            bool allowParallelEdges = false;
            EdgeEdgeDictionary<TVertex, TEdge> vertexEdges = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        isDirected = reader.ReadBoolean();
                        break;
                    case 1:
                        allowParallelEdges = reader.ReadBoolean();
                        break;
                    case 2:
                        IMessagePackFormatter<EdgeEdgeDictionary<TVertex, TEdge>> formatter =
                            options.Resolver.GetFormatterWithVerify<EdgeEdgeDictionary<TVertex, TEdge>>();
                        vertexEdges = formatter.Deserialize(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new EdgeListGraph<TVertex, TEdge>(
                isDirected,
                allowParallelEdges,
                vertexEdges ?? throw new InvalidOperationException("Deserialization process failed to get vertices and edges."));
        }
    }
}
#endif