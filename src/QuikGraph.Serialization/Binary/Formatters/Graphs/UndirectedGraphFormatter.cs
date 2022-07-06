#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Serialization.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="UndirectedGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class UndirectedGraphFormatter<TVertex, TEdge> : FormatterBase<UndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 4;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            UndirectedGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.AllowParallelEdges);
            writer.Write(value.EdgeCapacity);

            IMessagePackFormatter<SerializableArray<TVertex>> verticesFormatter =
                options.Resolver.GetFormatterWithVerify<SerializableArray<TVertex>>();
            verticesFormatter.Serialize(ref writer, new SerializableArray<TVertex>(value.Vertices.ToArray()), options);

            IMessagePackFormatter<SerializableArray<TEdge>> edgesFormatter =
                options.Resolver.GetFormatterWithVerify<SerializableArray<TEdge>>();
            edgesFormatter.Serialize(ref writer, new SerializableArray<TEdge>(value.Edges.ToArray()), options);
        }

        /// <inheritdoc />
        protected override UndirectedGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            bool allowParallelEdges = false;
            int edgeCapacity = 0;
            IEnumerable<TVertex> vertices = null;
            IEnumerable<TEdge> edges = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        allowParallelEdges = reader.ReadBoolean();
                        break;
                    case 1:
                        edgeCapacity = reader.ReadInt32();
                        break;
                    case 2:
                        IMessagePackFormatter<SerializableArray<TVertex>> verticesFormatter =
                            options.Resolver.GetFormatterWithVerify<SerializableArray<TVertex>>();
                        vertices = verticesFormatter.Deserialize(ref reader, options);
                        break;
                    case 3:
                        IMessagePackFormatter<SerializableArray<TEdge>> edgesFormatter =
                            options.Resolver.GetFormatterWithVerify<SerializableArray<TEdge>>();
                        edges = edgesFormatter.Deserialize(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            --reader.Depth;
            return new UndirectedGraph<TVertex, TEdge>(
                allowParallelEdges,
                edgeCapacity,
                vertices ?? throw new InvalidOperationException("Deserialization process failed to get vertices."),
                edges ?? throw new InvalidOperationException("Deserialization process failed to get edges."));
        }
    }
}
#endif