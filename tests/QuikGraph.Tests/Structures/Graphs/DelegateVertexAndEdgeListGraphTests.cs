using System;
using System.Linq;
using NUnit.Framework;
using static QuikGraph.Tests.AssertHelpers;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DelegateVertexAndEdgeListGraphTests : DelegateGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                EmptyGetter<int, IEdge<int>>());
            AssertGraphProperties(graph);

            graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                EmptyGetter<int, IEdge<int>>(),
                false);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                DelegateVertexAndEdgeListGraph<TVertex, TEdge> g,
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
                () => new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(null, EmptyGetter<int, IEdge<int>>()));
            Assert.Throws<ArgumentNullException>(
                () => new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(Enumerable.Empty<int>(), null));
            Assert.Throws<ArgumentNullException>(
                () => new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
        
        #region Vertices & Edges

        [Test]
        public void Vertices()
        {
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                EmptyGetter<int, IEdge<int>>());
            AssertNoVertex(graph);
            AssertNoVertex(graph);

            graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                EmptyGetter<int, IEdge<int>>());
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasVertices(new[] { 1, 2, 3 });
        }

        [Test]
        public void Edges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                data.GetEdges);

            data.ShouldReturnValue = false;
            data.ShouldReturnEdges = null;
            graph.AssertNoEdge();

            data.ShouldReturnValue = true;
            graph.AssertNoEdge();

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            data.ShouldReturnEdges = new[] { edge12, edge13 };
            graph.AssertNoEdge();    // No vertex so no possible edge!

            graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);

            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = null;
            graph.AssertNoEdge();

            var edge22 = Edge.Create(2, 2);
            var edge31 = Edge.Create(3, 1);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge22, edge31 };
            graph.AssertHasEdges(new[] { edge12, edge13, edge22, edge31 });

            var edge15 = Edge.Create(1, 5);
            var edge51 = Edge.Create(5, 1);
            var edge56 = Edge.Create(5, 6);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge22, edge31, edge15, edge51, edge56 };
            // Some edges skipped because they have vertices not in the graph
            graph.AssertHasEdges(new[] { edge12, edge13, edge22, edge31 });
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                data.GetEdges);

            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsFalse(graph.ContainsVertex(1));
            data.CheckCalls(0); // Implementation override

            data.ShouldReturnValue = true;
            Assert.IsFalse(graph.ContainsVertex(1));
            data.CheckCalls(0); // Implementation override


            graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2 },
                data.GetEdges);
            data.ShouldReturnValue = false;
            Assert.IsFalse(graph.ContainsVertex(10));
            data.CheckCalls(0); // Implementation override
            Assert.IsTrue(graph.ContainsVertex(2));
            data.CheckCalls(0); // Implementation override

            data.ShouldReturnValue = true;
            Assert.IsFalse(graph.ContainsVertex(10));
            data.CheckCalls(0); // Implementation override
            Assert.IsTrue(graph.ContainsVertex(2));
            data.CheckCalls(0); // Implementation override
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);
            ContainsEdge_Test(data, graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                data.GetEdges);
            ContainsEdge_NullThrows_Test(graph);
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                Enumerable.Empty<int>(),
                data.GetEdges);

            data.CheckCalls(0);

            data.ShouldReturnValue = false;
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnValue = true;
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnEdges = new[] { Edge.Create(1, 3), Edge.Create(1, 2) };
            Assert.IsFalse(graph.ContainsEdge(1, 2));   // Vertices 1 and 2 are not part or the graph
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code


            graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 3 },
                data.GetEdges);

            data.ShouldReturnValue = false;
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnValue = true;
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            data.ShouldReturnEdges = new[] { Edge.Create(1, 2), Edge.Create(1, 3) };
            Assert.IsFalse(graph.ContainsEdge(1, 2));   // Vertices 2 is not part or the graph
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(2, 1));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            Assert.IsTrue(graph.ContainsEdge(1, 3));
            data.CheckCalls(1);
            Assert.IsFalse(graph.ContainsEdge(3, 1));
            data.CheckCalls(1);
        }

        [Test]
        public void ContainsEdge_SourceTarget_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                data.GetEdges);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);
            OutEdge_Test(data, graph);

            // Additional tests
            var edge14 = Edge.Create(1, 4);
            var edge12 = Edge.Create(1, 2);
            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge14, edge12 };
            Assert.AreSame(edge12, graph.OutEdge(1, 0));
            data.CheckCalls(1);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2 },
                data.GetEdges);
            OutEdge_Throws_Test(data, graph1);

            // Additional tests
            data.ShouldReturnValue = true;
            var edge32 = Edge.Create(3, 2);
            data.ShouldReturnEdges = new[] { edge32 };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(graph1.OutEdge(3, 0));
            data.CheckCalls(0); // Vertex is not in graph so no need to call user code

            var edge14 = Edge.Create(1, 4);
            var edge12 = Edge.Create(1, 2);
            data.ShouldReturnEdges = new[] { edge14, edge12 };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            AssertIndexOutOfRange(() => graph1.OutEdge(1, 1));
            data.CheckCalls(1);

            var graph2 = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            OutEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void OutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);
            OutEdges_Test(data, graph);

            // Additional tests
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge21 = Edge.Create(2, 1);
            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge12, edge13, edge14, edge21 };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            AssertHasOutEdges(graph, 1, new[] { edge12, edge13 });
            data.CheckCalls(3);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var data1 = new GraphData<int, IEdge<int>>();
            var graph1 = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new [] { 1 },
                data1.GetEdges);
            OutEdges_Throws_Test(data1, graph1);

            // Additional tests
            data1.ShouldReturnValue = true;
            var edge32 = Edge.Create(3, 2);
            data1.ShouldReturnEdges = new[] { edge32 };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(graph1.OutEdges(3));
            data1.CheckCalls(0); // Vertex is not in graph so no need to call user code


            var graph2 = new DelegateVertexAndEdgeListGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(
                Enumerable.Empty<EquatableTestVertex>(),
                EmptyGetter<EquatableTestVertex, Edge<EquatableTestVertex>>());
            OutEdges_NullThrows_Test(graph2);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);
            TryGetEdge_Test(data, graph);

            // Additional tests
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge21 = Edge.Create(2, 1);
            data.ShouldReturnValue = true;
            data.ShouldReturnEdges = new[] { edge13, edge14, edge21 };

            Assert.IsFalse(graph.TryGetEdge(1, 2, out IEdge<int> gotEdge));

            var edge12 = Edge.Create(1, 2);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge14, edge21 };
            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge12, gotEdge);

            var edge51 = Edge.Create(5, 1);
            var edge56 = Edge.Create(5, 6);
            data.ShouldReturnEdges = new[] { edge12, edge13, edge51, edge56 };
            Assert.IsFalse(graph.TryGetEdge(1, 5, out _));
            Assert.IsFalse(graph.TryGetEdge(5, 1, out _));
            Assert.IsFalse(graph.TryGetEdge(5, 6, out _));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                data.GetEdges);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3 },
                data.GetEdges);
            GetEdges_Test(data, graph);
        }

        [Test]
        public void GetEdges_Throws()
        {
            var data = new GraphData<TestVertex, IEdge<TestVertex>>();
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                data.GetEdges);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var data = new GraphData<int, IEdge<int>>();
            var graph = new DelegateVertexAndEdgeListGraph<int, IEdge<int>>(
                new[] { 1, 2, 3, 4 }, data.GetEdges);
            GetOutEdges_Test(data, graph);
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var graph = new DelegateVertexAndEdgeListGraph<TestVertex, IEdge<TestVertex>>(
                Enumerable.Empty<TestVertex>(),
                EmptyGetter<TestVertex, IEdge<TestVertex>>());
            GetOutEdges_Throws_Test(graph);
        }

        #endregion
    }
}