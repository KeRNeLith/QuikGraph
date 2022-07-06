#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="ArrayAdjacencyGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class ArrayAdjacencyGraphFormatter<TVertex, TEdge> : FormatterBase<ArrayAdjacencyGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 3;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            ArrayAdjacencyGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.AllowParallelEdges);
            writer.Write(value.EdgeCount);

            IMessagePackFormatter<VertexEdgeDictionary<TVertex, TEdge>> formatter =
                options.Resolver.GetFormatterWithVerify<VertexEdgeDictionary<TVertex, TEdge>>();
            var vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>(value.VertexOutEdges.Count);
            foreach (KeyValuePair<TVertex, TEdge[]> pair in value.VertexOutEdges)
            {
                vertexOutEdges.Add(pair.Key, new EdgeList<TVertex, TEdge>(pair.Value));
            }
            formatter.Serialize(ref writer, vertexOutEdges, options);
        }

        /// <inheritdoc />
        protected override ArrayAdjacencyGraph<TVertex, TEdge> OnDeserialize(
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
            return new ArrayAdjacencyGraph<TVertex, TEdge>(
                allowParallelEdges,
                edgeCount,
                vertexOutEdges ?? throw new InvalidOperationException("Deserialization process failed to get out edges."));
        }
    }
}
#endif