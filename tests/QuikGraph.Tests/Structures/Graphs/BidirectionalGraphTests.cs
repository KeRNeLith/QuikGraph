using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BidirectionalGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AssertGraphProperties(graph);

            graph = new BidirectionalGraph<int, IEdge<int>>(true);
            AssertGraphProperties(graph);

            graph = new BidirectionalGraph<int, IEdge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new BidirectionalGraph<int, IEdge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new BidirectionalGraph<int, IEdge<int>>(false, 12);
            AssertGraphProperties(graph, false);

            graph = new BidirectionalGraph<int, IEdge<int>>(true, 42, 12);
            AssertGraphProperties(graph, edgeCapacity: 12);

            graph = new BidirectionalGraph<int, IEdge<int>>(false, 42, 12);
            AssertGraphProperties(graph, false, 12);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                BidirectionalGraph<TVertex, TEdge> g,
                bool parallelEdges = true,
                int edgeCapacity = 0)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
                Assert.AreEqual(edgeCapacity, g.EdgeCapacity);
                Assert.AreSame(typeof(int), g.VertexType);
                Assert.AreSame(typeof(IEdge<int>), g.EdgeType);
            }

            #endregion
        }

        #region Add Vertices

        [Test]
        public void AddVertex()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            AddVertex_Test(graph);
        }

        [Test]
        public void AddVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            AddVertex_Throws_Test(graph);
        }

        [Test]
        public void AddVertex_EquatableVertex()
        {
            var graph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            AddVertex_EquatableVertex_Test(graph);
        }

        [Test]
        public void AddVertexRange()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            AddVertexRange_Test(graph);
        }

        [Test]
        public void AddVertexRange_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            AddVertexRange_Throws_Test(graph);
        }

        #endregion

        #region Add Edges

        [Test]
        public void AddEdge_ParallelEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdge_ParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_ParallelEdges_EquatableEdge()
        {
            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
            AddEdge_ParallelEdges_EquatableEdge_Test(graph);
        }

        [Test]
        public void AddEdge_NoParallelEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>(false);
            AddEdge_NoParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_NoParallelEdges_EquatableEdge()
        {
            var graph = new BidirectionalGraph<int, EquatableEdge<int>>(false);
            AddEdge_NoParallelEdges_EquatableEdge_Test(graph);
        }

        [Test]
        public void AddEdge_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdge_Throws_Test(graph);
        }

        [Test]
        public void AddEdgeRange()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>(false);
            AddEdgeRange_Test(graph);
        }

        [Test]
        public void AddEdgeRange_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdgeRange_Throws_Test(graph);
        }

        #endregion

        #region Add Vertices & Edges

        [Test]
        public void AddVerticesAndEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AddVerticesAndEdge_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdge_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AddVerticesAndEdge_Throws_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>(false);
            AddVerticesAndEdgeRange_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>(false);
            AddVerticesAndEdgeRange_Throws_Test(graph);
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_Test(graph);
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var graph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_Test(graph);
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_Test(graph);
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_Test(graph);
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_Test(graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Test(graph);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            OutEdge_NullThrows_Test(graph1);

            var graph2 = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Throws_Test(graph2);
        }

        [Test]
        public void OutEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdges_Test(graph);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var graph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            OutEdges_NullThrows_Test(graph1);
        }

        [Test]
        public void OutEdgesEquatable_Throws()
        {
            var graph2 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            OutEdges_Throws_Test(graph2);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Test(graph);
        }

        [Test]
        public void InEdge_Throws()
        {
            var graph1 = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Throws_Test(graph1);

            var graph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            InEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void InEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            InEdges_Test(graph);
        }

        [Test]
        public void InEdges_Throws()
        {
            var graph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            InEdges_NullThrows_Test(graph1);

            var graph2 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            InEdges_Throws_Test(graph2);
        }

        #endregion

        [Test]
        public void Degree()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            Degree_Test(graph);
        }

        [Test]
        public void Degree_Throws()
        {
            var graph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            Degree_Throws_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            TryGetEdge_Test(graph);
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetEdges_Test(graph);
        }

        [Test]
        public void GetEdges_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetOutEdges_Test(graph);
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            GetOutEdges_Throws_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetInEdges_Test(graph);
        }

        [Test]
        public void GetInEdges_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            GetInEdges_Throws_Test(graph);
        }

        #endregion

        #region Merge

        public void Merge_Test(
            [NotNull] IEnumerable<int> setupVertices,
            [NotNull, ItemNotNull] IEnumerable<EquatableEdge<int>> setupEdges,
            int vertexToMerge,
            int expectedEdgesAdded,
            int expectedEdgesRemoved,
            [NotNull, ItemNotNull] IEnumerable<EquatableEdge<int>> expectedEdges)
        {
            int verticesAdded = 0;
            int edgesAdded = 0;
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();

            int[] verticesArray = setupVertices.ToArray();
            graph.AddVertexRange(verticesArray);
            graph.AddEdgeRange(setupEdges);

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++verticesAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                // ReSharper disable once AccessToModifiedClosure
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.MergeVertex(vertexToMerge, (source, target) => new EquatableEdge<int>(source, target));
            CheckCounters();
            graph.AssertHasVertices(verticesArray.Except(new[] { vertexToMerge }));
            graph.AssertHasEdges(expectedEdges);

            #region Local function

            void CheckCounters()
            {
                Assert.AreEqual(0, verticesAdded);
                Assert.AreEqual(1, verticesRemoved);
                Assert.AreEqual(expectedEdgesAdded, edgesAdded);
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                verticesRemoved = 0;
                edgesAdded = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void Merge1()
        {
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge13Bis = new EquatableEdge<int>(1, 3);
            var edge21 = new EquatableEdge<int>(2, 1);
            var edge23 = new EquatableEdge<int>(2, 3);
            var edge34 = new EquatableEdge<int>(3, 4);
            var edge35 = new EquatableEdge<int>(3, 5);
            var edge35Bis = new EquatableEdge<int>(3, 5);
            var edge45 = new EquatableEdge<int>(4, 5);

            Merge_Test(
                new[] { 1, 2, 3, 4, 5 },
                new[] { edge13, edge13Bis, edge21, edge23, edge34, edge35, edge35Bis, edge45 },
                3,
                9,
                6,
                new EquatableEdge<int>[]
                {
                    edge21, edge45,
                    new (1, 4),
                    new (1, 5),
                    new (1, 5),

                    new (1, 4),
                    new (1, 5),
                    new (1, 5),

                    new (2, 4),
                    new (2, 5),
                    new (2, 5)
                });
        }

        [Test]
        public void Merge2()
        {
            var edge23 = new EquatableEdge<int>(2, 3);
            var edge31 = new EquatableEdge<int>(3, 1);
            var edge33 = new EquatableEdge<int>(3, 3);
            var edge34 = new EquatableEdge<int>(3, 4);

            Merge_Test(
                new[] { 1, 2, 3, 4 },
                new[] { edge23, edge31, edge33, edge34 },
                3,
                2,
                4,
                new EquatableEdge<int>[]
                {
                    new(2, 1),
                    new(2, 4)
                });
        }

        [Test]
        public void Merge3()
        {
            var edge34 = new EquatableEdge<int>(3, 4);

            Merge_Test(
                new[] { 1, 2, 3, 4 },
                new[] { edge34 },
                1,
                0,
                0,
                new[] { edge34 });
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void Merge_Throws()
        {
            var graph1 = new BidirectionalGraph<int, IEdge<int>>();
            Assert.Throws<VertexNotFoundException>(() => graph1.MergeVertex(1, Edge.Create));

            var graph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            Assert.Throws<ArgumentNullException>(() => graph2.MergeVertex(null, Edge.Create));
            Assert.Throws<ArgumentNullException>(() => graph2.MergeVertex(new TestVertex("1"), null));
            Assert.Throws<ArgumentNullException>(() => graph2.MergeVertex(null, null));
        }

        public void MergeIf_Test(
            [NotNull] IEnumerable<int> setupVertices,
            [NotNull, ItemNotNull] IEnumerable<EquatableEdge<int>> setupEdges,
            [NotNull, InstantHandle] Func<int, bool> vertexPredicate,
            int expectedVerticesRemoved,
            int expectedEdgesAdded,
            int expectedEdgesRemoved,
            [NotNull] IEnumerable<int> expectedVertices,
            [NotNull, ItemNotNull] IEnumerable<EquatableEdge<int>> expectedEdges)
        {
            int verticesAdded = 0;
            int edgesAdded = 0;
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();

            graph.AddVertexRange(setupVertices);
            graph.AddEdgeRange(setupEdges);

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++verticesAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                // ReSharper disable once AccessToModifiedClosure
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.MergeVerticesIf(vertexPredicate, (source, target) => new EquatableEdge<int>(source, target));
            CheckCounters();
            graph.AssertHasVertices(expectedVertices);
            EquatableEdge<int>[] edges = expectedEdges.ToArray();
            if (!edges.Any())
                graph.AssertNoEdge();
            else
                graph.AssertHasEdges(edges);

            #region Local function

            void CheckCounters()
            {
                Assert.AreEqual(0, verticesAdded);
                Assert.AreEqual(expectedVerticesRemoved, verticesRemoved);
                Assert.AreEqual(expectedEdgesAdded, edgesAdded);
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                verticesRemoved = 0;
                edgesAdded = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void MergeIf1()
        {
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge13Bis = new EquatableEdge<int>(1, 3);
            var edge21 = new EquatableEdge<int>(2, 1);
            var edge23 = new EquatableEdge<int>(2, 3);
            var edge34 = new EquatableEdge<int>(3, 4);
            var edge35 = new EquatableEdge<int>(3, 5);
            var edge35Bis = new EquatableEdge<int>(3, 5);
            var edge45 = new EquatableEdge<int>(4, 5);

            MergeIf_Test(
                new[] { 1, 2, 3, 4, 5 },
                new[] { edge13, edge13Bis, edge21, edge23, edge34, edge35, edge35Bis, edge45 },
                vertex => vertex == 3 || vertex == 4,
                1 + 1,
                9 + 3,
                6 + 4,
                new[] { 1, 2, 5 },
                new[]
                {
                    edge21,
                    new (1, 5),
                    new (1, 5),

                    new (1, 5),
                    new (1, 5),

                    new (2, 5),
                    new (2, 5),


                    new (1, 5),
                    new (1, 5),
                    new (2, 5)
                });
        }

        [Test]
        public void MergeIf2()
        {
            var edge23 = new EquatableEdge<int>(2, 3);
            var edge31 = new EquatableEdge<int>(3, 1);
            var edge33 = new EquatableEdge<int>(3, 3);
            var edge34 = new EquatableEdge<int>(3, 4);

            MergeIf_Test(
                new[] { 1, 2, 3, 4 },
                new[] { edge23, edge31, edge33, edge34 },
                vertex => vertex == 3 || vertex == 4,
                1 + 1,
                2 + 0,
                4 + 1,
                new[] { 1, 2 },
                new[]
                {
                    new EquatableEdge<int>(2, 1)
                });
        }

        [Test]
        public void MergeIf3()
        {
            var edge34 = new EquatableEdge<int>(3, 4);

            MergeIf_Test(
                new[] { 1, 2, 3, 4 },
                new[] { edge34 },
                vertex => vertex == 1 || vertex == 2,
               1 + 1,
                0 + 0,
                0 + 0,
                new[] { 3, 4 },
                new[] { edge34 });
        }

        [Test]
        public void MergeIf4()
        {
            var edge34 = new EquatableEdge<int>(3, 4);

            MergeIf_Test(
                new[] { 1, 2, 3, 4 },
                new[] { edge34 },
                vertex => vertex == 1 || vertex == 3,
                1 + 1,
                0 + 0,
                0 + 1,
                new[] { 2, 4 },
                Enumerable.Empty<EquatableEdge<int>>());
        }

        [Test]
        public void MergeIf_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.MergeVerticesIf(null, Edge.Create));
            Assert.Throws<ArgumentNullException>(() => graph.MergeVerticesIf(_ => true, null));
            Assert.Throws<ArgumentNullException>(() => graph.MergeVerticesIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion

        #region Remove Vertices

        [Test]
        public void RemoveVertex()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveVertex_Test(graph);
        }

        [Test]
        public void RemoveVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveVertex_Throws_Test(graph);
        }

        [Test]
        public void RemoveVertexIf()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveVertexIf_Test(graph);

            graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveVertexIf_Test2(graph);
        }

        [Test]
        public void RemoveVertexIf_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveVertexIf_Throws_Test(graph);
        }

        #endregion

        #region Remove Edges

        [Test]
        public void RemoveEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveEdge_Test(graph);
        }

        [Test]
        public void RemoveEdge_EquatableEdge()
        {
            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
            RemoveEdge_EquatableEdge_Test(graph);
        }

        [Test]
        public void RemoveEdge_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdge_Throws_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveEdgeIf_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdgeIf_Throws_Test(graph);
        }

        [Test]
        public void RemoveOutEdgeIf()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveOutEdgeIf_Test(graph);
        }

        [Test]
        public void RemoveOutEdgeIf_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveOutEdgeIf_Throws_Test(graph);
        }

        [Test]
        public void RemoveInEdgeIf()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            RemoveInEdgeIf_Test(graph);
        }

        [Test]
        public void RemoveInEdgeIf_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            RemoveInEdgeIf_Throws_Test(graph);
        }

        #endregion

        #region Clear

        [Test]
        public void Clear()
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                // ReSharper disable once AccessToModifiedClosure
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounters(0, 0);

            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVerticesAndEdge(Edge.Create(2, 3));
            graph.AddVerticesAndEdge(Edge.Create(3, 1));

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounters(3, 3);

            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVerticesAndEdge(Edge.Create(3, 2));
            graph.AddVerticesAndEdge(Edge.Create(3, 1));
            graph.AddVerticesAndEdge(Edge.Create(3, 3));

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounters(3, 4);

            #region Local function

            void CheckCounters(int expectedVerticesRemoved, int expectedEdgesRemoved)
            {
                Assert.AreEqual(expectedVerticesRemoved, verticesRemoved);
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearOutEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            // Clear 1 => not in graph
            graph.ClearOutEdges(1);
            AssertEmptyGraph(graph);

            // Clear 1 => In graph but no out edges
            graph.AddVertex(1);
            graph.ClearOutEdges(1);
            graph.AssertHasVertices(1 );
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge23 );

            // Clear out 1
            graph.ClearOutEdges(1);

            graph.AssertHasEdges(edge23 );
            CheckCounter(1);

            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge31, edge32 );

            // Clear out 3
            graph.ClearOutEdges(3);

            graph.AssertHasEdges(edge12, edge13, edge23 );
            CheckCounter(2);

            // Clear out 1
            graph.ClearOutEdges(1);

            graph.AssertHasEdges(edge23 );
            CheckCounter(2);

            // Clear out 2 = Clear
            graph.ClearOutEdges(2);

            graph.AssertNoEdge();
            CheckCounter(1);

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearOutEdges_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BidirectionalGraph<TestVertex, IEdge<TestVertex>>().ClearOutEdges(null));
        }

        [Test]
        public void ClearInEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            // Clear 1 => not in graph
            graph.ClearInEdges(1);
            AssertEmptyGraph(graph);
            CheckCounter(0);

            // Clear 1 => In graph but no in edges
            graph.AddVertex(1);
            graph.ClearInEdges(1);
            graph.AssertHasVertices(1 );
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge23 );

            // Clear in 2
            graph.ClearInEdges(2);

            graph.AssertHasEdges(edge23 );
            CheckCounter(1);

            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge31, edge32 );

            // Clear in 3
            graph.ClearInEdges(3);

            graph.AssertHasEdges(edge12, edge31, edge32 );
            CheckCounter(2);

            // Clear in 1
            graph.ClearInEdges(1);

            graph.AssertHasEdges(edge12, edge32 );
            CheckCounter(1);

            // Clear 2 = Clear
            graph.ClearInEdges(2);

            graph.AssertNoEdge();
            CheckCounter(2);

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearInEdges_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BidirectionalGraph<TestVertex, IEdge<TestVertex>>().ClearInEdges(null));
        }

        [Test]
        public void ClearEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            // Clear 1 => not in graph
            graph.ClearEdges(1);
            AssertEmptyGraph(graph);
            CheckCounter(0);

            // Clear 1 => In graph but not in/out edges
            graph.AddVertex(1);
            graph.ClearEdges(1);
            graph.AssertHasVertices(1 );
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge23 );

            // Clear 2
            graph.ClearEdges(2);

            graph.AssertNoEdge();
            CheckCounter(2);

            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge31, edge32 );

            // Clear 3
            graph.ClearEdges(3);

            graph.AssertHasEdges(edge12 );
            CheckCounter(3);

            // Clear 1 = clear
            graph.ClearEdges(1);

            graph.AssertNoEdge();
            CheckCounter(1);

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearEdges_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BidirectionalGraph<TestVertex, IEdge<TestVertex>>().ClearEdges(null));
        }

        #endregion

        [Test]
        public void Clone()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            AssertEmptyGraph(graph);

            BidirectionalGraph<int, IEdge<int>> clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = new BidirectionalGraph<int, IEdge<int>>(graph);
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (BidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            graph.AddVertexRange( 1, 2, 3 );
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            clonedGraph = (BidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = new BidirectionalGraph<int, IEdge<int>>(graph);
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (BidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            graph.AddVertex(4);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (BidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }

        [Test]
        public void Clone_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BidirectionalGraph<int, IEdge<int>>(null));
        }

        [Test]
        public void TrimEdgeExcess()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>(true, 12)
            {
                EdgeCapacity = 50
            };

            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(1, 4)
            );

            Assert.DoesNotThrow(graph.TrimEdgeExcess);
        }
    }
}