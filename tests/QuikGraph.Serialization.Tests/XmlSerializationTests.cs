using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.GraphTestHelpers;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;
using static QuikGraph.Serialization.Tests.SerializationTestCaseSources;
using static QuikGraph.Serialization.Tests.TestHelpers;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Tests relative to XML serialization.
    /// </summary>
    [TestFixture]
    internal sealed class XmlSerializationTests
    {
        #region Constants

        [NotNull]
        private static readonly string TestNamespace = string.Empty;


        [NotNull]
        private const string IdTag = "id";

        [NotNull]
        private const string GraphTag = "graph";

        [NotNull]
        private const string NodeTag = "node";

        [NotNull]
        private const string EdgeTag = "edge";

        [NotNull]
        private const string SourceTag = "source";

        [NotNull]
        private const string TargetTag = "target";


        #region Custom data constants

        [NotNull]
        private const string WeightTag = "weight";

        [NotNull]
        private const string TypeTag = "type";

        [NotNull]
        private const string StringDefaultTag = "strDefault";

        [NotNull]
        private const string StringTag = "str";

        [NotNull]
        private const string BoolTag = "bool";

        [NotNull]
        private const string IntTag = "i32";

        [NotNull]
        private const string LongTag = "i64";

        [NotNull]
        private const string FloatTag = "f32";

        [NotNull]
        private const string DoubleTag = "f64";

        [NotNull]
        private const string AdditionalDataTag = "additional_data";

        [NotNull]
        private const string IsTaggedTag = "has_tag";

        [NotNull]
        private const string DoubleValueTag = "double_tag";

        [NotNull]
        private const string StringValueTag = "string_tag";

        #endregion


        [NotNull]
        private const string FamilyGraphTag = "family";

        [NotNull]
        private const string PersonNodeTag = "person";

        [NotNull]
        private const string RelationEdgeTag = "relationship";


        [NotNull]
        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";


        [NotNull]
        private const string Indent = "    ";


        [NotNull]
        private const string NullString = "<null>";

        #endregion

        #region Serialization

        #region Test helpers

        private static void SerializeAndRead(
            [NotNull, InstantHandle] Action<XmlWriter> onSerialize,
            [NotNull, InstantHandle] Action<string> checkSerializedContent)
        {
            var settings = new XmlWriterSettings { Indent = true, IndentChars = Indent };
            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    onSerialize(xmlWriter);
                }

                memory.Position = 0;

                using (var reader = new StreamReader(memory))
                {
                    checkSerializedContent(reader.ReadToEnd());
                }
            }
        }

        #endregion

        [Test]
        public void SerializeToXml_Empty()
        {
            var emptyGraph = new BidirectionalGraph<Person, Edge<Person>>();

            SerializeAndRead(
                writer => emptyGraph.SerializeToXml(
                    writer,
                    person => person.Id,
                    emptyGraph.GetEdgeIdentity(),
                    FamilyGraphTag,
                    PersonNodeTag,
                    RelationEdgeTag,
                    TestNamespace),
                content =>
                {
                    StringAssert.AreEqualIgnoringCase(
                        $"{XmlHeader}{Environment.NewLine}<{FamilyGraphTag} />",
                        content);
                });
            }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> XmlSerializationGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new AdjacencyGraph<Person, TaggedEdge<Person, string>>());
                yield return new TestCaseData(new BidirectionalGraph<Person, TaggedEdge<Person, string>>());
                yield return new TestCaseData(new UndirectedGraph<Person, TaggedEdge<Person, string>>());
            }
        }

        [TestCaseSource(nameof(XmlSerializationGraphTestCases))]
        public void SerializeToXml<TGraph>([NotNull] TGraph graph)
            where TGraph: IMutableVertexAndEdgeSet<Person, TaggedEdge<Person, string>>
        {
            var jacob = new Person("Jacob", "Hochstetler")
            {
                BirthDate = new DateTime(1712, 01, 01),
                BirthPlace = "Alsace, France",
                DeathDate = new DateTime(1776, 01, 01),
                DeathPlace = "Pennsylvania, USA",
                Gender = Gender.Male
            };

            var john = new Person("John", "Hochstetler")
            {
                BirthDate = new DateTime(1735, 01, 01),
                BirthPlace = "Alsace, France",
                DeathDate = new DateTime(1805, 04, 15),
                DeathPlace = "Summit Mills, PA",
                Gender = Gender.Male
            };

            var jonathon = new Person("Jonathon", "Hochstetler")
            {
                BirthPlace = "Pennsylvania",
                DeathDate = new DateTime(1823, 05, 08),
                Gender = Gender.Male
            };

            var emanuel = new Person("Emanuel", "Hochstedler")
            {
                BirthDate = new DateTime(1855, 01, 01),
                DeathDate = new DateTime(1900, 01, 01),
                Gender = Gender.Male
            };

            graph.AddVerticesAndEdgeRange(
                new TaggedEdge<Person, string>[]
                {
                    new(jacob, john, jacob.ChildRelationshipText),
                    new(john, jonathon, john.ChildRelationshipText),
                    new(jonathon, emanuel, jonathon.ChildRelationshipText)
                });

            SerializeAndRead(
                writer => graph.SerializeToXml(
                    writer,
                    person => person.Id,
                    graph.GetEdgeIdentity(),
                    FamilyGraphTag,
                    PersonNodeTag,
                    RelationEdgeTag,
                    TestNamespace),
                content => CheckXmlGraphSerialization(graph, content));

            #region Local function

            static void CheckXmlGraphSerialization(
                [NotNull] IEdgeListGraph<Person, TaggedEdge<Person, string>> graph,
                [NotNull] string xmlGraph)
            {
                var expectedSerializedGraph = new StringBuilder($"{XmlHeader}{Environment.NewLine}");
                expectedSerializedGraph.AppendLine($"<{FamilyGraphTag}>");

                foreach (Person person in graph.Vertices)
                {
                    expectedSerializedGraph.AppendLine($"{Indent}<{PersonNodeTag} id=\"{person.Id}\" />");
                }

                TaggedEdge<Person, string>[] relations = graph.Edges.ToArray();
                for (int i = 0; i < relations.Length; ++i)
                {
                    expectedSerializedGraph.AppendLine($"{Indent}<{RelationEdgeTag} id=\"{i}\" source=\"{relations[i].Source.Id}\" target=\"{relations[i].Target.Id}\" />");
                }

                expectedSerializedGraph.Append($"</{FamilyGraphTag}>");

                StringAssert.AreEqualIgnoringCase(
                    expectedSerializedGraph.ToString(),
                    xmlGraph);
            }

            #endregion
        }

        [Test]
        public void SerializationToXml_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            Assert.Throws<ArgumentNullException>(
                () => graph.SerializeToXml(
                    null,
                    vertex => vertex.ToString(),
                    graph.GetEdgeIdentity(),
                    FamilyGraphTag,
                    PersonNodeTag,
                    RelationEdgeTag,
                    TestNamespace));

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            using (var xmlWriter = XmlWriter.Create(writer))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<int, IEdge<int>>)null).SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        PersonNodeTag,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentNullException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        null,
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        PersonNodeTag,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentNullException>(
                    () => graph.SerializeToXml<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        null,
                        FamilyGraphTag,
                        PersonNodeTag,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        null,
                        PersonNodeTag,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        "",
                        PersonNodeTag,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        null,
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        "",
                        RelationEdgeTag,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        PersonNodeTag,
                        null,
                        TestNamespace));

                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        PersonNodeTag,
                        "",
                        ""));

                Assert.Throws<ArgumentNullException>(
                    () => graph.SerializeToXml(
                        xmlWriter,
                        vertex => vertex.ToString(),
                        graph.GetEdgeIdentity(),
                        FamilyGraphTag,
                        PersonNodeTag,
                        RelationEdgeTag,
                        null));
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion

        #region Deserialization

        [NotNull]
        private const string EmptyGraphFileName = "emptyGraph.xml";

        [NotNull]
        private const string TestGraphFileName = "testGraph.xml";

        #region Test helpers

        private static void AssetTestGraphContent<TEdge, TGraph>(
            [NotNull] TGraph graph,
            [NotNull, InstantHandle] Func<string, string, double, TEdge> edgeFactory)
            where TEdge : IEdge<string>
            where TGraph : IVertexSet<string>, IEdgeSet<string, TEdge>
        {
            graph.AssertHasVertices(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" });
            graph.AssertHasEdges(
                new[]
                {
                    edgeFactory("1", "2", 6.0),
                    edgeFactory("1", "4", 11.0),
                    edgeFactory("1", "5", 5.0),
                    edgeFactory("2", "5", 8.0),
                    edgeFactory("2", "3", 15.0),
                    edgeFactory("2", "4", 18.0),
                    edgeFactory("2", "7", 11.0),
                    edgeFactory("3", "4", 8.0),
                    edgeFactory("3", "8", 18.0),
                    edgeFactory("3", "9", 6.0),
                    edgeFactory("4", "6", 10.0),
                    edgeFactory("4", "7", 7.0),
                    edgeFactory("4", "11", 17.0),
                    edgeFactory("5", "6", 15.0),
                    edgeFactory("5", "7", 9.0),
                    edgeFactory("6", "11", 3.0),
                    edgeFactory("7", "8", 9.0),
                    edgeFactory("7", "9", 4.0),
                    edgeFactory("7", "11", 12.0),
                    edgeFactory("7", "10", 13.0),
                    edgeFactory("8", "9", 14.0),
                    edgeFactory("8", "12", 5.0),
                    edgeFactory("9", "10", 19.0),
                    edgeFactory("10", "12", 2.0),
                    edgeFactory("11", "12", 7.0)
                });
        }

        #endregion

        [Test]
        public void DeserializeFromXml_Document_Empty()
        {
            var document = new XPathDocument(GetGraphFilePath(EmptyGraphFileName));

            // Directed graph
            var adjacencyGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new AdjacencyGraph<string, IEdge<string>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => Edge.Create(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace)));
            AssertEmptyGraph(adjacencyGraph);

            // Directed bidirectional graph
            var bidirectionalGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new BidirectionalGraph<string, IEdge<string>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => Edge.Create(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace)));
            AssertEmptyGraph(bidirectionalGraph);

            // Undirected graph
            UndirectedGraph<string, TaggedEdge<string, double>> undirectedGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => new TaggedEdge<string, double>(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace),
                    int.Parse(nav.GetAttribute(WeightTag, TestNamespace))));
            AssertEmptyGraph(undirectedGraph);
        }

        [Test]
        public void DeserializeFromXml_Document()
        {
            var document = new XPathDocument(GetGraphFilePath(TestGraphFileName));

            // Directed graph
            AdjacencyGraph<string, EquatableEdge<string>> adjacencyGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new AdjacencyGraph<string, EquatableEdge<string>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => new EquatableEdge<string>(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace)));
            AssetTestGraphContent(
                adjacencyGraph,
                (v1, v2, _) => new EquatableEdge<string>(v1, v2));

            // Directed bidirectional graph
            BidirectionalGraph<string, EquatableEdge<string>> bidirectionalGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new BidirectionalGraph<string, EquatableEdge<string>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => new EquatableEdge<string>(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace)));
            AssetTestGraphContent(
                bidirectionalGraph,
                (v1, v2, _) => new EquatableEdge<string>(v1, v2));

            // Undirected graph
            UndirectedGraph<string, EquatableTaggedEdge<string, double>> undirectedGraph = document.DeserializeFromXml(
                GraphTag,
                NodeTag,
                EdgeTag,
                _ => new UndirectedGraph<string, EquatableTaggedEdge<string, double>>(),
                nav => nav.GetAttribute(IdTag, TestNamespace),
                nav => new EquatableTaggedEdge<string, double>(
                    nav.GetAttribute(SourceTag, TestNamespace),
                    nav.GetAttribute(TargetTag, TestNamespace),
                    int.Parse(nav.GetAttribute(WeightTag, TestNamespace))));
            AssetTestGraphContent(
                undirectedGraph,
                (v1, v2, weight) => new EquatableTaggedEdge<string, double>(v1, v2, weight));
        }

        [Test]
        public void DeserializeFromXml_Document_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((XPathDocument) null).DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, EquatableEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => new EquatableEdge<string>(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            var document = new XPathDocument(GetGraphFilePath(TestGraphFileName));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                null,
                NodeTag,
                EdgeTag,
                _ => new AdjacencyGraph<string, IEdge<string>>(),
                nav => nav.GetAttribute(IdTag, ""),
                nav => Edge.Create(
                    nav.GetAttribute(SourceTag, ""),
                    nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                    "",
                    NodeTag,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                    GraphTag,
                    null,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                    GraphTag,
                    "",
                    EdgeTag,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    null,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentException>(
                () => document.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentNullException>(
                () => document.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    null,
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentNullException>(
                () => document.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    null,
                    nav => Edge.Create(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));

            Assert.Throws<ArgumentNullException>(
                () => document.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    null));

            // No graph node found
            Assert.Throws<InvalidOperationException>(
                () => document.DeserializeFromXml(
                    "g",    // No node named "g" for the graph
                    NodeTag,
                    EdgeTag,
                    _ => new AdjacencyGraph<string, EquatableEdge<string>>(),
                    nav => nav.GetAttribute(IdTag, ""),
                    nav => new EquatableEdge<string>(
                        nav.GetAttribute(SourceTag, ""),
                        nav.GetAttribute(TargetTag, ""))));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void DeserializeFromXml_Reader_Empty()
        {
            using (var reader = XmlReader.Create(GetGraphFilePath(EmptyGraphFileName)))
            {
                var adjacencyGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute")));
                AssertEmptyGraph(adjacencyGraph);
            }

            using (var reader = XmlReader.Create(GetGraphFilePath(EmptyGraphFileName)))
            {
                var bidirectionalGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new BidirectionalGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute")));
                AssertEmptyGraph(bidirectionalGraph);
            }

            using (var reader = XmlReader.Create(GetGraphFilePath(EmptyGraphFileName)))
            {
                UndirectedGraph<string, TaggedEdge<string, double>> undirectedGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                    r => r.GetAttribute(IdTag),
                    r => new TaggedEdge<string, double>(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"),
                        int.Parse(r.GetAttribute(WeightTag) ?? throw new AssertionException("Must have weight attribute"))));
                AssertEmptyGraph(undirectedGraph);
            }
        }

        [Test]
        public void DeserializeFromXml_Reader()
        {
            using (var reader = XmlReader.Create(GetGraphFilePath(TestGraphFileName)))
            {
                AdjacencyGraph<string, EquatableEdge<string>> adjacencyGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new AdjacencyGraph<string, EquatableEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => new EquatableEdge<string>(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute")));
                AssetTestGraphContent(
                    adjacencyGraph,
                    (v1, v2, _) => new EquatableEdge<string>(v1, v2));
            }

            using (var reader = XmlReader.Create(GetGraphFilePath(TestGraphFileName)))
            {
                BidirectionalGraph<string, EquatableEdge<string>> bidirectionalGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new BidirectionalGraph<string, EquatableEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => new EquatableEdge<string>(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute")));
                AssetTestGraphContent(
                    bidirectionalGraph,
                    (v1, v2, _) => new EquatableEdge<string>(v1, v2));
            }

            using (var reader = XmlReader.Create(GetGraphFilePath(TestGraphFileName)))
            {
                UndirectedGraph<string, EquatableTaggedEdge<string, double>> undirectedGraph = reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new UndirectedGraph<string, EquatableTaggedEdge<string, double>>(),
                    r => r.GetAttribute(IdTag),
                    r => new EquatableTaggedEdge<string, double>(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"),
                        int.Parse(r.GetAttribute(WeightTag) ?? throw new AssertionException("Must have weight attribute"))));
                AssetTestGraphContent(
                    undirectedGraph,
                    (v1, v2, weight) => new EquatableTaggedEdge<string, double>(v1, v2, weight));
            }
        }

        [Test]
        public void DeserializeFromXml_Reader_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((XmlReader)null).DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

            using (var reader = XmlReader.Create(GetGraphFilePath(TestGraphFileName)))
            {
                Assert.Throws<ArgumentNullException>(() => reader.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    "",
                    null,
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentNullException>(() => reader.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    null,
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentNullException>(() => reader.DeserializeFromXml<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    null));


                Assert.Throws<ArgumentNullException>(
                    () => reader.DeserializeFromXml(
                        null,
                        r => r.Name == "vertex",
                        r => r.Name == EdgeTag,
                        _ => new AdjacencyGraph<string, IEdge<string>>(),
                        r => r.GetAttribute(IdTag),
                        r => Edge.Create(
                            r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                            r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentNullException>(
                    () => reader.DeserializeFromXml(
                        r => r.Name == GraphTag,
                        null,
                        r => r.Name == EdgeTag,
                        _ => new AdjacencyGraph<string, IEdge<string>>(),
                        r => r.GetAttribute(IdTag),
                        r => Edge.Create(
                            r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                            r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentNullException>(
                    () => reader.DeserializeFromXml(
                        r => r.Name == GraphTag,
                        r => r.Name == "vertex",
                        null,
                        _ => new AdjacencyGraph<string, IEdge<string>>(),
                        r => r.GetAttribute(IdTag),
                        r => Edge.Create(
                            r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                            r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));


                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    null,
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    "",
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    GraphTag,
                    null,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    GraphTag,
                    "",
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    null,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentException>(() => reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    "",
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));

                Assert.Throws<ArgumentNullException>(() => reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    null,
                    _ => new AdjacencyGraph<string, IEdge<string>>(),
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));


                // No graph node found
                Assert.Throws<InvalidOperationException>(() => reader.DeserializeFromXml(
                    "g",    // No node named "g" for the graph
                    NodeTag,
                    EdgeTag,
                    "",
                    _ => new AdjacencyGraph<string, IEdge<string>>(), 
                    r => r.GetAttribute(IdTag),
                    r => Edge.Create(
                        r.GetAttribute(SourceTag) ?? throw new AssertionException("Must have source attribute"),
                        r.GetAttribute(TargetTag) ?? throw new AssertionException("Must have target attribute"))));
            }
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Serialization/Deserialization

        #region Test Helpers

        [Pure]
        [NotNull]
        private static string GetSerializableString([CanBeNull] string str)
        {
            return str ?? NullString;
        }

        private static void SerializeEdge_Simple<TInEdge>([NotNull] XmlWriter writer, [NotNull] TInEdge edge)
            where TInEdge : IEdge<int>, IEquatable<TInEdge>
        {
            // Serialize only relevant stuff for the test
            switch (edge)
            {
                case EquatableTaggedEdge<int, double> edgeDoubleTag:
                    writer.WriteAttributeString(IsTaggedTag, TestNamespace, bool.TrueString);
                    writer.WriteAttributeString(DoubleValueTag, TestNamespace, edgeDoubleTag.Tag.ToString(CultureInfo.InvariantCulture));
                    break;

                case EquatableTaggedEdge<int, string> edgeStringTag:
                    writer.WriteAttributeString(IsTaggedTag, TestNamespace, bool.TrueString);
                    writer.WriteAttributeString(StringValueTag, TestNamespace, GetSerializableString(edgeStringTag.Tag));
                    break;

                default:
                    writer.WriteAttributeString(IsTaggedTag, TestNamespace, bool.FalseString);
                    break;
            }
        }

        [Pure]
        private static int DeserializeVertex_Simple([NotNull] XmlReader reader)
        {
            return int.Parse(reader.GetAttribute(IdTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize vertex."));
        }

        [Pure]
        [NotNull]
        private static EquatableEdge<int> DeserializeEdge_Simple([NotNull] XmlReader reader)
        {
            int source = int.Parse(reader.GetAttribute(SourceTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge source."));
            int target = int.Parse(reader.GetAttribute(TargetTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge target."));

            bool hasTag = bool.Parse(reader.GetAttribute(IsTaggedTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge tag info."));
            if (hasTag)
            {
                string attribute = reader.GetAttribute(DoubleValueTag, TestNamespace);
                if (attribute is null)
                {
                    attribute = reader.GetAttribute(StringValueTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize vertex tag.");
                    return new EquatableTaggedEdge<int, string>(source, target, GetString(attribute));
                }

                return new EquatableTaggedEdge<int, double>(
                    source,
                    target,
                    double.Parse(attribute, CultureInfo.InvariantCulture));
            }

            return new EquatableEdge<int>(source, target);


            #region Local function

            string GetString(string readStr)
            {
                return NullString.Equals(readStr, StringComparison.Ordinal) ? null : readStr;
            }

            #endregion
        }

        [Pure]
        private static SEquatableEdge<int> DeserializeSEdge_Simple([NotNull] XmlReader reader)
        {
            return new SEquatableEdge<int>(
                int.Parse(reader.GetAttribute(SourceTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge source.")),
                int.Parse(reader.GetAttribute(TargetTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge target.")));
        }

        private static void SerializeVertex_Complex([NotNull] XmlWriter writer, [NotNull] EquatableTestVertex vertex)
        {
            // Serialize only relevant stuff for the test
            Type vertexType = vertex.GetType();
            writer.WriteAttributeString(TypeTag, TestNamespace, TypeToSerializableType(vertexType));

            writer.WriteAttributeString(StringDefaultTag, TestNamespace, GetSerializableString(vertex.StringDefault));
            writer.WriteAttributeString(StringTag, TestNamespace, GetSerializableString(vertex.String));
            writer.WriteAttributeString(BoolTag, TestNamespace, vertex.Bool.ToString());
            writer.WriteAttributeString(IntTag, TestNamespace, vertex.Int.ToString());
            writer.WriteAttributeString(LongTag, TestNamespace, vertex.Long.ToString());
            writer.WriteAttributeString(FloatTag, TestNamespace, vertex.Float.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString(DoubleTag, TestNamespace, vertex.Double.ToString(CultureInfo.InvariantCulture));

            if (vertex is EquatableAdditionalDataTestVertex vertexWithData)
            {
                writer.WriteAttributeString(AdditionalDataTag, TestNamespace, vertexWithData.Data.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void SerializeEdge_Complex<TInEdge>([NotNull] XmlWriter writer, [NotNull] TInEdge edge)
            where TInEdge : IEdge<EquatableTestVertex>, IEquatable<TInEdge>
        {
            // Serialize only relevant stuff for the test
            IEdge<EquatableTestVertex> edgeToCheck = edge;
            if (edge is SReversedEdge<EquatableTestVertex, EquatableTestEdge> reversedEdge)
            {
                edgeToCheck = reversedEdge.OriginalEdge;
            }

            Type edgeType = edgeToCheck.GetType();
            writer.WriteAttributeString(TypeTag, TestNamespace, TypeToSerializableType(edgeType));

            if (edgeToCheck is EquatableTestEdge edgeToSerialize)
            {
                writer.WriteAttributeString(StringTag, TestNamespace, GetSerializableString(edgeToSerialize.String));
                writer.WriteAttributeString(BoolTag, TestNamespace, edgeToSerialize.Bool.ToString());
                writer.WriteAttributeString(IntTag, TestNamespace, edgeToSerialize.Int.ToString());
                writer.WriteAttributeString(LongTag, TestNamespace, edgeToSerialize.Long.ToString());
                writer.WriteAttributeString(FloatTag, TestNamespace, edgeToSerialize.Float.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString(DoubleTag, TestNamespace, edgeToSerialize.Double.ToString(CultureInfo.InvariantCulture));
            }

            if (edgeToCheck is EquatableAdditionalDataTestEdge edgeToSerializeWithData)
            {
                writer.WriteAttributeString(AdditionalDataTag, TestNamespace, edgeToSerializeWithData.Data.ToString(CultureInfo.InvariantCulture));
            }
        }

        [Pure]
        [NotNull]
        private static EquatableTestVertex DeserializeVertex_Complex([NotNull] XmlReader reader)
        {
            string id = reader.GetAttribute(IdTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize vertex id.");
            var type = Type.GetType(reader.GetAttribute(TypeTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize vertex type."));

            EquatableTestVertex vertex = type == typeof(EquatableAdditionalDataTestVertex)
                ? new EquatableAdditionalDataTestVertex(
                    id,
                    double.Parse(reader.GetAttribute(AdditionalDataTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize vertex data.")))
                : new EquatableTestVertex(id);

            return AssignData(vertex);

            #region Local function

            EquatableTestVertex AssignData(EquatableTestVertex v)
            {
                v.StringDefault = GetString(StringDefaultTag);
                v.String = GetString(StringTag);
                v.Bool = bool.Parse(reader.GetAttribute(BoolTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize vertex \"{BoolTag}\" tag."));
                v.Int = int.Parse(reader.GetAttribute(IntTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize vertex \"{IntTag}\" tag."));
                v.Long = long.Parse(reader.GetAttribute(LongTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize vertex \"{LongTag}\" tag."));
                v.Float = float.Parse(
                    reader.GetAttribute(FloatTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize vertex \"{FloatTag}\" tag."),
                    CultureInfo.InvariantCulture);
                v.Double = double.Parse(
                    reader.GetAttribute(DoubleTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize vertex \"{DoubleTag}\" tag."),
                    CultureInfo.InvariantCulture);

                return v;

                #region Local function

                string GetString(string tag)
                {
                    string readStr = reader.GetAttribute(tag, TestNamespace)
                                     ?? throw new AssertionException($"Unable to deserialize vertex \"{tag}\" tag.");
                    return NullString.Equals(readStr, StringComparison.Ordinal) ? null : readStr;
                }

                #endregion
            }

            #endregion
        }

        [Pure]
        [NotNull]
        private static EquatableTestVertex DeserializeVertex_Complex([NotNull] XmlReader reader, [NotNull] IDictionary<string, EquatableTestVertex> verticesCache)
        {
            EquatableTestVertex vertex = DeserializeVertex_Complex(reader);
            verticesCache[VertexIdentity_Complex(vertex)] = vertex;
            return vertex;
        }

        [Pure]
        [NotNull]
        private static EquatableTestEdge DeserializeEdge_Complex([NotNull] XmlReader reader, [NotNull] IDictionary<string, EquatableTestVertex> verticesCache)
        {
            string id = reader.GetAttribute(IdTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge id.");

            string sourceId = reader.GetAttribute(SourceTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge source id.");
            string targetId = reader.GetAttribute(TargetTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge target id.");
            EquatableTestVertex source = verticesCache[sourceId];
            EquatableTestVertex target = verticesCache[targetId];

            var type = Type.GetType(reader.GetAttribute(TypeTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge type."));

            EquatableTestEdge edge = type == typeof(EquatableAdditionalDataTestEdge)
                ? new EquatableAdditionalDataTestEdge(
                    source,
                    target,
                    id,
                    double.Parse(reader.GetAttribute(AdditionalDataTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge data."), CultureInfo.InvariantCulture))
                : new EquatableTestEdge(source, target, id);

            return AssignData(edge);

            #region Local function

            EquatableTestEdge AssignData(EquatableTestEdge e)
            {
                e.String = GetString(StringTag);
                e.Bool = bool.Parse(reader.GetAttribute(BoolTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize edge \"{BoolTag}\" tag."));
                e.Int = int.Parse(reader.GetAttribute(IntTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize edge \"{IntTag}\" tag."));
                e.Long = long.Parse(reader.GetAttribute(LongTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize edge \"{LongTag}\" tag."));
                e.Float = float.Parse(
                    reader.GetAttribute(FloatTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize edge \"{FloatTag}\" tag."),
                    CultureInfo.InvariantCulture);
                e.Double = double.Parse(
                    reader.GetAttribute(DoubleTag, TestNamespace) ?? throw new AssertionException($"Unable to deserialize edge \"{DoubleTag}\" tag."),
                    CultureInfo.InvariantCulture);

                return e;

                #region Local function

                string GetString(string tag)
                {
                    string readStr = reader.GetAttribute(tag, TestNamespace)
                                     ?? throw new AssertionException($"Unable to deserialize edge \"{tag}\" tag.");
                    return NullString.Equals(readStr, StringComparison.Ordinal) ? null : readStr;
                }

                #endregion
            }

            #endregion
        }

        [Pure]
        private static SEquatableEdge<EquatableTestVertex> DeserializeSEdge_Complex([NotNull] XmlReader reader, [NotNull] IDictionary<string, EquatableTestVertex> verticesCache)
        {
            string sourceId = reader.GetAttribute(SourceTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge source id.");
            string targetId = reader.GetAttribute(TargetTag, TestNamespace) ?? throw new AssertionException("Unable to deserialize edge target id.");
            EquatableTestVertex source = verticesCache[sourceId];
            EquatableTestVertex target = verticesCache[targetId];

            return new SEquatableEdge<EquatableTestVertex>(source, target);
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize<TVertex, TInEdge, TOutEdge, TInGraph, TOutGraph>(
            [NotNull] TInGraph graph,
            [NotNull, InstantHandle] VertexIdentity<TVertex> vertexIdentity,
            [CanBeNull, InstantHandle] Action<XmlWriter, TVertex> vertexAttributes,
            [CanBeNull, InstantHandle] Action<XmlWriter, TInEdge> edgeAttributes,
            [NotNull, InstantHandle] Func<XmlReader, TOutGraph> deserialize)
            where TInEdge : IEdge<TVertex>, IEquatable<TInEdge>
            where TOutEdge : IEdge<TVertex>, IEquatable<TOutEdge>
            where TInGraph : IEdgeListGraph<TVertex, TInEdge>
            where TOutGraph : IEdgeListGraph<TVertex, TOutEdge>
        {
            Assert.IsNotNull(graph);

            var settings = new XmlWriterSettings { Indent = true, IndentChars = Indent };
            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                // Serialize
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    graph.SerializeToXml(
                        xmlWriter,
                        vertexIdentity,
                        graph.GetEdgeIdentity(),
                        GraphTag,
                        NodeTag,
                        EdgeTag,
                        TestNamespace,
                        null,
                        vertexAttributes,
                        edgeAttributes);
                }

                memory.Position = 0;

                // Deserialize
                using (var xmlReader = XmlReader.Create(memory))
                {
                    TOutGraph deserializedGraph = deserialize(xmlReader);
                    Assert.IsNotNull(deserializedGraph);
                    Assert.AreNotSame(graph, deserializedGraph);
                    return deserializedGraph;
                }
            }
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Simple<TInEdge, TOutEdge, TInGraph, TOutGraph>(
            [NotNull] TInGraph graph,
            [NotNull, InstantHandle] Func<XmlReader, TOutGraph> deserialize)
            where TInEdge : IEdge<int>, IEquatable<TInEdge>
            where TOutEdge : IEdge<int>, IEquatable<TOutEdge>
            where TInGraph : IEdgeListGraph<int, TInEdge>
            where TOutGraph : IEdgeListGraph<int, TOutEdge>
        {
            return SerializeDeserialize<int, TInEdge, TOutEdge, TInGraph, TOutGraph>(
                graph,
                VertexIdentity_Simple,
                null,
                SerializeEdge_Simple,
                deserialize);
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Complex<TInEdge, TOutEdge, TInGraph, TOutGraph>(
            [NotNull] TInGraph graph,
            [NotNull, InstantHandle] Func<XmlReader, TOutGraph> deserialize)
            where TInEdge : IEdge<EquatableTestVertex>, IEquatable<TInEdge>
            where TOutEdge : IEdge<EquatableTestVertex>, IEquatable<TOutEdge>
            where TInGraph : IEdgeListGraph<EquatableTestVertex, TInEdge>
            where TOutGraph : IEdgeListGraph<EquatableTestVertex, TOutEdge>
        {
            return SerializeDeserialize<EquatableTestVertex, TInEdge, TOutEdge, TInGraph, TOutGraph>(
                graph,
                VertexIdentity_Complex,
                SerializeVertex_Complex,
                SerializeEdge_Complex,
                deserialize);
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, EquatableEdge<int>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, EquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<EquatableEdge<int>, EquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    DeserializeVertex_Simple,
                    DeserializeEdge_Simple));
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, EquatableTestEdge>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, EquatableTestEdge>, new()
        {
            var verticesCache = new Dictionary<string, EquatableTestVertex>();
            return SerializeDeserialize_Complex<EquatableTestEdge, EquatableTestEdge, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    r => DeserializeVertex_Complex(r, verticesCache),
                    r => DeserializeEdge_Complex(r, verticesCache)));
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_SEdge_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, SEquatableEdge<int>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, SEquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<SEquatableEdge<int>, SEquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    DeserializeVertex_Simple,
                    DeserializeSEdge_Simple));
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_SEdge_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>, new()
        {
            var verticesCache = new Dictionary<string, EquatableTestVertex>();
            return SerializeDeserialize_Complex<SEquatableEdge<EquatableTestVertex>, SEquatableEdge<EquatableTestVertex>, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    r => DeserializeVertex_Complex(r, verticesCache),
                    r => DeserializeSEdge_Complex(r, verticesCache)));
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Reversed_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, SReversedEdge<int, EquatableEdge<int>>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, EquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<SReversedEdge<int, EquatableEdge<int>>, EquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    DeserializeVertex_Simple,
                    DeserializeEdge_Simple));
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Reversed_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, SReversedEdge<EquatableTestVertex, EquatableTestEdge>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, EquatableTestEdge>, new()
        {
            var verticesCache = new Dictionary<string, EquatableTestVertex>();
            return SerializeDeserialize_Complex<SReversedEdge<EquatableTestVertex, EquatableTestEdge>, EquatableTestEdge, TInGraph, TOutGraph>(graph, reader =>
                reader.DeserializeFromXml(
                    GraphTag,
                    NodeTag,
                    EdgeTag,
                    TestNamespace,
                    _ => new TOutGraph(),
                    r => DeserializeVertex_Complex(r, verticesCache),
                    r => DeserializeEdge_Complex(r, verticesCache)));
        }

        #endregion

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphTestCases))]
        public void XmlSerialization_AdjacencyGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize_Simple<AdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayAdjacencyGraph<int, EquatableEdge<int>>(graph);
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize_Simple<ArrayAdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexTestCases))]
        public void XmlSerialization_AdjacencyGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph1 =
                SerializeDeserialize_Complex<AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize_Complex<ArrayAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphTestCases))]
        public void XmlSerialization_AdapterGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<int, EquatableEdge<int>>(graph);
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<BidirectionalAdapterGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexTestCases))]
        public void XmlSerialization_AdapterGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphTestCases))]
        public void XmlSerialization_ClusteredGraph_Simple([NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<ClusteredAdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphComplexTestCases))]
        public void XmlSerialization_ClusteredGraph_Complex([NotNull] ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphTestCases))]
        public void XmlSerialization_CompressedGraph_Simple([NotNull] CompressedSparseRowGraph<int> graph)
        {
            AdjacencyGraph<int, SEquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_SEdge_Simple<CompressedSparseRowGraph<int>, AdjacencyGraph<int, SEquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphComplexTestCases))]
        public void XmlSerialization_CompressedGraph_Complex([NotNull] CompressedSparseRowGraph<EquatableTestVertex> graph)
        {
            AdjacencyGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>> deserializedGraph =
                SerializeDeserialize_SEdge_Complex<CompressedSparseRowGraph<EquatableTestVertex>, AdjacencyGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphTestCases))]
        public void XmlSerialization_BidirectionalGraph_Simple([NotNull] BidirectionalGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<BidirectionalGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));

            var arrayGraph = new ArrayBidirectionalGraph<int, EquatableEdge<int>>(graph);
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize_Simple<ArrayBidirectionalGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));

            var reversedGraph = graph.CreateReversedBidirectionalGraph();
            BidirectionalGraph<int, EquatableEdge<int>> deserializedGraph3 =
                SerializeDeserialize_Reversed_Simple<ReversedBidirectionalGraph<int, EquatableEdge<int>>, BidirectionalGraph<int, EquatableEdge<int>>>(reversedGraph);
            Assert.IsTrue(
                EquateGraphs.IsEqualTo(
                    graph,
                    deserializedGraph3,
                    EqualityComparer<int>.Default,
                    LambdaEqualityComparer<EquatableEdge<int>>.Create(
                        (edge1, edge2) => Equals(edge1.Source, edge2.Target)
                                          && Equals(edge1.Target, edge2.Source)
                                          && edge1.GetType() == edge2.GetType(),
                        edge => edge.GetHashCode())));

            var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<int, EquatableEdge<int>>(graph);
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph4 =
                SerializeDeserialize_Simple<UndirectedBidirectionalGraph<int, EquatableEdge<int>>, UndirectedGraph<int, EquatableEdge<int>>>(undirectedBidirectionalGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(undirectedBidirectionalGraph, deserializedGraph4));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphComplexTestCases))]
        public void XmlSerialization_BidirectionalGraph_Complex([NotNull] BidirectionalGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));

            var arrayGraph = new ArrayBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize_Complex<ArrayBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));

            var reversedGraph = graph.CreateReversedBidirectionalGraph();
            BidirectionalGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph3 =
                SerializeDeserialize_Reversed_Complex<ReversedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>, BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>>(reversedGraph);
            Assert.IsTrue(
                EquateGraphs.IsEqualTo(
                    graph,
                    deserializedGraph3,
                    EqualityComparer<EquatableTestVertex>.Default,
                    LambdaEqualityComparer<EquatableTestEdge>.Create(
                        (edge1, edge2) => Equals(edge1.Source, edge2.Target) && Equals(edge1.Target, edge2.Source) && edge1.DataContentEquals(edge2),
                        edge => edge.GetHashCode())));

            var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            UndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph4 =
                SerializeDeserialize_Complex<UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>, UndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(undirectedBidirectionalGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(undirectedBidirectionalGraph, deserializedGraph4));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalMatrixGraphTestCases))]
        public void XmlSerialization_BidirectionalMatrixGraph([NotNull] BidirectionalMatrixGraph<EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<BidirectionalMatrixGraph<EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(
                EquateGraphs.IsEqualTo(
                    graph,
                    deserializedGraph,
                    EqualityComparer<int>.Default,
                    LambdaEqualityComparer<EquatableEdge<int>>.Create(
                        (edge1, edge2) => EqualityComparer<EquatableEdge<int>>.Default.Equals(edge1, edge2) && edge1.GetType() == edge2.GetType(),
                        edge => edge.GetHashCode())));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphTestCases))]
        public void XmlSerialization_UndirectedGraph_Simple([NotNull] UndirectedGraph<int, EquatableEdge<int>> graph)
        {
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize_Simple<UndirectedGraph<int, EquatableEdge<int>>, UndirectedGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayUndirectedGraph<int, EquatableEdge<int>>(graph);
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize_Simple<ArrayUndirectedGraph<int, EquatableEdge<int>>, UndirectedGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphComplexTestCases))]
        public void XmlSerialization_UndirectedGraph_Complex([NotNull] UndirectedGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            UndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph1 =
                SerializeDeserialize_Complex<UndirectedGraph<EquatableTestVertex, EquatableTestEdge>, UndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayUndirectedGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            UndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize_Complex<ArrayUndirectedGraph<EquatableTestVertex, EquatableTestEdge>, UndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationEdgeListGraphTestCases))]
        public void XmlSerialization_EdgeListGraph_Simple([NotNull] EdgeListGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<EdgeListGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationEdgeListGraphComplexTestCases))]
        public void XmlSerialization_EdgeListGraph_Complex([NotNull] EdgeListGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<EdgeListGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        #endregion
    }
}