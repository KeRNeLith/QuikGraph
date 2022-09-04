using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Collections;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="FibonacciQueue{TVertex,TDistance}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FibonacciQueueTests : QueueTestsBase
    {
        [Test]
        public void Constructor()
        {
            AssertQueueProperties(
                new FibonacciQueue<int, double>(_ => 1.0));


            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    0,
                    null,
                    _ => 1.0));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    12,
                    null,
                    _ => 1.0));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    0,
                    new[] { 1, 2 }, // Marked as removed
                    _ => 1.0));


            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    0,
                    null,
                    _ => 1.0,
                    (dist1, dist2) => dist1.CompareTo(dist2)));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    12,
                    null,
                    _ => 1.0,
                    (dist1, dist2) => dist1.CompareTo(dist2)));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    0,
                    new[] { 1, 2, 3 }, // Marked as removed
                    _ => 1.0,
                    (dist1, dist2) => dist1.CompareTo(dist2)));


            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    new Dictionary<int, double>()));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    new Dictionary<int, double> { [1] = 12.0, [2] = 42.0 }));   // Marked as removed


            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    new Dictionary<int, double>(),
                    (dist1, dist2) => dist1.CompareTo(dist2)));

            AssertQueueProperties(
                new FibonacciQueue<int, double>(
                    new Dictionary<int, double> { [1] = 12.0, [2] = 42.0 }, // Marked as removed
                    (dist1, dist2) => dist1.CompareTo(dist2)));

            #region Local function

            void AssertQueueProperties<TVertex, TDistance>(
                FibonacciQueue<TVertex, TDistance> queue)
            {
                Assert.AreEqual(0, queue.Count);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>((Func<int, double>)null));


            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), _ => 1.0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, Enumerable.Empty<int>(), null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, null, null));


            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), _ => 1.0, (dist1, dist2) => dist1.CompareTo(dist2)));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), null, (dist1, dist2) => dist1.CompareTo(dist2)));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), _ => 1.0, null));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FibonacciQueue<int, double>(-1, Enumerable.Empty<int>(), null, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, Enumerable.Empty<int>(), _ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, Enumerable.Empty<int>(), null, (dist1, dist2) => dist1.CompareTo(dist2)));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, Enumerable.Empty<int>(), null, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(12, null, null, null));


            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>((Dictionary<int, double>)null));


            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(new Dictionary<int, double>(), null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(null, (dist1, dist2) => dist1.CompareTo(dist2)));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciQueue<int, double>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Contains()
        {
            Contains_Test(
                new FibonacciQueue<int, double>(_ => 1.0),
                1,
                2);
            Contains_Test(
                new FibonacciQueue<TestVertex, double>(_ => 1.0),
                new TestVertex("1"),
                new TestVertex("2"));
            Contains_Test(
                new FibonacciQueue<EquatableTestVertex, double>(_ => 1.0),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));

            // Special case
            Contains_Test(
                new FibonacciQueue<int, double>(12, new[] { 1, 2 }, _ => 1.0),
                1,
                2);
        }

        [Test]
        public void Enqueue()
        {
            Enqueue_Test(
                new FibonacciQueue<int, double>(_ => 1.0),
                1,
                2);
            Enqueue_Test(
                new FibonacciQueue<TestVertex, double>(_ => 1.0),
                new TestVertex("1"),
                new TestVertex("2"));
            Enqueue_Test(
                new FibonacciQueue<EquatableTestVertex, double>(_ => 1.0),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));
        }

        [Test]
        public void Dequeue()
        {
            Dequeue_Test(
                distFunc => new FibonacciQueue<int, double>(distFunc),
                1,
                2,
                3,
                4);
            Dequeue_Test(
                distFunc => new FibonacciQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            Dequeue_Test(
                distFunc => new FibonacciQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));
        }

        [Test]
        public void Dequeue_Throws()
        {
            Dequeue_Throws_Test(
                new FibonacciQueue<int, double>(_ => 1));
        }

        [Test]
        public void Peek()
        {
            Peek_Test(
                distFunc => new FibonacciQueue<int, double>(distFunc),
                1,
                2,
                3);
            Peek_Test(
                distFunc => new FibonacciQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            Peek_Test(
                distFunc => new FibonacciQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));
        }

        [Test]
        public void Peek_Throws()
        {
            Peek_Throws_Test(
                new FibonacciQueue<int, double>(_ => 1));
        }

        [Test]
        public void Update()
        {
            Update_Test(
                distFunc => new FibonacciQueue<int, double>(distFunc),
                1,
                2);
            Update_Test(
                distFunc => new FibonacciQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"));
            Update_Test(
                distFunc => new FibonacciQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));
        }

        [Test]
        public void ToArray()
        {
            ToArray_Test(
                distFunc => new FibonacciQueue<int, double>(distFunc),
                1,
                2,
                3,
                4);
            ToArray_Test(
                distFunc => new FibonacciQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToArray_Test(
                distFunc => new FibonacciQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));
        }
    }
}