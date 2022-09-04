#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;
using QuikGraph.Serialization.Formatters;
using static QuikGraph.Serialization.Formatters.FormatterHelpers;
using static QuikGraph.Serialization.Tests.TestFormattersHelpers;

namespace QuikGraph.Serialization.Tests
{
    internal static class TestFormattersHelpers
    {
        public static void SerializeEquatableTestVertex(
            ref MessagePackWriter writer,
            [NotNull] EquatableTestVertex vertex,
            [NotNull] MessagePackSerializerOptions options)
        {
            NullableStringFormatter.Instance.Serialize(ref writer, vertex.ID, options);
            NullableStringFormatter.Instance.Serialize(ref writer, vertex.StringDefault, options);
            NullableStringFormatter.Instance.Serialize(ref writer, vertex.String, options);
            writer.Write(vertex.Bool);
            writer.WriteInt32(vertex.Int);
            writer.WriteInt64(vertex.Long);
            SingleFormatter.Instance.Serialize(ref writer, vertex.Float, options);
            DoubleFormatter.Instance.Serialize(ref writer, vertex.Double, options);
        }

        public static void DeserializeEquatableTestVertex(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            out string id,
            out string strDefault,
            out string str,
            out bool b,
            out int i,
            out long l,
            out float f,
            out double d)
        {
            id = NullableStringFormatter.Instance.Deserialize(ref reader, options);
            strDefault = NullableStringFormatter.Instance.Deserialize(ref reader, options);
            str = NullableStringFormatter.Instance.Deserialize(ref reader, options);
            b = reader.ReadBoolean();
            i = reader.ReadInt32();
            l = reader.ReadInt64();
            f = SingleFormatter.Instance.Deserialize(ref reader, options);
            d = DoubleFormatter.Instance.Deserialize(ref reader, options);
        }

        public static void SerializeEquatableTestEdge(
            ref MessagePackWriter writer,
            [NotNull] EquatableTestEdge edge,
            [NotNull] MessagePackSerializerOptions options)
        {
            SerializeEdge(ref writer, edge, options);
            NullableStringFormatter.Instance.Serialize(ref writer, edge.ID, options);
            NullableStringFormatter.Instance.Serialize(ref writer, edge.String, options);
            writer.Write(edge.Bool);
            writer.WriteInt32(edge.Int);
            writer.WriteInt64(edge.Long);
            SingleFormatter.Instance.Serialize(ref writer, edge.Float, options);
            DoubleFormatter.Instance.Serialize(ref writer, edge.Double, options);
        }

        public static void DeserializeEquatableTestEdge(
            ref MessagePackReader reader,
            [NotNull] MessagePackSerializerOptions options,
            out EquatableTestVertex source,
            out EquatableTestVertex target,
            out string id,
            out string str,
            out bool b,
            out int i,
            out long l,
            out float f,
            out double d)
        {
            DeserializeEdge(ref reader, options, out source, out target);
            id = NullableStringFormatter.Instance.Deserialize(ref reader, options);
            str = NullableStringFormatter.Instance.Deserialize(ref reader, options);
            b = reader.ReadBoolean();
            i = reader.ReadInt32();
            l = reader.ReadInt64();
            f = SingleFormatter.Instance.Deserialize(ref reader, options);
            d = DoubleFormatter.Instance.Deserialize(ref reader, options);
        }
    }

    internal sealed class EquatableTestVertexFormatter : FormatterBase<EquatableTestVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 8;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EquatableTestVertex value,
            MessagePackSerializerOptions options)
        {
            SerializeEquatableTestVertex(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override EquatableTestVertex OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeEquatableTestVertex(ref reader, options,
                out string id,
                out string strDefault,
                out string str,
                out bool b,
                out int i,
                out long l,
                out float f,
                out double d);

            readCount += 8;
            return new EquatableTestVertex(id)
            {
                StringDefault = strDefault,
                String = str,
                Bool = b,
                Int = i,
                Long = l,
                Float = f,
                Double = d
            };
        }
    }

    internal sealed class EquatableAdditionalDataTestVertexFormatter : FormatterBase<EquatableAdditionalDataTestVertex>
    {
        /// <inheritdoc />
        protected override int DataCount => 9;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EquatableAdditionalDataTestVertex value,
            MessagePackSerializerOptions options)
        {
            SerializeEquatableTestVertex(ref writer, value, options);
            DoubleFormatter.Instance.Serialize(ref writer, value.Data, options);
        }

        /// <inheritdoc />
        protected override EquatableAdditionalDataTestVertex OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeEquatableTestVertex(ref reader, options,
                out string id,
                out string strDefault,
                out string str,
                out bool b,
                out int i,
                out long l,
                out float f,
                out double d);
            double data = DoubleFormatter.Instance.Deserialize(ref reader, options);

            readCount += 9;
            return new EquatableAdditionalDataTestVertex(id, data)
            {
                StringDefault = strDefault,
                String = str,
                Bool = b,
                Int = i,
                Long = l,
                Float = f,
                Double = d
            };
        }
    }

    internal sealed class EquatableTestEdgeFormatter : FormatterBase<EquatableTestEdge>
    {
        /// <inheritdoc />
        protected override int DataCount => 9;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EquatableTestEdge value,
            MessagePackSerializerOptions options)
        {
            SerializeEquatableTestEdge(ref writer, value, options);
        }

        /// <inheritdoc />
        protected override EquatableTestEdge OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeEquatableTestEdge(ref reader, options,
                out EquatableTestVertex source,
                out EquatableTestVertex target,
                out string id,
                out string str,
                out bool b,
                out int i,
                out long l,
                out float f,
                out double d);

            readCount += 9;
            return new EquatableTestEdge(source, target, id)
            {
                String = str,
                Bool = b,
                Int = i,
                Long = l,
                Float = f,
                Double = d
            };
        }
    }

    internal sealed class EquatableAdditionalDataTestEdgeFormatter : FormatterBase<EquatableAdditionalDataTestEdge>
    {
        /// <inheritdoc />
        protected override int DataCount => 10;

        /// <inheritdoc />
        protected override void OnSerialize(
            ref MessagePackWriter writer,
            EquatableAdditionalDataTestEdge value,
            MessagePackSerializerOptions options)
        {
            SerializeEquatableTestEdge(ref writer, value, options);
            DoubleFormatter.Instance.Serialize(ref writer, value.Data, options);
        }

        /// <inheritdoc />
        protected override EquatableAdditionalDataTestEdge OnDeserialize(
            ref MessagePackReader reader,
            int dataCount,
            ref int readCount,
            MessagePackSerializerOptions options)
        {
            DeserializeEquatableTestEdge(ref reader, options,
                out EquatableTestVertex source,
                out EquatableTestVertex target,
                out string id,
                out string str,
                out bool b,
                out int i,
                out long l,
                out float f,
                out double d);
            double data = DoubleFormatter.Instance.Deserialize(ref reader, options);

            readCount += 10;
            return new EquatableAdditionalDataTestEdge(source, target, id, data)
            {
                String = str,
                Bool = b,
                Int = i,
                Long = l,
                Float = f,
                Double = d
            };
        }
    }
}
#endif