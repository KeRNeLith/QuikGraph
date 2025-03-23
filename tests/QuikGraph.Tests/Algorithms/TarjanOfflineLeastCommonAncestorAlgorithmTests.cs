using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;


namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="TarjanOfflineLeastCommonAncestorAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class TarjanOfflineLeastCommonAncestorAlgorithmTests : RootedAlgorithmTestsBase
    {
        #region Test helpers

        public void RunTarjanOfflineLeastCommonAncestorAndCheck<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] SEquatableEdge<TVertex>[] pairs)
            where TEdge : IEdge<TVertex>
        {
            TryFunc<SEquatableEdge<TVertex>, TVertex> lca = graph.OfflineLeastCommonAncestor(root, pairs);
            var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            var dfs = graph.CreateDepthFirstSearchAlgorithm();
            using (predecessors.Attach(dfs))
                dfs.Compute(root);

            foreach (SEquatableEdge<TVertex> pair in pairs)
            {
                if (lca(pair, out TVertex _))
                {
                    bool isPredecessor = predecessors.VerticesPredecessors.IsPredecessor(root, pair.Source);
                    Assert.IsTrue(isPredecessor);
                    bool predecessor = predecessors.VerticesPredecessors.IsPredecessor(root, pair.Target);
                    Assert.IsTrue(predecessor);
                }
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm(null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                CollectionAssert.IsEmpty(algo.Ancestors);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            IVertexListGraph<int, IEdge<int>> nullGraph = null;
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => nullGraph.CreateTarjanOfflineLeastCommonAncestorAlgorithm(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_Throws_Test(()
                => graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm());
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 0, 1 );
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            algorithm.SetVertexPairs(new SEquatableEdge<int>(0, 1) );
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm());
        }

        #endregion

        [Test]
        public void TryGetVertexPairs()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            Assert.IsNull(algorithm.VertexPairs());

            graph.AddVertexRange( 1, 2 );
            algorithm.SetVertexPairs(new SEquatableEdge<int>(1, 2) );
            var pairs = algorithm.VertexPairs();
            CollectionAssert.AreEqual(
                new[] { new SEquatableEdge<int>(1, 2) },
                pairs);
        }

        [Test]
        public void SetVertexPairs()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2 );
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();

            var pairs = new[]
            {
                new SEquatableEdge<int>(1, 2),
                new SEquatableEdge<int>(2, 1)
            };
            Assert.IsNull(algorithm.VertexPairs());
            algorithm.SetVertexPairs(pairs);
            var gotPairs = (algorithm.VertexPairs());
            CollectionAssert.AreEqual(pairs, gotPairs);
        }

        [Test]
        public void SetVertexPairs_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.SetVertexPairs(null));
            Assert.Throws<ArgumentException>(() => algorithm.SetVertexPairs(Enumerable.Empty<SEquatableEdge<int>>()));
            Assert.Throws<ArgumentException>(() => algorithm.SetVertexPairs(new SEquatableEdge<int>(1, 2)));
            graph.AddVertex(1);
            Assert.Throws<ArgumentException>(() => algorithm.SetVertexPairs(new SEquatableEdge<int>(1, 2)));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_SlowTests), new object[] { -1 })]
        public void TarjanOfflineLeastCommonAncestor(AdjacencyGraph<string, IEdge<string>> graph)
        {
            if (graph.VertexCount == 0)
                return;

            var pairs = new List<SEquatableEdge<string>>();
            foreach (string u in graph.Vertices)
            {
                foreach (string v in graph.Vertices)
                {
                    if (!u.Equals(v))
                        pairs.Add(new SEquatableEdge<string>(u, v));
                }
            }

            int count = 0;
            foreach (string root in graph.Vertices)
            {
                RunTarjanOfflineLeastCommonAncestorAndCheck(
                    graph,
                    root,
                    pairs.ToArray());

                if (count++ > 10)
                    break;
            }
        }

        [Test]
        public void TarjanOfflineLeastCommonAncestor_Throws()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var vertex3 = new TestVertex("3");
            var pairs = new[] { new SEquatableEdge<TestVertex>(vertex1, vertex2) };
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            graph.AddVertexRange( vertex1, vertex2 );
            var algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, pairs));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(vertex1, null));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, null));
            Assert.Throws<ArgumentException>(() => algorithm.Compute(vertex3, pairs));
            // ReSharper restore AssignNullToNotNullAttribute

            algorithm = graph.CreateTarjanOfflineLeastCommonAncestorAlgorithm();
            algorithm.SetRootVertex(vertex1);
            Assert.Throws<InvalidOperationException>(() => algorithm.Compute());
        }
    }
}