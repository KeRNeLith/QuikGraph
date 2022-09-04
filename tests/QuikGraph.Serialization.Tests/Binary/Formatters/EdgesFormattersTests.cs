#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System.IO;
using JetBrains.Annotations;
using MessagePack;
using NUnit.Framework;

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class EdgesFormattersTests
    {
        #region Helpers

        private static void AssetEdgesEqual<TVertex>(IEdge<TVertex> expected, IEdge<TVertex> actual)
        {
            Assert.AreEqual(expected.Source, actual.Source);
            Assert.AreEqual(expected.Target, actual.Target);
        }

        private static void AssetTaggedEdgesEqual<TVertex, TEdge, TTag>(TEdge expected, TEdge actual)
            where TEdge : IEdge<TVertex>, ITagged<TTag>
        {
            AssetEdgesEqual(expected, actual);
            Assert.AreEqual(expected.Tag, actual.Tag);
        }

        private static void AssetTermEdgesEqual<TVertex>(ITermEdge<TVertex> expected, ITermEdge<TVertex> actual)
        {
            AssetEdgesEqual(expected, actual);
            Assert.AreEqual(expected.SourceTerminal, actual.SourceTerminal);
            Assert.AreEqual(expected.TargetTerminal, actual.TargetTerminal);
        }

        [Pure]
        [NotNull]
        private static TEdge SerializeDeserialize<TVertex, TEdge>([NotNull] TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsNotNull(edge);

            // Serialize
            using (var stream = new MemoryStream())
            {
                MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(
                    BinaryConstants.DefaultResolver);

                MessagePackSerializer.Serialize(typeof(TEdge), stream, edge, options);

                stream.Position = 0;

                // Deserialize
                TEdge deserializedEdge = MessagePackSerializer.Deserialize<TEdge>(stream, options);
                Assert.IsNotNull(deserializedEdge);
                Assert.AreNotSame(edge, deserializedEdge);

                return deserializedEdge;
            }
        }

        #endregion

        [Test]
        public void EdgesSerialization()
        {
            var edge = new Edge<int>(1, 2);
            Edge<int> deserializedEdge = SerializeDeserialize<int, Edge<int>>(edge);
            AssetEdgesEqual(edge, deserializedEdge);

            var taggedEdge = new TaggedEdge<int, string>(1, 2, "tag");
            TaggedEdge<int, string> deserializedTaggedEdge =
                SerializeDeserialize<int, TaggedEdge<int, string>>(taggedEdge);
            AssetTaggedEdgesEqual<int, TaggedEdge<int, string>, string>(taggedEdge, deserializedTaggedEdge);

            taggedEdge = new TaggedEdge<int, string>(1, 2, null);
            deserializedTaggedEdge = SerializeDeserialize<int, TaggedEdge<int, string>>(taggedEdge);
            AssetTaggedEdgesEqual<int, TaggedEdge<int, string>, string>(taggedEdge, deserializedTaggedEdge);

            var termEdge = new TermEdge<int>(1, 2);
            TermEdge<int> deserializedTermEdge = SerializeDeserialize<int, TermEdge<int>>(termEdge);
            AssetTermEdgesEqual(termEdge, deserializedTermEdge);

            termEdge = new TermEdge<int>(1, 2, 21, 42);
            deserializedTermEdge = SerializeDeserialize<int, TermEdge<int>>(termEdge);
            AssetTermEdgesEqual(termEdge, deserializedTermEdge);


            var equatableEdge = new EquatableEdge<int>(1, 2);
            EquatableEdge<int> deserializedEquatableEdge = SerializeDeserialize<int, EquatableEdge<int>>(equatableEdge);
            AssetEdgesEqual(equatableEdge, deserializedEquatableEdge);

            var equatableTaggedEdge = new EquatableTaggedEdge<int, string>(1, 2, "tag");
            EquatableTaggedEdge<int, string> deserializedEquatableTaggedEdge =
                SerializeDeserialize<int, EquatableTaggedEdge<int, string>>(equatableTaggedEdge);
            AssetTaggedEdgesEqual<int, EquatableTaggedEdge<int, string>, string>(equatableTaggedEdge, deserializedEquatableTaggedEdge);

            equatableTaggedEdge = new EquatableTaggedEdge<int, string>(1, 2, null);
            deserializedEquatableTaggedEdge = SerializeDeserialize<int, EquatableTaggedEdge<int, string>>(equatableTaggedEdge);
            AssetTaggedEdgesEqual<int, EquatableTaggedEdge<int, string>, string>(equatableTaggedEdge, deserializedEquatableTaggedEdge);

            var equatableTermEdge = new EquatableTermEdge<int>(1, 2);
            TermEdge<int> deserializedEquatableTermEdge = SerializeDeserialize<int, EquatableTermEdge<int>>(equatableTermEdge);
            AssetTermEdgesEqual(equatableTermEdge, deserializedEquatableTermEdge);

            equatableTermEdge = new EquatableTermEdge<int>(1, 2, 21, 42);
            deserializedEquatableTermEdge = SerializeDeserialize<int, EquatableTermEdge<int>>(equatableTermEdge);
            AssetTermEdgesEqual(equatableTermEdge, deserializedEquatableTermEdge);
        }

        [Test]
        public void EdgesSerialization_Struct()
        {
            var edge = new SEdge<int>(1, 2);
            SEdge<int> deserializedEdge = SerializeDeserialize<int, SEdge<int>>(edge);
            AssetEdgesEqual(edge, deserializedEdge);

            var taggedEdge = new STaggedEdge<int, string>(1, 2, "tag");
            STaggedEdge<int, string> deserializedTaggedEdge =
                SerializeDeserialize<int, STaggedEdge<int, string>>(taggedEdge);
            AssetTaggedEdgesEqual<int, STaggedEdge<int, string>, string>(taggedEdge, deserializedTaggedEdge);

            taggedEdge = new STaggedEdge<int, string>(1, 2, null);
            deserializedTaggedEdge = SerializeDeserialize<int, STaggedEdge<int, string>>(taggedEdge);
            AssetTaggedEdgesEqual<int, STaggedEdge<int, string>, string>(taggedEdge, deserializedTaggedEdge);


            var equatableEdge = new SEquatableEdge<int>(1, 2);
            SEquatableEdge<int> deserializedEquatableEdge = SerializeDeserialize<int, SEquatableEdge<int>>(equatableEdge);
            AssetEdgesEqual(equatableEdge, deserializedEquatableEdge);

            var equatableTaggedEdge = new SEquatableTaggedEdge<int, string>(1, 2, "tag");
            SEquatableTaggedEdge<int, string> deserializedEquatableTaggedEdge =
                SerializeDeserialize<int, SEquatableTaggedEdge<int, string>>(equatableTaggedEdge);
            AssetTaggedEdgesEqual<int, SEquatableTaggedEdge<int, string>, string>(equatableTaggedEdge, deserializedEquatableTaggedEdge);

            equatableTaggedEdge = new SEquatableTaggedEdge<int, string>(1, 2, null);
            deserializedEquatableTaggedEdge = SerializeDeserialize<int, SEquatableTaggedEdge<int, string>>(equatableTaggedEdge);
            AssetTaggedEdgesEqual<int, SEquatableTaggedEdge<int, string>, string>(equatableTaggedEdge, deserializedEquatableTaggedEdge);


            var reversedEdge1 = new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 2));
            SReversedEdge<int, Edge<int>> deserializedReversedEdge1 = SerializeDeserialize<int, SReversedEdge<int, Edge<int>>>(reversedEdge1);
            AssetEdgesEqual(reversedEdge1, deserializedReversedEdge1);

            var reversedEdge2 = new SReversedEdge<int, EquatableTaggedEdge<int, double>>(new EquatableTaggedEdge<int, double>(1, 2, 45.5));
            SReversedEdge<int, EquatableTaggedEdge<int, double>> deserializedReversedEdge2 =
                SerializeDeserialize<int, SReversedEdge<int, EquatableTaggedEdge<int, double>>>(reversedEdge2);
            AssetEdgesEqual(reversedEdge2, deserializedReversedEdge2);
        }

        [Test]
        public void UndirectedEdgesSerialization()
        {
            var undirectedEdge = new UndirectedEdge<int>(1, 2);
            UndirectedEdge<int> deserializedUndirectedEdge = SerializeDeserialize<int, UndirectedEdge<int>>(undirectedEdge);
            AssetEdgesEqual(undirectedEdge, deserializedUndirectedEdge);

            var taggedUndirectedEdge = new TaggedUndirectedEdge<int, string>(1, 2, "tag");
            TaggedUndirectedEdge<int, string> deserializedTaggedUndirectedEdge =
                SerializeDeserialize<int, TaggedUndirectedEdge<int, string>>(taggedUndirectedEdge);
            AssetTaggedEdgesEqual<int, TaggedUndirectedEdge<int, string>, string>(taggedUndirectedEdge, deserializedTaggedUndirectedEdge);

            taggedUndirectedEdge = new TaggedUndirectedEdge<int, string>(1, 2, null);
            deserializedTaggedUndirectedEdge = SerializeDeserialize<int, TaggedUndirectedEdge<int, string>>(taggedUndirectedEdge);
            AssetTaggedEdgesEqual<int, TaggedUndirectedEdge<int, string>, string>(taggedUndirectedEdge, deserializedTaggedUndirectedEdge);


            var equatableUndirectedEdge = new EquatableUndirectedEdge<int>(1, 2);
            EquatableUndirectedEdge<int> deserializedEquatableUndirectedEdge =
                SerializeDeserialize<int, EquatableUndirectedEdge<int>>(equatableUndirectedEdge);
            AssetEdgesEqual(equatableUndirectedEdge, deserializedEquatableUndirectedEdge);
        }

        [Test]
        public void UndirectedEdgesSerialization_Struct()
        {
            var undirectedEdge = new SUndirectedEdge<int>(1, 2);
            SUndirectedEdge<int> deserializedUndirectedEdge = SerializeDeserialize<int, SUndirectedEdge<int>>(undirectedEdge);
            AssetEdgesEqual(undirectedEdge, deserializedUndirectedEdge);

            var taggedUndirectedEdge = new STaggedUndirectedEdge<int, string>(1, 2, "tag");
            STaggedUndirectedEdge<int, string> deserializedTaggedUndirectedEdge =
                SerializeDeserialize<int, STaggedUndirectedEdge<int, string>>(taggedUndirectedEdge);
            AssetTaggedEdgesEqual<int, STaggedUndirectedEdge<int, string>, string>(taggedUndirectedEdge, deserializedTaggedUndirectedEdge);

            taggedUndirectedEdge = new STaggedUndirectedEdge<int, string>(1, 2, null);
            deserializedTaggedUndirectedEdge = SerializeDeserialize<int, STaggedUndirectedEdge<int, string>>(taggedUndirectedEdge);
            AssetTaggedEdgesEqual<int, STaggedUndirectedEdge<int, string>, string>(taggedUndirectedEdge, deserializedTaggedUndirectedEdge);
        }
    }
}
#endif