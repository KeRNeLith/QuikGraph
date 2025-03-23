using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Serialization.DirectedGraphML;
using QuikGraph.Tests;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Tests for <see cref="DirectedGraphMLExtensions"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DirectedGraphMLExtensionsTests
    {
        #region Test helpers

        private static void SerializeAndRead(
            [NotNull] DirectedGraph graph,
            [NotNull, InstantHandle] Func<DirectedGraph, string> onSerialize,
            [NotNull, InstantHandle] Action<string> checkSerializedContent)
        {
            string xml = onSerialize(graph);
            checkSerializedContent(xml);
        }

        private static void AssertGraphContentEquivalent<TEdge>(
            [NotNull] IEdgeListGraph<string, TEdge> graph,
            [NotNull] DirectedGraph directedGraph)
            where TEdge : IEdge<string>
        {
            // Vertices
            Assert.AreEqual(graph.VertexCount, directedGraph.Nodes.Length);

            string[] expectedNodes = graph.Vertices.ToArray();
            for (int i = 0; i < graph.VertexCount; ++i)
            {
                Assert.AreEqual(expectedNodes[i], directedGraph.Nodes[i].Id);
            }

            // Edges
            Assert.AreEqual(graph.EdgeCount, directedGraph.Links.Length);

            TEdge[] expectedEdges = graph.Edges.ToArray();
            for (int i = 0; i < graph.EdgeCount; ++i)
            {
                Assert.AreEqual(expectedEdges[i].Source, directedGraph.Links[i].Source);
                Assert.AreEqual(expectedEdges[i].Target, directedGraph.Links[i].Target);
            }
        }

        #endregion

        [NotNull]
        private const string XmlHeaderRegex = @"<\?xml\ version=""1\.0""(\ encoding=""utf-(8|16)""|)\?>";

        [NotNull]
        private static readonly string WriteThrowsTestFilePath =
            Path.Combine(GetTemporaryTestDirectory(), "serialization_from_directegraph_to_xml_throws_test.xml");

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> WriteXmlTestCases
        {
            [UsedImplicitly]
            get
            {
                Func<DirectedGraph, string> serialize1 = graph =>
                {
                    string filePath = 
                        Path.Combine(GetTemporaryTestDirectory(), "serialization_from_directegraph_to_xml_test.xml");

                    graph.WriteXml(filePath);
                    Assert.IsTrue(File.Exists(filePath));
                    return File.ReadAllText(filePath);
                };
                yield return new TestCaseData(serialize1);

                Func<DirectedGraph, string> serialize2 = graph =>
                {
                    using (var writer = new StringWriter())
                    {
                        var settings = new XmlWriterSettings { Indent = true };
                        using (var xmlWriter = XmlWriter.Create(writer, settings))
                        {
                            graph.WriteXml(xmlWriter);
                        }

                        return writer.ToString();
                    }
                };
                yield return new TestCaseData(serialize2);

                Func<DirectedGraph, string> serialize3 = graph =>
                {
                    using (var writer = new MemoryStream())
                    {
                        graph.WriteXml(writer);

                        writer.Position = 0;

                        using (var reader = new StreamReader(writer))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                };
                yield return new TestCaseData(serialize3);

                Func<DirectedGraph, string> serialize4 = graph =>
                {
                    using (var memory = new MemoryStream())
                    using (var writer = new StreamWriter(memory))
                    {
                        graph.WriteXml(writer);

                        memory.Position = 0;

                        using (var reader = new StreamReader(memory))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                };
                yield return new TestCaseData(serialize4);
            }
        }

        [TestCaseSource(nameof(WriteXmlTestCases))]
        public void WriteXml(
            [NotNull] Func<DirectedGraph, string> onSerialize)
        {
            var graph = new DirectedGraph();
            var persons = new List<DirectedGraphNode>();
            var jacob = new DirectedGraphNode
            {
                Id = "Jacob"
            };
            persons.Add(jacob);

            var john = new DirectedGraphNode
            {
                Id = "John"
            };
            persons.Add(john);

            var jonathon = new DirectedGraphNode
            {
                Id = "Jonathon"
            };
            persons.Add(jonathon);

            var emanuel = new DirectedGraphNode
            {
                Id = "Emanuel"
            };
            persons.Add(emanuel);

            var relations = new List<DirectedGraphLink>
            {
                new DirectedGraphLink
                {
                    Source = jacob.Id,
                    Target = john.Id
                },
                new DirectedGraphLink
                {
                    Source = john.Id,
                    Target = jonathon.Id
                },
                new DirectedGraphLink
                {
                    Source = jonathon.Id,
                    Target = emanuel.Id
                }
            };

            graph.Nodes = persons.ToArray();
            graph.Links = relations.ToArray();

            SerializeAndRead(
                graph,
                onSerialize,
                content =>
                {
                    var graphContent = new StringBuilder();

                    graphContent.Append(@"<Nodes>\s*");
                    foreach (DirectedGraphNode node in graph.Nodes)
                    {
                        graphContent.Append(
                            $@"<Node\s*Id=""{node.Id}""\s*\/>\s*");
                    }
                    graphContent.Append(@"<\/Nodes>\s*");

                    graphContent.Append(@"<Links>\s*");
                    foreach (DirectedGraphLink link in graph.Links)
                    {
                        graphContent.Append(
                            $@"<Link\s*Source=""{link.Source}""\s*Target=""{link.Target}""\s*\/>\s*");
                    }
                    graphContent.Append(@"<\/Links>");

                    var regex = new Regex(
                        $@"{XmlHeaderRegex}\s*<DirectedGraph\s*.*?\s*>\s*{graphContent}\s*<\/DirectedGraph>");
                    Assert.IsTrue(regex.Match(content).Success);
                });
        }

        [Test]
        public void WriteXml_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var directedGraph = new DirectedGraph();

            // Filepath
            Assert.Throws<ArgumentNullException>(
                () => ((DirectedGraph)null).WriteXml(WriteThrowsTestFilePath));
            Assert.Throws<ArgumentException>(
                () => directedGraph.WriteXml((string)null));
            Assert.Throws<ArgumentException>(
                () => directedGraph.WriteXml(""));
            Assert.Throws<ArgumentException>(
                () => ((DirectedGraph)null).WriteXml((string)null));
            Assert.Throws<ArgumentException>(
                () => ((DirectedGraph)null).WriteXml(""));

            // XML writer
            using (var writer = XmlWriter.Create(WriteThrowsTestFilePath))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml(writer));
                Assert.Throws<ArgumentNullException>(
                    () => directedGraph.WriteXml((XmlWriter)null));
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml((XmlWriter)null));
            }

            // Stream
            using (var writer = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml(writer));
                Assert.Throws<ArgumentNullException>(
                    () => directedGraph.WriteXml((Stream)null));
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml((Stream)null));
            }

            // TextWriter
            using (var writer = new StreamWriter(WriteThrowsTestFilePath))
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml(writer));
                Assert.Throws<ArgumentNullException>(
                    () => directedGraph.WriteXml((TextWriter)null));
                Assert.Throws<ArgumentNullException>(
                    () => ((DirectedGraph)null).WriteXml((TextWriter)null));
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void ToDirectedGraphML(AdjacencyGraph<string, IEdge<string>> graph)
        {
            DirectedGraph directedGraph = graph.ToDirectedGraphML();
            Assert.IsNotNull(graph);

            AssertGraphContentEquivalent(graph, directedGraph);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void ToDirectedGraphML_WithIdentity(AdjacencyGraph<string, IEdge<string>> graph)
        {
            int i = 0;
            DirectedGraph directedGraph = graph.ToDirectedGraphML(
                vertex => vertex,
                _ => (++i).ToString());
            Assert.IsNotNull(graph);

            AssertGraphContentEquivalent(graph, directedGraph);
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void ToDirectedGraphML_WithColors(AdjacencyGraph<string, IEdge<string>> graph)
        {
            var random = new Random(123456);
            var verticesColors = graph.Vertices.ToDictionary(
                    vertex => vertex,
                    _ => (GraphColor)random.Next(0, 3));

            DirectedGraph directedGraph = graph.ToDirectedGraphML(
                vertex =>
                {
                    Assert.IsNotNull(vertex);
                    return verticesColors[vertex];
                });
            Assert.IsNotNull(graph);

            AssertGraphContentEquivalent(graph, directedGraph);

            foreach (DirectedGraphNode node in directedGraph.Nodes)
            {
                Assert.AreEqual(
                    ColorToStringColor(verticesColors[node.Id]),
                    node.Background);
            }

            string ColorToStringColor(GraphColor color)
            {
                switch (color)
                {
                    case GraphColor.Black: return "Black";
                    case GraphColor.Gray: return "LightGray";
                    case GraphColor.White: return "White";
                    default: Assert.Fail("Unknown color.");
                        return string.Empty;
                }
            }
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public void ToDirectedGraphML_WithFormat(AdjacencyGraph<string, IEdge<string>> graph)
        {
            int formattedNodes = 0;
            int formattedEdges = 0;
            // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
            DirectedGraph directedGraph = graph.ToDirectedGraphML(
                graph.GetVertexIdentity(),
                graph.GetEdgeIdentity(),
                (vertex, node) =>
                {
                    Assert.IsNotNull(vertex);
                    Assert.IsNotNull(node);
                    ++formattedNodes;
                },
                (edge, link) =>
                {
                    Assert.IsNotNull(edge);
                    Assert.IsNotNull(link);
                    ++formattedEdges;
                });
            // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local
            Assert.IsNotNull(graph);

            Assert.AreEqual(graph.VertexCount, formattedNodes);
            Assert.AreEqual(graph.EdgeCount, formattedEdges);
            AssertGraphContentEquivalent(graph, directedGraph);
        }

        [Test]
        public void ToDirectedGraphML_Throws()
        {
            var graph = new AdjacencyGraph<string, IEdge<string>>();
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML());


            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(_ => GraphColor.Black));
            Assert.Throws<ArgumentNullException>(
                () => graph.ToDirectedGraphML(null));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(null));


            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    vertex => vertex,
                    edge => $"{edge.Source}_{edge.Target}"));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    null,
                    edge => $"{edge.Source}_{edge.Target}"));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    vertex => vertex,
                    null));
            Assert.Throws<ArgumentNullException>(
                () => graph.ToDirectedGraphML(
                    null,
                    null));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    null,
                    null));


            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    vertex => vertex,
                    edge => $"{edge.Source}_{edge.Target}",
                    null, null));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    null,
                    edge => $"{edge.Source}_{edge.Target}",
                    null, null));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    vertex => vertex,
                    null,
                    null, null));
            Assert.Throws<ArgumentNullException>(
                () => graph.ToDirectedGraphML(
                    null,
                    null,
                    null, null));
            Assert.Throws<ArgumentNullException>(
                () => ((AdjacencyGraph<string, IEdge<string>>)null).ToDirectedGraphML(
                    null,
                    null,
                    null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}