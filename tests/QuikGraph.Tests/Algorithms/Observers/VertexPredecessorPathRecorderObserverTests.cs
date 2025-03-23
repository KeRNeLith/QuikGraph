using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="VertexPredecessorPathRecorderObserver{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class VertexPredecessorPathRecorderObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();
            CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
            CollectionAssert.IsEmpty(recorder.EndPathVertices);

            var predecessors = new Dictionary<int, IEdge<int>>();
            recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);
            CollectionAssert.IsEmpty(recorder.EndPathVertices);

            predecessors = new Dictionary<int, IEdge<int>>
            {
                [1] = Edge.Create(2, 1)
            };
            recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);
            CollectionAssert.IsEmpty(recorder.EndPathVertices);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(()
                => _ = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>((IDictionary<int, IEdge<int>>)null));
        }

        [Test]
        public void Attach()
        {
            // DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                    CollectionAssert.IsEmpty(recorder.EndPathVertices);
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                    CollectionAssert.AreEquivalent(new[] { 1, 2 }, recorder.EndPathVertices);
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                // Graph without cycle
                var edge12 = Edge.Create(1, 2);
                var edge13 = Edge.Create(1, 3);
                var edge14 = Edge.Create(1, 4);
                var edge24 = Edge.Create(2, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge13, edge14, edge24, edge31, edge33, edge34
                );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new Dictionary<int, IEdge<int>>
                        {
                            [2] = edge12,
                            [3] = edge13,
                            [4] = edge24
                        }, 
                        recorder.VerticesPredecessors);
                    CollectionAssert.AreEquivalent(new[] { 3, 4 }, recorder.EndPathVertices);
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                // Graph with cycle
                var edge12 = Edge.Create(1, 2);
                var edge13 = Edge.Create(1, 3);
                var edge14 = Edge.Create(1, 4);
                var edge24 = Edge.Create(2, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var edge41 = Edge.Create(4, 1);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge13, edge14, edge24, edge31, edge33, edge34, edge41
                );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new Dictionary<int, IEdge<int>>
                        {
                            [2] = edge12,
                            [3] = edge13,
                            [4] = edge24
                        },
                        recorder.VerticesPredecessors);
                    CollectionAssert.AreEquivalent(new[] { 3, 4 }, recorder.EndPathVertices);
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new VertexPredecessorPathRecorderObserver<int, IEdge<int>>());
        }

        [Test]
        public void AllPaths()
        {
            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.AllPaths());
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.AllPaths());
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                // Graph without cycle
                var edge12 = Edge.Create(1, 2);
                var edge13 = Edge.Create(1, 3);
                var edge14 = Edge.Create(1, 4);
                var edge24 = Edge.Create(2, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge13, edge14, edge24, edge31, edge33, edge34
                );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEquivalent(
                        new IEnumerable<IEdge<int>>[]
                        {
                            new[] { edge13 },
                            new[] { edge12, edge24 }
                        },
                        recorder.AllPaths());
                }
            }

            {
                var recorder = new VertexPredecessorPathRecorderObserver<int, IEdge<int>>();

                // Graph with cycle
                var edge12 = Edge.Create(1, 2);
                var edge13 = Edge.Create(1, 3);
                var edge14 = Edge.Create(1, 4);
                var edge24 = Edge.Create(2, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var edge41 = Edge.Create(4, 1);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge13, edge14, edge24, edge31, edge33, edge34, edge41
                );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEquivalent(
                        new IEnumerable<IEdge<int>>[]
                        {
                            new[] { edge13 },
                            new[] { edge12, edge24 }
                        },
                        recorder.AllPaths());
                }
            }
        }
    }
}