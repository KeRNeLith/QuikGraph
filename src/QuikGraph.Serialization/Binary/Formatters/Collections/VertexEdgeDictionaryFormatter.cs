#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="IVertexEdgeDictionary{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public sealed class VertexEdgeDictionaryFormatter<TVertex, TEdge>
        : DictionaryFormatterBase<
            TVertex,
            IEdgeList<TVertex, TEdge>,
            VertexEdgeDictionary<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int GetCount(VertexEdgeDictionary<TVertex, TEdge> dictionary)
        {
            return dictionary.Count;
        }

        /// <inheritdoc />
        protected override VertexEdgeDictionary<TVertex, TEdge> Create(int count, MessagePackSerializerOptions options)
        {
            return new VertexEdgeDictionary<TVertex, TEdge>(count);
        }

        /// <inheritdoc />
        protected override void Add(
            VertexEdgeDictionary<TVertex, TEdge> dictionary,
            int index,
            TVertex key,
            IEdgeList<TVertex, TEdge> value,
            MessagePackSerializerOptions options)
        {
            dictionary.Add(key, value);
        }
    }
}
#endif