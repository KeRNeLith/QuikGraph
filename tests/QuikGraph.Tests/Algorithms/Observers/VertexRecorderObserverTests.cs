using System;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="VertexRecorderObserver{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class VertexRecorderObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new VertexRecorderObserver<int>();
            CollectionAssert.IsEmpty(recorder.Vertices);

            recorder = new VertexRecorderObserver<int>(new[] { 1, 2, 3 });
            CollectionAssert.AreEqual(
                new[] { 1, 2, 3 },
                recorder.Vertices);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new VertexRecorderObserver<int>(null));
        }

        [Test]
        public void Attach()
        {
            // DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new VertexRecorderObserver<int>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.Vertices);
                }
            }

            {
                var recorder = new VertexRecorderObserver<int>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { 1, 2 },
                        recorder.Vertices);
                }
            }

            {
                var recorder = new VertexRecorderObserver<int>(new[] { 1 });

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange( 1, 2 );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { 1, 1, 2 },  // Add without checking if vertex already exists
                        recorder.Vertices);
                }
            }

            {
                var recorder = new VertexRecorderObserver<int>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVerticesAndEdgeRange(
                    Edge.Create(1, 2),
                    Edge.Create(2, 2),
                    Edge.Create(3, 4)
                );

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new[] { 1, 2, 3, 4 },
                        recorder.Vertices);
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new VertexRecorderObserver<int>());
        }
    }
}