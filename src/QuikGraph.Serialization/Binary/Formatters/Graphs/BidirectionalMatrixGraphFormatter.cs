#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="BidirectionalMatrixGraph{TEdge}"/>.
    /// </summary>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public class BidirectionalMatrixGraphFormatter<TEdge> : FormatterBase<BidirectionalMatrixGraph<TEdge>>
        where TEdge : class, IEdge<int>
    {
        internal struct EdgeStorage
        {
            public EdgeStorage(int x, int y, [NotNull] TEdge edge)
            {
                X = x;
                Y = y;
                Edge = edge;
            }

            public int X { get; }

            public int Y { get; }

            [NotNull]
            public TEdge Edge { get; }
        }

        internal sealed class EdgeStorageFormatter : IMessagePackFormatter<EdgeStorage>
        {
            /// <inheritdoc />
            public void Serialize(ref MessagePackWriter writer, EdgeStorage value, MessagePackSerializerOptions options)
            {
                writer.Write(value.X);
                writer.Write(value.Y);

                SerializeData(ref writer, value.Edge, options);
            }

            /// <inheritdoc />
            public EdgeStorage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                TEdge edge = DeserializeData<TEdge>(ref reader, options);

                return new EdgeStorage(x, y, edge);
            }
        }

        /// <inheritdoc />
        protected override int DataCount => 2;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            BidirectionalMatrixGraph<TEdge> value,
            MessagePackSerializerOptions options)
        {
            writer.Write(value.VertexCount);

            var edges = new List<EdgeStorage>(value.EdgeCount);
            for (int i = 0; i < value.VertexCount; ++i)
            {
                for (int j = 0; j < value.VertexCount; ++j)
                {
                    TEdge edge = value.EdgesMatrix[i, j];
                    if (edge != null)
                    {
                        edges.Add(new EdgeStorage(i, j, edge));
                    }
                }
            }

            IMessagePackFormatter<List<EdgeStorage>> formatter = options.Resolver.GetFormatterWithVerify<List<EdgeStorage>>();
            formatter.Serialize(ref writer, edges, options);
        }

        /// <inheritdoc />
        protected override BidirectionalMatrixGraph<TEdge> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            int vertexCount = 0;
            List<EdgeStorage> edges = null;

            for (int i = 0; i < dataCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        vertexCount = reader.ReadInt32();
                        break;
                    case 1:
                        IMessagePackFormatter<List<EdgeStorage>> formatter = options.Resolver.GetFormatterWithVerify<List<EdgeStorage>>();
                        edges = formatter.Deserialize(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                ++readCount;
            }

            if (vertexCount <= 0)
                throw new InvalidOperationException("Deserialization process failed to get vertex count.");
            if (edges is null)
                throw new InvalidOperationException("Deserialization process failed to get edges.");

            var computedEdges = new TEdge[vertexCount, vertexCount];
            foreach (EdgeStorage edgeStorage in edges)
            {
                computedEdges[edgeStorage.X, edgeStorage.Y] = edgeStorage.Edge;
            }
            return new BidirectionalMatrixGraph<TEdge>(vertexCount, edges.Count, computedEdges);
        }
    }
}
#endif