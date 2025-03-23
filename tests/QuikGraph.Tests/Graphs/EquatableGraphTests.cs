using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests related to "equals" of graphs.
    /// </summary>
    [TestFixture]
    internal sealed class EquatableGraphTests
    {
        #region Test helpers

        private class VertexTestComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }

        private class EdgeTestComparer : IEqualityComparer<Edge<int>>, IEqualityComparer<IEdge<int>>
        {
            public bool Equals(IEdge<int> x, IEdge<int> y)
            {
                if (x is null)
                    return y is null;
                if (y is null)
                    return false;
                return x.Source == y.Source && x.Target == y.Target;
            }

            public int GetHashCode(IEdge<int> obj)
            {
                return obj.GetHashCode();
            }

            bool IEqualityComparer<Edge<int>>.Equals(Edge<int> x, Edge<int> y) => Equals(x, y);

            int IEqualityComparer<Edge<int>>.GetHashCode(Edge<int> obj) => GetHashCode(obj);
        }

        #endregion

        #region Test cases

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> EquateWithComparerTestCases
        {
            [UsedImplicitly]
            get
            {
                var vertexComparer = new VertexTestComparer();
                var edgeComparer = new EdgeTestComparer();

                #region Same graph type

                // Directed graph
                yield return new TestCaseData(null, null, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var emptyAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                var emptyAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                yield return new TestCaseData(emptyAdjacencyGraph1, null, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(null, emptyAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(emptyAdjacencyGraph1, emptyAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyAdjacencyGraph1, emptyAdjacencyGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyAdjacencyGraph2, emptyAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var adjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph1.AddVertex(1);

                var adjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph2.AddVertex(1);

                yield return new TestCaseData(emptyAdjacencyGraph1, adjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph1, emptyAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph1, adjacencyGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var edge12 = Edge.Create(1, 2);
                var adjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph3.AddVerticesAndEdge(edge12);

                var adjacencyGraph4 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph4.AddVerticesAndEdge(edge12);

                var adjacencyGraph5 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph5.AddVertexRange( 1, 2 );

                yield return new TestCaseData(adjacencyGraph1, adjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph4, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph4, adjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph5, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph5, adjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var edge12Bis = Edge.Create(1, 2);
                var adjacencyGraph6 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph6.AddVerticesAndEdge(edge12Bis);

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph6, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph6, adjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var edge13 = Edge.Create(1, 3);
                var adjacencyGraph7 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph7.AddVerticesAndEdge(edge13);

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph7, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph7, adjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Undirected graph
                var emptyUndirectedGraph1 = new UndirectedGraph<int, IEdge<int>>();
                var emptyUndirectedGraph2 = new UndirectedGraph<int, IEdge<int>>();
                yield return new TestCaseData(emptyUndirectedGraph1, null, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(null, emptyUndirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(emptyUndirectedGraph1, emptyUndirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyUndirectedGraph1, emptyUndirectedGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyUndirectedGraph2, emptyUndirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var undirectedGraph1 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph1.AddVertex(1);

                var undirectedGraph2 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph2.AddVertex(1);

                yield return new TestCaseData(emptyUndirectedGraph1, undirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph1, emptyUndirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph1, undirectedGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var undirectedGraph3 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph3.AddVerticesAndEdge(edge12);

                var undirectedGraph4 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph4.AddVerticesAndEdge(edge12);

                var undirectedGraph5 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph5.AddVertexRange( 1, 2 );

                yield return new TestCaseData(undirectedGraph1, undirectedGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph4, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedGraph4, undirectedGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph5, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph5, undirectedGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var undirectedGraph6 = new AdjacencyGraph<int, IEdge<int>>();
                undirectedGraph6.AddVerticesAndEdge(edge12Bis);

                yield return new TestCaseData(undirectedGraph3, undirectedGraph6, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph6, undirectedGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var undirectedGraph7 = new AdjacencyGraph<int, IEdge<int>>();
                undirectedGraph7.AddVerticesAndEdge(edge13);

                yield return new TestCaseData(undirectedGraph3, undirectedGraph7, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph7, undirectedGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                #endregion

                #region Graph not constructed the same way

                var edge34 = Edge.Create(3, 4);
                var edge42 = Edge.Create(4, 2);
                var adjacencyGraph8 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph8.AddVertex(1);
                adjacencyGraph8.AddVertex(6);
                adjacencyGraph8.AddVerticesAndEdge(edge12);
                adjacencyGraph8.AddVertexRange( 3, 5, 4 );
                adjacencyGraph8.AddEdgeRange( edge42, edge34 );

                var adjacencyGraph9 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph9.AddVertexRange( 1, 2, 3, 4, 5, 6 );
                adjacencyGraph9.AddEdgeRange( edge12, edge34, edge42 );

                yield return new TestCaseData(adjacencyGraph8, adjacencyGraph9, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph9, adjacencyGraph8, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                #endregion

                #region Mixed graph types

                // Array adjacency
                var wrappedAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                wrappedAdjacencyGraph1.AddEdgeRange( edge12, edge34 );
                var arrayAdjacencyGraph1 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);
                var arrayAdjacencyGraph2 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayAdjacencyGraph2, arrayAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var wrappedAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph2.AddVertexRange( 0, 3, 1, 2, 4 );
                wrappedAdjacencyGraph2.AddEdgeRange( edge34, edge12 );
                var arrayAdjacencyGraph3 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph2);

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayAdjacencyGraph3, arrayAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                // => Content check (not type check)
                yield return new TestCaseData(arrayAdjacencyGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var wrappedAdjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph3.AddVertexRange( 1, 2 );
                wrappedAdjacencyGraph3.AddEdge(edge12);
                var arrayAdjacencyGraph4 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph3);
                
                yield return new TestCaseData(arrayAdjacencyGraph4, arrayAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph4, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                #endregion

                #region Other kind of graphs

                // Bidirectional
                var bidirectionalGraph1 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                bidirectionalGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(bidirectionalGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var bidirectionalGraph2 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph2.AddVertexRange( 1, 2 );
                bidirectionalGraph2.AddEdge(edge12);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(bidirectionalGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };
                
                // Array bidirectional
                var arrayBidirectionalGraph1 = new ArrayBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayBidirectionalGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var arrayBidirectionalGraph2 = new ArrayBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph2);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayBidirectionalGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayBidirectionalGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Bidirectional adapter
                var bidirectionalAdapterGraph1 = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalAdapterGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(bidirectionalAdapterGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var bidirectionalAdapterGraph2 = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedAdjacencyGraph3);

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalAdapterGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(bidirectionalAdapterGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Matrix graph
                var matrixGraph1 = new BidirectionalMatrixGraph<IEdge<int>>(5);
                matrixGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, matrixGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(matrixGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };
                
                var matrixGraph2 = new BidirectionalMatrixGraph<IEdge<int>>(5);
                matrixGraph2.AddEdge(edge12);

                yield return new TestCaseData(wrappedAdjacencyGraph1, matrixGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(matrixGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Cluster graph
                var clusterWrappedAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph1.AddEdgeRange( edge12, edge34 );
                var cluster1 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph1);

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(cluster1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var clusterWrappedAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph2.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph2.AddEdgeRange( edge12, edge34 );
                var cluster2 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph2);
                cluster2.AddCluster();
                cluster2.AddCluster();

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(cluster2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var clusterWrappedAdjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph3.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph3.AddEdgeRange( edge12, edge34 );
                var cluster3 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph3);
                ClusteredAdjacencyGraph<int, IEdge<int>> subGraph31 = cluster3.AddCluster();
                subGraph31.AddVertex(6);

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster3, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(cluster3, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Edge list
                var adjacencyGraph = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph.AddVerticesAndEdgeRange( edge12, edge34 );
                var edgeListGraph1 = new EdgeListGraph<int, IEdge<int>>();
                edgeListGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(adjacencyGraph, edgeListGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(edgeListGraph1, adjacencyGraph, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                var edgeListGraph2 = new EdgeListGraph<int, IEdge<int>>();
                edgeListGraph2.AddEdge(edge12);
                
                yield return new TestCaseData(adjacencyGraph, edgeListGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(edgeListGraph2, adjacencyGraph, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Undirected graph
                var undirectedGraph8 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph8.AddVertexRange( 0, 1, 2, 3, 4 );
                undirectedGraph8.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedGraph8, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph8, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var edge21 = Edge.Create(2, 1);
                var edge43 = Edge.Create(4, 3);
                var adjacencyGraph10 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph10.AddVertexRange( 0, 1, 2, 3, 4 );
                adjacencyGraph10.AddEdgeRange( edge12, edge21, edge34, edge43 );

                yield return new TestCaseData(adjacencyGraph10, undirectedGraph8, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph8, adjacencyGraph10, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var undirectedGraph9 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph9.AddVertexRange( 1, 2 );
                undirectedGraph9.AddEdge(edge12);

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedGraph9, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph9, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Array undirected graph
                var arrayUndirectedGraph1 = new ArrayUndirectedGraph<int, IEdge<int>>(undirectedGraph8);

                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayUndirectedGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayUndirectedGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                var arrayUndirectedGraph2 = new ArrayUndirectedGraph<int, IEdge<int>>(undirectedGraph9);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayUndirectedGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayUndirectedGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                // Undirected bidirectional graph
                var undirectedBidirectionalGraph1 = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                var undirectedBidirectionalGraph1Bis = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                var undirectedBidirectionalGraph2 = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph2);

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph1Bis, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1Bis, undirectedBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph2, undirectedBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };


                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph10, undirectedBidirectionalGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, adjacencyGraph10, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedBidirectionalGraph2, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph2, wrappedAdjacencyGraph1, vertexComparer, edgeComparer)
                {
                    ExpectedResult = false
                };

                #endregion
            }
        }

        #endregion

        [TestCaseSource(nameof(EquateWithComparerTestCases))]
        public bool EquateWithComparer(
            [NotNull] IEdgeListGraph<int, IEdge<int>> g,
            [NotNull] IEdgeListGraph<int, IEdge<int>> h,
            [NotNull] IEqualityComparer<int> vertexEquality,
            [NotNull] IEqualityComparer<IEdge<int>> edgeEquality)
        {
            return EquateGraphs.IsEqualTo(g, h, vertexEquality, edgeEquality);
        }

        [Test]
        public void EquateWithComparer_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.IsTrue(EquateGraphs.IsEqualTo<int, IEdge<int>>(null, null, EqualityComparer<int>.Default, null));
            Assert.IsTrue(EquateGraphs.IsEqualTo<int, IEdge<int>>(null, null, null, EqualityComparer<IEdge<int>>.Default));
            Assert.IsTrue(EquateGraphs.IsEqualTo<int, IEdge<int>>(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #region Test cases

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> EquateTestCases
        {
            [UsedImplicitly]
            get
            {
                #region Same graph type

                yield return new TestCaseData(null, null)
                {
                    ExpectedResult = true
                };

                // Directed graph
                var emptyAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                var emptyAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                yield return new TestCaseData(emptyAdjacencyGraph1, null)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(null, emptyAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(emptyAdjacencyGraph1, emptyAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyAdjacencyGraph1, emptyAdjacencyGraph2)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyAdjacencyGraph2, emptyAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var adjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph1.AddVertex(1);

                var adjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph2.AddVertex(1);

                yield return new TestCaseData(emptyAdjacencyGraph1, adjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph1, emptyAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph1, adjacencyGraph2)
                {
                    ExpectedResult = true
                };

                var edge12 = Edge.Create(1, 2);
                var adjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph3.AddVerticesAndEdge(edge12);

                var adjacencyGraph4 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph4.AddVerticesAndEdge(edge12);

                var adjacencyGraph5 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph5.AddVertexRange( 1, 2 );

                yield return new TestCaseData(adjacencyGraph1, adjacencyGraph3)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph4)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph4, adjacencyGraph3)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph5)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph5, adjacencyGraph3)
                {
                    ExpectedResult = false
                };

                var edge12Bis = Edge.Create(1, 2);
                var adjacencyGraph6 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph6.AddVerticesAndEdge(edge12Bis);

                yield return new TestCaseData(adjacencyGraph3, adjacencyGraph6)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph6, adjacencyGraph3)
                {
                    ExpectedResult = false
                };

                // Undirected graph
                var emptyUndirectedGraph1 = new UndirectedGraph<int, IEdge<int>>();
                var emptyUndirectedGraph2 = new UndirectedGraph<int, IEdge<int>>();
                yield return new TestCaseData(emptyUndirectedGraph1, null)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(null, emptyUndirectedGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(emptyUndirectedGraph1, emptyUndirectedGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyUndirectedGraph1, emptyUndirectedGraph2)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(emptyUndirectedGraph2, emptyUndirectedGraph1)
                {
                    ExpectedResult = true
                };

                var undirectedGraph1 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph1.AddVertex(1);

                var undirectedGraph2 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph2.AddVertex(1);

                yield return new TestCaseData(emptyUndirectedGraph1, undirectedGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph1, emptyUndirectedGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph1, undirectedGraph2)
                {
                    ExpectedResult = true
                };

                var undirectedGraph3 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph3.AddVerticesAndEdge(edge12);

                var undirectedGraph4 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph4.AddVerticesAndEdge(edge12);

                var undirectedGraph5 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph5.AddVertexRange( 1, 2 );

                yield return new TestCaseData(undirectedGraph1, undirectedGraph3)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph4)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedGraph4, undirectedGraph3)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedGraph3, undirectedGraph5)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph5, undirectedGraph3)
                {
                    ExpectedResult = false
                };

                var undirectedGraph6 = new AdjacencyGraph<int, IEdge<int>>();
                undirectedGraph6.AddVerticesAndEdge(edge12Bis);

                yield return new TestCaseData(undirectedGraph3, undirectedGraph6)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph6, undirectedGraph3)
                {
                    ExpectedResult = false
                };

                #endregion

                #region Graph not constructed the same way

                var edge34 = Edge.Create(3, 4);
                var edge42 = Edge.Create(4, 2);
                var adjacencyGraph7 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph7.AddVertex(1);
                adjacencyGraph7.AddVertex(6);
                adjacencyGraph7.AddVerticesAndEdge(edge12);
                adjacencyGraph7.AddVertexRange( 3, 5, 4 );
                adjacencyGraph7.AddEdgeRange( edge42, edge34 );

                var adjacencyGraph8 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph8.AddVertexRange( 1, 2, 3, 4, 5, 6 );
                adjacencyGraph8.AddEdgeRange( edge12, edge34, edge42 );

                yield return new TestCaseData(adjacencyGraph7, adjacencyGraph8)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(adjacencyGraph8, adjacencyGraph7)
                {
                    ExpectedResult = true
                };

                #endregion

                #region Mixed graph types

                // Array adjacency
                var wrappedAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                wrappedAdjacencyGraph1.AddEdgeRange( edge12, edge34 );
                var arrayAdjacencyGraph1 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);
                var arrayAdjacencyGraph2 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph2)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayAdjacencyGraph2, arrayAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var wrappedAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph2.AddVertexRange( 0, 3, 1, 2, 4 );
                wrappedAdjacencyGraph2.AddEdgeRange( edge34, edge12 );
                var arrayAdjacencyGraph3 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph2);

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph3)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayAdjacencyGraph3, arrayAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                // => Content check (not type check)
                yield return new TestCaseData(arrayAdjacencyGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var wrappedAdjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                wrappedAdjacencyGraph3.AddVertexRange( 1, 2 );
                wrappedAdjacencyGraph3.AddEdge(edge12);
                var arrayAdjacencyGraph4 = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedAdjacencyGraph3);
                
                yield return new TestCaseData(arrayAdjacencyGraph4, arrayAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayAdjacencyGraph1, arrayAdjacencyGraph4)
                {
                    ExpectedResult = false
                };

                #endregion

                #region Other kind of graphs

                // Bidirectional
                var bidirectionalGraph1 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                bidirectionalGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(bidirectionalGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var bidirectionalGraph2 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph2.AddVertexRange( 1, 2 );
                bidirectionalGraph2.AddEdge(edge12);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(bidirectionalGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };
                
                // Array bidirectional
                var arrayBidirectionalGraph1 = new ArrayBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayBidirectionalGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(arrayBidirectionalGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var arrayBidirectionalGraph2 = new ArrayBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph2);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayBidirectionalGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayBidirectionalGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Bidirectional adapter
                var bidirectionalAdapterGraph1 = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedAdjacencyGraph1);

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalAdapterGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(bidirectionalAdapterGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var bidirectionalAdapterGraph2 = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedAdjacencyGraph3);

                yield return new TestCaseData(wrappedAdjacencyGraph1, bidirectionalAdapterGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(bidirectionalAdapterGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Matrix graph
                var matrixGraph1 = new BidirectionalMatrixGraph<IEdge<int>>(5);
                matrixGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, matrixGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(matrixGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };
                
                var matrixGraph2 = new BidirectionalMatrixGraph<IEdge<int>>(5);
                matrixGraph2.AddEdge(edge12);

                yield return new TestCaseData(wrappedAdjacencyGraph1, matrixGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(matrixGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Cluster graph
                var clusterWrappedAdjacencyGraph1 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph1.AddEdgeRange( edge12, edge34 );
                var cluster1 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph1);

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(cluster1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var clusterWrappedAdjacencyGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph2.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph2.AddEdgeRange( edge12, edge34 );
                var cluster2 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph2);
                cluster2.AddCluster();
                cluster2.AddCluster();

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster2)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(cluster2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var clusterWrappedAdjacencyGraph3 = new AdjacencyGraph<int, IEdge<int>>();
                clusterWrappedAdjacencyGraph3.AddVertexRange( 0, 1, 2, 3, 4 );
                clusterWrappedAdjacencyGraph3.AddEdgeRange( edge12, edge34 );
                var cluster3 = new ClusteredAdjacencyGraph<int, IEdge<int>>(clusterWrappedAdjacencyGraph3);
                ClusteredAdjacencyGraph<int, IEdge<int>> subGraph31 = cluster3.AddCluster();
                subGraph31.AddVertex(6);

                yield return new TestCaseData(wrappedAdjacencyGraph1, cluster3)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(cluster3, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Edge list
                var adjacencyGraph = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph.AddVerticesAndEdgeRange( edge12, edge34 );
                var edgeListGraph1 = new EdgeListGraph<int, IEdge<int>>();
                edgeListGraph1.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(adjacencyGraph, edgeListGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(edgeListGraph1, adjacencyGraph)
                {
                    ExpectedResult = true
                };

                var edgeListGraph2 = new EdgeListGraph<int, IEdge<int>>();
                edgeListGraph2.AddEdge(edge12);
                
                yield return new TestCaseData(adjacencyGraph, edgeListGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(edgeListGraph2, adjacencyGraph)
                {
                    ExpectedResult = false
                };

                // Undirected graph
                var undirectedGraph7 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph7.AddVertexRange( 0, 1, 2, 3, 4 );
                undirectedGraph7.AddEdgeRange( edge12, edge34 );

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedGraph7)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph7, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                var edge21 = Edge.Create(2, 1);
                var edge43 = Edge.Create(4, 3);
                var adjacencyGraph9 = new AdjacencyGraph<int, IEdge<int>>();
                adjacencyGraph9.AddVertexRange( 0, 1, 2, 3, 4 );
                adjacencyGraph9.AddEdgeRange( edge12, edge21, edge34, edge43 );

                yield return new TestCaseData(adjacencyGraph9, undirectedGraph7)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph7, adjacencyGraph9)
                {
                    ExpectedResult = false
                };

                var undirectedGraph8 = new UndirectedGraph<int, IEdge<int>>();
                undirectedGraph8.AddVertexRange( 1, 2 );
                undirectedGraph8.AddEdge(edge12);

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedGraph8)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedGraph8, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Array undirected graph
                var arrayUndirectedGraph1 = new ArrayUndirectedGraph<int, IEdge<int>>(undirectedGraph7);

                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayUndirectedGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayUndirectedGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                var arrayUndirectedGraph2 = new ArrayUndirectedGraph<int, IEdge<int>>(undirectedGraph8);
                
                yield return new TestCaseData(wrappedAdjacencyGraph1, arrayUndirectedGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(arrayUndirectedGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                // Undirected bidirectional graph
                var undirectedBidirectionalGraph1 = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                var undirectedBidirectionalGraph1Bis = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph1);
                var undirectedBidirectionalGraph2 = new UndirectedBidirectionalGraph<int, IEdge<int>>(bidirectionalGraph2);

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph1Bis)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1Bis, undirectedBidirectionalGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, undirectedBidirectionalGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph2, undirectedBidirectionalGraph1)
                {
                    ExpectedResult = false
                };


                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedBidirectionalGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(adjacencyGraph9, undirectedBidirectionalGraph1)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph1, adjacencyGraph9)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(wrappedAdjacencyGraph1, undirectedBidirectionalGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(undirectedBidirectionalGraph2, wrappedAdjacencyGraph1)
                {
                    ExpectedResult = false
                };

                #endregion
            }
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> ReversedGraphEquateTestCases
        {
            [UsedImplicitly]
            get
            {
                var edge12 = Edge.Create(1, 2);
                var edge34 = Edge.Create(3, 4);

                var bidirectionalGraph1 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                bidirectionalGraph1.AddEdgeRange( edge12, edge34 );
                var bidirectionalGraph2 = new BidirectionalGraph<int, IEdge<int>>();
                bidirectionalGraph2.AddVertexRange( 1, 2 );
                bidirectionalGraph2.AddEdge(edge12);

                // Reversed graph
                var reversedGraph1 = bidirectionalGraph1.CreateReversedBidirectionalGraph();
                var reversedGraph2 = bidirectionalGraph1.CreateReversedBidirectionalGraph();

                yield return new TestCaseData(reversedGraph2, reversedGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(reversedGraph1, reversedGraph2)
                {
                    ExpectedResult = true
                };

                var reversedGraph3 = bidirectionalGraph2.CreateReversedBidirectionalGraph();

                yield return new TestCaseData(reversedGraph1, reversedGraph3)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(reversedGraph3, reversedGraph1)
                {
                    ExpectedResult = false
                };
            }
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CompressedGraphEquateTestCases
        {
            [UsedImplicitly]
            get
            {
                var edge12 = new SEquatableEdge<int>(1, 2);
                var edge34 = new SEquatableEdge<int>(3, 4);

                var adjacencyGraph1 = new AdjacencyGraph<int, SEquatableEdge<int>>();
                adjacencyGraph1.AddVertexRange( 0, 1, 2, 3, 4 );
                adjacencyGraph1.AddEdgeRange( edge12, edge34 );
                var adjacencyGraph2 = new AdjacencyGraph<int, SEquatableEdge<int>>();
                adjacencyGraph2.AddVertexRange( 1, 2 );
                adjacencyGraph2.AddEdge(edge12);

                // Compressed graph
                var compressedGraph1 = CompressedSparseRowGraph<int>.FromGraph(adjacencyGraph1);

                yield return new TestCaseData(adjacencyGraph1, compressedGraph1)
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(compressedGraph1, adjacencyGraph1)
                {
                    ExpectedResult = true
                };

                var compressedGraph2 = CompressedSparseRowGraph<int>.FromGraph(adjacencyGraph2);

                yield return new TestCaseData(adjacencyGraph1, compressedGraph2)
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(compressedGraph2, adjacencyGraph1)
                {
                    ExpectedResult = false
                };
            }
        }

        #endregion

        [TestCaseSource(nameof(EquateTestCases))]
        public bool Equate(
            [NotNull] IEdgeListGraph<int, IEdge<int>> g,
            [NotNull] IEdgeListGraph<int, IEdge<int>> h)
        {
            return EquateGraphs.IsEqualTo(g, h);
        }

        [TestCaseSource(nameof(ReversedGraphEquateTestCases))]
        public bool EquateReversedGraph(
            [NotNull] IEdgeListGraph<int, SReversedEdge<int, IEdge<int>>> g,
            [NotNull] IEdgeListGraph<int, SReversedEdge<int, IEdge<int>>> h)
        {
            return EquateGraphs.IsEqualTo(g, h);
        }

        [TestCaseSource(nameof(CompressedGraphEquateTestCases))]
        public bool EquateCompressedGraph(
            [NotNull] IEdgeListGraph<int, SEquatableEdge<int>> g,
            [NotNull] IEdgeListGraph<int, SEquatableEdge<int>> h)
        {
            return EquateGraphs.IsEqualTo(g, h);
        }
    }
}