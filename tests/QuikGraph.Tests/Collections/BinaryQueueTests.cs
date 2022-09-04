using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Collections;
using static QuikGraph.Collections.HeapConstants;
using static QuikGraph.Tests.AssertHelpers;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="BinaryQueue{TVertex,TDistance}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BinaryQueueTests : QueueTestsBase
    {
        [Test]
        public void Constructor()
        {
            AssertQueueProperties(
                new BinaryQueue<int, double>(_ => 1.0));

            AssertQueueProperties(
                new BinaryQueue<int, double>(
                    _ => 1.0,
                    (dist1, dist2) => dist1.CompareTo(dist2)));

            #region Local function

            void AssertQueueProperties<TVertex, TDistance>(
                BinaryQueue<TVertex, TDistance> queue)
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
                () => new BinaryQueue<int, double>(null));

            Assert.Throws<ArgumentNullException>(
                () => new BinaryQueue<int, double>(_ => 1.0, null));
            Assert.Throws<ArgumentNullException>(
                () => new BinaryQueue<int, double>(null, (dist1, dist2) => dist1.CompareTo(dist2)));
            Assert.Throws<ArgumentNullException>(
                () => new BinaryQueue<int, double>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Contains()
        {
            Contains_Test(
                new BinaryQueue<int, double>(_ => 1.0),
                1,
                2);
            Contains_Test(
                new BinaryQueue<TestVertex, double>(_ => 1.0),
                new TestVertex("1"),
                new TestVertex("2"));
            Contains_Test(
                new BinaryQueue<EquatableTestVertex, double>(_ => 1.0),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));
        }

        [Test]
        public void Enqueue()
        {
            Enqueue_Test(
                new BinaryQueue<int, double>(_ => 1.0),
                1,
                2);
            Enqueue_Test(
                new BinaryQueue<TestVertex, double>(_ => 1.0),
                new TestVertex("1"),
                new TestVertex("2"));
            Enqueue_Test(
                new BinaryQueue<EquatableTestVertex, double>(_ => 1.0),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));
        }

        [Test]
        public void Dequeue()
        {
            Dequeue_Test(
                distFunc => new BinaryQueue<int, double>(distFunc),
                1,
                2,
                3,
                4);
            Dequeue_Test(
                distFunc => new BinaryQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            Dequeue_Test(
                distFunc => new BinaryQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));
        }

        [Test]
        public void Dequeue_Throws()
        {
            Dequeue_Throws_Test(
                new BinaryQueue<int, double>(_ => 1));
        }

        [Test]
        public void Peek()
        {
            Peek_Test(
                distFunc => new BinaryQueue<int, double>(distFunc),
                1,
                2,
                3);
            Peek_Test(
                distFunc => new BinaryQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            Peek_Test(
                distFunc => new BinaryQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));
        }

        [Test]
        public void Peek_Throws()
        {
            Peek_Throws_Test(
                new BinaryQueue<int, double>(_ => 1));
        }

        [Test]
        public void Update()
        {
            UpdateInternalTest(
                1,
                2,
                3);
            UpdateInternalTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            UpdateInternalTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void UpdateInternalTest<TVertex>(
                TVertex vertex1,
                TVertex vertex2,
                TVertex vertex3)
            {
                BinaryQueue<TVertex, double> queue = null;
                Update_Test(
                    distFunc => queue = new BinaryQueue<TVertex, double>(distFunc),
                    vertex1,
                    vertex2);

                queue.Update(vertex3);  // Added with distance 0.5
                Assert.AreEqual(3, queue.Count);

                AssertEqual(vertex3, queue.Peek());
            }

            #endregion
        }

        [Test]
        public void ToArray()
        {
            ToArray_Test(
                distFunc => new BinaryQueue<int, double>(distFunc),
                1,
                2,
                3,
                4);
            ToArray_Test(
                distFunc => new BinaryQueue<TestVertex, double>(distFunc),
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToArray_Test(
                distFunc => new BinaryQueue<EquatableTestVertex, double>(distFunc),
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));
        }

        [Test]
        public void ToPairsArray()
        {
            ToPairsArrayTest(1, 2, 3, 4);
            ToPairsArrayTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToPairsArrayTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void ToPairsArrayTest<TVertex>(
                TVertex vertex1,
                TVertex vertex2,
                TVertex vertex3,
                TVertex vertex4)
            {
                var distances = new Stack<double>(new[] { 123.0, 3.0, 2.0, 4.0, 5.0, 1.0 });
                var queue = new BinaryQueue<TVertex, double>(_ => distances.Pop());

                // Empty heap
                CollectionAssert.IsEmpty(queue.ToPairsArray());

                queue.Enqueue(vertex1);
                queue.Enqueue(vertex2);
                queue.Enqueue(vertex2);
                queue.Enqueue(vertex3);
                queue.Enqueue(vertex1);

                // Array not sorted with distance
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        new KeyValuePair<double, TVertex>(1.0, vertex1),
                        new KeyValuePair<double, TVertex>(5.0, vertex2),
                        new KeyValuePair<double, TVertex>(4.0, vertex2),
                        new KeyValuePair<double, TVertex>(2.0, vertex3),
                        new KeyValuePair<double, TVertex>(3.0, vertex1)
                    },
                    queue.ToPairsArray());

                queue.Dequeue();
                queue.Dequeue();

                // Array not sorted with distance
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        new KeyValuePair<double, TVertex>(3.0, vertex1),
                        new KeyValuePair<double, TVertex>(5.0, vertex2),
                        new KeyValuePair<double, TVertex>(4.0, vertex2)
                    },
                    queue.ToPairsArray());

                queue.Dequeue();
                queue.Dequeue();
                queue.Dequeue();

                CollectionAssert.IsEmpty(queue.ToPairsArray());

                queue.Enqueue(vertex4);
                CollectionAssert.AreEqual(
                    new[] { new KeyValuePair<double, TVertex>(123.0, vertex4) },
                    queue.ToPairsArray());
            }

            #endregion
        }

        [Test]
        public void ToString2()
        {
            ToStringTest(1, 2, 3, 4);
            ToStringTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToStringTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void ToStringTest<TVertex>(
                TVertex vertex1,
                TVertex vertex2,
                TVertex vertex3,
                TVertex vertex4)
            {
                var queue = new BinaryQueue<TVertex, double>(_ => 1.0);

                // Empty heap => consistent
                StringAssert.StartsWith(Consistent, queue.ToString2());

                queue.Enqueue(vertex1);
                queue.Enqueue(vertex2);
                queue.Enqueue(vertex2);
                queue.Enqueue(vertex3);
                queue.Enqueue(vertex1);

                StringAssert.StartsWith(Consistent, queue.ToString2());

                queue.Dequeue();
                queue.Dequeue();

                StringAssert.StartsWith(Consistent, queue.ToString2());

                queue.Dequeue();
                queue.Dequeue();
                queue.Dequeue();

                StringAssert.StartsWith(Consistent, queue.ToString2());

                queue.Enqueue(vertex4);

                StringAssert.StartsWith(Consistent, queue.ToString2());
            }

            #endregion
        }
    }
}