#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="VertexList{TVertex}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [CLSCompliant(false)]
    public sealed class VertexListFormatter<TVertex> : CollectionFormatterBase<TVertex, VertexList<TVertex>>
    {
        /// <inheritdoc />
        protected override int GetCount(VertexList<TVertex> collection)
        {
            return collection.Count;
        }

        /// <inheritdoc />
        protected override VertexList<TVertex> Create(int count, MessagePackSerializerOptions options)
        {
            return new VertexList<TVertex>(count);
        }

        /// <inheritdoc />
        protected override void Add(VertexList<TVertex> collection, int index, TVertex value, MessagePackSerializerOptions options)
        {
            collection.Add(value);
        }
    }
}
#endif