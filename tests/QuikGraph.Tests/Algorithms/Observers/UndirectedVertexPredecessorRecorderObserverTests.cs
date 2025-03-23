using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="UndirectedVertexPredecessorRecorderObserver{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedVertexPredecessorRecorderObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();
            CollectionAssert.IsEmpty(recorder.VerticesPredecessors);

            var predecessors = new Dictionary<int, IEdge<int>>();
            recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);

            predecessors = new Dictionary<int, IEdge<int>>
            {
                [1] = Edge.Create(2, 1)
            };
            recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>(predecessors);
            Assert.AreSame(predecessors, recorder.VerticesPredecessors);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>(null));
        }

        [Test]
        public void Attach()
        {
            // Undirected DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new UndirectedGraph<int, IEdge<int>>();

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                }
            }

            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new UndirectedGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.VerticesPredecessors);
                }
            }

            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var edge12 = Edge.Create(1, 2);
                var edge14 = Edge.Create(1, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var edge42 = Edge.Create(4, 2);
                var graph = new UndirectedGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge14, edge31, edge33, edge34, edge42
                );

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new Dictionary<int, IEdge<int>>
                        {
                            [2] = edge12,
                            [3] = edge34,
                            [4] = edge42
                        },
                        recorder.VerticesPredecessors);
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>());
        }

        [Test]
        public void TryGetPath()
        {
            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new UndirectedGraph<int, IEdge<int>>();

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    // Vertex not in the graph
                    Assert.IsNull(recorder.GetPath(2));
                }
            }

            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var graph = new UndirectedGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    Assert.IsNull(recorder.GetPath(2));
                }
            }

            {
                var recorder = new UndirectedVertexPredecessorRecorderObserver<int, IEdge<int>>();

                var edge12 = Edge.Create(1, 2);
                var edge14 = Edge.Create(1, 4);
                var edge31 = Edge.Create(3, 1);
                var edge33 = Edge.Create(3, 3);
                var edge34 = Edge.Create(3, 4);
                var edge42 = Edge.Create(4, 2);
                var graph = new UndirectedGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    edge12, edge14, edge31, edge33, edge34, edge42
                );

                var dfs = new UndirectedDepthFirstSearchAlgorithm<int, IEdge<int>>(graph);
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    var path = recorder.GetPath(4);
                    CollectionAssert.AreEqual(new[] { edge12, edge42 }, path);
                }
            }
        }

        [Test]
        public void TryGetPath_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new UndirectedVertexPredecessorRecorderObserver<TestVertex, IEdge<TestVertex>>()
                .GetPath(null));
        }
    }
}