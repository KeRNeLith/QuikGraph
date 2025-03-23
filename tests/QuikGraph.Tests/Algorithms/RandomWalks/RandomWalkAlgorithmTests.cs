using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.RandomWalks;

namespace QuikGraph.Tests.Algorithms.RandomWalks
{
    /// <summary>
    /// Tests for <see cref="RandomWalkAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class RandomWalkAlgorithmTests : RootedAlgorithmTestsBase
    {

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public static void RunRandomWalkAndCheck<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph.VertexCount == 0)
                return;

            foreach (TVertex root in graph.Vertices)
            {
                RandomWalkAlgorithm<TVertex, TEdge> walker1 = CreateAlgorithm();
                bool calledStart1 = false;
                bool calledEnd1 = false;
                var encounteredEdges1 = new List<TEdge>();
                walker1.StartVertex += vertex =>
                {
                    Assert.IsFalse(calledStart1);
                    calledStart1 = true;
                    Assert.AreEqual(root, vertex);
                };
                walker1.TreeEdge += edge =>
                {
                    Assert.IsNotNull(edge);
                    encounteredEdges1.Add(edge);
                };
                walker1.EndVertex += vertex =>
                {
                    Assert.IsFalse(calledEnd1);
                    calledEnd1 = true;
                    Assert.IsNotNull(vertex);
                };

                RandomWalkAlgorithm<TVertex, TEdge> walker2 = CreateAlgorithm();
                bool calledStart2 = false;
                bool calledEnd2 = false;
                var encounteredEdges2 = new List<TEdge>();
                walker2.StartVertex += vertex =>
                {
                    Assert.IsFalse(calledStart2);
                    calledStart2 = true;
                    Assert.AreEqual(root, vertex);
                };
                walker2.TreeEdge += edge =>
                {
                    Assert.IsNotNull(edge);
                    encounteredEdges2.Add(edge);
                };
                walker2.EndVertex += vertex =>
                {
                    Assert.IsFalse(calledEnd2);
                    calledEnd2 = true;
                    Assert.IsNotNull(vertex);
                };

                RandomWalkAlgorithm<TVertex, TEdge> walker3 = CreateAlgorithm();
                bool calledStart3 = false;
                bool calledEnd3 = false;
                var encounteredEdges3 = new List<TEdge>();
                walker3.StartVertex += vertex =>
                {
                    Assert.IsFalse(calledStart3);
                    calledStart3 = true;
                    Assert.AreEqual(root, vertex);
                };
                walker3.TreeEdge += edge =>
                {
                    Assert.IsNotNull(edge);
                    encounteredEdges3.Add(edge);
                };
                walker3.EndVertex += vertex =>
                {
                    Assert.IsFalse(calledEnd3);
                    calledEnd3 = true;
                    Assert.IsNotNull(vertex);
                };

                var vis1 = new EdgeRecorderObserver<TVertex, TEdge>();
                using (vis1.Attach(walker1))
                    walker1.Generate(root);
                Assert.IsTrue(calledStart1);
                Assert.IsTrue(calledEnd1);

                walker2.SetRootVertex(root);
                var vis2 = new EdgeRecorderObserver<TVertex, TEdge>();
                using (vis2.Attach(walker2))
                    walker2.Compute();
                Assert.IsTrue(calledStart2);
                Assert.IsTrue(calledEnd2);

                var vis3 = new EdgeRecorderObserver<TVertex, TEdge>();
                using (vis3.Attach(walker3))
                    walker3.Generate(root, 100);
                Assert.IsTrue(calledStart3);
                Assert.IsTrue(calledEnd3);

                CollectionAssert.AreEqual(vis1.Edges, encounteredEdges1);
                CollectionAssert.AreEqual(vis1.Edges, encounteredEdges2);
                CollectionAssert.AreEqual(vis1.Edges, encounteredEdges3);
                CollectionAssert.AreEqual(vis1.Edges, vis2.Edges);
                CollectionAssert.AreEqual(vis1.Edges, vis3.Edges);
            }

            #region Local function

            RandomWalkAlgorithm<TVertex, TEdge> CreateAlgorithm()
            {
                var walker = new RandomWalkAlgorithm<TVertex, TEdge>(graph)
                {
                    EdgeChain = new NormalizedMarkovEdgeChain<TVertex, TEdge>
                    {
                        Rand = new Random(123456)
                    }
                };

                return walker;
            }

            #endregion
        }

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var chain = new WeightedMarkovEdgeChain<int, IEdge<int>>(new Dictionary<IEdge<int>, double>());
            Func<IEdge<int>, bool> predicate = _ => true;
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph, chain);
            AssertAlgorithmProperties(algorithm, graph, chain);

            algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph)
            {
                EndPredicate = predicate
            };
            AssertAlgorithmProperties(algorithm, graph, p: predicate);

            algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph)
            {
                EdgeChain = chain
            };
            AssertAlgorithmProperties(algorithm, graph, chain);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                RandomWalkAlgorithm<TVertex, TEdge> algo,
                IVertexListGraph<TVertex, TEdge> g,
                IEdgeChain<TVertex, TEdge> c = null,
                Func<TEdge, bool> p = null)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                if (c is null)
                    Assert.IsNotNull(algo.EdgeChain);
                else
                    Assert.AreSame(c , algo.EdgeChain);
                Assert.AreEqual(p, algo.EndPredicate);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var chain = new WeightedMarkovEdgeChain<int, IEdge<int>>(new Dictionary<IEdge<int>, double>());

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new RandomWalkAlgorithm<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomWalkAlgorithm<int, IEdge<int>>(graph, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomWalkAlgorithm<int, IEdge<int>>(null, chain));
            Assert.Throws<ArgumentNullException>(
                () => new RandomWalkAlgorithm<int, IEdge<int>>(null, null));

            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph, chain);
            Assert.Throws<ArgumentNullException>(() => algorithm.EdgeChain = null);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var algorithm = new RandomWalkAlgorithm<TestVertex, IEdge<TestVertex>>(graph);
            SetRootVertex_Null_Should_Throw_ArgumentNullException(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);
            ClearRootVertex_RaisesVertexChanged_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ComputeWithoutRoot_Throws_Test(
                () => new RandomWalkAlgorithm<int, IEdge<int>>(graph));
        }

        [Test]
        public void ComputeWithRoot()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertex(0);
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);
            ComputeWithRoot_Test(algorithm);
        }

        [Test]
        public void ComputeWithRoot_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ComputeWithUnknownRootOrNull_Throws_Test(
                () => new RandomWalkAlgorithm<TestVertex, IEdge<TestVertex>>(graph));
        }

        #endregion

        [Test]
        public void RandomWalkWithPredicate()
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            var edge4 = Edge.Create(3, 4);
            var edge5 = Edge.Create(4, 5);
            var edge6 = Edge.Create(5, 4);
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(edge1, edge2, edge3, edge4, edge5, edge6);
            var chain = new NormalizedMarkovEdgeChain<int, IEdge<int>>
            {
                Rand = new Random(123456)
            };

            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph, chain)
            {
                EndPredicate = edge => edge == edge4
            };

            var encounteredEdges = new List<IEdge<int>>();
            algorithm.TreeEdge += encounteredEdges.Add;
            algorithm.EndVertex += vertex => Assert.AreEqual(3, vertex); 

            algorithm.Generate(1, int.MaxValue);

            CollectionAssert.IsNotEmpty(encounteredEdges);
            Assert.AreEqual(3, encounteredEdges.Last().Target);
            Assert.IsTrue(
                edge2 == encounteredEdges.Last()
                || 
                edge3 == encounteredEdges.Last());
        }

        [Test]
        public void RandomWalk_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new RandomWalkAlgorithm<int, IEdge<int>>(graph);

            Assert.Throws<Exception>(() => algorithm.Generate(1));
            Assert.Throws<Exception>(() => algorithm.Generate(1, 12));
        }
    }
}