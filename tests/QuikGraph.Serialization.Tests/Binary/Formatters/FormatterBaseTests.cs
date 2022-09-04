#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.IO;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using NUnit.Framework;
using QuikGraph.Serialization.Formatters;

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class FormatterBaseTests
    {
        #region Test classes

        public sealed class TestObject
        {
            public int Property1 { get; set; }
            public int Property2 { get; set; }
            public EquatableTestVertex Property3 { get; set; } = new("0");
            public int Property4 { get; set; }
        }

        #endregion

        #region Test formatters

        private sealed class TestObjectFormatter : FormatterBase<TestObject>
        {
            /// <inheritdoc />
            protected override int DataCount => 4;

            /// <inheritdoc />
            protected override void OnSerialize(
                ref MessagePackWriter writer,
                TestObject value,
                MessagePackSerializerOptions options)
            {
                writer.Write(value.Property1);
                writer.Write(value.Property2);

                IMessagePackFormatter<EquatableTestVertex> formatter = options.Resolver.GetFormatterWithVerify<EquatableTestVertex>();
                formatter.Serialize(ref writer, value.Property3, options);

                writer.Write(value.Property4);
            }

            /// <inheritdoc />
            protected override TestObject OnDeserialize(
                ref MessagePackReader reader,
                int dataCount,
                ref int readCount,
                MessagePackSerializerOptions options)
            {
                int property2 = 0;
                EquatableTestVertex property3 = null;
                for (int i = 0; i < 3; ++i)
                {
                    switch (i)
                    {
                        // Property 1 ignored
                        case 1:
                            property2 = reader.ReadInt32();
                            break;
                        case 2:
                            IMessagePackFormatter<EquatableTestVertex> formatter =
                                options.Resolver.GetFormatterWithVerify<EquatableTestVertex>();
                            property3 = formatter.Deserialize(ref reader, options);
                            break;
                        // Property 5 unknown

                        default:
                            reader.Skip();
                            break;
                    }

                    ++readCount;
                }

                return new TestObject
                {
                    Property2 = property2,
                    Property3 = property3
                };
            }
        }

        #endregion

        #region Helpers

        [NotNull, ItemNotNull]
        private static readonly IMessagePackFormatter[] TestFormatters =
        {
            new EquatableTestVertexFormatter(),
            new TestObjectFormatter()
        };

        [Pure]
        [NotNull]
        private static TestObject SerializeDeserialize([NotNull] TestObject @object)
        {
            Assert.IsNotNull(@object);

            // Serialize
            using (var stream = new MemoryStream())
            {
                IFormatterResolver resolver = CompositeResolver.Create(
                    TestFormatters,
                    new[] { BinaryConstants.DefaultResolver });
                MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

                MessagePackSerializer.Serialize(typeof(TestObject), stream, @object, options);

                stream.Position = 0;

                // Deserialize
                TestObject deserializedObject = MessagePackSerializer.Deserialize<TestObject>(stream, options);
                Assert.IsNotNull(deserializedObject);
                Assert.AreNotSame(@object, deserializedObject);

                return deserializedObject;
            }
        }

        private static void SerializeAndCheckEqual([NotNull] TestObject testObject)
        {
            TestObject deserializedObject = SerializeDeserialize(testObject);
            Assert.AreEqual(testObject.Property2, deserializedObject.Property2);
            Assert.AreEqual(testObject.Property3, deserializedObject.Property3);
        }

        #endregion

        [Test]
        public void SerializationUnreadData()
        {
            var testObject = new TestObject { Property2 = 42 };
            SerializeAndCheckEqual(testObject);
        }

        [Test]
        public void SerializationNullData()
        {
            var testObject = new TestObject { Property3 = null };
            SerializeAndCheckEqual(testObject);
        }
    }
}
#endif