using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Graphviz.Dot;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace QuikGraph.Graphviz.Tests
{
    /// <summary>
    /// Tests related to <see cref="GraphvizAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class GraphvizAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var otherGraph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph, GraphvizImageType.Fig);
            AssertAlgorithmProperties(algorithm, graph, GraphvizImageType.Fig);

            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph, GraphvizImageType.Ps);
            AssertAlgorithmProperties(algorithm, graph, GraphvizImageType.Ps);

            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph, GraphvizImageType.Hpgl);
            algorithm.ImageType = GraphvizImageType.Gd;
            AssertAlgorithmProperties(algorithm, graph, GraphvizImageType.Gd);

            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            algorithm.VisitedGraph = otherGraph;
            AssertAlgorithmProperties(algorithm, otherGraph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                GraphvizAlgorithm<TVertex, TEdge> algo,
                IEdgeListGraph<TVertex, TEdge> treatedGraph,
                GraphvizImageType imageType = GraphvizImageType.Png)
                where TEdge : IEdge<TVertex>
            {
                Assert.AreSame(treatedGraph, algo.VisitedGraph);
                Assert.IsNotNull(algo.GraphFormat);
                Assert.IsNotNull(algo.CommonVertexFormat);
                Assert.IsNotNull(algo.CommonEdgeFormat);
                Assert.AreEqual(imageType, algo.ImageType);
                Assert.IsNull(algo.Output);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new GraphvizAlgorithm<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(() => new GraphvizAlgorithm<int, IEdge<int>>(null, GraphvizImageType.Gif));

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm.VisitedGraph = null);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void FormatHandlers()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            algorithm.FormatVertex += NoVertexOnFormatVertex;
            algorithm.FormatEdge += NoEdgeOnFormatEdge;
            algorithm.FormatCluster += NoClusterOnFormatCluster;

            // Empty graph
            algorithm.Generate();

            // Only vertices
            graph.AddVertexRange(1, 2 );
            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            List<int> notFormattedVertices = RegisterOnFormatVertex(algorithm, graph.Vertices);
            algorithm.FormatEdge += NoEdgeOnFormatEdge;
            algorithm.FormatCluster += NoClusterOnFormatCluster;

            algorithm.Generate();

            CollectionAssert.IsEmpty(notFormattedVertices);

            // With edges
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(3, 1)
            );
            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            notFormattedVertices = RegisterOnFormatVertex(algorithm, graph.Vertices);
            List<IEdge<int>> notFormattedEdges = RegisterOnFormatEdge(algorithm, graph.Edges);
            algorithm.FormatCluster += NoClusterOnFormatCluster;

            algorithm.Generate();

            CollectionAssert.IsEmpty(notFormattedVertices);
            CollectionAssert.IsEmpty(notFormattedEdges);

            // With no cluster
            var clusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(graph);
            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(clusteredGraph);
            notFormattedVertices = RegisterOnFormatVertex(algorithm, clusteredGraph.Vertices);
            notFormattedEdges = RegisterOnFormatEdge(algorithm, clusteredGraph.Edges);
            algorithm.FormatCluster += NoClusterOnFormatCluster;

            algorithm.Generate();

            CollectionAssert.IsEmpty(notFormattedVertices);
            CollectionAssert.IsEmpty(notFormattedEdges);

            // With clusters
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph1 = clusteredGraph.AddCluster();
            subGraph1.AddVertexRange(4, 5 );
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph2 = clusteredGraph.AddCluster();
            subGraph2.AddVerticesAndEdge(Edge.Create(1, 6));
            algorithm = new GraphvizAlgorithm<int, IEdge<int>>(clusteredGraph);
            notFormattedVertices = RegisterOnFormatVertex(algorithm, clusteredGraph.Vertices);
            notFormattedEdges = RegisterOnFormatEdge(algorithm, clusteredGraph.Edges);
            List<IVertexAndEdgeListGraph<int, IEdge<int>>> notFormattedClusters = RegisterOnFormatCluster(
                algorithm,
                new[] { subGraph1, subGraph2 });

            algorithm.Generate();

            CollectionAssert.IsEmpty(notFormattedVertices);
            CollectionAssert.IsEmpty(notFormattedEdges);
            CollectionAssert.IsEmpty(notFormattedClusters);

            #region Local functions

            void NoVertexOnFormatVertex(object sender, FormatVertexEventArgs<int> args)
            {
                Assert.Fail($"{nameof(GraphvizAlgorithm<object, Edge<object>>.FormatVertex)} called while no vertex in graph.");
            }

            List<TVertex> RegisterOnFormatVertex<TVertex, TEdge>(GraphvizAlgorithm<TVertex, TEdge> algo, IEnumerable<TVertex> vertices)
                where TEdge : IEdge<TVertex>
            {
                var verticesList = new List<TVertex>(vertices);
                algo.FormatVertex += (_, args) =>
                {
                    Assert.IsTrue(verticesList.Remove(args.Vertex));
                };
                return verticesList;
            }

            void NoEdgeOnFormatEdge(object sender, FormatEdgeEventArgs<int, IEdge<int>> args)
            {
                Assert.Fail($"{nameof(GraphvizAlgorithm<object, Edge<object>>.FormatEdge)} called while no edge in graph.");
            }

            List<TEdge> RegisterOnFormatEdge<TVertex, TEdge>(GraphvizAlgorithm<TVertex, TEdge> algo, IEnumerable<TEdge> edges)
                where TEdge : IEdge<TVertex>
            {
                var edgeList = new List<TEdge>(edges);
                algo.FormatEdge += (_, args) =>
                {
                    Assert.IsTrue(edgeList.Remove(args.Edge));
                };
                return edgeList;
            }

            void NoClusterOnFormatCluster(object sender, FormatClusterEventArgs<int, IEdge<int>> args)
            {
                Assert.Fail($"{nameof(GraphvizAlgorithm<object, Edge<object>>.FormatCluster)} called while no cluster in graph.");
            }

            List<IVertexAndEdgeListGraph<TVertex, TEdge>> RegisterOnFormatCluster<TVertex, TEdge>(GraphvizAlgorithm<TVertex, TEdge> algo, IEnumerable<IVertexAndEdgeListGraph<TVertex, TEdge>> clusters)
                where TEdge : IEdge<TVertex>
            {
                var clusterList = new List<IVertexAndEdgeListGraph<TVertex, TEdge>>(clusters);
                algo.FormatCluster += (_, args) =>
                {
                    Assert.IsTrue(clusterList.Remove(args.Cluster));
                };
                return clusterList;
            }

            #endregion
        }

        [Test]
        public void GenerateSameDot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            // Empty graph
            TestGenerate(graph);

            // Only vertices
            graph.AddVertexRange(1, 2 );
            TestGenerate(graph);

            // With edges
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(3, 1)
            );
            TestGenerate(graph);

            // With no cluster
            var clusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(graph);
            TestGenerate(clusteredGraph);

            // With clusters
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph1 = clusteredGraph.AddCluster();
            subGraph1.AddVertexRange( 4, 5 );
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph2 = clusteredGraph.AddCluster();
            subGraph2.AddVerticesAndEdge(Edge.Create(1, 6));
            TestGenerate(clusteredGraph);

            #region Local function

            void TestGenerate<TVertex, TEdge>(IEdgeListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                var algorithm = new GraphvizAlgorithm<TVertex, TEdge>(g);
                string generatedDot = algorithm.Generate();
                Assert.IsNotEmpty(generatedDot);

                var dotEngine = new TestDotEngine { ExpectedDot = generatedDot };
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                algorithm.Generate(dotEngine, "NotSaved.dot");
            }

            #endregion
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GenerateTestCases
        {
            [UsedImplicitly]
            get
            {
                // Empty graphs
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                yield return new TestCaseData(graph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "}"
                };

                var undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
                yield return new TestCaseData(undirectedGraph)
                {
                    ExpectedResult =
                        "graph G {" + Environment.NewLine
                        + "}"
                };

                // Only vertices
                graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange(1, 2, 3 );
                yield return new TestCaseData(graph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "}"
                };

                undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph.AddVertexRange(1, 2, 3 );
                yield return new TestCaseData(undirectedGraph)
                {
                    ExpectedResult =
                        "graph G {" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "}"
                };

                // With edges
                graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 3),
                    Edge.Create(3, 1)
                );
                yield return new TestCaseData(graph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "0 -> 1;" + Environment.NewLine
                        + "1 -> 2;" + Environment.NewLine
                        + "2 -> 0;" + Environment.NewLine
                        + "}"
                };

                undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph.AddVerticesAndEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 3),
                    Edge.Create(3, 1)
                );
                yield return new TestCaseData(undirectedGraph)
                {
                    ExpectedResult =
                        "graph G {" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "0 -- 1;" + Environment.NewLine
                        + "1 -- 2;" + Environment.NewLine
                        + "2 -- 0;" + Environment.NewLine
                        + "}"
                };

                // With no cluster
                var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
                wrappedGraph.AddVertexRange(1, 2 );
                var clusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
                yield return new TestCaseData(clusteredGraph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "}"
                };

                // With clusters
                wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
                wrappedGraph.AddVertexRange(1, 2 );
                clusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
                ClusteredAdjacencyGraph<int, IEdge<int>> subGraph1 = clusteredGraph.AddCluster();
                subGraph1.AddVerticesAndEdgeRange(
                    Edge.Create(3, 4),
                    Edge.Create(4, 1)
                );
                ClusteredAdjacencyGraph<int, IEdge<int>> subGraph2 = clusteredGraph.AddCluster();
                subGraph2.AddVerticesAndEdge(Edge.Create(1, 5));
                yield return new TestCaseData(clusteredGraph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "subgraph cluster1 {" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "3;" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "2 -> 3;" + Environment.NewLine
                        + "3 -> 0;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster2 {" + Environment.NewLine
                        + "4;" + Environment.NewLine
                        + "0 -> 4;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "}"
                };

                // With clusters (not collapsed and collapsed)
                // Cluster hierarchy
                // Root
                // \-> sub1
                // \-> sub2
                //     \-> nested2_1
                //     \-> nested2_2
                // \-> sub3
                // \-> sub4
                //     \-> nested4_1
                //     \-> nested4_2
                // ReSharper disable InconsistentNaming
                wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
                var rootClusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
                ClusteredAdjacencyGraph<int, IEdge<int>> subClusteredGraph1 = rootClusteredGraph.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> subClusteredGraph2 = rootClusteredGraph.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> nestedSubClusteredGraph2_1 = subClusteredGraph2.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> nestedSubClusteredGraph2_2 = subClusteredGraph2.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> subClusteredGraph3 = rootClusteredGraph.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> subClusteredGraph4 = rootClusteredGraph.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> nestedSubClusteredGraph4_1 = subClusteredGraph4.AddCluster();
                ClusteredAdjacencyGraph<int, IEdge<int>> nestedSubClusteredGraph4_2 = subClusteredGraph4.AddCluster();
                // ReSharper restore InconsistentNaming

                // Fill graphs
                wrappedGraph.AddVerticesAndEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 2)
                );
                wrappedGraph.AddVertex(3);

                subClusteredGraph1.AddVerticesAndEdge(Edge.Create(4, 5));
                subClusteredGraph1.AddVertex(6);

                subClusteredGraph2.AddVerticesAndEdge(Edge.Create(7, 8));
                subClusteredGraph2.AddVertex(9);

                nestedSubClusteredGraph2_1.AddVerticesAndEdge(Edge.Create(10, 11));
                nestedSubClusteredGraph2_1.AddVertex(12);

                nestedSubClusteredGraph2_2.AddVerticesAndEdge(Edge.Create(13, 14));
                nestedSubClusteredGraph2_2.AddVertex(15);

                subClusteredGraph3.AddVerticesAndEdge(Edge.Create(16, 17));
                subClusteredGraph3.AddVertex(18);

                subClusteredGraph4.AddVerticesAndEdge(Edge.Create(19, 20));
                subClusteredGraph4.AddVertex(21);

                nestedSubClusteredGraph4_1.AddVerticesAndEdge(Edge.Create(22, 23));
                nestedSubClusteredGraph4_1.AddVertex(24);

                nestedSubClusteredGraph4_2.AddVerticesAndEdge(Edge.Create(25, 26));
                nestedSubClusteredGraph4_2.AddVertex(27);

                yield return new TestCaseData(rootClusteredGraph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "subgraph cluster1 {" + Environment.NewLine
                        + "3;" + Environment.NewLine
                        + "4;" + Environment.NewLine
                        + "5;" + Environment.NewLine
                        + "3 -> 4;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster2 {" + Environment.NewLine
                        + "subgraph cluster3 {" + Environment.NewLine
                        + "9;" + Environment.NewLine
                        + "10;" + Environment.NewLine
                        + "11;" + Environment.NewLine
                        + "9 -> 10;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster4 {" + Environment.NewLine
                        + "12;" + Environment.NewLine
                        + "13;" + Environment.NewLine
                        + "14;" + Environment.NewLine
                        + "12 -> 13;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "6;" + Environment.NewLine
                        + "7;" + Environment.NewLine
                        + "8;" + Environment.NewLine
                        + "6 -> 7;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster5 {" + Environment.NewLine
                        + "15;" + Environment.NewLine
                        + "16;" + Environment.NewLine
                        + "17;" + Environment.NewLine
                        + "15 -> 16;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster6 {" + Environment.NewLine
                        + "subgraph cluster7 {" + Environment.NewLine
                        + "21;" + Environment.NewLine
                        + "22;" + Environment.NewLine
                        + "23;" + Environment.NewLine
                        + "21 -> 22;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster8 {" + Environment.NewLine
                        + "24;" + Environment.NewLine
                        + "25;" + Environment.NewLine
                        + "26;" + Environment.NewLine
                        + "24 -> 25;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "18;" + Environment.NewLine
                        + "19;" + Environment.NewLine
                        + "20;" + Environment.NewLine
                        + "18 -> 19;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "0 -> 1;" + Environment.NewLine
                        + "1 -> 1;" + Environment.NewLine
                        + "}"
                };

                // Cluster hierarchy
                // Root
                // \-> sub1
                // \-> sub2
                //     \-> nested2_1
                //     \-> nested2_2
                // \-> sub3 (collapsed)
                // \-> sub4 (collapsed)
                //     \-> nested4_1
                //     \-> nested4_2
                wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
                rootClusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
                subClusteredGraph1 = rootClusteredGraph.AddCluster();
                subClusteredGraph2 = rootClusteredGraph.AddCluster();
                nestedSubClusteredGraph2_1 = subClusteredGraph2.AddCluster();
                nestedSubClusteredGraph2_2 = subClusteredGraph2.AddCluster();
                subClusteredGraph3 = rootClusteredGraph.AddCluster();
                subClusteredGraph4 = rootClusteredGraph.AddCluster();
                nestedSubClusteredGraph4_1 = subClusteredGraph4.AddCluster();
                nestedSubClusteredGraph4_2 = subClusteredGraph4.AddCluster();
                // ReSharper restore InconsistentNaming

                // Fill graphs
                wrappedGraph.AddVerticesAndEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 2)
                );
                wrappedGraph.AddVertex(3);

                subClusteredGraph1.AddVerticesAndEdge(Edge.Create(4, 5));
                subClusteredGraph1.AddVertex(6);

                subClusteredGraph2.AddVerticesAndEdge(Edge.Create(7, 8));
                subClusteredGraph2.AddVertex(9);

                nestedSubClusteredGraph2_1.AddVerticesAndEdge(Edge.Create(10, 11));
                nestedSubClusteredGraph2_1.AddVertex(12);

                nestedSubClusteredGraph2_2.AddVerticesAndEdge(Edge.Create(13, 14));
                nestedSubClusteredGraph2_2.AddVertex(15);

                subClusteredGraph3.AddVerticesAndEdge(Edge.Create(16, 17));
                subClusteredGraph3.AddVertex(18);

                subClusteredGraph4.AddVerticesAndEdge(Edge.Create(19, 20));
                subClusteredGraph4.AddVertex(21);

                nestedSubClusteredGraph4_1.AddVerticesAndEdge(Edge.Create(22, 23));
                nestedSubClusteredGraph4_1.AddVertex(24);

                nestedSubClusteredGraph4_2.AddVerticesAndEdge(Edge.Create(25, 26));
                nestedSubClusteredGraph4_2.AddVertex(27);

                // Collapse graphs
                subClusteredGraph3.Collapsed = true;
                subClusteredGraph4.Collapsed = true;
                yield return new TestCaseData(rootClusteredGraph)
                {
                    ExpectedResult =
                        "digraph G {" + Environment.NewLine
                        + "subgraph cluster1 {" + Environment.NewLine
                        + "3;" + Environment.NewLine
                        + "4;" + Environment.NewLine
                        + "5;" + Environment.NewLine
                        + "3 -> 4;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster2 {" + Environment.NewLine
                        + "subgraph cluster3 {" + Environment.NewLine
                        + "9;" + Environment.NewLine
                        + "10;" + Environment.NewLine
                        + "11;" + Environment.NewLine
                        + "9 -> 10;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster4 {" + Environment.NewLine
                        + "12;" + Environment.NewLine
                        + "13;" + Environment.NewLine
                        + "14;" + Environment.NewLine
                        + "12 -> 13;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "6;" + Environment.NewLine
                        + "7;" + Environment.NewLine
                        + "8;" + Environment.NewLine
                        + "6 -> 7;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster5 {" + Environment.NewLine
                        + "15;" + Environment.NewLine
                        + "16;" + Environment.NewLine
                        + "17;" + Environment.NewLine
                        + "15 -> 16;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster6 {" + Environment.NewLine
                        + "subgraph cluster7 {" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "subgraph cluster8 {" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "18;" + Environment.NewLine
                        + "19;" + Environment.NewLine
                        + "20;" + Environment.NewLine
                        + "18 -> 19;" + Environment.NewLine
                        + "}" + Environment.NewLine
                        + "0;" + Environment.NewLine
                        + "1;" + Environment.NewLine
                        + "2;" + Environment.NewLine
                        + "0 -> 1;" + Environment.NewLine
                        + "1 -> 1;" + Environment.NewLine
                        + "}"
                };
            }
        }

        [TestCaseSource(nameof(GenerateTestCases))]
        public string Generate([NotNull] IEdgeListGraph<int, IEdge<int>> graph)
        {
            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            return algorithm.Generate();
        }

        [Test]
        public void GenerateWithFormats()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(3, 2),
                Edge.Create(3, 4),
                Edge.Create(4, 6),
                Edge.Create(5, 2),
                Edge.Create(5, 5)
            );
            graph.AddVertex(7);
            var clusteredGraph = new ClusteredAdjacencyGraph<int, IEdge<int>>(graph);
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph1 = clusteredGraph.AddCluster();
            subGraph1.AddVertexRange(8, 9, 10 );
            ClusteredAdjacencyGraph<int, IEdge<int>> subGraph2 = clusteredGraph.AddCluster();
            subGraph2.AddVerticesAndEdgeRange(
                Edge.Create(11, 12),
                Edge.Create(11, 13),
                Edge.Create(12, 13)
            );

            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(clusteredGraph);
            algorithm.GraphFormat.Name = "MyGraph";
            algorithm.GraphFormat.NodeSeparation = 2;
            algorithm.GraphFormat.FontColor = GraphvizColor.Red;
            algorithm.CommonVertexFormat.Url = "https://myurl.com";
            algorithm.CommonVertexFormat.FillColor = GraphvizColor.LightYellow;
            algorithm.CommonVertexFormat.Style = GraphvizVertexStyle.Filled;
            algorithm.CommonEdgeFormat.Direction = GraphvizEdgeDirection.Back;
            algorithm.CommonEdgeFormat.ToolTip = "Edge";

            algorithm.FormatCluster += (_, args) =>
            {
                args.GraphFormat.Label = args.Cluster == subGraph1
                    ? "Only Vertices cluster"
                    : "Triangle cluster";
            };

            algorithm.FormatVertex += (_, args) =>
            {
                if (args.Vertex == 2 || args.Vertex == 11)
                {
                    args.VertexFormat.Label = "Special Node";
                }
            };

            algorithm.FormatEdge += (_, args) =>
            {
                if (args.Edge.Source ==  args.Edge.Target)
                {
                    args.EdgeFormat.StrokeColor = GraphvizColor.Gold;
                }
            };

            string dot = algorithm.Generate();
            string expectedDot = "digraph MyGraph {" + Environment.NewLine
            + "fontcolor=\"#FF0000FF\"; nodesep=2;" + Environment.NewLine
            + "node [URL=\"https://myurl.com\", style=filled, fillcolor=\"#FFFFE0FF\"];" + Environment.NewLine
            + "edge [dir=back, tooltip=\"Edge\"];" + Environment.NewLine
            + "subgraph cluster1 {" + Environment.NewLine
            + "label=\"Only Vertices cluster\"" + Environment.NewLine
            + "7;" + Environment.NewLine
            + "8;" + Environment.NewLine
            + "9;" + Environment.NewLine
            + "}" + Environment.NewLine
            + "subgraph cluster2 {" + Environment.NewLine
            + "label=\"Triangle cluster\"" + Environment.NewLine
            + "10 [label=\"Special Node\"];" + Environment.NewLine
            + "11;" + Environment.NewLine
            + "12;" + Environment.NewLine
            + "10 -> 11;" + Environment.NewLine
            + "10 -> 12;" + Environment.NewLine
            + "11 -> 12;" + Environment.NewLine
            + "}" + Environment.NewLine
            + "0;" + Environment.NewLine
            + "1 [label=\"Special Node\"];" + Environment.NewLine
            + "2;" + Environment.NewLine
            + "3;" + Environment.NewLine
            + "4;" + Environment.NewLine
            + "5;" + Environment.NewLine
            + "6;" + Environment.NewLine
            + "0 -> 1;" + Environment.NewLine
            + "0 -> 2;" + Environment.NewLine
            + "2 -> 1;" + Environment.NewLine
            + "2 -> 3;" + Environment.NewLine
            + "3 -> 4;" + Environment.NewLine
            + "5 -> 1;" + Environment.NewLine
            + "5 -> 5 [color=\"#FFD700FF\"];" + Environment.NewLine
            + "}";
            Assert.AreEqual(expectedDot, dot);
        }

        [Test]
        public void Generate_Throws()
        {
            var dotEngine = new TestDotEngine();
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new GraphvizAlgorithm<int, IEdge<int>>(graph);
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Generate(null, "NotSaved.dot"));
            Assert.Throws<ArgumentException>(() => algorithm.Generate(dotEngine, null));
            Assert.Throws<ArgumentException>(() => algorithm.Generate(dotEngine, string.Empty));
            Assert.Throws<ArgumentNullException>(() => algorithm.Generate(null, null));
            Assert.Throws<ArgumentNullException>(() => algorithm.Generate(null, string.Empty));
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}