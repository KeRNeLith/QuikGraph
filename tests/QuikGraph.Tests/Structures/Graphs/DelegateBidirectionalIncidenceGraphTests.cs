using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="DelegateBidirectionalIncidenceGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DelegateBidirectionalIncidenceGraphTests : DelegateGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                EmptyGetter<int, IEdge<int>>());
            AssertGraphProperties(graph);

            graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                EmptyGetter<int, IEdge<int>>(),
                false);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                DelegateBidirectionalIncidenceGraph<TVertex, TEdge> g,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                    EmptyGetter<int, IEdge<int>>(),
                    null));
            Assert.Throws<ArgumentNullException>(
                () => new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                    null,
                    EmptyGetter<int, IEdge<int>>()));
            Assert.Throws<ArgumentNullException>(
                () => new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            ContainsVertex_Test(data, graph);
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new DelegateBidirectionalIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            OutEdge_Test(data, graph);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new DelegateBidirectionalIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            OutEdge_NullThrows_Test(graph1);

            var data = new GraphData<int, IEdge<int>>();
            var graph2 = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            OutEdge_Throws_Test(data, graph2);
        }

        [Test]
        public void OutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            OutEdges_Test(data, graph);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            OutEdges_Throws_Test(data, graph1);

            var graph2 = new DelegateBidirectionalIncidenceGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>(),
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>());
            OutEdges_NullThrows_Test(graph2);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                data.GetEdges);
            InEdge_Test(data, graph);
        }

        [Test]
        public void InEdge_Throws()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                data.GetEdges);
            InEdge_Throws_Test(data, graph1);

            var graph2 = new DelegateBidirectionalIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            InEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void InEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                data.GetEdges);
            InEdges_Test(data, graph);
        }

        [Test]
        public void InEdges_Throws()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                data.GetEdges);
            InEdges_Throws_Test(data, graph1);

            var graph2 = new DelegateBidirectionalIncidenceGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>(),
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>());
            InEdges_NullThrows_Test(graph2);
        }

        #endregion

        [Test]
        public void Degree()
        {
            var data1 = new GraphData<int, IEdge<int>>();
            var data2 = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data1.GetEdges,
                data2.GetEdges);
            Degree_Test(data1, data2, graph);
        }

        [Test]
        public void Degree_Throws()
        {
            var graph = new DelegateBidirectionalIncidenceGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>(),
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>());
            Degree_Throws_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void GetOutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                data.GetEdges,
                EmptyGetter<int, IEdge<int>>());
            GetOutEdges_Test(data, graph);
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var graph = new DelegateBidirectionalIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            GetOutEdges_Throws_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateBidirectionalIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                data.GetEdges);
            GetInEdges_Test(data, graph);
        }

        [Test]
        public void GetInEdges_Throws()
        {
            var graph = new DelegateBidirectionalIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            GetInEdges_Throws_Test(graph);
        }

        #endregion
    }
}