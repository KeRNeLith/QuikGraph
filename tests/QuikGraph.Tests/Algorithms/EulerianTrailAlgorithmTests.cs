using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.QuikGraphUnitTestsHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary> Tests for <see cref="EulerianTrailAlgorithm{TVertex,TEdge}"/>. </summary>
    /// <inheritdoc cref="EulerianTrailAlgorithm{TVertex,TEdge}"/>
    [TestFixture]
    internal sealed class EulerianTrailAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        private static void ComputeTrailsAndCheck<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory,
            [NotNull, ItemNotNull] out ICollection<TEdge>[] trails,
            [NotNull, ItemNotNull] out TEdge[] circuit)
            where TEdge : IEdge<TVertex>
        {
            if (!graph.TryComputeTrails(edgeFactory, out trails, out circuit))
            {
                return;
            }

            // make sure all the edges are in the trail
            var edges = new HashSet<TEdge>();
            foreach (TEdge edge in graph.Edges)
                Assert.IsTrue(edges.Add(edge));

            foreach (ICollection<TEdge> trail in trails)
            {
                Assert.AreEqual(graph.EdgeCount, edges.Count);
                QuikGraphAssert.TrueForAll(trail, edges.Contains);
            }
        }

        private static void ComputeLongestTrails<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory,
            [NotNull, ItemNotNull] out ICollection<TEdge>[] trails,
            [NotNull, ItemNotNull] out TEdge[] circuit)
            where TEdge : IEdge<TVertex>
        {
            trails = new ICollection<TEdge>[0];
            circuit = new TEdge[0];

            int circuitCount = graph.ComputeEulerianPathCount();
            if (circuitCount == 0)
                return;

            TEdge[] graphEdges = graph.Edges.ToArray();

            var algorithm = graph.CreateEulerianTrailAlgorithm();
            TEdge[] tempEdges = algorithm.AddTemporaryEdges((s, t) => edgeFactory(s, t));
            TEdge[] augmentedGraphEdges = graph.Edges.ToArray();
            Assert.GreaterOrEqual(augmentedGraphEdges.Length, graphEdges.Length);
            TEdge[] temporaryEdges = augmentedGraphEdges.Except(graphEdges).ToArray();
            Assert.AreEqual(augmentedGraphEdges.Length - graphEdges.Length, temporaryEdges.Length);

            CollectionAssert.AreEquivalent(tempEdges, temporaryEdges);

            algorithm.Compute();
            trails = algorithm.Trails(root).ToArray();

            algorithm.RemoveTemporaryEdges();
            Assert.IsNotNull(algorithm.Circuit);
            circuit = algorithm.Circuit;

            // Lets make sure all the edges are in the trail
            var edges = new HashSet<TEdge>();
            foreach (TEdge edge in graph.Edges)
                Assert.IsTrue(edges.Add(edge));

            foreach (ICollection<TEdge> trail in trails)
            {
                Assert.AreEqual(graph.EdgeCount, edges.Count);
                QuikGraphAssert.TrueForAll(
                    trail,
                    // Edge in graph or part of temporary ones but is a root
                    edge => edges.Contains(edge)
                            || (temporaryEdges.Contains(edge) && Equals(edge.Source, root)));
            }
        }

        #endregion Test helpers

        static readonly AdjacencyGraph<int, IEdge<int>> graph = new(), nullGraph = null;

        [Test]
        public void Constructor()
        {
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateEulerianTrailAlgorithm(null);
            AssertAlgorithmProperties(algorithm, graph);
            return;

            void AssertAlgorithmProperties<TVertex, TEdge>(
                EulerianTrailAlgorithm<TVertex, TEdge> algo,
                IMutableVertexAndEdgeListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                CollectionAssert.IsEmpty(algo.Circuit);
            }
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEulerianTrailAlgorithm());
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateEulerianTrailAlgorithm(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        /// <inheritdoc cref="RootedAlgorithmTestsBase.TryGetRootVertex_Test{TVertex,TGraph}"/>
        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            TryGetRootVertex_Test(algorithm);
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.SetRootVertex_Test{TGraph}(RootedAlgorithmBase{int, TGraph})"/>
        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            SetRootVertex_Test(algorithm);
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.SetRootVertex_Null_Should_Throw_ArgumentNullException{TVertex,TGraph}"/>
        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.ClearRootVertex_RaisesVertexChanged_Test{TVertex,TGraph}"/>
        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.ComputeWithoutRoot_ShouldNotThrow_Test{TGraph}"/>
        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_ShouldNotThrow_Test(graph, ()
                => graph.CreateEulerianTrailAlgorithm());
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.ComputeWithRoot_Test{TVertex,TGraph}"/>
        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            ComputeWithRoot_Test(algorithm);
        }

        /// <inheritdoc cref="RootedAlgorithmTestsBase.ComputeWithUnknownRootOrNull_Throws_Test{TVertex,TGraph}"/>
        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(()
                => graph.CreateEulerianTrailAlgorithm());
        }

        #endregion Rooted algorithm

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> ComputeEulerianPathCountTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, IEdge<int>>();
                yield return new TestCaseData(emptyGraph) { ExpectedResult = 1 };

                var moreVerticesThanEdgesGraph = new AdjacencyGraph<int, IEdge<int>>();
                moreVerticesThanEdgesGraph.AddVertexRange( 1, 2 );
                moreVerticesThanEdgesGraph.AddEdge(Edge.Create(1, 2));
                yield return new TestCaseData(moreVerticesThanEdgesGraph) { ExpectedResult = 0 };


                var sameVerticesAndEdgesCountGraph = new AdjacencyGraph<int, IEdge<int>>();
                sameVerticesAndEdgesCountGraph.AddVertexRange( 1, 2 );
                sameVerticesAndEdgesCountGraph.AddEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 1)
                );
                yield return new TestCaseData(sameVerticesAndEdgesCountGraph) { ExpectedResult = 1 };


                var sameVerticesAndEdgesCountGraph2 = new AdjacencyGraph<int, IEdge<int>>();
                sameVerticesAndEdgesCountGraph2.AddVertexRange( 1, 2, 3 );
                sameVerticesAndEdgesCountGraph2.AddEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 1),
                    Edge.Create(1, 3)
                );
                yield return new TestCaseData(sameVerticesAndEdgesCountGraph2) { ExpectedResult = 1 };


                var moreEdgesThanEdgesGraph = new AdjacencyGraph<int, IEdge<int>>();
                moreEdgesThanEdgesGraph.AddVertexRange( 1, 2, 3, 4, 5 );
                moreEdgesThanEdgesGraph.AddEdgeRange(
                    Edge.Create(1, 5),
                    Edge.Create(1, 2),
                    Edge.Create(2, 1),

                    Edge.Create(1, 3),
                    Edge.Create(1, 4),
                    Edge.Create(3, 4),
                    Edge.Create(3, 4)
                );
                yield return new TestCaseData(moreEdgesThanEdgesGraph) { ExpectedResult = 2 };
            }
        }

        /// <inheritdoc cref="VertexAndEdgeListGraphX.ComputeEulerianPathCount{TVertex,TEdge}"/>
        [TestCaseSource(nameof(ComputeEulerianPathCountTestCases))]
        public int ComputeEulerianPathCount([NotNull] AdjacencyGraph<int, IEdge<int>> graph)
            => graph.ComputeEulerianPathCount();

        /// <inheritdoc cref="VertexAndEdgeListGraphX.ComputeEulerianPathCount{TVertex,TEdge}"/>
        [Test]
        public void ComputeEulerianPathCount_Throws() => Assert.Throws<ArgumentNullException>(()
            => _ = ComputeEulerianPathCount(null!));

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> AddTemporaryEdgesTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph, new EquatableEdge<int>[0]);


                var evenVerticesGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                evenVerticesGraph.AddVertexRange( 1, 2, 3, 4 );
                evenVerticesGraph.AddEdgeRange(
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 4),
                    new EquatableEdge<int>(3, 4)
                );
                yield return new TestCaseData(evenVerticesGraph, new EquatableEdge<int>[0]);


                var oddVerticesGraph1 = new AdjacencyGraph<int, EquatableEdge<int>>();
                oddVerticesGraph1.AddVertexRange( 1, 2, 3 );
                oddVerticesGraph1.AddEdgeRange(
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(1, 3)
                );
                yield return new TestCaseData(oddVerticesGraph1, new[] { new EquatableEdge<int>(1, 3) });


                var oddVerticesGraph2 = new AdjacencyGraph<int, EquatableEdge<int>>();
                oddVerticesGraph2.AddVertexRange( 1, 2, 3, 4, 5 );
                oddVerticesGraph2.AddEdgeRange(
                    new EquatableEdge<int>[] {
                        new (1, 2),
                        new (2, 1),
                        new (1, 4),
                        new (3, 1),
                        new (1, 5)
                    }
                );
                yield return new TestCaseData(oddVerticesGraph2,
                    new EquatableEdge<int>[] {
                        new (1, 4),
                        new (3, 5)
                        }
                    );


                var oddVerticesGraph3 = new AdjacencyGraph<int, EquatableEdge<int>>();
                oddVerticesGraph3.AddVertexRange( 1, 2, 3, 4, 5 );
                oddVerticesGraph3.AddEdgeRange(
                    new EquatableEdge<int>[]{
                        new (1, 2),
                        new (2, 1),
                        new (1, 3),
                        new (1, 4),
                        new (3, 4),
                        new (3, 4),
                        new (1, 5)
                    }
                );
                yield return new TestCaseData(oddVerticesGraph3,
                        new EquatableEdge<int>[] {
                            new (1, 3),
                            new (4, 5)
                        }
                    );
            }
        }

        /// <inheritdoc cref="EulerianTrailAlgorithm{TVertex,TEdge}.AddTemporaryEdges"/>
        [TestCaseSource(nameof(AddTemporaryEdgesTestCases))]
        public void Test_AddTemporaryEdges(
            [NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph,
            [NotNull, ItemNotNull] EquatableEdge<int>[] expectedTemporaryEdges)
        {
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            int edgeCount = graph.EdgeCount;
            EquatableEdge<int>[] tmpEdges = algorithm.AddTemporaryEdges(
                (source, target) => new EquatableEdge<int>(source, target));
            CollectionAssert.AreEquivalent(expectedTemporaryEdges, tmpEdges);

            Assert.AreEqual(edgeCount + tmpEdges.Length, graph.EdgeCount);
            EquatableEdge<int>[] graphEdges = graph.Edges.ToArray();
            foreach (EquatableEdge<int> edge in tmpEdges)
            {
                Assert.Contains(edge, graphEdges);
            }
        }

        /// <inheritdoc cref="EulerianTrailAlgorithm{TVertex,TEdge}.AddTemporaryEdges"/>
        [Test]
        public void AddTemporaryEdges_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.AddTemporaryEdges(null));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> RemoveTemporaryEdgesTestCases
        {
            [UsedImplicitly]
            get
            {
                var emptyGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                yield return new TestCaseData(emptyGraph);


                var evenVerticesGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
                evenVerticesGraph.AddVertexRange( 1, 2, 3, 4 );
                evenVerticesGraph.AddEdgeRange(
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 4),
                    new EquatableEdge<int>(3, 4)
                );
                yield return new TestCaseData(evenVerticesGraph);


                var oddVerticesGraph1 = new AdjacencyGraph<int, EquatableEdge<int>>();
                oddVerticesGraph1.AddVertexRange( 1, 2, 3 );
                oddVerticesGraph1.AddEdgeRange(
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(1, 3)
                );
                yield return new TestCaseData(oddVerticesGraph1);


                var oddVerticesGraph2 = new AdjacencyGraph<int, EquatableEdge<int>>();
                oddVerticesGraph2.AddVertexRange( 1, 2, 3, 4, 5 );
                oddVerticesGraph2.AddEdgeRange(
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(2, 1),
                    new EquatableEdge<int>(1, 4),
                    new EquatableEdge<int>(3, 1),
                    new EquatableEdge<int>(1, 5)
                );
                yield return new TestCaseData(oddVerticesGraph2);
            }
        }

        /// <inheritdoc cref="EulerianTrailAlgorithm{TVertex,TEdge}.RemoveTemporaryEdges"/>
        [TestCaseSource(nameof(RemoveTemporaryEdgesTestCases))]
        public void RemoveTemporaryEdges([NotNull] AdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            int edgeCount = graph.EdgeCount;
            EquatableEdge<int>[] tmpEdges = algorithm.AddTemporaryEdges(
                (source, target) => new EquatableEdge<int>(source, target));
            Assert.AreEqual(edgeCount + tmpEdges.Length, graph.EdgeCount);

            algorithm.RemoveTemporaryEdges();
            Assert.AreEqual(edgeCount, graph.EdgeCount);
        }

        #region Trails

        [Test]
        public void NotEulerianTrailGraph()
        {
            var graph = TestGraphFactory.LoadGraph(GetGraphFilePath("g.42.34.graphml"));
            Assert.True(graph.IsDirected);
            // No trails in tests graphs there
            ComputeTrailsAndCheck(graph, Edge.Create,
                out ICollection<IEdge<string>>[] trails,
                out IEdge<string>[] circuit);
            CollectionAssert.IsEmpty(trails);
            CollectionAssert.IsEmpty(circuit);
        }

        /// <summary> Directed Graph forms a 7-edge Loop with a Crossing at c: ab, bc, cd, de, ec, cf, fa </summary>
        [Test]
        public void SingleEulerianTrailGraph()
        {
            var bc = Edge.Create('b', 'c');
            var fa = Edge.Create('f', 'a');
            var ab = Edge.Create('a', 'b');
            var cd = Edge.Create('c', 'd');
            var ec = Edge.Create('e', 'c');
            var de = Edge.Create('d', 'e');
            var cf = Edge.Create('c', 'f');

            var graph = new AdjacencyGraph<char, IEdge<char>>();
            graph.AddVerticesAndEdgeRange(bc, fa, ab, cd, ec, de, cf);

            ComputeTrailsAndCheck(graph, Edge.Create, out var trails, out var circuit);

            IEdge<char>[] expectedTrail = { ab, bc, cd, de, ec, cf, fa };
            Assert.AreEqual(1, trails.Length);
            Assert.IsTrue(trails[0].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, trails[0]);

            Assert.IsTrue(circuit.IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, circuit);
            Assert.IsTrue(circuit.IsCircuit(graph.AreVerticesEqual));
        }

        [Test]
        public void SingleEulerianTrailGraph2()
        {
            var bc = Edge.Create('b', 'c');
            var fa = Edge.Create('f', 'a');
            var ab = Edge.Create('a', 'b');
            var cd = Edge.Create('c', 'd');
            var ec = Edge.Create('e', 'c');
            var de = Edge.Create('d', 'e');
            var cf = Edge.Create('c', 'f');
            var be = Edge.Create('b', 'e');

            var graph = new AdjacencyGraph<char, IEdge<char>>();
            graph.AddVerticesAndEdgeRange(bc, fa, ab, cd, ec, de, cf, be);

            ComputeTrailsAndCheck(graph, Edge.Create, out var trails, out var circuit);

            IEdge<char>[] expectedTrail = { ab, bc, cd, de, ec, cf, fa };
            Assert.AreEqual(1, trails.Length);
            Assert.IsTrue(trails[0].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, trails[0]);

            Assert.IsTrue(circuit.IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, circuit);
            Assert.IsTrue(circuit.IsCircuit());
        }

        [Test]
        public void SingleEulerianTrailGraph3()
        {
            var _12 = Edge.Create(1, 2);
            var _21 = Edge.Create(2, 1);

            var _13 = Edge.Create(1, 3);
            var _31 = Edge.Create(3, 1);

            var _24 = Edge.Create(2, 4);
            var _42 = Edge.Create(4, 2);

            var _34 = Edge.Create(3, 4);
            var _43 = Edge.Create(4, 3);

            var _44 = Edge.Create(4, 4);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(_12, _21, _13, _31, _24, _42, _34, _43, _44);

            ComputeTrailsAndCheck(graph, Edge.Create, out ICollection<IEdge<int>>[] trails, out IEdge<int>[] circuit);

            IEdge<int>[] expectedTrail = { _13, _34, _44, _42, _24, _43, _31, _12, _21 };
            Assert.AreEqual(1, trails.Length);
            Assert.IsTrue(trails[0].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, trails[0]);

            Assert.IsTrue(circuit.IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, circuit);
            Assert.IsTrue(circuit.IsCircuit());
        }

        [Test]
        public void MultipleEulerianTrailsGraph()
        {
            var _12 = Edge.Create(1, 2);
            var _21 = Edge.Create(2, 1);

            var _13 = Edge.Create(1, 3);
            var _31 = Edge.Create(3, 1);

            var _42 = Edge.Create(4, 2);

            var _34 = Edge.Create(3, 4);
            var _43 = Edge.Create(4, 3);

            var _44 = Edge.Create(4, 4);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(_12, _21, _13, _31, _42, _34, _43, _44);

            ComputeTrailsAndCheck(graph, Edge.Create, out ICollection<IEdge<int>>[] trails, out IEdge<int>[] circuit);

            IEdge<int>[] expectedTrail1 = { _13, _34, _44, _42 }; //_12 missing
            IEdge<int>[] expectedTrail2 = { _43, _31, _12, _21 }; //_44 and _42 missing
            Assert.AreEqual(2, trails.Length);
            Assert.IsTrue(trails[0].IsPath());
            Assert.IsTrue(trails[1].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail1, trails[0]);
            CollectionAssert.AreEquivalent(expectedTrail2, trails[1]);

            Assert.IsTrue(circuit.IsPath());
            Assert.AreEqual(expectedTrail1.Length + expectedTrail2.Length + 1 /* Temporary edge */,
                circuit.Length);
            foreach (var edge in expectedTrail1.Concat(expectedTrail2))
            {
                Assert.Contains(edge, circuit);
            }
            // + Temporary edge
            Assert.IsNotNull(circuit.FirstOrDefault(e => e.Source == 2 && e.Target == 4));
        }

        #endregion

        #region Rooted trails

        [Test]
        public void RootedNotEulerianTrailGraph_Throws()
        {
            var graph = TestGraphFactory.LoadGraph(GetGraphFilePath("g.10.0.graphml"));
            Assert.Throws<InvalidOperationException>(()
                => ComputeLongestTrails(graph, graph.Vertices.First(), Edge.Create, out _, out _));
        }

        /// <summary>
        /// Single 7-Path cd,de,ec,cf,fa,ab,bc,closed, rooted in c andcrossing in c,
        /// but leaving out 'be'.
        /// Starting at 'c'
        /// </summary>
        [Test]
        public void SingleRootedEulerianTrailGraph()
        {
            var bc = Edge.Create('b', 'c');
            var fa = Edge.Create('f', 'a');
            var ab = Edge.Create('a', 'b');
            var cd = Edge.Create('c', 'd');
            var ec = Edge.Create('e', 'c');
            var de = Edge.Create('d', 'e');
            var cf = Edge.Create('c', 'f');
            var be = Edge.Create('b', 'e');

            var graph = new AdjacencyGraph<char, IEdge<char>>();
            graph.AddVerticesAndEdgeRange(bc, fa, ab, cd, ec, de, cf, be);

            ComputeLongestTrails(graph, 'c', Edge.Create, out var trails, out var circuit);

            IEdge<char>[] expectedTrail = { cd, de, ec, cf, fa, ab, bc };
            Assert.AreEqual(1, trails.Length); //only 1 trail
            Assert.IsTrue(trails[0].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, trails[0]);
            Assert.AreEqual('c', trails[0].ElementAt(0).Source);

            Assert.IsTrue(circuit.IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, circuit);
            Assert.IsTrue(circuit.IsCircuit());
        }

        /// <summary> 9 Edges, 8 of them bidirectional and one self-edge 4-4 </summary>
        /// <remarks>
        /// Starting at 4, a full Cycle is reported both as Trail and Circuit.
        /// Expected Trail is _44, _42, _24, _43, _31, _12, _21, _13, _34
        /// </remarks>
        [Test]
        public void SingleRootedEulerianTrailGraph2()
        {
            var _12 = Edge.Create(1, 2);
            var _21 = Edge.Create(2, 1);

            var _13 = Edge.Create(1, 3);
            var _31 = Edge.Create(3, 1);

            var _24 = Edge.Create(2, 4);
            var _42 = Edge.Create(4, 2);

            var _34 = Edge.Create(3, 4);
            var _43 = Edge.Create(4, 3);

            var _44 = Edge.Create(4, 4);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(_12, _21, _13, _31, _24, _42, _34, _43, _44);

            ComputeLongestTrails(graph, 4, Edge.Create, out var trails, out var circuit);

            IEdge<int>[] expectedTrail = { _44, _42, _24, _43, _31, _12, _21, _13, _34 };
            Assert.AreEqual(1, trails.Length);
            Assert.IsTrue(trails[0].IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, trails[0]);
            Assert.AreEqual(4, trails[0].ElementAt(0).Source);

            Assert.IsTrue(circuit.IsPath());
            CollectionAssert.AreEquivalent(expectedTrail, circuit);
            Assert.IsTrue(circuit.IsCircuit());
        }

        [Test]
        public void MultipleRootedEulerianTrailsGraph()
        {
            var _12 = new EquatableEdge<int>(1, 2);
            var _21 = new EquatableEdge<int>(2, 1);

            var _13 = new EquatableEdge<int>(1, 3);
            var _31 = new EquatableEdge<int>(3, 1);

            var _42 = new EquatableEdge<int>(4, 2);

            var _34 = new EquatableEdge<int>(3, 4);
            var _43 = new EquatableEdge<int>(4, 3);

            var _44 = new EquatableEdge<int>(4, 4);

            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(_12, _21, _13, _31, _42, _34, _43, _44);

            // start at Root 2
            ComputeLongestTrails(graph, 2, (s, t) => new EquatableEdge<int>(s, t),
                out ICollection<EquatableEdge<int>>[] trails,
                out EquatableEdge<int>[] circuit);
            EquatableEdge<int>[] trail1 = { _21, _13, _34, _44, _42 }; /* Include temporary edge */
            EquatableEdge<int>[] trail2 = { new EquatableEdge<int>(2, 4), _43, _31, _12 };
            CheckTrails(trails, trail1, trail2);

            Assert.IsTrue(circuit.IsPath());
            Assert.AreEqual(trail1.Length + trail2.Length, circuit.Length);
            foreach (EquatableEdge<int> edge in trail1.Concat(trail2))
            {
                Assert.Contains(edge, circuit);
            }

            // start at Root 3
            ComputeLongestTrails(graph, 3, (s, t) => new EquatableEdge<int>(s, t), out trails, out circuit);
            trail1 = new[] { _34, _44, _42 };
            trail2 = new[] { _34, _43, _31, _12, _21, _13 };
            CheckTrails(trails, trail1, trail2);

            Assert.IsTrue(circuit.IsPath());
            Assert.AreEqual(
                trail1.Concat(trail2).Distinct().Count() /* duplicate Edge present in both paths */ + 1 /* One temporary edge */,
                circuit.Length);
            foreach (EquatableEdge<int> edge in trail1.Concat(trail2))
            {
                Assert.Contains(edge, circuit);
            }
            // + Temporary edge
            Assert.IsNotNull(circuit.FirstOrDefault(e => e.Source == 2 && e.Target == 4));
            return;

            void CheckTrails(
                IList<ICollection<EquatableEdge<int>>> computedTrails,
                IEnumerable<EquatableEdge<int>> expectedTrail1,
                IEnumerable<EquatableEdge<int>> expectedTrail2)
            {
                Assert.AreEqual(2, computedTrails.Count);
                Assert.IsTrue(computedTrails[0].Cast<IEdge<int>>().IsPath());
                Assert.IsTrue(computedTrails[1].Cast<IEdge<int>>().IsPath());
                CollectionAssert.AreEquivalent(expectedTrail1, computedTrails[0]);
                CollectionAssert.AreEquivalent(expectedTrail2, computedTrails[1]);
            }
        }

        [Test]
        public void RootedEulerianTrails_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateEulerianTrailAlgorithm();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Trails(null));
        }

        #endregion
    }
}