using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Collections;
using static QuikGraph.Collections.HeapConstants;
using static QuikGraph.Tests.AssertHelpers;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="BinaryHeap{TPriority,TValue}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BinaryHeapTests : HeapTestsBase
    {
        #region Test helpers

        /// <summary>
        /// Checks heap invariant values.
        /// </summary>
        private static void AssertInvariants<TPriority, TValue>(
            [NotNull] BinaryHeap<TPriority, TValue> heap)
        {
            Assert.IsTrue(heap.Capacity >= 0, "Capacity test failed.");
            Assert.IsTrue(heap.Count >= 0, "Count test failed.");
            Assert.IsTrue(heap.Count <= heap.Capacity, "Count and capacity comparison failed.");
            Assert.IsTrue(heap.IsConsistent(), "IsConsistent test failed.");
        }

        private static void AssertHeapSize<TPriority, TValue>(
            [NotNull] BinaryHeap<TPriority, TValue> heap,
            int expectedCount)
        {
            AssertInvariants(heap);
            Assert.AreEqual(expectedCount, heap.Count);
        }

        private static void AssertHeapSizes<TPriority, TValue>(
            [NotNull] BinaryHeap<TPriority, TValue> heap,
            int expectedCapacity,
            int expectedCount)
        {
            AssertHeapSize(heap, expectedCount);
            Assert.AreEqual(expectedCapacity, heap.Capacity);
        }

        // ReSharper disable once InconsistentNaming
        [Pure]
        [NotNull]
        private static BinaryHeap<int, int> GetHeapFromTopologicalSortOfDCT8()
        {
            var heap = new BinaryHeap<int, int>(20)
            {
                {0, 255},
                {0, 256},
                {0, 257},
                {0, 258},
                {0, 259},
                {0, 260},
                {0, 261},
                {0, 262},
                {2, 263},
                {2, 264},
                {2, 265},
                {2, 266},
                {2, 267},
                {2, 268},
                {2, 269},
                {2, 270},
                {1, 271},
                {1, 272},
                {1, 273},
                {1, 274},
                {1, 275},
                {1, 276},
                {1, 277},
                {1, 278},
                {2, 279},
                {2, 280},
                {1, 281},
                {1, 282},
                {1, 283},
                {1, 284},
                {2, 285},
                {2, 286},
                {2, 287},
                {2, 288},
                {1, 289},
                {1, 290},
                {1, 291},
                {1, 292},
                {1, 293},
                {1, 294},
                {1, 295},
                {1, 296},
                {1, 297},
                {1, 298},
                {1, 299},
                {2, 300},
                {2, 301},
                {2, 302},
                {2, 303},
                {1, 304},
                {1, 305},
                {1, 306},
                {1, 307},
                {2, 308},
                {2, 309},
                {2, 310},
                {1, 311},
                {2, 312},
                {2, 313},
                {2, 314},
                {1, 315},
                {1, 316},
                {1, 317},
                {1, 318},
                {2, 319},
                {2, 320},
                {2, 321},
                {2, 322},
                {2, 323},
                {2, 324},
                {1, 325},
                {2, 326},
                {2, 327},
                {2, 328},
                {2, 329},
                {1, 330},
                {1, 331},
                {1, 332},
                {1, 333},
                {0, 334},
                {0, 335},
                {0, 336},
                {0, 337},
                {0, 338}
            };
            AssertInvariants(heap);

            return heap;
        }

        #endregion

        [Test]
        public void Constructor()
        {
            AssertHeapBaseProperties(new BinaryHeap<int, IEdge<int>>());

            AssertHeapBaseProperties(new BinaryHeap<int, IEdge<int>>(0), 0);

            AssertHeapBaseProperties(new BinaryHeap<int, IEdge<int>>(42), 42);

            AssertHeapBaseProperties(new BinaryHeap<int, IEdge<int>>(Comparer<int>.Default.Compare));

            AssertHeapBaseProperties(
                new BinaryHeap<int, IEdge<int>>(12, Comparer<int>.Default.Compare),
                12);

            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            AssertHeapProperties(
                new BinaryHeap<int, IEdge<int>>(12, comparer),
                12,
                comparer);

            #region Local functions

            void AssertHeapProperties<TPriority, TValue>(
                BinaryHeap<TPriority, TValue> heap,
                int expectedCapacity,
                Comparison<TPriority> expectedComparer)
            {
                Assert.AreEqual(0, heap.Count);
                Assert.AreEqual(expectedCapacity, heap.Capacity);
                Assert.AreEqual(expectedComparer, heap.PriorityComparison);
            }

            void AssertHeapBaseProperties<TPriority, TValue>(
                BinaryHeap<TPriority, TValue> heap,
                int expectedCapacity = 16)
            {
                AssertHeapProperties(heap, expectedCapacity, Comparer<TPriority>.Default.Compare);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BinaryHeap<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(() => new BinaryHeap<int, IEdge<int>>(12, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BinaryHeap<int, IEdge<int>>(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BinaryHeap<int, IEdge<int>>(-1, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BinaryHeap<int, IEdge<int>>(-1, (x, y) => x.CompareTo(y)));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void HeapSizes()
        {
            {
                var heap = new BinaryHeap<int, int>(0);
                AssertHeapSizes(heap, 0, 0);

                heap.Add(1, 1);
                AssertHeapSizes(heap, 1, 1);

                heap.Add(1, 2);
                AssertHeapSizes(heap, 3, 2);

                heap.Add(1, 3);
                AssertHeapSizes(heap, 3, 3);

                heap.Add(1, 4);
                AssertHeapSizes(heap, 7, 4);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 7, 3);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 7, 2);
            }
            {
                var heap = new BinaryHeap<int, int>(2);
                AssertHeapSizes(heap, 2, 0);

                heap.Add(1, 1);
                AssertHeapSizes(heap, 2, 1);

                heap.Add(1, 2);
                AssertHeapSizes(heap, 2, 2);

                heap.Add(1, 3);
                AssertHeapSizes(heap, 5, 3);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 5, 2);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 5, 1);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 5, 0);
            }
            {
                var heap = new BinaryHeap<int, int>();
                AssertHeapSizes(heap, 16, 0);

                for (int i = 0; i <= 16; ++i)
                {
                    heap.Add(1, i);
                }

                AssertHeapSizes(heap, 33, 17);

                heap.RemoveMinimum();
                AssertHeapSizes(heap, 33, 16);
            }
        }

        [Test]
        public void IndexOf()
        {
            IndexOfTest(1, 2);
            IndexOfTest(
                new TestVertex("1"),
                new TestVertex("2"));
            IndexOfTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"));

            #region Local function

            void IndexOfTest<TValue>(TValue value1, TValue value2)
            {
                IndexOfInternalTest();
                IndexOfInternalTest2();
                IndexOfSamePriorityInternalTest();

                #region Local functions

                void IndexOfInternalTest()
                {
                    var heap = new BinaryHeap<int, TValue>();

                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(10, value1);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(2, value2);
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(5, default);
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.Add(1, value1);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));


                    heap.RemoveMinimum();
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(0, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));
                }

                void IndexOfInternalTest2()
                {
                    var heap = new BinaryHeap<int, TValue>();

                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(10, value1);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(2, value2);
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(5, default);
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.Add(1, value1);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));


                    heap.RemoveMinimum();
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(0, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));
                }

                void IndexOfSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<int, TValue>();

                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(1, value1);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(1, value2);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.Add(1, default);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.Add(1, value2);
                    Assert.AreEqual(0, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(2, heap.IndexOf(default));


                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));   // There is another value2 at index 1 at this step
                    Assert.AreEqual(2, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(1, heap.IndexOf(value2));
                    Assert.AreEqual(0, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(0, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));

                    heap.RemoveMinimum();
                    Assert.AreEqual(-1, heap.IndexOf(value1));
                    Assert.AreEqual(-1, heap.IndexOf(value2));
                    Assert.AreEqual(-1, heap.IndexOf(default));
                }

                #endregion
            }

            #endregion
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
                    var heap = new BinaryHeap<int, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1, value1);
                    AssertHeapSize(heap, 1);

                    heap.Add(1, value2);
                    AssertHeapSize(heap, 2);

                    heap.Add(1, value3);
                    AssertHeapSize(heap, 3);

                    heap.Add(int.MinValue, value1);
                    AssertHeapSize(heap, 4);

                    heap.Add(int.MaxValue, value2);
                    AssertHeapSize(heap, 5);

                    heap.Add(150, value3);
                    AssertHeapSize(heap, 6);
                }

                void AddInternalTest2()
                {
                    var heap = new BinaryHeap<TestPriority, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(new TestPriority(1), value1);
                    AssertHeapSize(heap, 1);

                    heap.Add(new TestPriority(1), value2);
                    AssertHeapSize(heap, 2);

                    heap.Add(new TestPriority(1), value3);
                    AssertHeapSize(heap, 3);

                    heap.Add(new TestPriority(int.MinValue), value1);
                    AssertHeapSize(heap, 4);

                    heap.Add(new TestPriority(int.MaxValue), value2);
                    AssertHeapSize(heap, 5);

                    heap.Add(new TestPriority(150), value3);
                    AssertHeapSize(heap, 6);
                }

                void AddSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<int, TValue>();
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
            var heap = new BinaryHeap<TestPriority, int>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => heap.Add(null, 1));
        }

        [Test]
        public void Minimum()
        {
            MinimumTest(1, 2, 3);
            MinimumTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            MinimumTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void MinimumTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                MinimumInternalTest();
                MinimumInternalTest2();
                MinimumSamePriorityInternalTest();

                #region Local functions

                void MinimumInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(10.0, value3);
                    heap.Add(1.0, value2);
                    heap.Add(9.0, value1);
                    heap.Add(2.0, value2);
                    heap.Add(3.0, value1);
                    heap.Add(4.0, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 6);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 4);

                    pair = heap.Minimum();
                    Assert.AreEqual(3.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 4);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 1);

                    pair = heap.Minimum();
                    Assert.AreEqual(10.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);
                }

                void MinimumInternalTest2()
                {
                    var heap = new BinaryHeap<TestPriority, TValue>();
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority3 = new TestPriority(3);
                    var priority10 = new TestPriority(10);
                    heap.Add(priority10, value3);
                    heap.Add(priority1, value2);
                    heap.Add(new TestPriority(9), value1);
                    heap.Add(new TestPriority(2), value2);
                    heap.Add(priority3, value1);
                    heap.Add(new TestPriority(4), default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<TestPriority, TValue> pair = heap.Minimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 6);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 4);

                    pair = heap.Minimum();
                    Assert.AreSame(priority3, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 4);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 1);

                    pair = heap.Minimum();
                    Assert.AreSame(priority10, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);
                }

                void MinimumSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1.0, value3);
                    heap.Add(1.0, value2);
                    heap.Add(1.0, value1);
                    heap.Add(1.0, value2);
                    heap.Add(1.0, value1);
                    AssertHeapSize(heap, 5);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 5);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 3);

                    heap.RemoveMinimum();
                    heap.RemoveMinimum();
                    AssertHeapSize(heap, 1);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 1);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void Minimum_Throws()
        {
            var heap = new BinaryHeap<int, double>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => heap.Minimum());
        }

        [Test]
        public void RemoveMinimum()
        {
            RemoveMinimumTest(1, 2, 3, 4);
            RemoveMinimumTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            RemoveMinimumTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void RemoveMinimumTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3,
                TValue value4)
            {
                RemoveMinimumInternalTest();
                RemoveMinimumInternalTest2();
                RemoveMinimumSamePriorityInternalTest();

                #region Local functions

                void RemoveMinimumInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(10.0, value1);
                    heap.Add(1.0, value2);
                    heap.Add(9.0, value3);
                    heap.Add(2.0, value2);
                    heap.Add(3.0, value4);
                    heap.Add(6.0, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<double, TValue> pair = heap.RemoveMinimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(2.0, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(3.0, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(6.0, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(9.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(10.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void RemoveMinimumInternalTest2()
                {
                    var heap = new BinaryHeap<TestPriority, TValue>();
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority2 = new TestPriority(2);
                    var priority3 = new TestPriority(3);
                    var priority6 = new TestPriority(6);
                    var priority9 = new TestPriority(9);
                    var priority10 = new TestPriority(10);
                    heap.Add(priority10, value1);
                    heap.Add(priority1, value2);
                    heap.Add(priority9, value3);
                    heap.Add(priority2, value2);
                    heap.Add(priority3, value4);
                    heap.Add(priority6, default);
                    AssertHeapSize(heap, 6);

                    KeyValuePair<TestPriority, TValue> pair = heap.RemoveMinimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 5);

                    pair = heap.RemoveMinimum();
                    Assert.AreSame(priority2, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.RemoveMinimum();
                    Assert.AreSame(priority3, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.RemoveMinimum();
                    Assert.AreSame(priority6, pair.Key);
                    AssertEqual(default, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.RemoveMinimum();
                    Assert.AreSame(priority9, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.RemoveMinimum();
                    Assert.AreSame(priority10, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                void RemoveMinimumSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<int, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1, value1);
                    heap.Add(1, value2);
                    heap.Add(1, value3);
                    heap.Add(1, value2);
                    heap.Add(1, value4);
                    AssertHeapSize(heap, 5);

                    KeyValuePair<int, TValue> pair = heap.RemoveMinimum();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value1, pair.Value);
                    AssertHeapSize(heap, 4);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value4, pair.Value);
                    AssertHeapSize(heap, 3);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 2);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value3, pair.Value);
                    AssertHeapSize(heap, 1);

                    pair = heap.RemoveMinimum();
                    Assert.AreEqual(1, pair.Key);
                    AssertEqual(value2, pair.Value);
                    AssertHeapSize(heap, 0);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void RemoveMinimumUsing_DCT8()
        {
            BinaryHeap<int, int> heap = GetHeapFromTopologicalSortOfDCT8();
            heap.RemoveMinimum();
            AssertInvariants(heap);
        }

        [Test]
        public void RemoveMinimum_Throws()
        {
            var heap = new BinaryHeap<int, double>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => heap.RemoveMinimum());
        }

        [Test]
        public void Update()
        {
            UpdateTest(1, 2, 3);
            UpdateTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            UpdateTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void UpdateTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                UpdateInternalTest();
                UpdateInternalTest2();
                UpdateSamePriorityInternalTest();

                #region Local functions

                void UpdateInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1.0, value1);
                    heap.Add(5.0, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    heap.Update(10.0, value1);  // Priority from 1.0 to 10.0
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(5.0, pair.Key);
                    AssertEqual(value2, pair.Value);

                    heap.Update(11.0, value2);  // Priority from 5.0 to 11.0
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(10.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    heap.Update(2.0, value3);  // Added with priority 2.0
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(2.0, pair.Key);
                    AssertEqual(value3, pair.Value);

                    heap.Update(2.0, value3);  // Already with the given priority
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(2.0, pair.Key);
                    AssertEqual(value3, pair.Value);

                    heap.Update(1.0, value3);  // Priority from 2.0 to 1.0
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value3, pair.Value);
                }

                void UpdateInternalTest2()
                {
                    var heap = new BinaryHeap<TestPriority, TValue>();
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority5 = new TestPriority(5);
                    heap.Add(priority1, value1);
                    heap.Add(priority5, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<TestPriority, TValue> pair = heap.Minimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value1, pair.Value);

                    var priority10 = new TestPriority(10);
                    heap.Update(priority10, value1);  // Priority from 1 to 10
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreSame(priority5, pair.Key);
                    AssertEqual(value2, pair.Value);

                    heap.Update(new TestPriority(11), value2);  // Priority from 5 to 11
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreSame(priority10, pair.Key);
                    AssertEqual(value1, pair.Value);

                    var priority2 = new TestPriority(2);
                    heap.Update(priority2, value3);  // Added with priority 2
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreSame(priority2, pair.Key);
                    AssertEqual(value3, pair.Value);

                    var priority2Bis = new TestPriority(2);
                    heap.Update(priority2Bis, value3);  // Already with the given priority
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreSame(priority2Bis, pair.Key);
                    AssertEqual(value3, pair.Value);

                    heap.Update(priority1, value3);  // Priority from 2 to 1
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value3, pair.Value);
                }

                void UpdateSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1.0, value1);
                    heap.Add(1.0, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    heap.Update(1.0, value1);  // Already with the given priority
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    heap.Update(1.0, value2);  // Already with the given priority
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    heap.Update(1.0, value3);
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void UpdateUsing_DCT8()
        {
            BinaryHeap<int, int> heap = GetHeapFromTopologicalSortOfDCT8();
            heap.Update(1, 320);
            AssertInvariants(heap);
        }

        [Test]
        public void MinimalUpdate()
        {
            MinimalUpdateTest(1, 2, 3);
            MinimalUpdateTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"));
            MinimalUpdateTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"));

            #region Local function

            void MinimalUpdateTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3)
            {
                MinimalUpdateInternalTest();
                MinimalUpdateInternalTest2();
                MinimalUpdateSamePriorityInternalTest();

                #region Local functions

                void MinimalUpdateInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1.0, value1);
                    heap.Add(5.0, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    Assert.IsFalse(heap.MinimumUpdate(10.0, value1));  // Priority not updated
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(0.5, value2));  // Priority from 5.0 to 0.5
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(0.5, pair.Key);
                    AssertEqual(value2, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(0.25, value3));  // Added with priority 0.25
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(0.25, pair.Key);
                    AssertEqual(value3, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(0.25, value3));  // Already with the given priority
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(0.25, pair.Key);
                    AssertEqual(value3, pair.Value);
                }

                void MinimalUpdateInternalTest2()
                {
                    var heap = new BinaryHeap<TestPriority, TValue>();
                    AssertHeapSize(heap, 0);

                    var priority1 = new TestPriority(1);
                    var priority5 = new TestPriority(5);
                    heap.Add(priority1, value1);
                    heap.Add(priority5, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<TestPriority, TValue> pair = heap.Minimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value1, pair.Value);

                    var priority10 = new TestPriority(10);
                    Assert.IsFalse(heap.MinimumUpdate(priority10, value1));  // Priority not updated
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreSame(priority1, pair.Key);
                    AssertEqual(value1, pair.Value);

                    var priority0 = new TestPriority(0);
                    Assert.IsTrue(heap.MinimumUpdate(priority0, value2));  // Priority from 5 to 0
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreSame(priority0, pair.Key);
                    AssertEqual(value2, pair.Value);

                    var priorityMinus1 = new TestPriority(-1);
                    Assert.IsTrue(heap.MinimumUpdate(priorityMinus1, value3));  // Added with priority -1
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreSame(priorityMinus1, pair.Key);
                    AssertEqual(value3, pair.Value);

                    var priorityMinus1Bis = new TestPriority(-1);
                    Assert.IsTrue(heap.MinimumUpdate(priorityMinus1Bis, value3));  // Already with the given priority
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreSame(priorityMinus1Bis, pair.Key);
                    AssertEqual(value3, pair.Value);
                }

                void MinimalUpdateSamePriorityInternalTest()
                {
                    var heap = new BinaryHeap<double, TValue>();
                    AssertHeapSize(heap, 0);

                    heap.Add(1.0, value1);
                    heap.Add(1.0, value2);
                    AssertHeapSize(heap, 2);

                    KeyValuePair<double, TValue> pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(1.0, value1));  // Already with the given priority
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(1.0, value2));  // Already with the given priority
                    AssertHeapSize(heap, 2);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);

                    Assert.IsTrue(heap.MinimumUpdate(1.0, value3));
                    AssertHeapSize(heap, 3);

                    pair = heap.Minimum();
                    Assert.AreEqual(1.0, pair.Key);
                    AssertEqual(value1, pair.Value);
                }

                #endregion
            }

            #endregion
        }

        [Test]
        public void EnumerateHeap()
        {
            var heap = new BinaryHeap<double, int>();
            CollectionAssert.IsEmpty(heap);

            heap.Add(1.0, 1);
            CollectionAssert.AreEquivalent(
                new[] { new KeyValuePair<double, int>(1.0, 1) },
                heap);

            heap.Add(12.0, 1);
            heap.Add(10.0, 2);
            heap.Add(5.0, 4);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new KeyValuePair<double, int>(1.0, 1),
                    new KeyValuePair<double, int>(5.0, 4),
                    new KeyValuePair<double, int>(10.0, 2),
                    new KeyValuePair<double, int>(12.0, 1)
                },
                heap);

            heap.RemoveMinimum();
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new KeyValuePair<double, int>(5.0, 4),
                    new KeyValuePair<double, int>(10.0, 2),
                    new KeyValuePair<double, int>(12.0, 1)
                },
                heap);

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsFalse(enumerator.MoveNext());

                enumerator.Reset();

                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsFalse(enumerator.MoveNext());
            }

            while (heap.Count > 0)
            {
                heap.RemoveMinimum();
            }

            CollectionAssert.IsEmpty(heap);
        }

        [Test]
        public void EnumerateHeap_Throws()
        {
            var heap = new BinaryHeap<double, int> {{1.0, 1}, {12.0, 1}, {10.0, 2}, {5.0, 4}};

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                for (int i = 0; i <= heap.Count; ++i)
                    enumerator.MoveNext();

                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.Add(111.111, 12);

                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(enumerator.Reset);
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.RemoveMinimum();

                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(enumerator.Reset);
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.Update(13.0, 42);

                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(enumerator.Reset);
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.Update(12.0, 42);

                // No new or removed element
                Assert.DoesNotThrow(() => { var _ = enumerator.Current; });
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(enumerator.Reset);
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.MinimumUpdate(12.0, 25);

                Assert.Throws<InvalidOperationException>(() => { var _ = enumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(enumerator.Reset);
            }

            using (IEnumerator<KeyValuePair<double, int>> enumerator = heap.GetEnumerator())
            {
                enumerator.MoveNext();

                heap.MinimumUpdate(1.0, 42);

                // No new or removed element
                Assert.DoesNotThrow(() => { var _ = enumerator.Current; });
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(enumerator.Reset);
            }
        }

        #region Test helpers

        private static void ToStringCommon<TValue>(
            [NotNull] TValue value1,
            [NotNull] TValue value2,
            [NotNull] TValue value3,
            [NotNull] TValue value4,
            [NotNull, InstantHandle] Func<BinaryHeap<int, TValue>, string> toString)
        {
            var heap = new BinaryHeap<int, TValue>();

            // Empty heap => consistent
            StringAssert.StartsWith(Consistent, toString(heap));

            heap.Add(1, value1);
            heap.Add(5, value2);
            heap.Add(3, value2);
            heap.Add(2, value3);
            heap.Add(5, value1);

            StringAssert.StartsWith(Consistent, toString(heap));

            heap.RemoveMinimum();
            heap.RemoveMinimum();

            StringAssert.StartsWith(Consistent, toString(heap));

            heap.RemoveMinimum();
            heap.RemoveMinimum();

            StringAssert.StartsWith(Consistent, toString(heap));

            heap.Add(12, value4);

            StringAssert.StartsWith(Consistent, toString(heap));
        }

        private static void ToStringClassCommon<TValue>(
            [NotNull, InstantHandle] Func<BinaryHeap<int, TValue>, string> toString)
            where TValue : class
        {
            var heap = new BinaryHeap<int, TValue>();

            // Empty heap => consistent
            StringAssert.StartsWith(Consistent, toString(heap));

            heap.Add(1, null);

            StringAssert.StartsWith(Consistent, toString(heap));

            heap.RemoveMinimum();

            StringAssert.StartsWith(Consistent, toString(heap));
        }

        private static void ToString2Common<TValue>(
            [NotNull] TValue value1,
            [NotNull] TValue value2,
            [NotNull] TValue value3,
            [NotNull] TValue value4)
        {
            ToStringCommon(value1, value2, value3, value4, heap => heap.ToString2());
        }

        private static void ToStringTreeCommon<TValue>(
            [NotNull] TValue value1,
            [NotNull] TValue value2,
            [NotNull] TValue value3,
            [NotNull] TValue value4)
        {
            ToStringCommon(value1, value2, value3, value4, heap => heap.ToStringTree());
        }

        #endregion

        [Test]
        public void ToString2()
        {
            ToString2Common(1, 2, 3, 4);
            ToString2Common(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToString2Common(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            ToStringClassCommon<TestVertex>(heap => heap.ToString2());
        }

        [Test]
        public void ToString2Format()
        {
            var heap = new BinaryHeap<int, int>(20)
            {
                {1, 0},
                {2, 1},
                {1, 2},
                {2, 3},
                {2, 4},
                {1, 5},
                {1, 6},
                {2, 7},
                {2, 8},
                {2, 9},
                {2, 10},
                {1, 11},
                {1, 12},
                {1, 13},
                {1, 14}
            };

            string toString =
                $"{Consistent}: 1 0, 2 1, 1 2, 2 3, 2 4, 1 5, 1 6, 2 7, 2 8, 2 9, 2 10, 1 11, 1 12, 1 13, 1 14, null, null, null, null, null";
            StringAssert.AreEqualIgnoringCase(toString, heap.ToString2());
            AssertHeapSizes(heap, 20, 15);
        }

        [Test]
        public void ToStringTree()
        {
            ToStringTreeCommon(1, 2, 3, 4);
            ToStringTreeCommon(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToStringTreeCommon(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            ToStringClassCommon<TestVertex>(heap => heap.ToStringTree());
        }

        [Test]
        public void ToStringTreeFormat()
        {
            var heap = new BinaryHeap<int, int>(20)
            {
                {1, 0},
                {2, 1},
                {1, 2},
                {2, 3},
                {2, 4},
                {1, 5},
                {1, 6},
                {2, 7},
                {2, 8},
                {2, 9},
                {2, 10},
                {1, 11},
                {1, 12},
                {1, 13},
                {1, 14}
            };

            var toString = new StringBuilder($"{Consistent}{Environment.NewLine}");
            toString.AppendLine("index0 1 0 -> 2 1 and 1 2");
            toString.AppendLine("index1 2 1 -> 2 3 and 2 4");
            toString.AppendLine("index2 1 2 -> 1 5 and 1 6");
            toString.AppendLine("index3 2 3 -> 2 7 and 2 8");
            toString.AppendLine("index4 2 4 -> 2 9 and 2 10");
            toString.AppendLine("index5 1 5 -> 1 11 and 1 12");
            toString.AppendLine("index6 1 6 -> 1 13 and 1 14");
            toString.AppendLine("index7 2 7 -> null and null");
            toString.AppendLine("index8 2 8 -> null and null");
            toString.AppendLine("index9 2 9 -> null and null");
            toString.AppendLine("index10 2 10 -> null and null");
            toString.AppendLine("index11 1 11 -> null and null");
            toString.AppendLine("index12 1 12 -> null and null");
            toString.AppendLine("index13 1 13 -> null and null");
            toString.Append("index14 1 14 -> null and null");
            StringAssert.AreEqualIgnoringCase(
                toString.ToString(),
                heap.ToStringTree());
            AssertHeapSizes(heap, 20, 15);
        }

        [Test]
        public void ToArray()
        {
            ToArrayTest(1, 2, 3, 4);
            ToArrayTest(
                new TestVertex("1"),
                new TestVertex("2"),
                new TestVertex("3"),
                new TestVertex("4"));
            ToArrayTest(
                new EquatableTestVertex("1"),
                new EquatableTestVertex("2"),
                new EquatableTestVertex("3"),
                new EquatableTestVertex("4"));

            #region Local function

            void ToArrayTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3,
                TValue value4)
            {
                var heap = new BinaryHeap<double, TValue>();

                // Empty heap
                CollectionAssert.IsEmpty(heap.ToArray());

                heap.Add(1.0, value1);
                heap.Add(5.0, value2);
                heap.Add(4.0, value2);
                heap.Add(2.0, value3);
                heap.Add(3.0, value1);
                heap.Add(6.0, default);

                // Array not sorted
                CollectionAssert.AreEquivalent(
                    new[] { value1, value2, value2, value3, value1, default },
                    heap.ToArray());

                heap.RemoveMinimum();
                heap.RemoveMinimum();

                // Array not sorted
                CollectionAssert.AreEquivalent(
                    new[] { value1, value2, value2, default },
                    heap.ToArray());

                heap.RemoveMinimum();
                heap.RemoveMinimum();
                heap.RemoveMinimum();
                heap.RemoveMinimum();

                CollectionAssert.IsEmpty(heap.ToArray());

                heap.Add(12.0, value4);
                CollectionAssert.AreEqual(
                    new[] { value4 },
                    heap.ToArray());
            }

            #endregion
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

            void ToPairsArrayTest<TValue>(
                TValue value1,
                TValue value2,
                TValue value3,
                TValue value4)
            {
                var heap = new BinaryHeap<double, TValue>();

                // Empty heap
                CollectionAssert.IsEmpty(heap.ToPairsArray());

                heap.Add(1.0, value1);
                heap.Add(5.0, value2);
                heap.Add(4.0, value2);
                heap.Add(2.0, value3);
                heap.Add(3.0, value1);
                heap.Add(6.0, default);

                // Array not sorted
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        new KeyValuePair<double, TValue>(1.0, value1),
                        new KeyValuePair<double, TValue>(5.0, value2),
                        new KeyValuePair<double, TValue>(4.0, value2),
                        new KeyValuePair<double, TValue>(2.0, value3),
                        new KeyValuePair<double, TValue>(3.0, value1),
                        new KeyValuePair<double, TValue>(6.0, default)
                    },
                    heap.ToPairsArray());

                heap.RemoveMinimum();
                heap.RemoveMinimum();

                // Array not sorted
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        new KeyValuePair<double, TValue>(3.0, value1),
                        new KeyValuePair<double, TValue>(5.0, value2),
                        new KeyValuePair<double, TValue>(4.0, value2),
                        new KeyValuePair<double, TValue>(6.0, default)
                    },
                    heap.ToPairsArray());

                heap.RemoveMinimum();
                heap.RemoveMinimum();
                heap.RemoveMinimum();
                heap.RemoveMinimum();

                CollectionAssert.IsEmpty(heap.ToPairsArray());

                heap.Add(12.0, value4);
                CollectionAssert.AreEqual(
                    new[]
                    {
                        new KeyValuePair<double, TValue>(12.0, value4)
                    },
                    heap.ToPairsArray());
            }

            #endregion
        }
    }
}