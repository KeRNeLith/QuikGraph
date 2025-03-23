using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Tests relative to XML serialization of some serializable structures.
    /// </summary>
    [TestFixture]
    internal sealed class XmlSerializableStructuresTests
    {
        [NotNull]
        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        #region Test helpers

        private static void SerializeAndRead<TVertex, TEdge, TGraph>(
            [NotNull] XmlSerializableGraph<TVertex, TEdge, TGraph> graph,
            [NotNull, InstantHandle] Action<string> checkSerializedContent)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>, new()
        {
            using (var memory = new MemoryStream())
            {
                var writer = new StreamWriter(memory);
                try
                {
                    var serializer = new XmlSerializer(graph.GetType());
                    serializer.Serialize(writer, graph);

                    memory.Position = 0;

                    using (var reader = new StreamReader(memory))
                    {
                        checkSerializedContent(reader.ReadToEnd());
                    }
                }
                finally
                {
                    writer.Dispose();
                }
            }
        }

        #endregion

        [Test]
        public void SerializeToXml_Empty()
        {
            var emptyGraph = new XmlSerializableGraph<Person, XmlSerializableEdge<Person>, AdjacencyGraph<Person, XmlSerializableEdge<Person>>>();

            SerializeAndRead(
                emptyGraph,
                content =>
                {
                    var regex = new Regex($@"{Regex.Escape(XmlHeader)}\s*<graph\s*.*?\s*>\s*<vertices\s*\/>\s*<edges\s*\/>\s*<\/graph>");
                    Assert.IsTrue(regex.Match(content).Success);
                });
        }

        [Test]
        public void SerializeToXml()
        {
            var wrappedGraph = new AdjacencyGraph<int, XmlSerializableEdge<int>>();
            wrappedGraph.AddVerticesAndEdgeRange(
                new XmlSerializableEdge<int> { Source = 1, Target = 2 },
                new XmlSerializableEdge<int> { Source = 1, Target = 3 },
                new XmlSerializableEdge<int> { Source = 2, Target = 2 }
            );
            wrappedGraph.AddVertex(4);

            var graph = new XmlSerializableGraph<int, XmlSerializableEdge<int>, AdjacencyGraph<int, XmlSerializableEdge<int>>>(wrappedGraph);
            SerializeAndRead(
                graph,
                content =>
                {
                    var graphContent = new StringBuilder();

                    graphContent.Append(@"<vertices>\s*");
                    foreach (int vertex in graph.Vertices)
                    {
                        graphContent.Append(
                            $@"<vertex>{vertex}<\/vertex>\s*");
                    }
                    graphContent.Append(@"<\/vertices>\s*");

                    graphContent.Append(@"<edges>\s*");
                    foreach (XmlSerializableEdge<int> edge in graph.Edges)
                    {
                        graphContent.Append(
                            $@"<edge>\s*<Source>{edge.Source}<\/Source>\s*<Target>{edge.Target}<\/Target>\s*<\/edge>\s*");
                    }
                    graphContent.Append(@"<\/edges>");

                    var regex = new Regex(
                        $@"{Regex.Escape(XmlHeader)}\s*<graph\s*.*?\s*>\s*{graphContent}\s*<\/graph>");
                    Assert.IsTrue(regex.Match(content).Success);
                });
        }

        [Test]
        public void XmlSerializableGraph()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            var graph = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int,IEdge<int>>>(wrappedGraph);

            CollectionAssert.IsEmpty(graph.Vertices);
            CollectionAssert.IsEmpty(graph.Edges);

            var vertices = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlVertexList(wrappedGraph)
            {
                1, 2
            };
            graph.Vertices = vertices;

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                graph.Vertices);
            CollectionAssert.IsEmpty(graph.Edges);

            var edge12 = Edge.Create(1, 2);
            var edge22 = Edge.Create(2, 2);
            var edges = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(wrappedGraph)
            {
                edge12, edge22
            };
            graph.Edges = edges;

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                graph.Vertices);
            CollectionAssert.AreEqual(
                new[] { edge12, edge22 },
                graph.Edges);
        }

        [Test]
        public void XmlSerializableGraph_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(null));
        }

        [Test]
        public void XmlVertexList()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var vertexList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlVertexList(graph);

            CollectionAssert.IsEmpty(vertexList);

            var edge12 = Edge.Create(1, 2);
            var edge22 = Edge.Create(2, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge22 );

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                vertexList);

            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange( edge12, edge22 );
            vertexList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlVertexList(graph);

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                vertexList);
        }

        [Test]
        public void XmlVertexList_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlVertexList(null));
        }

        [Test]
        public void XmlVertexList_Add()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var vertexList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlVertexList(graph);

            CollectionAssert.IsEmpty(vertexList);

            vertexList.Add(1);

            CollectionAssert.AreEqual(
                new[] { 1 },
                vertexList);

            vertexList.Add(2);

            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                vertexList);
        }

        [Test]
        public void XmlVertexList_Add_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var vertexList = new XmlSerializableGraph<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>.XmlVertexList(graph);
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => vertexList.Add(null));
        }

        [Test]
        public void XmlEdgeList()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var edgeList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(graph);

            CollectionAssert.IsEmpty(edgeList);

            var edge12 = Edge.Create(1, 2);
            var edge22 = Edge.Create(2, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge22 );

            CollectionAssert.AreEqual(
                new[] { edge12, edge22 },
                edgeList);

            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange( edge12, edge22 );
            edgeList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(graph);

            CollectionAssert.AreEqual(
                new[] { edge12, edge22 },
                edgeList);
        }

        [Test]
        public void XmlEdgeList_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(null));
        }

        [Test]
        public void XmlEdgeList_Add()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var edgeList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(graph);
            
            CollectionAssert.IsEmpty(edgeList);

            var edge12 = Edge.Create(1, 2);
            edgeList.Add(edge12);

            CollectionAssert.AreEqual(
                new[] { edge12 },
                edgeList);

            var edge22 = Edge.Create(2, 2);
            edgeList.Add(edge22);

            CollectionAssert.AreEqual(
                new[] { edge12, edge22 },
                edgeList);
        }

        [Test]
        public void XmlEdgeList_Add_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var edgeList = new XmlSerializableGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>.XmlEdgeList(graph);
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => edgeList.Add(null));
        }
    }
}