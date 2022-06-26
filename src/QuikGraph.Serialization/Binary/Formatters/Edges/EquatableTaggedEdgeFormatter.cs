#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using MessagePack;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;

namespace QuikGraph.Serialization.Formatters
{
    /// <summary>
    /// MessagePack formatter for <see cref="EquatableTaggedEdge{TVertex,TTag}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TTag">Tag type.</typeparam>
    [CLSCompliant(false)]
    public class EquatableTaggedEdgeFormatter<TVertex, TTag> : FormatterBase<EquatableTaggedEdge<TVertex, TTag>>
    {
        /// <inheritdoc />
        protected override int DataCount => 3;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EquatableTaggedEdge<TVertex, TTag> value,
            MessagePackSerializerOptions options)
        {
            SerializeTaggedEdge<TVertex, EquatableTaggedEdge<TVertex, TTag>, TTag>(
                ref writer,
                value,
                options);
        }

        /// <inheritdoc />
        protected override EquatableTaggedEdge<TVertex, TTag> OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeTaggedEdge(
                ref reader,
                options,
                out TVertex source,
                out TVertex target,
                out TTag tag);
            readCount += 3;

            return new EquatableTaggedEdge<TVertex, TTag>(source, target, tag);
        }
    }
}
#endif