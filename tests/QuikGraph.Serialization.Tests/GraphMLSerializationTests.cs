using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
#if SUPPORTS_XML_DTD_PROCESSING
using System.Linq;
using System.Xml.Schema;
#endif
using System.Xml;
using System.Xml.XPath;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Tests;
using static QuikGraph.Serialization.Tests.SerializationTestCaseSources;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;
using static QuikGraph.Serialization.Tests.TestHelpers;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Tests for <see cref="GraphMLSerializer{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class GraphMLSerializerTests
    {
        #region Test helpers

        /// <summary> Verifies that the deserialized and serialized graphMl is equal to the original <paramref name="graph"/> . </summary>
        [Pure]
        [NotNull]
        private static TGraph VerifyDoubleSerialization<TGraph>(
            [NotNull] TGraph graph,
            [NotNull, InstantHandle] Func<TGraph, string> serializeGraph,
            [NotNull, InstantHandle] Func<string, TGraph> deserializeGraph)
        {
            // Serialize the graph to GraphML
            string graphml = serializeGraph(graph);

            // Deserialize a graph from previous serialization
            TGraph serializedGraph = deserializeGraph(graphml);

            // Serialize the deserialized graph
            string newGraphml = serializeGraph(serializedGraph);

            // => Serialization should produce the same result
            Assert.AreEqual(graphml, newGraphml);

            Assert.AreNotSame(graph, serializeGraph);

            return serializedGraph;
        }

        #endregion

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void GraphMLSerialization_HeaderCheck(bool emitDeclarationOnSerialize, bool emitDeclarationOnDeserialize)
        {
            var graph = new EquatableTestGraph
            {
                String = "graph",
                Int = 42
            };

            var vertex1 = new EquatableTestVertex("v1")
            {
                StringDefault = "foo",
                String = "string",
                Int = 10,
                Long = 20,
                Float = 25.0F,
                Double = 30.0,
                Bool = true
            };

            var vertex2 = new EquatableTestVertex("v2")
            {
                StringDefault = "bar",
                String = "string2",
                Int = 110,
                Long = 120,
                Float = 125.0F,
                Double = 130.0,
                Bool = true
            };

            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);

            var edge1 = new EquatableTestEdge(vertex1, vertex2, "e_1")
            {
                String = "edge",
                Int = 90,
                Long = 100,
                Float = 25.0F,
                Double = 110.0,
                Bool = true
            };

            graph.AddEdge(edge1);

            EquatableTestGraph serializedGraph = VerifyDoubleSerialization(
                graph,
                g =>
                {
                    using (var writer = new StringWriter())
                    {
                        var settings = new XmlWriterSettings { Indent = true };
                        using (var xmlWriter = XmlWriter.Create(writer, settings))
                        {
                            var serializer = new GraphMLSerializer<EquatableTestVertex, EquatableTestEdge, EquatableTestGraph>
                            {
                                EmitDocumentDeclaration = emitDeclarationOnDeserialize
                            };

                            serializer.Serialize(
                                xmlWriter,
                                g,
                                vertex => vertex.ID,
                                edge => edge.ID);
                        }

                        return writer.ToString();
                    }
                },
                graphml =>
                {
                    using (var reader = new StringReader(graphml))
                    {
                        var serializer = new GraphMLDeserializer<EquatableTestVertex, EquatableTestEdge, EquatableTestGraph>
                        {
                            EmitDocumentDeclaration = emitDeclarationOnDeserialize
                        };

#if SUPPORTS_XML_DTD_PROCESSING
                        var settings = new XmlReaderSettings
                        {
                            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
                            XmlResolver = new GraphMLXmlResolver(),
                            DtdProcessing = DtdProcessing.Ignore
                        };

                        using (var xmlReader = XmlReader.Create(reader, settings))
                        {
#else
                        var xmlReader = new XmlTextReader(reader);
                        {
                            xmlReader.ProhibitDtd = false;
                            xmlReader.XmlResolver = null;
#endif
                            var g = new EquatableTestGraph();
                            serializer.Deserialize(
                                xmlReader,
                                g,
                                id => new EquatableTestVertex(id),
                                (source, target, id) => new EquatableTestEdge(source, target, id));
                            return g;
                        }
                    }
                });

            Assert.IsTrue(graph.IsEqualTo(serializedGraph));
        }

        #region Serialization

        [NotNull]
        private static readonly string WriteThrowsTestFilePath =
            Path.Combine(GetTemporaryTestDirectory(), "serialization_to_graphml_throws_test.graphml");

        [Test]
        public void SerializeToGraphML_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // Filepath
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<TestVertex, TestEdge>)null).SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(WriteThrowsTestFilePath));

            var graph = new AdjacencyGraph<TestVertex, TestEdge>();
            Assert.Throws<ArgumentException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>((string)null));
            Assert.Throws<ArgumentException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(""));

            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<TestVertex, TestEdge>)null).SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    WriteThrowsTestFilePath,
                    vertex => vertex.ID,
                    edge => edge.ID));

            Assert.Throws<ArgumentException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    (string)null,
                    vertex => vertex.ID,
                    edge => edge.ID));
            Assert.Throws<ArgumentException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    "",
                    vertex => vertex.ID,
                    edge => edge.ID));

            Assert.Throws<ArgumentNullException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    WriteThrowsTestFilePath,
                    null,
                    edge => edge.ID));

            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<TestVertex, TestEdge>)null).SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    WriteThrowsTestFilePath,
                    vertex => vertex.ID,
                    null));

            // XML writer
            Assert.Throws<ArgumentNullException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>((XmlWriter)null));
            Assert.Throws<ArgumentNullException>(
                () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                    (XmlWriter)null,
                    vertex => vertex.ID,
                    edge => edge.ID));

            using (var writer = XmlWriter.Create(WriteThrowsTestFilePath))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<TestVertex, TestEdge>)null).SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(writer));

                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<TestVertex, TestEdge>)null).SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                        writer,
                        vertex => vertex.ID,
                        edge => edge.ID));

                Assert.Throws<ArgumentNullException>(
                    () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                        writer,
                        null,
                        edge => edge.ID));

                Assert.Throws<ArgumentNullException>(
                    () => graph.SerializeToGraphML<TestVertex, TestEdge, AdjacencyGraph<TestVertex, TestEdge>>(
                        writer,
                        vertex => vertex.ID,
                        null));
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void SerializeToGraphML_Throws_InvalidData()
        {
            AssertSerializationFail<TestGraphArrayDefaultValue, NotSupportedException>(new TestGraphArrayDefaultValue());
            AssertSerializationFail<TestGraphNoGetter, TypeInitializationException>(new TestGraphNoGetter());
            AssertSerializationFail<TestGraphNullDefaultValue, TypeInitializationException>(new TestGraphNullDefaultValue());
            AssertSerializationFail<TestGraphWrongDefaultValue, TypeInitializationException>(new TestGraphWrongDefaultValue());
            AssertSerializationFail<TestGraphNotSupportedType, TypeInitializationException>(new TestGraphNotSupportedType());

            AssertSerializationFail<TestGraphObjectDefaultValue, NotSupportedException>(new TestGraphObjectDefaultValue());
            AssertSerializationFail<TestGraphNotSupportedType2, NotSupportedException>(new TestGraphNotSupportedType2());
            AssertSerializationFail<TestGraphNotSupportedType3, NotSupportedException>(new TestGraphNotSupportedType3());
            AssertSerializationFail<TestGraphWrongDefaultValue2, NotSupportedException>(new TestGraphWrongDefaultValue2());

            #region Local function

            void AssertSerializationFail<TGraph, TException>(TGraph g)
                where TGraph : IMutableVertexAndEdgeListGraph<TestVertex, TestEdge>
                where TException : Exception
            {
                Assert.Throws<TException>(
                    () => g.SerializeToGraphML<TestVertex, TestEdge, TGraph>(
                        WriteThrowsTestFilePath,
                        vertex => vertex.ID,
                        edge => edge.ID));
            }

            #endregion
        }

        #endregion

        #region Deserialization

        [NotNull]
        private const string TestGraphFileName = "DCT8.graphml";

        [NotNull]
        private const string MissingAttributeTestGraphFileName = "DCT8_with_missing_attribute.graphml";

        [NotNull]
        private const string MissingSourceTestGraphFileName = "DCT8_with_missing_source_id.graphml";

        [NotNull]
        private const string MissingTargetTestGraphFileName = "DCT8_with_missing_target_id.graphml";

        [NotNull]
        private const string InvalidTagTestGraphFileName = "DCT8_invalid_tag.graphml";

        [NotNull]
        private const string MissingGraphMLTestGraphFileName = "DCT8_missing_graphml_tag.graphml";

        [NotNull]
        private const string MissingGraphTestGraphFileName = "DCT8_missing_graph_tag.graphml";

        [Test]
        public void DeserializeFromGraphML()
        {
            foreach (string graphMLFilePath in TestGraphFactory.GetGraphMLFilePaths())
            {
                var graph = new AdjacencyGraph<string, IEdge<string>>();
                using (var reader = new StreamReader(graphMLFilePath))
                {
                    graph.DeserializeFromGraphML(
                        reader,
                        id => id,
                        (source, target, _) => Edge.Create(source, target));
                }

                var vertices = new Dictionary<string, string>();
                foreach (string vertex in graph.Vertices)
                    vertices.Add(vertex, vertex);

                // Check all nodes are loaded
#if SUPPORTS_XML_DTD_PROCESSING
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    XmlResolver = new GraphMLXmlResolver(),
                    ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
                };

                using (var reader = XmlReader.Create(graphMLFilePath, settings))
                {
#else
                using (var reader = new XmlTextReader(graphMLFilePath))
                {
                    reader.ProhibitDtd = false;
                    reader.XmlResolver = null;
#endif
                    var document = new XPathDocument(reader);

                    foreach (XPathNavigator node in document.CreateNavigator().Select("/graphml/graph/node"))
                    {
                        string id = node.GetAttribute("id", "");
                        Assert.IsTrue(vertices.ContainsKey(id));
                    }

                    // Check all edges are loaded
                    foreach (XPathNavigator node in document.CreateNavigator().Select("/graphml/graph/edge"))
                    {
                        string source = node.GetAttribute("source", "");
                        string target = node.GetAttribute("target", "");
                        Assert.IsTrue(graph.ContainsEdge(vertices[source], vertices[target]));
                    }
                }
            }
        }

        [Test]
        public void DeserializeFromGraphML_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // Filepath
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).DeserializeFromGraphML(
                    GetGraphFilePath(TestGraphFileName),
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            var graph = new AdjacencyGraph<string, IEdge<string>>();
            Assert.Throws<ArgumentException>(
                () => graph.DeserializeFromGraphML(
                    (string)null,
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            Assert.Throws<ArgumentException>(
                () => graph.DeserializeFromGraphML(
                    "",
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            Assert.Throws<ArgumentNullException>(
                () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GetGraphFilePath(TestGraphFileName),
                    null,
                    (source, target, _) => Edge.Create(source, target)));

            Assert.Throws<ArgumentNullException>(
                () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                    GetGraphFilePath(TestGraphFileName),
                    id => id,
                    null));

            // Text reader
            Assert.Throws<ArgumentNullException>(
                () => graph.DeserializeFromGraphML(
                    (TextReader)null,
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            using (var reader = new StreamReader(GetGraphFilePath(TestGraphFileName)))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<string, IEdge<string>>)null).DeserializeFromGraphML(
                        reader,
                        id => id,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        null,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        id => id,
                        null));
            }

            // XML reader
            Assert.Throws<ArgumentNullException>(
                () => graph.DeserializeFromGraphML(
                    (XmlReader)null,
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            using (var reader = XmlReader.Create(GetGraphFilePath(TestGraphFileName)))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<string, IEdge<string>>)null).DeserializeFromGraphML(
                        reader,
                        id => id,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        null,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        id => id,
                        null));
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void DeserializeFromGraphML_Throws_InvalidData()
        {
            AssertDeserializationFail<TestGraphArrayDefaultValue, TypeInitializationException>(new TestGraphArrayDefaultValue());
            AssertDeserializationFail<TestGraphNoSetter, TypeInitializationException>(new TestGraphNoSetter());
            AssertDeserializationFail<TestGraphNullDefaultValue, TypeInitializationException>(new TestGraphNullDefaultValue());
            AssertDeserializationFail<TestGraphWrongDefaultValue, TypeInitializationException>(new TestGraphWrongDefaultValue());
            AssertDeserializationFail<TestGraphNotSupportedType, TypeInitializationException>(new TestGraphNotSupportedType());

            var graph = new TestGraph();
            AssertDeserializationFail<TestGraph, ArgumentException>(graph, MissingAttributeTestGraphFileName);
            AssertDeserializationFail<TestGraph, ArgumentException>(graph, MissingSourceTestGraphFileName);
            AssertDeserializationFail<TestGraph, ArgumentException>(graph, MissingTargetTestGraphFileName);
            AssertDeserializationFail<TestGraph, InvalidOperationException>(graph, InvalidTagTestGraphFileName);
            AssertDeserializationFail<TestGraph, InvalidOperationException>(graph, MissingGraphTestGraphFileName);
            AssertDeserializationFail<TestGraph, InvalidOperationException>(graph, MissingGraphMLTestGraphFileName);

            #region Local function

            void AssertDeserializationFail<TGraph, TException>(TGraph g, string fileName = TestGraphFileName)
                where TGraph : IMutableVertexAndEdgeListGraph<TestVertex, TestEdge>
                where TException : Exception
            {
                Assert.Throws<TException>(
                    () => g.DeserializeFromGraphML(
                        GetGraphFilePath(fileName),
                        id => new TestVertex(id),
                        (source, target, id) => new TestEdge(source, target, id)));
            }

            #endregion
        }

        #region Test class

        private class TestClass
        {
            [UsedImplicitly]
            public char Char { get; }
        }

        #endregion

        [Test]
        public void EmitValue_Throws()
        {
            var method = new DynamicMethod("TestMethod", typeof(void), Type.EmptyTypes);
            ILGenerator generator = method.GetILGenerator();
            Assert.Throws<NotSupportedException>(
                () => ILHelpers.EmitValue(
                    generator,
                    typeof(TestClass).GetProperty(nameof(TestClass.Char)) ?? throw new AssertionException("Property must exist."),
                    'a'));
        }

#if SUPPORTS_XML_DTD_PROCESSING
        #region Test data

        [NotNull]
        private static readonly bool[] Bools = { true, false, true, true };

        [NotNull]
        private static readonly int[] Ints = { 2, 3, 45, 3, 44, -2, 3, 5, 99999999 };

        [NotNull]
        private static readonly long[] Longs = { 3, 4, 43, 999999999999999999L, 445, 55, 3, 98, 49789238740598170, 987459, 97239, 234245, 0, -2232 };

        [NotNull]
        private static readonly float[] Floats = { 3.14159265F, 1.1F, 1, 23, -2, 987459, 97239, 234245, 0, -2232, 234.55345F };

        [NotNull]
        private static readonly double[] Doubles = { 3.14159265, 1.1, 1, 23, -2, 987459, 97239, 234245, 0, -2232, 234.55345 };

        [NotNull, ItemNotNull]
        private static readonly string[] Strings = { "", "Quick", "", "brown", "fox", "jumps", "over", "the", "lazy", "dog", ".", "" };


        [NotNull]
        private static readonly IList<bool> BoolsList = new[] { true, false, true, true };

        [NotNull]
        private static readonly IList<int> IntsList = new[] { 2, 3, 45, 3, 44, -2, 3, 5, 99999999 };

        [NotNull]
        private static readonly IList<long> LongsList = new[] { 3, 4, 43, 999999999999999999L, 445, 55, 3, 98, 49789238740598170, 987459, 97239, 234245, 0, -2232 };

        [NotNull]
        private static readonly IList<float> FloatsList = new[] { 3.14159265F, 1.1F, 1, 23, -2, 987459, 97239, 234245, 0, -2232, 234.55345F };

        [NotNull]
        private static readonly IList<double> DoublesList = new[] { 3.14159265, 1.1, 1, 23, -2, 987459, 97239, 234245, 0, -2232, 234.55345 };

        [NotNull, ItemNotNull]
        private static readonly IList<string> StringsList = new[] { "", "Quick", "", "brown", "fox", "jumps", "over", "the", "lazy", "dog", ".", "" };

        #endregion

        #region Test helpers

        [Pure]
        [NotNull]
        private static string SerializeGraph1([NotNull] TestGraph graph)
        {
            string filePath = Path.Combine(
                GetTemporaryTestDirectory(),
                $"serialization_to_graphml_test_{Guid.NewGuid()}.graphml");

            graph.SerializeToGraphML<TestVertex, TestEdge, TestGraph>(filePath);
            Assert.IsTrue(File.Exists(filePath));
            return File.ReadAllText(filePath);
        }

        [Pure]
        [NotNull]
        private static string SerializeGraph2([NotNull] TestGraph graph)
        {
            string filePath = Path.Combine(
                GetTemporaryTestDirectory(),
                $"serialization_to_graphml_test_{Guid.NewGuid()}.graphml");

            graph.SerializeToGraphML<TestVertex, TestEdge, TestGraph>(
                filePath,
                vertex => vertex.ID,
                edge => edge.ID);
            Assert.IsTrue(File.Exists(filePath));
            return File.ReadAllText(filePath);
        }

        [Pure]
        [NotNull]
        private static string SerializeGraph3([NotNull] TestGraph graph)
        {
            using (var writer = new StringWriter())
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    graph.SerializeToGraphML<TestVertex, TestEdge, TestGraph>(xmlWriter);
                }

                return writer.ToString();
            }
        }

        [Pure]
        [NotNull]
        private static string SerializeGraph4([NotNull] TestGraph graph)
        {
            using (var writer = new StringWriter())
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    graph.SerializeToGraphML<TestVertex, TestEdge, TestGraph>(
                        xmlWriter,
                        vertex => vertex.ID,
                        edge => edge.ID);
                }

                return writer.ToString();
            }
        }

        [Pure]
        [NotNull]
        private static TestGraph DeserializeGraph([NotNull] string graphml)
        {
            using (var reader = new StringReader(graphml))
            {
                var serializedGraph = new TestGraph();
                serializedGraph.DeserializeAndValidateFromGraphML(
                    reader,
                    id => new TestVertex(id),
                    (source, target, id) => new TestEdge(source, target, id));
                return serializedGraph;
            }
        }

        [Pure]
        [NotNull]
        private static TestGraph VerifySerialization(
            [NotNull] TestGraph graph,
            [NotNull, InstantHandle] Func<TestGraph, string> serializeGraph)
        {
            return VerifyDoubleSerialization(graph, serializeGraph, DeserializeGraph);
        }

        #endregion

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GraphSerializationTestCases
        {
            [UsedImplicitly]
            get
            {
                // Second parameter is used to know if serialization
                // will keep objects Ids or use other ones

                Func<TestGraph, string> serialize1 = SerializeGraph1;
                yield return new TestCaseData(serialize1, false);

                Func<TestGraph, string> serialize2 = SerializeGraph2;
                yield return new TestCaseData(serialize2, true);

                Func<TestGraph, string> serialize3 = SerializeGraph3;
                yield return new TestCaseData(serialize3, false);

                Func<TestGraph, string> serialize4 = SerializeGraph4;
                yield return new TestCaseData(serialize4, true);
            }
        }

        [TestCaseSource(nameof(GraphSerializationTestCases))]
        public void GraphMLSerializationWithValidation_WriteVertex(
            [NotNull, InstantHandle] Func<TestGraph, string> serializeGraph,
            bool _)
        {
            var graph = new TestGraph
            {
                Bool = true,
                Double = 1.0,
                Float = 2.0F,
                Int = 10,
                Long = 100,
                String = "foo",
                BoolArray = Bools,
                IntArray = Ints,
                LongArray = Longs,
                FloatArray = Floats,
                DoubleArray = Doubles,
                StringArray = Strings,
                NullArray = null,
                BoolIList = BoolsList,
                IntIList = IntsList,
                LongIList = LongsList,
                FloatIList = FloatsList,
                DoubleIList = DoublesList,
                StringIList = StringsList
            };

            var vertex1 = new TestVertex("v1")
            {
                StringDefault = "bar",
                String = "string",
                Long = 20,
                Float = 25.0F,
                Double = 30.0,
                Bool = true,
                IntArray = new[] { 1, 2, 3, 4 },
                IntIList = new[] { 4, 5, 6, 7 }
            };

            var vertex2 = new TestVertex("v2")
            {
                StringDefault = null,
                String = "",
                Int = 42,
                IntArray = null
            };

            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);

            TestGraph serializedGraph = VerifySerialization(graph, serializeGraph);

            Assert.AreEqual(graph.Bool, serializedGraph.Bool);
            Assert.AreEqual(graph.Double, serializedGraph.Double);
            Assert.AreEqual(graph.Float, serializedGraph.Float);
            Assert.AreEqual(graph.Int, serializedGraph.Int);
            Assert.AreEqual(graph.Long, serializedGraph.Long);
            Assert.AreEqual(graph.String, serializedGraph.String);
            CollectionAssert.AreEqual(graph.BoolArray, serializedGraph.BoolArray);
            CollectionAssert.AreEqual(graph.IntArray, serializedGraph.IntArray);
            CollectionAssert.AreEqual(graph.LongArray, serializedGraph.LongArray);
            CollectionAssert.AreEqual(graph.StringArray, serializedGraph.StringArray);
            CollectionAssert.AreEqual(graph.FloatArray, serializedGraph.FloatArray, new FloatComparer(0.001F));
            CollectionAssert.AreEqual(graph.DoubleArray, serializedGraph.DoubleArray, new DoubleComparer(0.0001));
            CollectionAssert.AreEqual(graph.BoolIList, serializedGraph.BoolIList);
            CollectionAssert.AreEqual(graph.IntIList, serializedGraph.IntIList);
            CollectionAssert.AreEqual(graph.LongIList, serializedGraph.LongIList);
            CollectionAssert.AreEqual(graph.StringIList, serializedGraph.StringIList);
            CollectionAssert.AreEqual(graph.FloatIList, serializedGraph.FloatIList, new FloatComparer(0.001F));
            CollectionAssert.AreEqual(graph.DoubleIList, serializedGraph.DoubleIList, new DoubleComparer(0.0001));

            TestVertex serializedVertex1 = serializedGraph.Vertices.First();
            Assert.AreEqual(vertex1.StringDefault, serializedVertex1.StringDefault);
            Assert.AreEqual(vertex1.String, serializedVertex1.String);
            Assert.AreEqual(vertex1.Int, serializedVertex1.Int);
            Assert.AreEqual(vertex1.Long, serializedVertex1.Long);
            Assert.AreEqual(vertex1.Float, serializedVertex1.Float);
            Assert.AreEqual(vertex1.Double, serializedVertex1.Double);
            Assert.AreEqual(vertex1.Bool, serializedVertex1.Bool);
            CollectionAssert.AreEqual(vertex1.IntArray, serializedVertex1.IntArray);
            CollectionAssert.AreEqual(vertex1.IntIList, serializedVertex1.IntIList);

            TestVertex serializedVertex2 = serializedGraph.Vertices.Last();
            Assert.AreEqual(vertex2.StringDefault, serializedVertex2.StringDefault);
            Assert.AreEqual(vertex2.String, serializedVertex2.String);
            Assert.AreEqual(vertex2.Int, serializedVertex2.Int);
            Assert.AreEqual(vertex2.Long, serializedVertex2.Long);
            Assert.AreEqual(vertex2.Float, serializedVertex2.Float);
            Assert.AreEqual(vertex2.Double, serializedVertex2.Double);
            Assert.AreEqual(vertex2.Bool, serializedVertex2.Bool);
            CollectionAssert.AreEqual(vertex2.IntArray, serializedVertex2.IntArray);
            CollectionAssert.AreEqual(vertex2.IntIList, serializedVertex2.IntIList);
        }

        [TestCaseSource(nameof(GraphSerializationTestCases))]
        public void GraphMLSerializationWithValidation_WriteEdge(
            [NotNull, InstantHandle] Func<TestGraph, string> serializeGraph,
            bool keepIds)
        {
            var graph = new TestGraph
            {
                Bool = true,
                Double = 1.0,
                Float = 2.0F,
                Int = 10,
                Long = 100,
                String = "foo"
            };

            var vertex1 = new TestVertex("v1")
            {
                StringDefault = "foo",
                String = "string",
                Int = 10,
                Long = 20,
                Float = 25.0F,
                Double = 30.0,
                Bool = true
            };

            var vertex2 = new TestVertex("v2")
            {
                StringDefault = "bar",
                String = "string2",
                Int = 110,
                Long = 120,
                Float = 125.0F,
                Double = 130.0,
                Bool = true
            };

            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);

            var edge1 = new TestEdge(
                vertex1, vertex2,
                "e1",
                "edge",
                90,
                100,
                25.0F,
                110.0,
                true);

            var edge2 = new TestEdge(vertex1, vertex2, "e2")
            {
                Float = 13.4f,
                Long = 42L
            };

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);

            TestGraph serializedGraph = VerifySerialization(graph, serializeGraph);

            TestEdge serializedEdge1 = serializedGraph.Edges.First();
            if (keepIds)
            {
                Assert.AreEqual(edge1.ID, serializedEdge1.ID);
            }
            Assert.AreEqual(edge1.String, serializedEdge1.String);
            Assert.AreEqual(edge1.Int, serializedEdge1.Int);
            Assert.AreEqual(edge1.Long, serializedEdge1.Long);
            Assert.AreEqual(edge1.Float, serializedEdge1.Float);
            Assert.AreEqual(edge1.Double, serializedEdge1.Double);
            Assert.AreEqual(edge1.Bool, serializedEdge1.Bool);

            TestEdge serializedEdge2 = serializedGraph.Edges.Last();
            if (keepIds)
            {
                Assert.AreEqual(edge2.ID, serializedEdge2.ID);
            }
            Assert.AreEqual(edge2.String, serializedEdge2.String);
            Assert.AreEqual(edge2.Int, serializedEdge2.Int);
            Assert.AreEqual(edge2.Long, serializedEdge2.Long);
            Assert.AreEqual(edge2.Float, serializedEdge2.Float);
            Assert.AreEqual(edge2.Double, serializedEdge2.Double);
            Assert.AreEqual(edge2.Bool, serializedEdge2.Bool);
        }

        [Test]
        public void DeserializeAndValidateFromGraphML_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<string, IEdge<string>>();

            // Text reader
            Assert.Throws<ArgumentNullException>(
                () => graph.DeserializeAndValidateFromGraphML(
                    null,
                    id => id,
                    (source, target, _) => Edge.Create(source, target)));

            using (var reader = new StreamReader(GetGraphFilePath(TestGraphFileName)))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<string, IEdge<string>>)null).DeserializeAndValidateFromGraphML(
                        reader,
                        id => id,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeAndValidateFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        null,
                        (source, target, _) => Edge.Create(source, target)));

                Assert.Throws<ArgumentNullException>(
                    () => graph.DeserializeAndValidateFromGraphML<string, IEdge<string>, AdjacencyGraph<string, IEdge<string>>>(
                        reader,
                        id => id,
                        null));
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }
#endif

        #endregion

        #region Serialization/Deserialization

        #region Test Helpers

        [Pure]
        private static int DeserializeVertex_Simple([NotNull] string id)
        {
            return int.Parse(id);
        }

        [Pure]
        [NotNull]
        private static EquatableEdge<int> DeserializeEdge_Simple(int source, int target, [NotNull] string id)
        {
            return new EquatableEdge<int>(source, target);
        }

        [Pure]
        private static SEquatableEdge<int> DeserializeSEdge_Simple(int source, int target, [NotNull] string id)
        {
            return new SEquatableEdge<int>(source, target);
        }

        [Pure]
        [NotNull]
        private static EquatableTestVertex DeserializeVertex_Complex([NotNull] string id)
        {
            return new EquatableTestVertex(id);
        }

        [Pure]
        [NotNull]
        private static EquatableTestEdge DeserializeEdge_Complex([NotNull] EquatableTestVertex source, [NotNull] EquatableTestVertex target, [NotNull] string id)
        {
            return new EquatableTestEdge(source, target, id);
        }

        [Pure]
        private static SEquatableEdge<EquatableTestVertex> DeserializeSEdge_Complex([NotNull] EquatableTestVertex source, [NotNull] EquatableTestVertex target, [NotNull] string id)
        {
            return new SEquatableEdge<EquatableTestVertex>(source, target);
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize<TVertex, TInEdge, TOutEdge, TInGraph, TOutGraph>(
            [NotNull] TInGraph graph,
            [NotNull, InstantHandle] VertexIdentity<TVertex> vertexIdentity,
            [NotNull, InstantHandle] Func<XmlReader, TOutGraph> deserialize)
            where TInEdge : IEdge<TVertex>, IEquatable<TInEdge>
            where TOutEdge : IEdge<TVertex>, IEquatable<TOutEdge>
            where TInGraph : IEdgeListGraph<TVertex, TInEdge>
            where TOutGraph : IEdgeListGraph<TVertex, TOutEdge>
        {
            Assert.IsNotNull(graph);

            using (var stream = new MemoryStream())
            {
                // Serialize
                using (var writer = XmlWriter.Create(stream))
                {
                    graph.SerializeToGraphML(
                        writer,
                        vertexIdentity,
                        graph.GetEdgeIdentity());
                }

                stream.Position = 0;

                // Deserialize
                using (var reader = XmlReader.Create(stream))
                {
                    TOutGraph deserializedGraph = deserialize(reader);
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
                deserialize);
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, EquatableEdge<int>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, EquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<EquatableEdge<int>, EquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Simple,
                    DeserializeEdge_Simple);
                return deserializedGraph;
            });
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, EquatableTestEdge>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, EquatableTestEdge>, new()
        {
            return SerializeDeserialize_Complex<EquatableTestEdge, EquatableTestEdge, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Complex,
                    DeserializeEdge_Complex);
                return deserializedGraph;
            });
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_SEdge_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, SEquatableEdge<int>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, SEquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<SEquatableEdge<int>, SEquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Simple,
                    DeserializeSEdge_Simple);
                return deserializedGraph;
            });
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_SEdge_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>, new()
        {
            return SerializeDeserialize_Complex<SEquatableEdge<EquatableTestVertex>, SEquatableEdge<EquatableTestVertex>, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Complex,
                    DeserializeSEdge_Complex);
                return deserializedGraph;
            });
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Reversed_Simple<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<int, SReversedEdge<int, EquatableEdge<int>>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<int, EquatableEdge<int>>, new()
        {
            return SerializeDeserialize_Simple<SReversedEdge<int, EquatableEdge<int>>, EquatableEdge<int>, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Simple,
                    DeserializeEdge_Simple);
                return deserializedGraph;
            });
        }

        [Pure]
        [NotNull]
        private static TOutGraph SerializeDeserialize_Reversed_Complex<TInGraph, TOutGraph>([NotNull] TInGraph graph)
            where TInGraph : IEdgeListGraph<EquatableTestVertex, SReversedEdge<EquatableTestVertex, EquatableTestEdge>>
            where TOutGraph : class, IMutableVertexAndEdgeSet<EquatableTestVertex, EquatableTestEdge>, new()
        {
            return SerializeDeserialize_Complex<SReversedEdge<EquatableTestVertex, EquatableTestEdge>, EquatableTestEdge, TInGraph, TOutGraph>(graph, reader =>
            {
                var deserializedGraph = new TOutGraph();
                deserializedGraph.DeserializeFromGraphML(
                    reader,
                    DeserializeVertex_Complex,
                    DeserializeEdge_Complex);
                return deserializedGraph;
            });
        }

        #endregion

        // TODO Support heterogeneous types for vertices and edges

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphTestCases))]
        public void GraphMLSerialization_AdjacencyGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize_Simple<AdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayAdjacencyGraph<int, EquatableEdge<int>>(graph);
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize_Simple<ArrayAdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_AdjacencyGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
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
        public void GraphMLSerialization_AdapterGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<int, EquatableEdge<int>>(graph);
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<BidirectionalAdapterGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_AdapterGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphTestCases))]
        public void GraphMLSerialization_ClusteredGraph_Simple([NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<ClusteredAdjacencyGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_ClusteredGraph_Complex([NotNull] ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphTestCases))]
        public void GraphMLSerialization_CompressedGraph_Simple([NotNull] CompressedSparseRowGraph<int> graph)
        {
            AdjacencyGraph<int, SEquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_SEdge_Simple<CompressedSparseRowGraph<int>, AdjacencyGraph<int, SEquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_CompressedGraph_Complex([NotNull] CompressedSparseRowGraph<EquatableTestVertex> graph)
        {
            AdjacencyGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>> deserializedGraph =
                SerializeDeserialize_SEdge_Complex<CompressedSparseRowGraph<EquatableTestVertex>, AdjacencyGraph<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphTestCases))]
        public void GraphMLSerialization_BidirectionalGraph_Simple([NotNull] BidirectionalGraph<int, EquatableEdge<int>> graph)
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

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_BidirectionalGraph_Complex([NotNull] BidirectionalGraph<EquatableTestVertex, EquatableTestEdge> graph)
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
                        // TODO SReversedEdge<T> original edge is not parsed by the serializer so data are not serialized
                        (edge1, edge2) => Equals(edge1.Source, edge2.Target) && Equals(edge1.Target, edge2.Source)/* && edge1.DataContentEquals(edge2)*/,
                        edge => edge.GetHashCode())));

            var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            UndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph4 =
                SerializeDeserialize_Complex<UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>, UndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(undirectedBidirectionalGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(undirectedBidirectionalGraph, deserializedGraph4));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalMatrixGraphTestCases))]
        public void GraphMLSerialization_BidirectionalMatrixGraph([NotNull] BidirectionalMatrixGraph<EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<BidirectionalMatrixGraph<EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphTestCases))]
        public void GraphMLSerialization_UndirectedGraph_Simple([NotNull] UndirectedGraph<int, EquatableEdge<int>> graph)
        {
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize_Simple<UndirectedGraph<int, EquatableEdge<int>>, UndirectedGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayUndirectedGraph<int, EquatableEdge<int>>(graph);
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize_Simple<ArrayUndirectedGraph<int, EquatableEdge<int>>, UndirectedGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_UndirectedGraph_Complex([NotNull] UndirectedGraph<EquatableTestVertex, EquatableTestEdge> graph)
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
        public void GraphMLSerialization_EdgeListGraph_Simple([NotNull] EdgeListGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize_Simple<EdgeListGraph<int, EquatableEdge<int>>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationEdgeListGraphComplexNotHeterogeneousTestCases))]
        public void GraphMLSerialization_EdgeListGraph_Complex([NotNull] EdgeListGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize_Complex<EdgeListGraph<EquatableTestVertex, EquatableTestEdge>, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        #endregion
    }
}