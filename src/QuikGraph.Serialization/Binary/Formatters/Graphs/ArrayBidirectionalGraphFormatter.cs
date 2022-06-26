#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="ArrayBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class ArrayBidirectionalGraphFormatter<TVertex, TEdge> : FormatterBase<ArrayBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 3;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            ArrayBidirectionalGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.AllowParallelEdges);
            writer.Write(value.EdgeCount);

            var vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>(value.VertexCount);
            foreach (TVertex vertex in value.Vertices)
            {
                vertexOutEdges.Add(
                    vertex,
                    new EdgeList<TVertex, TEdge>(value.OutEdges(vertex)));
            }
            IMessagePackFormatter<VertexEdgeDictionary<TVertex, TEdge>> formatter =
                options.Resolver.GetFormatterWithVerify<VertexEdgeDictionary<TVertex, TEdge>>();
            formatter.Serialize(ref writer, vertexOutEdges, options);
        }

        /// <inheritdoc />
        protected override ArrayBidirectionalGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            bool allowParallelEdges = false;
            int edgeCount = 0;
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
                throw new InvalidOperationException("Deserialization process failed to get edge count.");
            return new ArrayBidirectionalGraph<TVertex, TEdge>(
                allowParallelEdges,
                edgeCount,
                vertexOutEdges ?? throw new InvalidOperationException("Deserialization process failed to get out edges."));
        }
    }
}
#endif