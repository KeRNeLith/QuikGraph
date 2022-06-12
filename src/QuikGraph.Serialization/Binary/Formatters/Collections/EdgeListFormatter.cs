#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using QuikGraph.Collections;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="EdgeList{TVertex,TEdge}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public sealed class EdgeListFormatter<TVertex, TEdge> : CollectionFormatterBase<TEdge, EdgeList<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        protected override int GetCount(EdgeList<TVertex, TEdge> collection)
        {
            return collection.Count;
        }

        /// <inheritdoc />
        protected override EdgeList<TVertex, TEdge> Create(int count, MessagePackSerializerOptions options)
        {
            return new EdgeList<TVertex, TEdge>();
        }

        /// <inheritdoc />
        protected override void Add(EdgeList<TVertex, TEdge> collection, int index, TEdge value, MessagePackSerializerOptions options)
        {
            collection.Add(value);
        }
    }
}
#endif