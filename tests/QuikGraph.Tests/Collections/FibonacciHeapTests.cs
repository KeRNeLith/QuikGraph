using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Collections;
using static QuikGraph.Tests.AssertHelpers;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="FibonacciHeap{TPriority,TValue}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FibonacciHeapTests : HeapTestsBase
    {
        #region Test helpers

        private static void AssertHeapSize<TPriority, TValue>(
            [NotNull] FibonacciHeap<TPriority, TValue> heap,
            int expectedCount)
        {
            if (expectedCount > 0)
            {
                Assert.IsFalse(heap.IsEmpty);
                Assert.IsNotNull(heap.Top);
            }
            else
            {
                Assert.IsTrue(heap.IsEmpty);
                Assert.IsNull(heap.Top);
            }

            Assert.AreEqual(expectedCount, heap.Count);
        }

        /// <summary>
        /// Asserts that heap conditions are fulfilled.
        /// </summary>
        /// <remarks>This is a destructive assertion.</remarks>
        private static void AssertHeapCondition<TPriority, TValue>(
            [NotNull] FibonacciHeap<TPriority, TValue> heap,
            HeapDirection direction,
            int expectedCount,
            [CanBeNull, ItemNotNull] IEnumerable<FibonacciHeapCell<TPriority, TValue>> deletedCells = null)
            where TPriority : struct, IComparable<TPriority>
        {
            FibonacciHeapCell<TPriority, TValue>[] deletedCellsArray = null;
            if (deletedCells != null)
                deletedCellsArray = deletedCells.ToArray();

            TPriority? lastValue = null;
            foreach (KeyValuePair<TPriority, TValue> value in heap.GetDestructiveEnumerator())
            {
                if (lastValue is null)
                {
                    lastValue = value.Key;
                }

                if (direction == HeapDirection.Increasing && lastValue.Value.CompareTo(value.Key) > 0
                    || direction == HeapDirection.Decreasing && lastValue.Value.CompareTo(value.Key) < 0)
                {
                    Assert.Fail("Heap condition has been violated.");
                }

                if (deletedCellsArray != null)
                    CollectionAssert.DoesNotContain(deletedCellsArray, value, "Found item that was deleted.");

                lastValue = value.Key;
                --expectedCount;
            }

            Assert.AreEqual(0, expectedCount, "Not all elements enqueued were dequeued.");
        }

        /// <summary>
        /// Asserts that heap conditions are fulfilled.
        /// </summary>
        /// <remarks>This is a destructive assertion.</remarks>
        private static void AssertHeapConditionClass<TPriority, TValue>(
            [NotNull] FibonacciHeap<TPriority, TValue> heap,
            HeapDirection direction,
            int expectedCount,
            [CanBeNull, ItemNotNull] IEnumerable<FibonacciHeapCell<TPriority, TValue>> deletedCells = null)
            where TPriority : class, IComparable<TPriority>
        {
            FibonacciHeapCell<TPriority, TValue>[] deletedCellsArray = null;
            if (deletedCells != null)
                deletedCellsArray = deletedCells.ToArray();

            TPriority lastValue = null;
            foreach (KeyValuePair<TPriority, TValue> value in heap.GetDestructiveEnumerator())
            {
                if (lastValue is null)
                {
                    lastValue = value.Key;
                }

                if (direction == HeapDirection.Increasing && lastValue.CompareTo(value.Key) > 0
                    || direction == HeapDirection.Decreasing && lastValue.CompareTo(value.Key) < 0)
                {
                    Assert.Fail("Heap condition has been violated.");
                }

                if (deletedCellsArray != null)
                    CollectionAssert.DoesNotContain(deletedCellsArray, value, "Found item that was deleted.");

                lastValue = value.Key;
                --expectedCount;
            }

            Assert.AreEqual(0, expectedCount, "Not all elements enqueued were dequeued.");
        }

        /// <summary>
        /// Asserts that heap conditions are fulfilled.
        /// </summary>
        /// <remarks>This is a destructive assertion.</remarks>
        private static void AssertHeapCondition<TPriority, TValue>(
            [NotNull] FibonacciHeap<TPriority, TValue> heap,
            HeapDirection direction,
            [NotNull] TPriority lastValue,
            int expectedCount)
            where TPriority : IComparable<TPriority>
        {
            foreach (KeyValuePair<TPriority, TValue> value in heap.GetDestructiveEnumerator())
            {
                if (direction == HeapDirection.Increasing && lastValue.CompareTo(value.Key) > 0
                    || direction == HeapDirection.Decreasing && lastValue.CompareTo(value.Key) < 0)
                {
                    Assert.Fail("Heap condition has been violated.");
                }

                lastValue = value.Key;
                --expectedCount;
            }

            Assert.AreEqual(0, expectedCount, "Not all elements enqueued were dequeued.");
        }

        private static void AssertCell<TPriority, TValue>(
            [NotNull] FibonacciHeapCell<TPriority, TValue> cell,
            [NotNull] TPriority expectedPriority,
            [NotNull] TValue expectedValue,
            bool expectedMarked = false,
            bool expectedRemoved = false,
            int expectedDegree = 1,
            [CanBeNull] FibonacciHeapCell<TPriority, TValue> expectedPrevious = null,
            [CanBeNull] FibonacciHeapCell<TPriority, TValue> expectedNext = null,
            [CanBeNull] FibonacciHeapCell<TPriority, TValue> expectedParent = null)
        {
            Assert.AreEqual(expectedPriority, cell.Priority);
            Assert.AreEqual(expectedValue, cell.Value);
            Assert.AreEqual(expectedMarked, cell.Marked);
            Assert.AreEqual(expectedRemoved, cell.Removed);
            Assert.AreEqual(expectedDegree, cell.Degree);
            Assert.AreEqual(expectedPrevious, cell.Previous);
            Assert.AreEqual(expectedNext, cell.Next);
            Assert.AreEqual(expectedParent, cell.Parent);
        }

        private static void AssertNewCell<TPriority, TValue>(
            [NotNull] FibonacciHeapCell<TPriority, TValue> cell,
            [NotNull] TPriority expectedPriority,
            [NotNull] TValue expectedValue,
            [CanBeNull] FibonacciHeapCell<TPriority, TValue> expectedPrevious = null,
            [CanBeNull] FibonacciHeapCell<TPriority, TValue> expectedNext = null)
        {
            AssertCell(
                cell,
                expectedPriority,
                expectedValue,
                false,
                false,
                1,
                expectedPrevious,
                expectedNext);
        }

        #endregion

        [Test]
        public void Constructor()
        {
            AssertHeapBaseProperties(new FibonacciHeap<int, IEdge<int>>());

            AssertHeapBaseProperties(new FibonacciHeap<int, IEdge<int>>(HeapDirection.Increasing));
            AssertHeapBaseProperties(new FibonacciHeap<int, IEdge<int>>(HeapDirection.Decreasing),
                HeapDirection.Decreasing);

            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            AssertHeapProperties(new FibonacciHeap<int, IEdge<int>>(HeapDirection.Increasing, comparer), comparer);
            AssertHeapProperties(new FibonacciHeap<int, IEdge<int>>(HeapDirection.Decreasing, comparer), comparer,
                HeapDirection.Decreasing);

            #region Local functions

            void AssertHeapProperties<TPriority, TValue>(
                FibonacciHeap<TPriority, TValue> heap,
                Comparison<TPriority> expectedComparer,
                HeapDirection expectedDirection = HeapDirection.Increasing)
            {
                Assert.AreEqual(0, heap.Count);
                Assert.IsTrue(heap.IsEmpty);
                Assert.IsNull(heap.Top);
                Assert.AreEqual(expectedDirection, heap.Direction);
                Assert.AreEqual(expectedComparer, heap.PriorityComparison);
            }

            void AssertHeapBaseProperties<TPriority, TValue>(
                FibonacciHeap<TPriority, TValue> heap,
                HeapDirection expectedDirection = HeapDirection.Increasing)
            {
                AssertHeapProperties(heap, Comparer<TPriority>.Default.Compare, expectedDirection);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<int, IEdge<int>>(HeapDirection.Increasing, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<int, IEdge<int>>(HeapDirection.Decreasing, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestCase(HeapDirection.Increasing)]
        [TestCase(HeapDirection.Decreasing)]
        public void Enqueue(HeapDirection direction)
        {
            EnqueueTest(1, 2, 3);
            EnqueueTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            EnqueueTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void EnqueueTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                EnqueueInternalTest();
                EnqueueInternalTest2();
                EnqueueSamePriorityInternalTest();

                #region Local functions

                void EnqueueInternalTest()
                {
                    var heap = new FibonacciHeap<int, TValue>(direction);
                    AssertHeapSize(heap, 0);

                    int topPriority = 1;
                    TValue topValue = value1;
                    FibonacciHeapCell<int, TValue> cell1 = heap.Enqueue(1, value1);
                    AssertNewCell(cell1, 1, value1);
                    AssertHeapSize(heap, 1);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell2 = heap.Enqueue(1, value2);
                    AssertNewCell(cell2, 1, value2, cell1);
                    AssertHeapSize(heap, 2);
                    // Not changed
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell3 = heap.Enqueue(1, value3);
                    AssertNewCell(cell3, 1, value3, cell2);
                    AssertHeapSize(heap, 3);
                    // Not changed
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell4 = heap.Enqueue(int.MinValue, value2);
                    AssertNewCell(cell4, int.MinValue, value2, cell3);
                    AssertHeapSize(heap, 4);
                    if (direction == HeapDirection.Increasing)
                    {
                        topPriority = int.MinValue;
                        topValue = value2;
                    }

                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell5 = heap.Enqueue(int.MaxValue, value3);
                    AssertNewCell(cell5, int.MaxValue, value3, cell4);
                    AssertHeapSize(heap, 5);
                    if (direction == HeapDirection.Decreasing)
                    {
                        topPriority = int.MaxValue;
                        topValue = value3;
                    }

                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell6 = heap.Enqueue(150, value1);
                    AssertNewCell(cell6, 150, value1, cell5);
                    AssertHeapSize(heap, 6);
                    // Not changed
                    AssertHeapTop();

                    AssertHeapCondition(heap, direction, 6);

                    #region Local function

                    void AssertHeapTop()
                    {
                        Assert.AreEqual(topPriority, heap.Top.Priority);
                        Assert.AreEqual(topValue, heap.Top.Value);
                    }

                    #endregion
                }

                void EnqueueInternalTest2()
                {
                    var heap = new FibonacciHeap<TestPriority, TValue>(direction);
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    TestPriority topPriority = priority1;
                    TValue topValue = value1;
                    FibonacciHeapCell<TestPriority, TValue> cell1 = heap.Enqueue(priority1, value1);
                    AssertNewCell(cell1, priority1, value1);
                    AssertHeapSize(heap, 1);
                    AssertHeapTop();

                    var priority1Bis = new TestPriority(1);
                    FibonacciHeapCell<TestPriority, TValue> cell2 = heap.Enqueue(priority1Bis, value2);
                    AssertNewCell(cell2, priority1Bis, value2, cell1);
                    AssertHeapSize(heap, 2);
                    // Not changed
                    AssertHeapTop();

                    var priority1Second = new TestPriority(1);
                    FibonacciHeapCell<TestPriority, TValue> cell3 = heap.Enqueue(priority1Second, value3);
                    AssertNewCell(cell3, priority1Second, value3, cell2);
                    AssertHeapSize(heap, 3);
                    // Not changed
                    AssertHeapTop();

                    var minPriority = new TestPriority(int.MinValue);
                    FibonacciHeapCell<TestPriority, TValue> cell4 = heap.Enqueue(minPriority, value2);
                    AssertNewCell(cell4, minPriority, value2, cell3);
                    AssertHeapSize(heap, 4);
                    if (direction == HeapDirection.Increasing)
                    {
                        topPriority = minPriority;
                        topValue = value2;
                    }

                    AssertHeapTop();

                    var maxPriority = new TestPriority(int.MaxValue);
                    FibonacciHeapCell<TestPriority, TValue> cell5 = heap.Enqueue(maxPriority, value1);
                    AssertNewCell(cell5, maxPriority, value1, cell4);
                    AssertHeapSize(heap, 5);
                    if (direction == HeapDirection.Decreasing)
                    {
                        topPriority = maxPriority;
                        topValue = value1;
                    }

                    AssertHeapTop();

                    var priority150 = new TestPriority(150);
                    FibonacciHeapCell<TestPriority, TValue> cell6 = heap.Enqueue(priority150, value3);
                    AssertNewCell(cell6, priority150, value3, cell5);
                    AssertHeapSize(heap, 6);
                    // Not changed
                    AssertHeapTop();

                    AssertHeapConditionClass(heap, direction, 6);

                    #region Local function

                    void AssertHeapTop()
                    {
                        Assert.AreSame(topPriority, heap.Top.Priority);
                        Assert.AreEqual(topValue, heap.Top.Value);
                    }

                    #endregion
                }

                void EnqueueSamePriorityInternalTest()
                {
                    var heap = new FibonacciHeap<int, TValue>(direction);
                    AssertHeapSize(heap, 0);

                    FibonacciHeapCell<int, TValue> cell1 = heap.Enqueue(1, value1);
                    AssertNewCell(cell1, 1, value1);
                    AssertHeapSize(heap, 1);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell2 = heap.Enqueue(1, value2);
                    AssertNewCell(cell2, 1, value2, cell1);
                    AssertHeapSize(heap, 2);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell3 = heap.Enqueue(1, value3);
                    AssertNewCell(cell3, 1, value3, cell2);
                    AssertHeapSize(heap, 3);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell4 = heap.Enqueue(1, value1);
                    AssertNewCell(cell4, 1, value1, cell3);
                    AssertHeapSize(heap, 4);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell5 = heap.Enqueue(1, value2);
                    AssertNewCell(cell5, 1, value2, cell4);
                    AssertHeapSize(heap, 5);
                    AssertHeapTop();

                    FibonacciHeapCell<int, TValue> cell6 = heap.Enqueue(1, value3);
                    AssertNewCell(cell6, 1, value3, cell5);
                    AssertHeapSize(heap, 6);
                    AssertHeapTop();

                    AssertHeapCondition(heap, direction, 6);

                    #region Local function

                    void AssertHeapTop()
                    {
                        Assert.AreEqual(1, heap.Top.Priority);
                        Assert.AreEqual(value1, heap.Top.Value);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void Add_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new FibonacciHeap<TestPriority, int>(HeapDirection.Increasing).Enqueue(null, 1));
            Assert.Throws<ArgumentNullException>(() => new FibonacciHeap<TestPriority, int>(HeapDirection.Decreasing).Enqueue(null, 1));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestCase(HeapDirection.Increasing)]
        [TestCase(HeapDirection.Decreasing)]
        public void ChangeKey(HeapDirection direction)
        {
            ChangeKeyTest(1, 2, 3);
            ChangeKeyTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            ChangeKeyTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void ChangeKeyTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                ChangeKeyInternalTest();
                ChangeKeyInternalTest2();
                ChangeKeySamePriorityInternalTest();

                #region Local functions

                void ChangeKeyInternalTest()
                {
                    FibonacciHeap<int, TValue> heap = CreateHeap(
                        out _,
                        out _,
                        out FibonacciHeapCell<int, TValue> cell3,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell3, 160);
                    AssertHeapCondition(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out FibonacciHeapCell<int, TValue> cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell2, 140);
                    AssertHeapCondition(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out _,
                        out _,
                        out _,
                        out _,
                        out FibonacciHeapCell<int, TValue> cell6);

                    // Decrease priority
                    heap.ChangeKey(cell6, -150);
                    AssertHeapCondition(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Decrease priority
                    heap.ChangeKey(cell2, -140);
                    AssertHeapCondition(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Same priority
                    heap.ChangeKey(cell2, 1);
                    AssertHeapCondition(heap, direction, 6);


                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out FibonacciHeapCell<int, TValue> cell4,
                        out _,
                        out _);

                    heap.Delete(cell4);
                    // Decrease priority
                    heap.ChangeKey(cell2, -120);
                    AssertHeapCondition(heap, direction, 5);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out FibonacciHeapCell<int, TValue> cell5,
                        out _);

                    heap.Delete(cell5);
                    // Increase priority
                    heap.ChangeKey(cell2, 200);
                    AssertHeapCondition(heap, direction, 5);


                    // Cascade cuts (decrease priority)
                    heap = new FibonacciHeap<int, TValue>(direction);
                    var cells = new List<FibonacciHeapCell<int, TValue>>();
                    for (int i = 0; i < 10; ++i)
                        cells.Add(heap.Enqueue(i, value1));

                    int lastValue = heap.Top.Priority;
                    heap.Dequeue();
                    heap.ChangeKey(cells[6], 3);
                    heap.ChangeKey(cells[7], 2);
                    AssertHeapCondition(heap, direction, lastValue, 9);

                    // Cascade cuts (increase priority)
                    heap = new FibonacciHeap<int, TValue>(direction);
                    cells = new List<FibonacciHeapCell<int, TValue>>();
                    for (int i = 0; i < 10; ++i)
                        cells.Add(heap.Enqueue(i, value1));

                    heap.Dequeue();
                    heap.ChangeKey(cells[5], 10);
                    AssertHeapCondition(heap, direction, 9);

                    #region Local function

                    FibonacciHeap<int, TValue> CreateHeap(
                        out FibonacciHeapCell<int, TValue> c1,
                        out FibonacciHeapCell<int, TValue> c2,
                        out FibonacciHeapCell<int, TValue> c3,
                        out FibonacciHeapCell<int, TValue> c4,
                        out FibonacciHeapCell<int, TValue> c5,
                        out FibonacciHeapCell<int, TValue> c6)
                    {
                        var h = new FibonacciHeap<int, TValue>(direction);

                        c1 = h.Enqueue(1, value1);
                        c2 = h.Enqueue(1, value2);
                        c3 = h.Enqueue(1, value3);
                        c4 = h.Enqueue(int.MinValue, value2);
                        c5 = h.Enqueue(int.MaxValue, value3);
                        c6 = h.Enqueue(150, value1);
                        AssertHeapSize(h, 6);

                        return h;
                    }

                    #endregion
                }

                void ChangeKeyInternalTest2()
                {
                    FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell3,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell3, new TestPriority(160));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell2, new TestPriority(140));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out _,
                        out _,
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell6);

                    // Decrease priority
                    heap.ChangeKey(cell6, new TestPriority(-150));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Decrease priority
                    heap.ChangeKey(cell2, new TestPriority(-140));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Same priority
                    heap.ChangeKey(cell2, new TestPriority(1));
                    AssertHeapConditionClass(heap, direction, 6);


                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell4,
                        out _,
                        out _);

                    heap.Delete(cell4);
                    // Decrease priority
                    heap.ChangeKey(cell2, new TestPriority(-120));
                    AssertHeapConditionClass(heap, direction, 5);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell5,
                        out _);

                    heap.Delete(cell5);
                    // Increase priority
                    heap.ChangeKey(cell2, new TestPriority(200));
                    AssertHeapConditionClass(heap, direction, 5);


                    // Cascade cuts (decrease priority)
                    heap = new FibonacciHeap<TestPriority, TValue>(direction);
                    var cells = new List<FibonacciHeapCell<TestPriority, TValue>>();
                    for (int i = 0; i < 10; ++i)
                        cells.Add(heap.Enqueue(new TestPriority(i), value1));

                    TestPriority lastValue = heap.Top.Priority;
                    heap.Dequeue();
                    heap.ChangeKey(cells[6], new TestPriority(3));
                    heap.ChangeKey(cells[7], new TestPriority(2));
                    AssertHeapCondition(heap, direction, lastValue, 9);

                    // Cascade cuts (increase priority)
                    heap = new FibonacciHeap<TestPriority, TValue>(direction);
                    cells = new List<FibonacciHeapCell<TestPriority, TValue>>();
                    for (int i = 0; i < 10; ++i)
                        cells.Add(heap.Enqueue(new TestPriority(i), value1));

                    heap.Dequeue();
                    heap.ChangeKey(cells[5], new TestPriority(10));
                    AssertHeapConditionClass(heap, direction, 9);

                    #region Local function

                    FibonacciHeap<TestPriority, TValue> CreateHeap(
                        out FibonacciHeapCell<TestPriority, TValue> c1,
                        out FibonacciHeapCell<TestPriority, TValue> c2,
                        out FibonacciHeapCell<TestPriority, TValue> c3,
                        out FibonacciHeapCell<TestPriority, TValue> c4,
                        out FibonacciHeapCell<TestPriority, TValue> c5,
                        out FibonacciHeapCell<TestPriority, TValue> c6)
                    {
                        var h = new FibonacciHeap<TestPriority, TValue>(direction);

                        c1 = h.Enqueue(new TestPriority(1), value1);
                        c2 = h.Enqueue(new TestPriority(1), value2);
                        c3 = h.Enqueue(new TestPriority(1), value3);
                        c4 = h.Enqueue(new TestPriority(int.MinValue), value2);
                        c5 = h.Enqueue(new TestPriority(int.MaxValue), value3);
                        c6 = h.Enqueue(new TestPriority(150), value1);
                        AssertHeapSize(h, 6);

                        return h;
                    }

                    #endregion
                }

                void ChangeKeySamePriorityInternalTest()
                {
                    FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell3,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell3, new TestPriority(160));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Increase priority
                    heap.ChangeKey(cell2, new TestPriority(140));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out _,
                        out _,
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell6);

                    // Decrease priority
                    heap.ChangeKey(cell6, new TestPriority(-150));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Decrease priority
                    heap.ChangeKey(cell2, new TestPriority(-140));
                    AssertHeapConditionClass(heap, direction, 6);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out _,
                        out _);

                    // Same priority
                    heap.ChangeKey(cell2, new TestPriority(1));
                    AssertHeapConditionClass(heap, direction, 6);


                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell4,
                        out _,
                        out _);

                    heap.Delete(cell4);
                    // Decrease priority
                    heap.ChangeKey(cell2, new TestPriority(-120));
                    AssertHeapConditionClass(heap, direction, 5);

                    heap = CreateHeap(
                        out _,
                        out cell2,
                        out _,
                        out _,
                        out FibonacciHeapCell<TestPriority, TValue> cell5,
                        out _);

                    heap.Delete(cell5);
                    // Increase priority
                    heap.ChangeKey(cell2, new TestPriority(200));
                    AssertHeapConditionClass(heap, direction, 5);

                    #region Local function

                    FibonacciHeap<TestPriority, TValue> CreateHeap(
                        out FibonacciHeapCell<TestPriority, TValue> c1,
                        out FibonacciHeapCell<TestPriority, TValue> c2,
                        out FibonacciHeapCell<TestPriority, TValue> c3,
                        out FibonacciHeapCell<TestPriority, TValue> c4,
                        out FibonacciHeapCell<TestPriority, TValue> c5,
                        out FibonacciHeapCell<TestPriority, TValue> c6)
                    {
                        var h = new FibonacciHeap<TestPriority, TValue>(direction);

                        c1 = h.Enqueue(new TestPriority(1), value1);
                        c2 = h.Enqueue(new TestPriority(1), value2);
                        c3 = h.Enqueue(new TestPriority(1), value3);
                        c4 = h.Enqueue(new TestPriority(1), value2);
                        c5 = h.Enqueue(new TestPriority(1), value3);
                        c6 = h.Enqueue(new TestPriority(1), value1);
                        AssertHeapSize(h, 6);

                        return h;
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void ChangeKey_Throws()
        {
            var priority = new TestPriority(1);
            var cell = new FibonacciHeapCell<TestPriority, int>();
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Increasing).ChangeKey(null, priority));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Decreasing).ChangeKey(null, priority));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Increasing).ChangeKey(null, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Decreasing).ChangeKey(null, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Increasing).ChangeKey(cell, null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Decreasing).ChangeKey(cell, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestCase(HeapDirection.Increasing)]
        [TestCase(HeapDirection.Decreasing)]
        public void DeleteKey(HeapDirection direction)
        {
            DeleteKeyTest(1);
            DeleteKeyTest(new TestVertex("1"));
            DeleteKeyTest(new EquatableTestVertex("1"));

            #region Local function

            void DeleteKeyTest<TValue>(TValue value1)
            {
                DeleteKeyInternalTest();
                DeleteKeyInternalTest2();
                DeleteKeySamePriorityInternalTest();

                #region Local functions

                void DeleteKeyInternalTest()
                {
                    // One delete
                    {
                        for (int i = 0; i < 10; ++i)
                        {
                            FibonacciHeap<int, TValue> heap = CreateHeap(
                                out FibonacciHeapCell<int, TValue>[] cells);

                            heap.Delete(cells[i]);
                            AssertHeapCondition(heap, direction, 9, new[] {cells[i]});
                        }
                    }

                    // Multiple deletes
                    {
                        FibonacciHeap<int, TValue> heap = CreateHeap(
                            out FibonacciHeapCell<int, TValue>[] cells);

                        heap.Delete(cells[2]);
                        heap.Delete(cells[5]);
                        AssertHeapCondition(heap, direction, 8, new[] {cells[2], cells[5]});
                    }

                    #region Local function

                    FibonacciHeap<int, TValue> CreateHeap(out FibonacciHeapCell<int, TValue>[] cs)
                    {
                        var h = new FibonacciHeap<int, TValue>(direction);

                        var localCells = new List<FibonacciHeapCell<int, TValue>>();
                        for (int i = -5; i < 5; ++i)
                        {
                            localCells.Add(h.Enqueue(i, value1));
                        }

                        cs = localCells.ToArray();

                        AssertHeapSize(h, 10);

                        h.ChangeKey(cs[2], 12);
                        h.ChangeKey(cs[4], 2);
                        h.ChangeKey(cs[6], -5);
                        AssertHeapSize(h, 10);

                        return h;
                    }

                    #endregion
                }

                void DeleteKeyInternalTest2()
                {
                    // One delete
                    {
                        for (int i = 0; i < 10; ++i)
                        {
                            FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                                out FibonacciHeapCell<TestPriority, TValue>[] cells);

                            heap.Delete(cells[i]);
                            AssertHeapConditionClass(heap, direction, 9, new[] {cells[i]});
                        }
                    }

                    // Multiple deletes
                    {
                        FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                            out FibonacciHeapCell<TestPriority, TValue>[] cells);

                        heap.Delete(cells[2]);
                        heap.Delete(cells[5]);
                        AssertHeapConditionClass(heap, direction, 8, new[] {cells[2], cells[5]});
                    }

                    #region Local function

                    FibonacciHeap<TestPriority, TValue> CreateHeap(out FibonacciHeapCell<TestPriority, TValue>[] cs)
                    {
                        var h = new FibonacciHeap<TestPriority, TValue>(direction);

                        var localCells = new List<FibonacciHeapCell<TestPriority, TValue>>();
                        for (int i = -5; i < 5; ++i)
                        {
                            localCells.Add(h.Enqueue(new TestPriority(i), value1));
                        }

                        cs = localCells.ToArray();

                        AssertHeapSize(h, 10);

                        h.ChangeKey(cs[2], new TestPriority(12));
                        h.ChangeKey(cs[4], new TestPriority(2));
                        h.ChangeKey(cs[6], new TestPriority(-5));
                        AssertHeapSize(h, 10);

                        return h;
                    }

                    #endregion
                }

                void DeleteKeySamePriorityInternalTest()
                {
                    // One delete
                    {
                        FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                            out FibonacciHeapCell<TestPriority, TValue>[] cells);

                        heap.Delete(cells[0]);
                        AssertHeapConditionClass(heap, direction, 9, new[] {cells[0]});


                        heap = CreateHeap(out cells);

                        heap.Delete(cells[4]);
                        AssertHeapConditionClass(heap, direction, 9, new[] {cells[4]});


                        heap = CreateHeap(out cells);

                        heap.Delete(cells[9]);
                        AssertHeapConditionClass(heap, direction, 9, new[] {cells[9]});
                    }

                    // Multiple deletes
                    {
                        FibonacciHeap<TestPriority, TValue> heap = CreateHeap(
                            out FibonacciHeapCell<TestPriority, TValue>[] cells);

                        heap.Delete(cells[2]);
                        heap.Delete(cells[5]);
                        AssertHeapConditionClass(heap, direction, 8, new[] {cells[2], cells[5]});
                    }

                    #region Local function

                    FibonacciHeap<TestPriority, TValue> CreateHeap(out FibonacciHeapCell<TestPriority, TValue>[] cs)
                    {
                        var h = new FibonacciHeap<TestPriority, TValue>(direction);

                        var localCells = new List<FibonacciHeapCell<TestPriority, TValue>>();
                        for (int i = 0; i < 10; ++i)
                        {
                            localCells.Add(h.Enqueue(new TestPriority(1), value1));
                        }

                        cs = localCells.ToArray();

                        AssertHeapSize(h, 10);

                        return h;
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void DeleteKey_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Increasing).Delete(null));
            Assert.Throws<ArgumentNullException>(
                () => new FibonacciHeap<TestPriority, int>(HeapDirection.Decreasing).Delete(null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void DequeueOnIncreasing()
        {
            DequeueTest(1, 2, 3, 4);
            DequeueTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            DequeueTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void DequeueTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3,
                TValue value4)
            {
                DequeueInternalTest();
                DequeueInternalTest2();
                DequeueSamePriorityInternalTest();

                #region Local functions

                void DequeueInternalTest()
                {
                    var heap = new FibonacciHeap<double, TValue>(HeapDirection.Increasing);
                    AssertHeapSize(heap, 0);

                    heap.Enqueue(10.0, value1);
                    heap.Enqueue(1.0, value2);
                    heap.Enqueue(9.0, value3);
                    heap.Enqueue(2.0, value2);
                    heap.Enqueue(3.0, value4);
                    heap.Enqueue(6.0, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<double, TValue> pair = heap.Dequeue();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.Dequeue();
                    Assert.AreEqual(2.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreEqual(3.0, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreEqual(6.0, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreEqual(9.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreEqual(10.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void DequeueInternalTest2()
                {
                    var heap = new FibonacciHeap<TestPriority, TValue>(HeapDirection.Increasing);
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority2 = new TestPriority(2);
                    var priority3 = new TestPriority(3);
                    var priority6 = new TestPriority(6);
                    var priority9 = new TestPriority(9);
                    var priority10 = new TestPriority(10);
                    heap.Enqueue(priority10, value1);
                    heap.Enqueue(priority1, value2);
                    heap.Enqueue(priority9, value3);
                    heap.Enqueue(priority2, value2);
                    heap.Enqueue(priority3, value4);
                    heap.Enqueue(priority6, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<TestPriority, TValue> pair = heap.Dequeue();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority2, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority3, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority6, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority9, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority10, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void DequeueSamePriorityInternalTest()
                {
                    var heap = new FibonacciHeap<int, TValue>(HeapDirection.Increasing);
                    AssertHeapSize(heap, 0);

                    heap.Enqueue(1, value1);
                    heap.Enqueue(1, value2);
                    heap.Enqueue(1, value3);
                    heap.Enqueue(1, value2);
                    heap.Enqueue(1, value4);
                    AssertHeapSize(heap, 5);

                    KeyValuePair<int, TValue> pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void DequeueOnDecreasing()
        {
            DequeueTest(1, 2, 3, 4);
            DequeueTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            DequeueTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void DequeueTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3,
                TValue value4)
            {
                DequeueInternalTest();
                DequeueInternalTest2();
                DequeueSamePriorityInternalTest();

                #region Local functions

                void DequeueInternalTest()
                {
                    var heap = new FibonacciHeap<double, TValue>(HeapDirection.Decreasing);
                    AssertHeapSize(heap, 0);

                    heap.Enqueue(10.0, value1);
                    heap.Enqueue(1.0, value2);
                    heap.Enqueue(9.0, value3);
                    heap.Enqueue(2.0, value2);
                    heap.Enqueue(3.0, value4);
                    heap.Enqueue(6.0, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<double, TValue> pair = heap.Dequeue();
                    Assert.AreEqual(10.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.Dequeue();
                    Assert.AreEqual(9.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreEqual(6.0, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreEqual(3.0, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreEqual(2.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void DequeueInternalTest2()
                {
                    var heap = new FibonacciHeap<TestPriority, TValue>(HeapDirection.Decreasing);
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority2 = new TestPriority(2);
                    var priority3 = new TestPriority(3);
                    var priority6 = new TestPriority(6);
                    var priority9 = new TestPriority(9);
                    var priority10 = new TestPriority(10);
                    heap.Enqueue(priority10, value1);
                    heap.Enqueue(priority1, value2);
                    heap.Enqueue(priority9, value3);
                    heap.Enqueue(priority2, value2);
                    heap.Enqueue(priority3, value4);
                    heap.Enqueue(priority6, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<TestPriority, TValue> pair = heap.Dequeue();
                    Assert.AreSame(priority10, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority9, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority6, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority3, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority2, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void DequeueSamePriorityInternalTest()
                {
                    var heap = new FibonacciHeap<int, TValue>(HeapDirection.Decreasing);
                    AssertHeapSize(heap, 0);

                    heap.Enqueue(1, value1);
                    heap.Enqueue(1, value2);
                    heap.Enqueue(1, value3);
                    heap.Enqueue(1, value2);
                    heap.Enqueue(1, value4);
                    AssertHeapSize(heap, 5);

                    KeyValuePair<int, TValue> pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.Dequeue();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void Dequeue_Throws()
        {
            var heap = new FibonacciHeap<int, double>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => heap.Dequeue());
        }

        [TestCase(HeapDirection.Increasing)]
        [TestCase(HeapDirection.Decreasing)]
        public void Merge(HeapDirection direction)
        {
            MergeTest(1, 2);
            MergeTest(
                new TestVertex("1"),
                new TestVertex("2"));
            MergeTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));

            #region Local function

            void MergeTest<TValue>(TValue value1, TValue value2)
            {
                MergeInternalTest();
                MergeInternalTest2();
                MergeSamePriorityInternalTest();

                #region Local functions

                void MergeInternalTest()
                {
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(i, i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        heap.Merge(emptyHeap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(i, i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        emptyHeap.Merge(heap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                    {
                        var emptyHeap = new FibonacciHeap<int, TValue>(direction);
                        var emptyHeap2 = new FibonacciHeap<int, TValue>(direction);

                        AssertHeapSize(emptyHeap, 0);
                        AssertHeapSize(emptyHeap2, 0);

                        emptyHeap.Merge(emptyHeap2);

                        AssertHeapSize(emptyHeap, 0);
                        AssertHeapCondition(emptyHeap, direction, 0);
                    }
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var heap2 = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 0;
                        for (int i = 0; i < 11; i++)
                        {
                            heap.Enqueue(i, i % 2 == 0 ? value1 : value2);
                            heap2.Enqueue(i * 11, i % 2 == 0 ? value2 : value1);
                            expectedCount += 2;
                        }

                        AssertHeapSize(heap, expectedCount / 2);
                        AssertHeapSize(heap2, expectedCount / 2);

                        heap.Merge(heap2);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                }

                void MergeInternalTest2()
                {
                    {
                        var heap = new FibonacciHeap<TestPriority, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<TestPriority, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(new TestPriority(i), i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        heap.Merge(emptyHeap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapConditionClass(heap, direction, expectedCount);
                    }
                    {
                        var heap = new FibonacciHeap<TestPriority, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<TestPriority, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(new TestPriority(i), i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        emptyHeap.Merge(heap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapConditionClass(heap, direction, expectedCount);
                    }
                    {
                        var emptyHeap = new FibonacciHeap<TestPriority, TValue>(direction);
                        var emptyHeap2 = new FibonacciHeap<TestPriority, TValue>(direction);

                        AssertHeapSize(emptyHeap, 0);
                        AssertHeapSize(emptyHeap2, 0);

                        emptyHeap.Merge(emptyHeap2);

                        AssertHeapSize(emptyHeap, 0);
                        AssertHeapConditionClass(emptyHeap, direction, 0);
                    }
                    {
                        var heap = new FibonacciHeap<TestPriority, TValue>(direction);
                        var heap2 = new FibonacciHeap<TestPriority, TValue>(direction);
                        int expectedCount = 0;
                        for (int i = 0; i < 11; i++)
                        {
                            heap.Enqueue(new TestPriority(i), i % 2 == 0 ? value1 : value2);
                            heap2.Enqueue(new TestPriority(i* 11), i % 2 == 0 ? value2 : value1);
                            expectedCount += 2;
                        }

                        AssertHeapSize(heap, expectedCount / 2);
                        AssertHeapSize(heap2, expectedCount / 2);

                        heap.Merge(heap2);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapConditionClass(heap, direction, expectedCount);
                    }
                }

                void MergeSamePriorityInternalTest()
                {
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(1, i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        heap.Merge(emptyHeap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var emptyHeap = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 11;
                        for (int i = 0; i < expectedCount; i++)
                        {
                            heap.Enqueue(1, i % 2 == 0 ? value1 : value2);
                        }

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapSize(emptyHeap, 0);

                        emptyHeap.Merge(heap);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                    {
                        var heap = new FibonacciHeap<int, TValue>(direction);
                        var heap2 = new FibonacciHeap<int, TValue>(direction);
                        int expectedCount = 0;
                        for (int i = 0; i < 11; i++)
                        {
                            heap.Enqueue(1, i % 2 == 0 ? value1 : value2);
                            heap2.Enqueue(1, i % 2 == 0 ? value2 : value1);
                            expectedCount += 2;
                        }

                        AssertHeapSize(heap, expectedCount / 2);
                        AssertHeapSize(heap2, expectedCount / 2);

                        heap.Merge(heap2);

                        AssertHeapSize(heap, expectedCount);
                        AssertHeapCondition(heap, direction, expectedCount);
                    }
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void Merge_Throws()
        {
            var increaseHeap = new FibonacciHeap<int, int>(HeapDirection.Increasing);
            var decreaseHeap = new FibonacciHeap<int, int>(HeapDirection.Decreasing);
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => increaseHeap.Merge(null));

            Assert.Throws<InvalidOperationException>(() => increaseHeap.Merge(decreaseHeap));
            Assert.Throws<InvalidOperationException>(() => decreaseHeap.Merge(increaseHeap));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void NextCutOnLessThan()
        {
            var heap = new FibonacciHeap<int, string>(HeapDirection.Increasing);
            var heap2 = new FibonacciHeap<int, string>(HeapDirection.Increasing);
            var toCutNodes = new List<FibonacciHeapCell<int, string>>();

            heap.Enqueue(0, "0");
            toCutNodes.Add(heap.Enqueue(5, "5"));
            toCutNodes.Add(heap.Enqueue(6, "6"));
            toCutNodes.Add(heap.Enqueue(7, "7"));
            heap.Enqueue(-10, "-10");
            heap.Dequeue();
            heap.Enqueue(1, "1");
            heap2.Enqueue(4, "4");
            heap2.Enqueue(5, "5");
            heap2.Enqueue(-10, "-10");
            heap2.Dequeue();
            heap.Merge(heap2);
            heap.Enqueue(-10, "-10");
            heap.Dequeue();
            toCutNodes.ForEach(x => heap.ChangeKey(x, -5));
            heap.Enqueue(-10, "-10");
            heap.Dequeue();

            AssertHeapCondition(heap, HeapDirection.Increasing, 7);
        }

        [Test]
        public void NextCutOnGreaterThan()
        {
            var heap = new FibonacciHeap<int, string>(HeapDirection.Increasing);
            var heap2 = new FibonacciHeap<int, string>(HeapDirection.Increasing);
            var toCutNodes = new List<FibonacciHeapCell<int, string>>();

            heap.Enqueue(1, "1");
            toCutNodes.Add(heap.Enqueue(5, "5"));
            toCutNodes.Add(heap.Enqueue(6, "6"));
            toCutNodes.Add(heap.Enqueue(7, "7"));
            heap.Enqueue(-10, "-10");
            heap.Dequeue();
            heap.Enqueue(0, "0");
            heap2.Enqueue(-1, "-1");
            heap2.Enqueue(5, "5");
            heap2.Enqueue(-10, "-10");
            heap2.Dequeue();
            heap.Merge(heap2);
            heap.Enqueue(-10, "-10");
            heap.Dequeue();
            toCutNodes.ForEach(x => heap.ChangeKey(x, -5));
            heap.Enqueue(-10, "-10");
            heap.Dequeue();

            AssertHeapCondition(heap, HeapDirection.Increasing, 7);
        }

        [TestCase(HeapDirection.Increasing)]
        [TestCase(HeapDirection.Decreasing)]
        public void EnumerateHeap(HeapDirection direction)
        {
            var heap = new FibonacciHeap<double, int>(direction);
            CollectionAssert.IsEmpty(heap);

            heap.Enqueue(1.0, 1);
            CollectionAssert.AreEquivalent(
                new[] {new KeyValuePair<double, int>(1.0, 1)},
                heap);

            heap.Enqueue(12.0, 1);
            FibonacciHeapCell<double, int> cell = heap.Enqueue(10.0, 2);
            heap.Enqueue(5.0, 4);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new KeyValuePair<double, int>(1.0, 1),
                    new KeyValuePair<double, int>(5.0, 4),
                    new KeyValuePair<double, int>(10.0, 2),
                    new KeyValuePair<double, int>(12.0, 1)
                },
                heap);

            heap.Dequeue();
            CollectionAssert.AreEquivalent(
                new[]
                {
                    direction == HeapDirection.Increasing
                        ? new KeyValuePair<double, int>(12.0, 1)
                        : new KeyValuePair<double, int>(1.0, 1),
                    new KeyValuePair<double, int>(5.0, 4),
                    new KeyValuePair<double, int>(10.0, 2)
                },
                heap);

            heap.ChangeKey(cell, -1);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    direction == HeapDirection.Increasing
                        ? new KeyValuePair<double, int>(12.0, 1)
                        : new KeyValuePair<double, int>(1.0, 1),
                    new KeyValuePair<double, int>(5.0, 4),
                    new KeyValuePair<double, int>(-1, 2)
                },
                heap);

            while (heap.Count > 0)
            {
                heap.Dequeue();
            }

            CollectionAssert.IsEmpty(heap);
        }

        [Test]
        public void EnumerateHeapLinkedList()
        {
            var list = new FibonacciHeapLinkedList<int, int>();
            CollectionAssert.IsEmpty(list);

            var cell1 = new FibonacciHeapCell<int, int>();
            list.AddLast(cell1);
            CollectionAssert.AreEqual(
                new[] { cell1 },
                list);

            var cell2 = new FibonacciHeapCell<int, int>();
            list.AddLast(cell2);
            CollectionAssert.AreEqual(
                new[] { cell1, cell2 },
                list);

            list.Remove(cell1);
            CollectionAssert.AreEqual(
                new[] { cell2 },
                list);

            list.Remove(cell2);
            CollectionAssert.IsEmpty(list);
        }

        [Test]
        public void DrawHeap()
        {
            var heap = new FibonacciHeap<int, int>();
            heap.Enqueue(1, 0);
            heap.Enqueue(2, 1);
            heap.Enqueue(1, 2);
            heap.Enqueue(2, 3);
            FibonacciHeapCell<int, int> cell1 = heap.Enqueue(2, 4);
            heap.Enqueue(1, 5);
            heap.Enqueue(1, 6);
            heap.Enqueue(2, 7);
            heap.Enqueue(2, 8);
            heap.Enqueue(2, 9);
            heap.Enqueue(2, 10);
            FibonacciHeapCell<int, int> cell2 = heap.Enqueue(1, 11);
            heap.Enqueue(1, 12);
            heap.Enqueue(1, 13);
            heap.Enqueue(1, 14);

            StringAssert.AreEqualIgnoringCase(
                "1 2 1 2 2 1 1 2 2 2 2 1 1 1 1 ",
                heap.DrawHeap());

            heap.ChangeKey(cell1, 10);
            heap.ChangeKey(cell2, -1);
            StringAssert.AreEqualIgnoringCase(
                $"1        -1  1 1 {Environment.NewLine}2 1 1    2 2 1 {Environment.NewLine}  2 10 1   2 {Environment.NewLine}       2 ",
                heap.DrawHeap());
        }
    }
}