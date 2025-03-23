using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;

namespace QuikGraph.Tests.Algorithms.Observers
{
    /// <summary>
    /// Tests for <see cref="VertexTimeStamperObserver{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class VertexTimeStamperObserverTests : ObserverTestsBase
    {
        [Test]
        public void Constructor()
        {
            var recorder = new VertexTimeStamperObserver<int>();
            CollectionAssert.IsEmpty(recorder.DiscoverTimes);
            CollectionAssert.IsEmpty(recorder.FinishTimes);

            var discoverTimes = new Dictionary<int, int>();
            recorder = new VertexTimeStamperObserver<int>(discoverTimes);
            Assert.AreSame(discoverTimes, recorder.DiscoverTimes);
            Assert.IsNull(recorder.FinishTimes);

            discoverTimes = new Dictionary<int, int>
            {
                [1] = 0,
                [2] = 1,
                [3] = 3,
                [4] = 2
            };
            recorder = new VertexTimeStamperObserver<int>(discoverTimes);
            Assert.AreSame(discoverTimes, recorder.DiscoverTimes);
            Assert.IsNull(recorder.FinishTimes);

            discoverTimes = new Dictionary<int, int>();
            var finishTimes = new Dictionary<int, int>();
            recorder = new VertexTimeStamperObserver<int>(
                discoverTimes,
                finishTimes);
            Assert.AreSame(discoverTimes, recorder.DiscoverTimes);
            Assert.AreSame(finishTimes, recorder.FinishTimes);

            discoverTimes = new Dictionary<int, int>
            {
                [1] = 0,
                [2] = 1,
                [3] = 3,
                [4] = 2
            };
            finishTimes = new Dictionary<int, int>
            {
                [1] = 4,
                [2] = 6,
                [3] = 5,
                [4] = 7
            };
            recorder = new VertexTimeStamperObserver<int>(
                discoverTimes,
                finishTimes);
            CollectionAssert.AreEqual(discoverTimes, recorder.DiscoverTimes);
            CollectionAssert.AreEqual(finishTimes, recorder.FinishTimes);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new VertexTimeStamperObserver<int>(null));
            Assert.Throws<ArgumentNullException>(() =>
                new VertexTimeStamperObserver<int>(new Dictionary<int, int>(), null));
            Assert.Throws<ArgumentNullException>(() =>
                new VertexTimeStamperObserver<int>(null, new Dictionary<int, int>()));
            Assert.Throws<ArgumentNullException>(() => new VertexTimeStamperObserver<int>(null, null));
            // ReSharper restore ObjectCreationAsStatement
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void Attach()
        {
            // DFS is used for tests but result may change if using another search algorithm
            // or another starting point
            {
                var recorder = new VertexTimeStamperObserver<int>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.IsEmpty(recorder.DiscoverTimes);
                    CollectionAssert.IsEmpty(recorder.FinishTimes);
                }
            }

            {
                var recorder = new VertexTimeStamperObserver<int>();

                var graph = new AdjacencyGraph<int, IEdge<int>>();
                graph.AddVertexRange(1, 2);

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
                using (recorder.Attach(dfs))
                {
                    dfs.Compute();

                    CollectionAssert.AreEqual(
                        new Dictionary<int, int>
                        {
                            [1] = 0,
                            [2] = 2
                        },
                        recorder.DiscoverTimes);

                    CollectionAssert.AreEqual(
                        new Dictionary<int, int>
                        {
                            [1] = 1,
                            [2] = 3
                        },
                        recorder.FinishTimes);
                }
            }

            {
                var recorder = new VertexTimeStamperObserver<int>();

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
                        new Dictionary<int, int>
                        {
                            [1] = 0,
                            [2] = 1,
                            [3] = 4,
                            [4] = 5
                        },
                        recorder.DiscoverTimes);

                    CollectionAssert.AreEqual(
                        new Dictionary<int, int>
                        {
                            [1] = 3,
                            [2] = 2,
                            [3] = 7,
                            [4] = 6
                        },
                        recorder.FinishTimes);
                }
            }
        }

        [Test]
        public void Attach_Throws()
        {
            Attach_Throws_Test(new VertexTimeStamperObserver<int>());
        }
    }
}