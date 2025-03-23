using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Collections;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="SoftHeap{TKey,TValue}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class SoftHeapTests : HeapTestsBase
    {
        private const double ErrorRate = 1 / 3.0;
        private const double ErrorRate2 = 1 / 2.0;
        private const double FailErrorRate1 = 0;
        private const double FailErrorRate2 = 0.6;

        #region Test helpers

        private static void AssertHeapSize<TPriority, TValue>(
            [NotNull] SoftHeap<TPriority, TValue> heap,
            int expectedCount)
        {
            Assert.AreEqual(expectedCount, heap.Count);
        }

        #endregion

        [Test]
        public void Constructor()
        {
            AssertHeapBaseProperties(
                new SoftHeap<int, IEdge<int>>(ErrorRate, 10),
                ErrorRate,
                10);
            AssertHeapBaseProperties(
                new SoftHeap<int, IEdge<int>>(ErrorRate2, 42),
                ErrorRate2,
                42);

            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            AssertHeapProperties(
                new SoftHeap<int, IEdge<int>>(ErrorRate, 12, comparer),
                ErrorRate,
                12,
                comparer);

            #region Local functions

            void AssertHeapProperties<TPriority, TValue>(
                SoftHeap<TPriority, TValue> heap,
                double expectedErrorRate,
                TPriority expectedMaxPriority,
                Comparison<TPriority> expectedComparer)
            {
                AssertHeapSize(heap, 0);
                Assert.AreEqual(expectedErrorRate, heap.ErrorRate);
                Assert.AreEqual(2 + 2 * (int) Math.Ceiling(Math.Log(1.0 / expectedErrorRate, 2.0)), heap.MinRank);
                Assert.AreEqual(expectedComparer, heap.KeyComparison);
                Assert.AreEqual(expectedMaxPriority, heap.KeyMaxValue);
            }

            void AssertHeapBaseProperties<TPriority, TValue>(
                SoftHeap<TPriority, TValue> heap,
                double expectedErrorRate,
                TPriority expectedMaxPriority)
            {
                AssertHeapProperties(
                    heap,
                    expectedErrorRate,
                    expectedMaxPriority,
                    Comparer<TPriority>.Default.Compare);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SoftHeap<TestPriority, double>(ErrorRate, null));
            Assert.Throws<ArgumentNullException>(() => new SoftHeap<TestPriority, double>(-ErrorRate, null));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(-ErrorRate, new TestPriority(1)));

            Assert.Throws<ArgumentNullException>(() =>
                new SoftHeap<TestPriority, double>(ErrorRate, null, (p1, p2) => p1.CompareTo(p2)));
            Assert.Throws<ArgumentNullException>(() =>
                new SoftHeap<TestPriority, double>(ErrorRate, new TestPriority(10), null));
            Assert.Throws<ArgumentNullException>(() => new SoftHeap<TestPriority, double>(ErrorRate, null, null));
            Assert.Throws<ArgumentNullException>(() =>
                new SoftHeap<TestPriority, double>(-ErrorRate, null, (p1, p2) => p1.CompareTo(p2)));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(-ErrorRate, new TestPriority(10), null));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(FailErrorRate1, new TestPriority(10), null));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(FailErrorRate2, new TestPriority(10), null));
            Assert.Throws<ArgumentNullException>(() => new SoftHeap<TestPriority, double>(-ErrorRate, null, null));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(-ErrorRate, new TestPriority(1), (p1, p2) => p1.CompareTo(p2)));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(FailErrorRate1, new TestPriority(1), (p1, p2) => p1.CompareTo(p2)));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SoftHeap<TestPriority, double>(FailErrorRate2, new TestPriority(1), (p1, p2) => p1.CompareTo(p2)));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Add()
        {
            AddTest(1, 2, 3);
            AddTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            AddTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void AddTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                AddInternalTest();
                AddInternalTest2();
                AddSamePriorityInternalTest();

                #region Local functions

                void AddInternalTest()
                {
                    var heap = new SoftHeap<int, TValue>(ErrorRate, 25);
                    AssertHeapSize(heap, 0);

                    heap.Add(1, value1);
                    AssertHeapSize(heap, 1);

                    heap.Add(1, value2);
                    AssertHeapSize(heap, 2);

                    heap.Add(1, value3);
                    AssertHeapSize(heap, 3);

                    heap.Add(int.MinValue, value1);
                    AssertHeapSize(heap, 4);

                    heap.Add(24, value3);
                    AssertHeapSize(heap, 5);
                }

                void AddInternalTest2()
                {
                    var heap = new SoftHeap<TestPriority, TValue>(ErrorRate, new TestPriority(25));
                    AssertHeapSize(heap, 0);

                    heap.Add(new TestPriority(1), value1);
                    AssertHeapSize(heap, 1);

                    heap.Add(new TestPriority(1), value2);
                    AssertHeapSize(heap, 2);

                    heap.Add(new TestPriority(1), value3);
                    AssertHeapSize(heap, 3);

                    heap.Add(new TestPriority(int.MinValue), value1);
                    AssertHeapSize(heap, 4);

                    heap.Add(new TestPriority(24), value3);
                    AssertHeapSize(heap, 5);
                }

                void AddSamePriorityInternalTest()
                {
                    var heap = new SoftHeap<int, TValue>(ErrorRate, 25);
                    AssertHeapSize(heap, 0);

                    heap.Add(1, value1);
                    AssertHeapSize(heap, 1);

                    heap.Add(1, value2);
                    AssertHeapSize(heap, 2);

                    heap.Add(1, value3);
                    AssertHeapSize(heap, 3);

                    heap.Add(1, value1);
                    AssertHeapSize(heap, 4);

                    heap.Add(1, value2);
                    AssertHeapSize(heap, 5);

                    heap.Add(1, value3);
                    AssertHeapSize(heap, 6);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void Add_Throws()
        {
            AddThrowsTest(1);
            AddThrowsTest(new TestVertex("1"));
            AddThrowsTest(new EquatableTestVertex("1"));

            #region Local function

            void AddThrowsTest<TValue>(TValue value)
            {
                AddInternalTest();
                AddInternalTest2();

                #region Local functions

                void AddInternalTest()
                {
                    var heap = new SoftHeap<int, TValue>(
                        ErrorRate,
                        10);
                    Assert.Throws<ArgumentException>(() => heap.Add(150, value));
                }

                void AddInternalTest2()
                {
                    var heap = new SoftHeap<TestPriority, TValue>(
                        ErrorRate,
                        new TestPriority(10));
                    Assert.Throws<ArgumentException>(() => heap.Add(new TestPriority(150), value));
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Assert.Throws<ArgumentNullException>(() => heap.Add(null, value));
                }

                #endregion
            }

            #endregion
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> RemoveMinimumTestCases
        {
            [UsedImplicitly]
            get
            {
                int[] keys1 = { 42 };

                yield return new TestCaseData(keys1, ErrorRate);
                yield return new TestCaseData(keys1, ErrorRate2);

                int[] keys2 = { 1, 2, 3, 5, 10, 2, 4, 6, 4, 3, 2, 150, 11, 42, 13 };

                yield return new TestCaseData(keys2, ErrorRate);
                yield return new TestCaseData(keys2, ErrorRate2);

                int[] keys3 =
                {
                    1, 2, 4, 3, 2, 15, 0, 11, 3, 5, 10, 2, 4, 6, 42, 13,
                    1, 2, 4, 2, 4, 6, 42, 13, 3, 2, 15, 0, 11, 3, 5, 10
                };

                yield return new TestCaseData(keys3, ErrorRate);
                yield return new TestCaseData(keys3, ErrorRate2);

                int[] keys4 =
                {
                    1, 2, 4, 3, 2, 15, 0, 11, 3, 5, 10, 2, 4, 6, 42, 13,
                    1, 2, 4, 2, 4, 6, 42, 13, 3, 2, 15, 0, 11, 3, 5, 10,
                    4, 6, 42, 1, 2, 0, 11, 3, 5, 10, 2, 13, 4, 3, 2, 15,
                    2, 4, 6, 42, 2, 4, 15, 13, 3, 2, 1, 0, 11, 3, 5, 10
                };

                yield return new TestCaseData(keys4, ErrorRate);
                yield return new TestCaseData(keys4, ErrorRate2);
            }
        }

        [TestCaseSource(nameof(RemoveMinimumTestCases))]
        public void RemoveMinimum([NotNull] int[] keys, double errorRate)
        {
            QuikGraphAssert.TrueForAll(keys, k => k < int.MaxValue);
            Assert.IsTrue(keys.Length > 0);

            var heap = new SoftHeap<int, string>(errorRate, int.MaxValue);
            foreach (int key in keys)
            {
                heap.Add(key, key.ToString());
            }
            Assert.AreEqual(keys.Length, heap.Count);

            int lastMinimum = int.MaxValue;
            int nbError = 0;
            while (heap.Count > 0)
            {
                KeyValuePair<int, string> pair = heap.RemoveMinimum();
                // Removed pair can be not the minimal
                // Since it's a "soft" heap that not guarantee 100% result
                // But a faster management
                if (lastMinimum < pair.Key)
                    ++nbError;
                lastMinimum = pair.Key;
                Assert.AreEqual(pair.Key.ToString(), pair.Value);
            }

            Assert.IsTrue(nbError / (double)keys.Length <= errorRate);
        }

        [Test]
        public void RemoveMinimum_Throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => new SoftHeap<int, int>(ErrorRate, int.MaxValue).RemoveMinimum());
        }

        [Test]
        public void EnumerateHeap()
        {
            var heap = new SoftHeap<double, int>(ErrorRate, 10.0);
            CollectionAssert.IsEmpty(heap);

            heap.Add(1.0, 1);
            // Enumerator does nothing!
            CollectionAssert.IsEmpty(heap);

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                Assert.AreEqual(default(KeyValuePair<double, int>), enumerator.Current);
                Assert.Throws<NotSupportedException>(enumerator.Reset);
            }
        }
    }
}