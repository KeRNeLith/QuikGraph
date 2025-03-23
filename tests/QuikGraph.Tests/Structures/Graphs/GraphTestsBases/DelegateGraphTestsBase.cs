using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.AssertHelpers;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Base class for tests over delegated graphs.
    /// </summary>
    internal class DelegateGraphTestsBase : GraphTestsBase
    {
        [Pure]
        [NotNull]
        protected static Func<TVertex, IEnumerable<TEdge>> EmptyGetter<TVertex, TEdge>()
            where TEdge : IEdge<TVertex>
        {
            return (TVertex vertex) =>
            {
                if (vertex is null)
                {
                    throw new ArgumentNullException(nameof(vertex));
                }

                return null;
            };
        }

        protected class GraphData<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            public GraphData()
            {
                GetEdges = _ =>
                {
                    ++_nbCalls;

                    if (ShouldReturnValue)
                    {
                        return ShouldReturnEdges ?? Enumerable.Empty<TEdge>();
                    }

                    return null;
                };
            }

            private int _nbCalls;

            [NotNull]
            public Func<TVertex, IEnumerable<TEdge>> GetEdges { get; }

            /// <summary> Optional Default Edges when <see cref="ShouldReturnValue"/> is true </summary>
            [CanBeNull, ItemNotNull]
            public IEnumerable<TEdge> ShouldReturnEdges { get; set; }

            /// <summary> Flag to return a value instead of null </summary>
            public bool ShouldReturnValue { get; set; }

            public void CheckCalls(int expectedCalls)
            {
                Assert.AreEqual(expectedCalls, _nbCalls);
                _nbCalls = 0;
            }
        }

        #region Test helpers

        #region Contains Vertex

        protected static void ContainsVertex_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitVertexSet<int> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsFalse(graph.ContainsVertex(1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            Assert.IsTrue(graph.ContainsVertex(1));
            data.CheckCalls(1);
        }

        #endregion

        #region Contains Edge

        protected static void ContainsEdge_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IEdgeSet<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            var edge12 = Edge.Create(1, 2);
            var edge21 = Edge.Create(2, 1);
            Assert.IsFalse(graph.ContainsEdge(edge12));
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(edge21));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            Assert.IsFalse(graph.ContainsEdge(edge12));
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(edge21));
            data.CheckCalls(1);

            var edge13 = Edge.Create(1, 3);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge21 };
            Assert.IsTrue(graph.ContainsEdge(edge12));
            data.CheckCalls(1);
            Assert.IsTrue(graph.ContainsEdge(edge21));
            data.CheckCalls(1);

            var edge15 = Edge.Create(1, 5);
            var edge51 = Edge.Create(5, 1);
            var edge56 = Edge.Create(5, 6);
            Assert.IsFalse(graph.ContainsEdge(edge15));
            Assert.IsFalse(graph.ContainsEdge(edge51));
            Assert.IsFalse(graph.ContainsEdge(edge56));
        }

        private static void ContainsEdge_SourceTarget_GenericTest(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull, InstantHandle] Func<int, int, bool> hasEdge,
            bool isDirected = true)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsFalse(hasEdge(1, 2));
            data.CheckCalls(1);
            Assert.IsFalse(hasEdge(2, 1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            Assert.IsFalse(hasEdge(1, 2));
            data.CheckCalls(1);
            Assert.IsFalse(hasEdge(2, 1));
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 3), Edge.Create(1, 2) };
            Assert.IsTrue(hasEdge(1, 2));
            data.CheckCalls(1);
            if (isDirected)
                Assert.IsFalse(hasEdge(2, 1));
            else
                Assert.IsTrue(hasEdge(2, 1));
            data.CheckCalls(1);

            Assert.IsFalse(hasEdge(1, 5));
            Assert.IsFalse(hasEdge(5, 1));
            Assert.IsFalse(hasEdge(5, 6));
        }

        protected static void ContainsEdge_SourceTarget_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph)
        {
            ContainsEdge_SourceTarget_GenericTest(
                data,
                graph.ContainsEdge);
        }

        protected static void ContainsEdge_SourceTarget_UndirectedGraph_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            ContainsEdge_SourceTarget_GenericTest(
                data,
                graph.ContainsEdge,
                false);
        }

        #endregion

        #region Out Edges

        protected static void OutEdge_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitGraph<int, IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);

            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge11, edge12, edge13 };
            Assert.AreSame(edge11, graph.OutEdge(1, 0));
            data.CheckCalls(1);

            Assert.AreSame(edge13, graph.OutEdge(1, 2));
            data.CheckCalls(1);
        }

        protected static void OutEdge_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.OutEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            AssertIndexOutOfRange(() => graph.OutEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2) };
            AssertIndexOutOfRange(() => graph.OutEdge(1, 1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            AssertNoOutEdge(graph, 1);
            data.CheckCalls(3);

            IEdge<int>[] edges =
            {
                Edge.Create(1, 2),
                Edge.Create(1, 3)
            };
            data.ShouldReturnEdges = edges;
            AssertHasOutEdges(graph, 1, edges);
            data.CheckCalls(3);
        }

        protected static void OutEdges_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.AreEqual(null, graph.IsOutEdgesEmpty(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.OutDegree(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.OutEdges(1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Adjacent Edges

        protected static void AdjacentEdge_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);

            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge11, edge12, edge13 };
            Assert.AreSame(edge11, graph.AdjacentEdge(1, 0));
            data.CheckCalls(1);

            Assert.AreSame(edge13, graph.AdjacentEdge(1, 2));
            data.CheckCalls(1);
        }

        protected static void AdjacentEdge_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.AdjacentEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            AssertIndexOutOfRange(() => graph.AdjacentEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2) };
            AssertIndexOutOfRange(() => graph.AdjacentEdge(1, 1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void AdjacentEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            AssertNoAdjacentEdge(graph, 1);
            data.CheckCalls(3);

            IEdge<int>[] edges =
            {
                Edge.Create(1, 2),
                Edge.Create(1, 3)
            };
            data.ShouldReturnEdges = edges;
            graph.AssertHasAdjacentEdges(1, edges);
            data.CheckCalls(3);
        }

        protected static void AdjacentEdges_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.IsAdjacentEdgesEmpty(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.AdjacentDegree(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.AdjacentEdges(1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Out Edges

        protected static void InEdge_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge21 = Edge.Create(2, 1);
            var edge31 = Edge.Create(3, 1);

            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge11, edge21, edge31 };
            Assert.AreSame(edge11, graph.InEdge(1, 0));
            data.CheckCalls(1);

            Assert.AreSame(edge31, graph.InEdge(1, 2));
            data.CheckCalls(1);
        }

        protected static void InEdge_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.InEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            AssertIndexOutOfRange(() => graph.InEdge(1, 0));
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2) };
            AssertIndexOutOfRange(() => graph.InEdge(1, 1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void InEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = true;
            AssertNoInEdge(graph, 1);
            data.CheckCalls(3);

            IEdge<int>[] edges =
            {
                Edge.Create(1, 2),
                Edge.Create(1, 3)
            };
            data.ShouldReturnEdges = edges;
            AssertHasInEdges(graph, 1, edges);
            data.CheckCalls(3);
        }

        protected static void InEdges_Throws_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.Null(graph.IsInEdgesEmpty(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.InDegree(1));
            data.CheckCalls(1);

            Assert.IsNull(graph.InEdges(1));
            data.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Degree

        protected static void Degree_Test(
            [NotNull] GraphData<int, IEdge<int>> data1,
            [NotNull] GraphData<int, IEdge<int>> data2,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            data1.CheckCalls(0);
            data2.CheckCalls(0);

            data1.ShouldReturnValue = false;
            data2.ShouldReturnValue = false;
            Assert.IsNull(graph.Degree(1));
            data1.CheckCalls(1);
            data2.CheckCalls(1);

            data1.ShouldReturnValue = true;
            data2.ShouldReturnValue = false;
            Assert.IsNull(graph.Degree(1));
            data1.CheckCalls(1);
            data2.CheckCalls(1);

            data1.ShouldReturnValue = false;
            data2.ShouldReturnValue = true;
            Assert.IsNull(graph.Degree(1));
            data1.CheckCalls(1);
            data2.CheckCalls(1);
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            data1.ShouldReturnValue = true;
            data2.ShouldReturnValue = true;
            Assert.AreEqual(0, graph.Degree(1));

            data1.ShouldReturnEdges = new[] { Edge.Create(1, 2) };
            data2.ShouldReturnEdges = null;
            Assert.AreEqual(1, graph.Degree(1));

            data1.ShouldReturnEdges = null;
            data2.ShouldReturnEdges = new[] { Edge.Create(3, 1) };
            Assert.AreEqual(1, graph.Degree(1));

            data1.ShouldReturnEdges = new[] { Edge.Create(1, 2), Edge.Create(1, 3) };
            data2.ShouldReturnEdges = new[] { Edge.Create(4, 1) };
            Assert.AreEqual(3, graph.Degree(1));

            // Self edge
            data1.ShouldReturnEdges = new[] { Edge.Create(1, 2), Edge.Create(1, 3), Edge.Create(1, 1) };
            data2.ShouldReturnEdges = new[] { Edge.Create(4, 1), Edge.Create(1, 1) };
            Assert.AreEqual(5, graph.Degree(1));
        }

        #endregion

        #region Try Get Edges

        protected static void TryGetEdge_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph)
        {
            ContainsEdge_SourceTarget_GenericTest(
                data,
                (source, target) => graph.TryGetEdge(source, target, out _));
        }

        protected static void TryGetEdge_UndirectedGraph_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            ContainsEdge_SourceTarget_GenericTest(
                data,
                (source, target) => graph.TryGetEdge(source, target, out _),
                false);
        }

        protected static void GetEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsEmpty(graph.GetEdges(0, 1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            var edges = graph.GetEdges(1, 2);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2), Edge.Create(1, 2) };
            edges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);
        }

        protected static void GetEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] DelegateVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsEmpty(graph.GetEdges(0, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnValue = true;
            var edges = graph.GetEdges(1, 2);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2), Edge.Create(1, 2) };
            edges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);

            var edge14 = Edge.Create(1, 4);
            var edge12 = Edge.Create(1, 2);
            var edge12Bis = Edge.Create(1, 2);
            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge14, edge12 };
            edges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(new[] { edge12 }, edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { edge14, edge12, edge12Bis };
            edges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(new[] { edge12, edge12Bis }, edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { edge14, edge12 };
            edges = graph.GetEdges(2, 1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            var edge41 = Edge.Create(4, 1);
            data.ShouldReturnEdges = new[] { edge14, edge41 };
            edges = graph.GetEdges(1, 4);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            Assert.IsEmpty(graph.GetEdges(4, 1));
            data.CheckCalls(0);

            var edge45 = Edge.Create(4, 5);
            data.ShouldReturnEdges = new[] { edge14, edge41, edge45 };
            Assert.IsEmpty(graph.GetEdges(4, 5));
            data.CheckCalls(0);
        }

        protected static void GetOutEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IImplicitGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.OutEdges(1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            var edges = graph.OutEdges(1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 4), Edge.Create(1, 2) };
            edges = graph.OutEdges(1);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);
        }

        protected static void GetOutEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] DelegateVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.OutEdges(5));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnValue = true;
            var edges = graph.OutEdges(1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 4), Edge.Create(1, 2) };
            edges = graph.OutEdges(1);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = null;
            var outEdges = graph.OutEdges(1);
            CollectionAssert.IsEmpty(outEdges);
            data.CheckCalls(1);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge15 = Edge.Create(1, 5);
            var edge21 = Edge.Create(2, 1);
            var edge23 = Edge.Create(2, 3);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge15, edge21, edge23 };
            outEdges = graph.OutEdges(1);
            CollectionAssert.AreEqual(
                new[] { edge12, edge13 },
                outEdges);
            data.CheckCalls(1);

            var edge52 = Edge.Create(5, 2);
            data.ShouldReturnEdges = new[] { edge15, edge52 };
            Assert.IsNull(graph.OutEdges(5));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code
        }

        protected static void GetAdjacentEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] DelegateImplicitUndirectedGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.AdjacentEdges(1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            var edges = graph.AdjacentEdges(1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 4), Edge.Create(1, 2) };
            edges = graph.AdjacentEdges(1);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);
        }

        protected static void GetAdjacentEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] DelegateUndirectedGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.AdjacentEdges(5));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnValue = true;
            var edges = graph.AdjacentEdges(1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(1, 4), Edge.Create(1, 2) };
            edges = graph.AdjacentEdges(1);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = null;
            var adjacentEdges = graph.AdjacentEdges(1);
            CollectionAssert.IsEmpty(adjacentEdges);
            data.CheckCalls(1);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge15 = Edge.Create(1, 5);
            var edge21 = Edge.Create(2, 1);
            var edge23 = Edge.Create(2, 3);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge15, edge21, edge23 };
            adjacentEdges = graph.AdjacentEdges(1);
            CollectionAssert.AreEqual(
                new[] { edge12, edge13, edge21 },
                adjacentEdges);
            data.CheckCalls(1);

            var edge52 = Edge.Create(5, 2);
            data.ShouldReturnEdges = new[] { edge15, edge52 };
            Assert.IsNull(graph.AdjacentEdges(5));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code
        }

        protected static void GetAdjacentEdges_Throws_Test<TVertex, TEdge>(
            [NotNull] DelegateImplicitUndirectedGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AdjacentEdges(null));
        }

        protected static void GetInEdges_Test(
            [NotNull] GraphData<int, IEdge<int>> data,
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph)
        {
            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsNull(graph.InEdges(1));
            data.CheckCalls(1);

            data.ShouldReturnValue = true;
            var edges = graph.InEdges(1);
            CollectionAssert.IsEmpty(edges);
            data.CheckCalls(1);

            data.ShouldReturnEdges = new[] { Edge.Create(4, 1), Edge.Create(2, 1) };
            edges = graph.InEdges(1);
            CollectionAssert.AreEqual(data.ShouldReturnEdges, edges);
            data.CheckCalls(1);
        }

        #endregion

        #endregion
    }
}