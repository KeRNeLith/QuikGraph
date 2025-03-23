using System;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="EdgeRecorderObserver{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdgeRecorderObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new EdgeRecorderObserver<int, IEdge<int>>();
            CollectionAssert.IsEmpty(recorder.Edges);

            var edge12 = Edge.Create(1, 2);
            var edge22 = Edge.Create(2, 2);
            var edge31 = Edge.Create(3, 1);
            recorder = new EdgeRecorderObserver<int, IEdge<int>>(
                edge12, edge22, edge31
            );
            CollectionAssert.AreEqual(
                new[] { edge12, edge22, edge31 },
                recorder.Edges);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new EdgeRecorderObserver<int, IEdge<int>>(null));
        }

        [Test]
        public void Attach()
        {
            // DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new EdgeRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.Edges);
                }
            }

            {
                var recorder = new EdgeRecorderObserver<int, IEdge<int>>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.Edges);
                }
            }

            {
                var edge12 = Edge.Create(1, 2);
                var recorder = new EdgeRecorderObserver<int, IEdge<int>>(edge12);

                var edge23 = Edge.Create(2, 3);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange( edge12, edge23 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { edge12, edge12, edge23 },  // Add without checking if edge already exists
                        recorder.Edges);
                }
            }

            {
                var recorder = new EdgeRecorderObserver<int, IEdge<int>>();

                var edge12 = Edge.Create(1, 2);
                var edge32 = Edge.Create(3, 2);   // Is not reachable
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange( edge12, edge32 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { edge12 },  // 3 -> 2 is not reachable (wrong orientation)
                        recorder.Edges);
                }
            }

            {
                var recorder = new EdgeRecorderObserver<int, IEdge<int>>();

                var edge12 = Edge.Create(1, 2);
                var edge22 = Edge.Create(2, 2);
                var edge23 = Edge.Create(2, 3);
                var edge34 = Edge.Create(3, 4);
                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(edge12, edge22, edge23, edge34);

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { edge12, edge23, edge34 },   // Self edge skipped
                        recorder.Edges);
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new EdgeRecorderObserver<int, IEdge<int>>());
        }
    }
}