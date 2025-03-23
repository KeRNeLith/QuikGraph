using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="VertexPredecessorRecorderObserver{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class VertexPredecessorRecorderObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();
            CollectionAssert.IsEmpty(recorder.VerticesPredecessors);

            var predecessors = new Dictionary<int, IEdge<int>>();
            recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);

            predecessors = new Dictionary<int, IEdge<int>>
            {
                [1] = Edge.Create(2, 1)
            };
            recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            IDictionary<int, IEdge<int>> verticesPredecessors = null;
            Assert.Throws<ArgumentNullException>(() => _ = new VertexPredecessorRecorderObserver<int, IEdge<int>>(verticesPredecessors));
        }

        [Test]
        public void Attach()
        {
            // DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

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
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

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
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new VertexPredecessorRecorderObserver<int, IEdge<int>>());
        }

        [Test]
        public void TryGetPath()
        {
            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    // Vertex not in the graph
                    Assert.IsNull(recorder.GetPath(2));
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    Assert.IsNull(recorder.GetPath(2));
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

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

                    var path = recorder.GetPath(4);
                    CollectionAssert.AreEqual(new[] { edge12, edge24 }, path);
                }
            }

            {
                var recorder = new VertexPredecessorRecorderObserver<int, IEdge<int>>();

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

                    var path = recorder.GetPath(4);
                    CollectionAssert.AreEqual(new[] { edge12, edge24 }, path);
                }
            }
        }

        [Test]
        public void TryGetPath_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => _ = new VertexPredecessorRecorderObserver<TestVertex, IEdge<TestVertex>>().GetPath(null));
        }
    }
}