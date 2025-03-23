using System;
using System.IO;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Serialization.Tests.SerializationTestCaseSources;

#pragma warning disable 618 // Obsolete API usage

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Tests relative to serialization via standard API.
    /// </summary>
    /// <remarks>
    /// Rather use <see cref="GraphMLSerializer{TVertex, TEdge, TGraph}"/> for a readable Result.
    /// Tests can be fount in <see cref="GraphMLSerializationTests"/>
    /// </remarks>
    [TestFixture]
    internal sealed class SystemSerializationTests
    {
        #region Helpers

        [Pure]
        [NotNull]
        private static TGraph SerializeDeserialize<TVertex, TEdge, TGraph>([NotNull] TGraph graph)
            where TGraph : IGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            Assert.IsNotNull(graph);

            // Serialize
            using (var stream = new MemoryStream())
            {
                graph.SerializeToBinary(stream);

                // Deserialize
                stream.Position = 0;

                TGraph deserializedGraph = stream.DeserializeFromBinary<TVertex, TEdge, TGraph>();
                Assert.IsNotNull(deserializedGraph);
                Assert.AreNotSame(graph, deserializedGraph);

                return deserializedGraph;
            }
        }

        #endregion

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphTestCases))]
        public void BinarySerialization_AdjacencyGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            AdjacencyGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize<int, EquatableEdge<int>, AdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayAdjacencyGraph<int, EquatableEdge<int>>(graph);
            ArrayAdjacencyGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize<int, EquatableEdge<int>, ArrayAdjacencyGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexTestCases))]
        public void BinarySerialization_AdjacencyGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph1 =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, AdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            ArrayAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, ArrayAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphTestCases))]
        public void BinarySerialization_AdapterGraph_Simple([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<int, EquatableEdge<int>>(graph);
            BidirectionalAdapterGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize<int, EquatableEdge<int>, BidirectionalAdapterGraph<int, EquatableEdge<int>>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationAdjacencyGraphComplexTestCases))]
        public void BinarySerialization_AdapterGraph_Complex([NotNull] AdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, BidirectionalAdapterGraph<EquatableTestVertex, EquatableTestEdge>>(bidirectionalAdapterGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphTestCases))]
        public void BinarySerialization_ClusteredGraph_Simple([NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            ClusteredAdjacencyGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize<int, EquatableEdge<int>, ClusteredAdjacencyGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationClusteredAdjacencyGraphComplexTestCases))]
        public void BinarySerialization_ClusteredGraph_Complex([NotNull] ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, ClusteredAdjacencyGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphTestCases))]
        public void BinarySerialization_CompressedGraph_Simple([NotNull] CompressedSparseRowGraph<int> graph)
        {
            CompressedSparseRowGraph<int> deserializedGraph =
                SerializeDeserialize<int, SEquatableEdge<int>, CompressedSparseRowGraph<int>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationCompressedGraphComplexTestCases))]
        public void BinarySerialization_CompressedGraph_Complex([NotNull] CompressedSparseRowGraph<EquatableTestVertex> graph)
        {
            CompressedSparseRowGraph<EquatableTestVertex> deserializedGraph =
                SerializeDeserialize<EquatableTestVertex, SEquatableEdge<EquatableTestVertex>, CompressedSparseRowGraph<EquatableTestVertex>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphTestCases))]
        public void BinarySerialization_BidirectionalGraph_Simple([NotNull] BidirectionalGraph<int, EquatableEdge<int>> graph)
        {
            BidirectionalGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize<int, EquatableEdge<int>, BidirectionalGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));

            var arrayGraph = new ArrayBidirectionalGraph<int, EquatableEdge<int>>(graph);
            ArrayBidirectionalGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize<int, EquatableEdge<int>, ArrayBidirectionalGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));

            var reversedGraph = graph.CreateReversedBidirectionalGraph();
            ReversedBidirectionalGraph<int, EquatableEdge<int>> deserializedGraph3 =
                SerializeDeserialize<int, SReversedEdge<int, EquatableEdge<int>>, ReversedBidirectionalGraph<int, EquatableEdge<int>>>(reversedGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(reversedGraph, deserializedGraph3));

            // TODO: re-enable this; Null-Ref on Deserialization was introduced in Commit SHA-1: 770acd47b0b872c9f85b5fb87e7a6279bb6f67be
            //var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<int, EquatableEdge<int>>(graph);
            //UndirectedBidirectionalGraph<int, EquatableEdge<int>> deserializedGraph4 =
            //    SerializeDeserialize<int, EquatableEdge<int>, UndirectedBidirectionalGraph<int, EquatableEdge<int>>>(undirectedBidirectionalGraph);
            //Assert.IsTrue(EquateGraphs.Equate(undirectedBidirectionalGraph, deserializedGraph4));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalGraphComplexTestCases))]
        public void BinarySerialization_BidirectionalGraph_Complex([NotNull] BidirectionalGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            BidirectionalGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, BidirectionalGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));

            var arrayGraph = new ArrayBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            ArrayBidirectionalGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, ArrayBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(arrayGraph, deserializedGraph2));

            var reversedGraph = graph.CreateReversedBidirectionalGraph();
            ReversedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph3 =
                SerializeDeserialize<EquatableTestVertex, SReversedEdge<EquatableTestVertex, EquatableTestEdge>, ReversedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>>(reversedGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(reversedGraph, deserializedGraph3));

            // TODO: re-enable this; Null-Ref on Deserialization was introduced in Commit SHA-1: 770acd47b0b872c9f85b5fb87e7a6279bb6f67be
            //var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            //var deserializedGraph4 =
            //    SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, UndirectedBidirectionalGraph<EquatableTestVertex, EquatableTestEdge>>(undirectedBidirectionalGraph);
            //Assert.IsTrue(EquateGraphs.Equate(undirectedBidirectionalGraph, deserializedGraph4));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationBidirectionalMatrixGraphTestCases))]
        public void BinarySerialization_BidirectionalMatrixGraph([NotNull] BidirectionalMatrixGraph<EquatableEdge<int>> graph)
        {
            BidirectionalMatrixGraph<EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize<int, EquatableEdge<int>, BidirectionalMatrixGraph<EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphTestCases))]
        [Ignore("TODO: re-enable this; Null-Ref on Deserialization was introduced in Commit SHA-1: 770acd47b0b872c9f85b5fb87e7a6279bb6f67be")]
        public void BinarySerialization_UndirectedGraph_Simple([NotNull] UndirectedGraph<int, EquatableEdge<int>> graph)
        {
            UndirectedGraph<int, EquatableEdge<int>> deserializedGraph1 =
                SerializeDeserialize<int, EquatableEdge<int>, UndirectedGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayUndirectedGraph<int, EquatableEdge<int>>(graph);
            ArrayUndirectedGraph<int, EquatableEdge<int>> deserializedGraph2 =
                SerializeDeserialize<int, EquatableEdge<int>, ArrayUndirectedGraph<int, EquatableEdge<int>>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationUndirectedGraphComplexTestCases))]
        [Ignore("TODO: re-enable this; Null-Ref on Deserialization was introduced in Commit SHA-1: 770acd47b0b872c9f85b5fb87e7a6279bb6f67be")]
        public void BinarySerialization_UndirectedGraph_Complex([NotNull] UndirectedGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            UndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph1 =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, UndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph1));

            var arrayGraph = new ArrayUndirectedGraph<EquatableTestVertex, EquatableTestEdge>(graph);
            ArrayUndirectedGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph2 =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, ArrayUndirectedGraph<EquatableTestVertex, EquatableTestEdge>>(arrayGraph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph2));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationEdgeListGraphTestCases))]
        public void BinarySerialization_EdgeListGraph_Simple([NotNull] EdgeListGraph<int, EquatableEdge<int>> graph)
        {
            EdgeListGraph<int, EquatableEdge<int>> deserializedGraph =
                SerializeDeserialize<int, EquatableEdge<int>, EdgeListGraph<int, EquatableEdge<int>>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        [TestCaseSource(typeof(SerializationTestCaseSources), nameof(SerializationEdgeListGraphComplexTestCases))]
        public void BinarySerialization_EdgeListGraph_Complex([NotNull] EdgeListGraph<EquatableTestVertex, EquatableTestEdge> graph)
        {
            EdgeListGraph<EquatableTestVertex, EquatableTestEdge> deserializedGraph =
                SerializeDeserialize<EquatableTestVertex, EquatableTestEdge, EdgeListGraph<EquatableTestVertex, EquatableTestEdge>>(graph);
            Assert.IsTrue(EquateGraphs.IsEqualTo(graph, deserializedGraph));
        }

        #region Test classes

        public class TestStream : Stream
        {
            public override void Flush()
            {
                // Fake
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;   // Fake
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return 0;   // Fake
            }

            public override void SetLength(long value)
            {
                // Fake
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                // Fake
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => 0;

            public override bool CanTimeout => true;

            public override long Position
            {
                get => 0;
                set { }
            }
        }

        #endregion

        [Test]
        public void BinarySerialization_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            using (var stream = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((AdjacencyGraph<int, IEdge<int>>)null).SerializeToBinary(stream));
            }

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            using (var stream = new TestStream())
            {
                Assert.Throws<ArgumentException>(
                    () => graph.SerializeToBinary(stream));
            }

            Assert.Throws<ArgumentNullException>(
                () => graph.SerializeToBinary(null));

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(
                () => ((Stream)null).DeserializeFromBinary<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>());

            using (var stream = new TestStream())
            {
                Assert.Throws<ArgumentException>(
                    () => stream.DeserializeFromBinary<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>());
            }
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}