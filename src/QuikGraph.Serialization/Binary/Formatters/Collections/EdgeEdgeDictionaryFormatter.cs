#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="EdgeEdgeDictionary{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public sealed class EdgeEdgeDictionaryFormatter<TVertex, TEdge>
        : DictionaryFormatterBase<
            TEdge,
            TEdge,
            EdgeEdgeDictionary<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int GetCount(EdgeEdgeDictionary<TVertex, TEdge> dictionary)
        {
            return dictionary.Count;
        }

        /// <inheritdoc />
        protected override EdgeEdgeDictionary<TVertex, TEdge> Create(int count, MessagePackSerializerOptions options)
        {
            return new EdgeEdgeDictionary<TVertex, TEdge>(count);
        }

        /// <inheritdoc />
        protected override void Add(
            EdgeEdgeDictionary<TVertex, TEdge> collection,
            int index,
            TEdge key,
            TEdge value,
            MessagePackSerializerOptions options)
        {
            collection.Add(key, value);
        }
    }
}
#endif