using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Test case sources for serialization tests.
    /// </summary>
    internal static class SerializationTestCaseSources
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationAdjacencyGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph);

                var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3, 4 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );
                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationAdjacencyGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0);
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );
                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationClusteredAdjacencyGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                var clusterEmptyGraph = new ClusteredAdjacencyGraph<int, EquatableEdge<int>>(emptyGraph);
                yield return new TestCaseData(clusterEmptyGraph);

                var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3, 4 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );
                var clusterGraph = new ClusteredAdjacencyGraph<int, EquatableEdge<int>>(graph);
                yield return new TestCaseData(clusterGraph);

                graph = new AdjacencyGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(3, 2)
                );
                clusterGraph = new ClusteredAdjacencyGraph<int, EquatableEdge<int>>(graph);
                ClusteredAdjacencyGraph<int, EquatableEdge<int>> subGraph = clusterGraph.AddCluster();
                subGraph.AddVertexRange( 4, 5, 6 );
                subGraph.AddEdge(new EquatableEdge<int>(4, 6));
                yield return new TestCaseData(clusterGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationClusteredAdjacencyGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var clusterEmptyGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(emptyGraph);
                yield return new TestCaseData(clusterEmptyGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0);
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );
                var clusterGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
                yield return new TestCaseData(clusterGraph);

                graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex0, vertex1, "0"),
                    new EquatableTestEdge(vertex1, vertex2, "1") { Long = 66L },
                    new EquatableTestEdge(vertex2, vertex0, "2"),
                    new EquatableTestEdge(vertex3, vertex2, "3")
                );
                clusterGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
                ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> subGraph = clusterGraph.AddCluster();
                var vertex5 = new EquatableTestVertex("5");
                var vertex6 = new EquatableAdditionalDataTestVertex("6", 45.0) { Double = 22.9 };
                subGraph.AddVertexRange( vertex4, vertex5, vertex6 );
                subGraph.AddEdge(new EquatableTestEdge(vertex4, vertex6, "4"));
                yield return new TestCaseData(clusterGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationCompressedGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, IEdge<int>>();
                var emptyCompressedGraph = CompressedSparseRowGraph<int>.FromGraph(emptyGraph);
                yield return new TestCaseData(emptyCompressedGraph);

                var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3, 4 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );
                var compressedGraph = CompressedSparseRowGraph<int>.FromGraph(emptyGraph);
                yield return new TestCaseData(compressedGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationCompressedGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var emptyCompressedGraph = CompressedSparseRowGraph<EquatableTestVertex>.FromGraph(emptyGraph);
                yield return new TestCaseData(emptyCompressedGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0);
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );
                var compressedGraph = CompressedSparseRowGraph<EquatableTestVertex>.FromGraph(graph);
                yield return new TestCaseData(compressedGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationBidirectionalGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new BidirectionalGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph);

                var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3, 4 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationBidirectionalGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0);
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationBidirectionalMatrixGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new BidirectionalMatrixGraph<EquatableEdge<int>>(10);
                yield return new TestCaseData(emptyGraph);

                var graph = new BidirectionalMatrixGraph<EquatableEdge<int>>(5);
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );

                yield return new TestCaseData(graph);

                graph = new BidirectionalMatrixGraph<EquatableEdge<int>>(5);
                graph.AddEdgeRange(
                    new EquatableTaggedEdge<int, string>(1, 2, "test"),
                    new EquatableEdge<int>(1, 3),
                    new EquatableTaggedEdge<int, double>(1, 4, 42.0),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(3, 4)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationUndirectedGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new UndirectedGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph);

                var graph = new UndirectedGraph<int, EquatableEdge<int>>();
                graph.AddVertexRange( 0, 1, 2, 3, 4 );
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationUndirectedGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new UndirectedGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new UndirectedGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0);
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationEdgeListGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new EdgeListGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph);

                var graph = new EdgeListGraph<int, EquatableEdge<int>>();
                graph.AddEdgeRange(
                    new EquatableEdge<int>(0, 1),
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 0),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(4, 2)
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationEdgeListGraphComplexTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new EdgeListGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new EdgeListGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableAdditionalDataTestVertex("2", 25.0) { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableAdditionalDataTestVertex("4", 42.0) { String = "", StringDefault = null };
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableAdditionalDataTestEdge(vertex1, vertex3, "1", 12.0) { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableAdditionalDataTestEdge(vertex3, vertex4, "4", 45.5)
                );

                yield return new TestCaseData(graph);
            }
        }

        #region GraphML Test cases

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationAdjacencyGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4");
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );
                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationClusteredAdjacencyGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var clusterEmptyGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(emptyGraph);
                yield return new TestCaseData(clusterEmptyGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4");
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );
                var clusterGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
                yield return new TestCaseData(clusterGraph);

                graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex0, vertex1, "0"),
                    new EquatableTestEdge(vertex1, vertex2, "1") { Long = 66L },
                    new EquatableTestEdge(vertex2, vertex0, "2"),
                    new EquatableTestEdge(vertex3, vertex2, "3")
                );
                clusterGraph = new ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
                ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> subGraph = clusterGraph.AddCluster();
                var vertex5 = new EquatableTestVertex("5");
                var vertex6 = new EquatableTestVertex("6") { Double = 22.9 };
                subGraph.AddVertexRange( vertex4, vertex5, vertex6 );
                subGraph.AddEdge(new EquatableTestEdge(vertex4, vertex6, "4"));
                yield return new TestCaseData(clusterGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationCompressedGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var emptyCompressedGraph = CompressedSparseRowGraph<EquatableTestVertex>.FromGraph(emptyGraph);
                yield return new TestCaseData(emptyCompressedGraph);

                var graph = new AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4");
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );
                var compressedGraph = CompressedSparseRowGraph<EquatableTestVertex>.FromGraph(graph);
                yield return new TestCaseData(compressedGraph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationBidirectionalGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4");
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationUndirectedGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new UndirectedGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new UndirectedGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex0 = new EquatableTestVertex("0") { String = "", StringDefault = null };
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4");
                graph.AddVertexRange( vertex0, vertex1, vertex2, vertex3, vertex4 );
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );

                yield return new TestCaseData(graph);
            }
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<TestCaseData> SerializationEdgeListGraphComplexNotHeterogeneousTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new EdgeListGraph<EquatableTestVertex, EquatableTestEdge>();
                yield return new TestCaseData(emptyGraph);

                var graph = new EdgeListGraph<EquatableTestVertex, EquatableTestEdge>();
                var vertex1 = new EquatableTestVertex("1") { Float = 1.3f, String = "test" };
                var vertex2 = new EquatableTestVertex("2") { Double = 12.5, Bool = true };
                var vertex3 = new EquatableTestVertex("3") { Int = 45, Long = 51L };
                var vertex4 = new EquatableTestVertex("4") { String = "", StringDefault = null };
                graph.AddEdgeRange(
                    new EquatableTestEdge(vertex1, vertex2, "0") { Bool = true, Int = 77 },
                    new EquatableTestEdge(vertex1, vertex3, "1") { Long = 99L },
                    new EquatableTestEdge(vertex1, vertex4, "2") { String = "test" },
                    new EquatableTestEdge(vertex2, vertex2, "3"),
                    new EquatableTestEdge(vertex3, vertex4, "4")
                );

                yield return new TestCaseData(graph);
            }
        }

        #endregion
    }
}