#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Serialization.Collections;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <remarks>
    /// <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}"/> is a possibly re-entrant class through
    /// <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}.Parent"/> property. This cause support
    /// of binary serialization a bit limited meaning that only the serialization starting from
    /// a <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}"/> with a <see langword="null"/>
    /// <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}.Parent"/> is supported, otherwise it
    /// may produce stack overflow. Because of this the <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}.Parent"/>
    /// property is not serialized and the link is artificially restored at deserialization.
    /// </remarks>
    [CLSCompliant(false)]
    public class ClusteredAdjacencyGraphFormatter<TVertex, TEdge> : FormatterBase<ClusteredAdjacencyGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 3;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            ClusteredAdjacencyGraph<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.Collapsed);
            SerializeData(ref writer, value.WrappedGraph, options);

            IMessagePackFormatter<SerializableArray<IClusteredGraph>> formatter =
                options.Resolver.GetFormatterWithVerify<SerializableArray<IClusteredGraph>>();
            formatter.Serialize(
                ref writer,
                new SerializableArray<IClusteredGraph>(value.Clusters.Cast<IClusteredGraph>().ToArray()),
                options);
        }

        /// <inheritdoc />
        protected override ClusteredAdjacencyGraph<TVertex, TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            bool collapsed = false;
            AdjacencyGraph<TVertex, TEdge> wrappedGraph = null;
            IClusteredGraph[] clusters = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        collapsed = reader.ReadBoolean();
                        break;
                    case 1:
                        wrappedGraph = DeserializeData<AdjacencyGraph<TVertex, TEdge>>(ref reader, options);
                        break;
                    case 2:
                        IMessagePackFormatter<SerializableArray<IClusteredGraph>> formatter =
                            options.Resolver.GetFormatterWithVerify<SerializableArray<IClusteredGraph>>();
                        clusters = formatter.Deserialize(ref reader, options).Array;
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            return new ClusteredAdjacencyGraph<TVertex, TEdge>(
                wrappedGraph ?? throw new InvalidOperationException("Deserialization process failed to get wrapped graph."),
                clusters ?? throw new InvalidOperationException("Deserialization process failed to get clusters."))
            {
                Collapsed = collapsed
            };
        }
    }
}
#endif