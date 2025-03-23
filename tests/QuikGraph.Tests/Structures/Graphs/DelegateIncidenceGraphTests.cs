using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="DelegateIncidenceGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DelegateIncidenceGraphTests : DelegateGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>());
            AssertGraphProperties(graph);

            graph = new DelegateIncidenceGraph<int, IEdge<int>>(
                EmptyGetter<int, IEdge<int>>(),
                false);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                DelegateIncidenceGraph<TVertex, TEdge> g,
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
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new DelegateIncidenceGraph<int, IEdge<int>>(null));
        }

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            ContainsVertex_Test(data, graph);
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            ContainsEdge_SourceTarget_Test(data, graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(data.GetEdges);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            OutEdge_Test(data, graph);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            OutEdge_NullThrows_Test(graph1);

            var data = new GraphData<int, IEdge<int>>();
            var graph2 = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            OutEdge_Throws_Test(data, graph2);
        }

        [Test]
        public void OutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            OutEdges_Test(data, graph);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            OutEdges_Throws_Test(data, graph1);

            var graph2 = new DelegateIncidenceGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>());
            OutEdges_NullThrows_Test(graph2);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            TryGetEdge_Test(data, graph);
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(data.GetEdges);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            GetEdges_Test(data, graph);
        }

        [Test]
        public void GetEdges_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(data.GetEdges);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateIncidenceGraph<int, IEdge<int>>(data.GetEdges);
            GetOutEdges_Test(data, graph);
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var graph = new DelegateIncidenceGraph<TestVertex, IEdge<TestVertex>>(
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            GetOutEdges_Throws_Test(graph);
        }

        #endregion
    }
}