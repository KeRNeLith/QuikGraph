#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using NUnit.Framework;
using QuikGraph.Collections;
using QuikGraph.Serialization.Collections;
using QuikGraph.Serialization.Formatters;

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class CollectionFormattersTests
    {
        #region Test classes

        public sealed class TestObject
        {
            public SerializableArray<EquatableTestVertex> Array { get; set; }
            public SerializableDictionary<EquatableTestVertex, EquatableTestEdge> Map { get; set; }
            public VertexList<EquatableTestVertex> VertexList { get; set; }
            public EdgeList<EquatableTestVertex, EquatableTestEdge> EdgeList { get; set; }
            public VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge> VertexEdgeMap { get; set; }
            public EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge> EdgeEdgeMap { get; set; }
        }

        #endregion

        #region Test formatters

        private sealed class TestObjectFormatter : FormatterBase<TestObject>
        {
            /// <inheritdoc />
            protected override int DataCount => 6;

            /// <inheritdoc />
            protected override void OnSerialize(
                ref MessagePackWriter writer,
                TestObject value,
                MessagePackSerializerOptions options)
            {
                IMessagePackFormatter<SerializableArray<EquatableTestVertex>> arrayFormatter =
                    options.Resolver.GetFormatter<SerializableArray<EquatableTestVertex>>();
                arrayFormatter.Serialize(ref writer, value.Array, options);

                IMessagePackFormatter<SerializableDictionary<EquatableTestVertex, EquatableTestEdge>> mapFormatter =
                    options.Resolver.GetFormatter<SerializableDictionary<EquatableTestVertex, EquatableTestEdge>>();
                mapFormatter.Serialize(ref writer, value.Map, options);

                IMessagePackFormatter<VertexList<EquatableTestVertex>> vertexListFormatter =
                    options.Resolver.GetFormatter<VertexList<EquatableTestVertex>>();
                vertexListFormatter.Serialize(ref writer, value.VertexList, options);

                IMessagePackFormatter<EdgeList<EquatableTestVertex, EquatableTestEdge>> edgeListFormatter =
                    options.Resolver.GetFormatter<EdgeList<EquatableTestVertex, EquatableTestEdge>>();
                edgeListFormatter.Serialize(ref writer, value.EdgeList, options);

                IMessagePackFormatter<VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge>> vertexEdgeMapFormatter =
                    options.Resolver.GetFormatter<VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge>>();
                vertexEdgeMapFormatter.Serialize(ref writer, value.VertexEdgeMap, options);

                IMessagePackFormatter<EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge>> edgeEdgeMapFormatter =
                    options.Resolver.GetFormatter<EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge>>();
                edgeEdgeMapFormatter.Serialize(ref writer, value.EdgeEdgeMap, options);
            }

            /// <inheritdoc />
            protected override TestObject OnDeserialize(
                ref MessagePackReader reader,
                int dataCount,
                ref int readCount,
                MessagePackSerializerOptions options)
            {
                IMessagePackFormatter<SerializableArray<EquatableTestVertex>> arrayFormatter =
                    options.Resolver.GetFormatter<SerializableArray<EquatableTestVertex>>();
                SerializableArray<EquatableTestVertex> array = arrayFormatter.Deserialize(ref reader, options);

                IMessagePackFormatter<SerializableDictionary<EquatableTestVertex, EquatableTestEdge>> mapFormatter =
                    options.Resolver.GetFormatter<SerializableDictionary<EquatableTestVertex, EquatableTestEdge>>();
                SerializableDictionary<EquatableTestVertex, EquatableTestEdge> map = mapFormatter.Deserialize(ref reader, options);

                IMessagePackFormatter<VertexList<EquatableTestVertex>> vertexListFormatter =
                    options.Resolver.GetFormatter<VertexList<EquatableTestVertex>>();
                VertexList<EquatableTestVertex> vertexList = vertexListFormatter.Deserialize(ref reader, options);

                IMessagePackFormatter<EdgeList<EquatableTestVertex, EquatableTestEdge>> edgeListFormatter =
                    options.Resolver.GetFormatter<EdgeList<EquatableTestVertex, EquatableTestEdge>>();
                EdgeList<EquatableTestVertex, EquatableTestEdge> edgeList = edgeListFormatter.Deserialize(ref reader, options);

                IMessagePackFormatter<VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge>> vertexEdgeMapFormatter =
                    options.Resolver.GetFormatter<VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge>>();
                VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge> vertexEdgeMap = vertexEdgeMapFormatter.Deserialize(ref reader, options);

                IMessagePackFormatter<EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge>> edgeEdgeMapFormatter =
                    options.Resolver.GetFormatter<EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge>>();
                EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge> edgeEdgeMap = edgeEdgeMapFormatter.Deserialize(ref reader, options);

                readCount += 6;

                return new TestObject
                {
                    Array = array,
                    Map = map,
                    VertexList = vertexList,
                    EdgeList = edgeList,
                    VertexEdgeMap = vertexEdgeMap,
                    EdgeEdgeMap = edgeEdgeMap
                };
            }
        }

        #endregion

        #region Helpers

        [NotNull, ItemNotNull]
        private static readonly IMessagePackFormatter[] TestFormatters =
        {
            new EquatableTestVertexFormatter(),
            new EquatableTestEdgeFormatter(),
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

        #endregion

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CollectionSerializationTestCases
        {
            get
            {
                yield return new TestCaseData(new TestObject());

                var vertex1 = new EquatableTestVertex("1");
                var vertex2 = new EquatableTestVertex("2");
                var vertex3 = new EquatableTestVertex("3");
                var edge1 = new EquatableTestEdge(vertex1, vertex2, "12");
                var edge2 = new EquatableTestEdge(vertex2, vertex3, "23");
                var edge3 = new EquatableTestEdge(vertex3, vertex1, "31");
                yield return new TestCaseData(new TestObject
                {
                    Array = new SerializableArray<EquatableTestVertex>(new[] { vertex1, vertex2 }),
                    Map = new SerializableDictionary<EquatableTestVertex, EquatableTestEdge>
                    {
                        [vertex1] = edge1,
                        [vertex2] = null,
                        [vertex3] = edge3
                    },
                    VertexList = new VertexList<EquatableTestVertex>
                    {
                        vertex1,
                        null,
                        vertex2
                    },
                    EdgeList = new EdgeList<EquatableTestVertex, EquatableTestEdge>
                    {
                        edge1,
                        null,
                        edge2
                    },
                    VertexEdgeMap = new VertexEdgeDictionary<EquatableTestVertex, EquatableTestEdge>
                    {
                        [vertex1] = new EdgeList<EquatableTestVertex, EquatableTestEdge> { edge1, null },
                        [vertex2] = null,
                        [vertex3] = new EdgeList<EquatableTestVertex, EquatableTestEdge> { edge3 }
                    },
                    EdgeEdgeMap = new EdgeEdgeDictionary<EquatableTestVertex, EquatableTestEdge>
                    {
                        [edge1] = edge3,
                        [edge2] = null,
                        [edge3] = edge1,
                    }
                });
            }
        }

        [TestCaseSource(nameof(CollectionSerializationTestCases))]
        public void CollectionSerialization([NotNull] TestObject testObject)
        {
            TestObject deserializedObject = SerializeDeserialize(testObject);
            CollectionAssert.AreEqual(testObject.Array, deserializedObject.Array);
            CollectionAssert.AreEqual(testObject.Map, deserializedObject.Map);
            CollectionAssert.AreEqual(testObject.VertexList, deserializedObject.VertexList);
            CollectionAssert.AreEqual(testObject.EdgeList, deserializedObject.EdgeList);
            CollectionAssert.AreEqual(testObject.VertexEdgeMap, deserializedObject.VertexEdgeMap);
            CollectionAssert.AreEqual(testObject.EdgeEdgeMap, deserializedObject.EdgeEdgeMap);
        }
    }
}
#endif