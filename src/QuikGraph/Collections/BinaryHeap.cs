using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using static QuikGraph.Collections.HeapConstants;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Binary heap.
    /// </summary>
    /// <remarks>
    /// Indexing rules:
    /// 
    /// parent index: (index - 1)/2
    /// left child: 2 * index + 1
    /// right child: 2 * index + 2
    /// 
    /// Reference:
    /// http://dotnetslackers.com/Community/files/folders/data-structures-and-algorithms/entry28722.aspx
    /// </remarks>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <typeparam name="TPriority">Priority metric type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class BinaryHeap<TPriority, TValue> : IEnumerable<KeyValuePair<TPriority, TValue>>
    {
        private int _version;

        private const int DefaultCapacity = 16;

        [NotNull]
        private KeyValuePair<TPriority, TValue>[] _items;

        /// <summary>
        /// Initializes a new <see cref="BinaryHeap{TPriority,TValue}"/> class.
        /// </summary>
        public BinaryHeap()
            : this(DefaultCapacity, Comparer<TPriority>.Default.Compare)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="BinaryHeap{TPriority,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Heap capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public BinaryHeap(int capacity)
            : this(capacity, Comparer<TPriority>.Default.Compare)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="BinaryHeap{TPriority,TValue}"/> class.
        /// </summary>
        /// <param name="priorityComparison">Priority comparer.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="priorityComparison"/> is <see langword="null"/>.</exception>
        public BinaryHeap([NotNull] Comparison<TPriority> priorityComparison)
            : this(DefaultCapacity, priorityComparison)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="BinaryHeap{TPriority,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Heap capacity.</param>
        /// <param name="priorityComparison">Priority comparer.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="priorityComparison"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public BinaryHeap(int capacity, [NotNull] Comparison<TPriority> priorityComparison)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

            _items = new KeyValuePair<TPriority, TValue>[capacity];
            PriorityComparison = priorityComparison ?? throw new ArgumentNullException(nameof(priorityComparison));
        }

        /// <summary>
        /// Priority comparer.
        /// </summary>
        [NotNull]
        public Comparison<TPriority> PriorityComparison { get; }

        /// <summary>
        /// Heap capacity.
        /// </summary>
        public int Capacity => _items.Length;

        /// <summary>
        /// Number of element.
        /// </summary>
        public int Count { get; private set; }

        #region Helpers

        [Pure]
        private bool Less(int i, int j)
        {
            Debug.Assert(i >= 0 && i < Count);
            Debug.Assert(j >= 0 && j < Count);

            return PriorityComparison(_items[i].Key, _items[j].Key) < 0;
        }

        private void Swap(int i, int j)
        {
            Debug.Assert(i >= 0 && i < Count);
            Debug.Assert(j >= 0 && j < Count);
            Debug.Assert(i != j);

            KeyValuePair<TPriority, TValue> kv = _items[i];
            _items[i] = _items[j];
            _items[j] = kv;
        }

        #endregion

        /// <summary>
        /// Adds the given <paramref name="value"/> (with priority) into the heap.
        /// </summary>
        /// <param name="priority">Item priority.</param>
        /// <param name="value">The value.</param>
        public void Add([NotNull] TPriority priority, [CanBeNull] TValue value)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine($"{nameof(Add)}({priority}, {value})");
#endif
            if (priority == null)
                throw new ArgumentNullException(nameof(priority));

            ++_version;
            ResizeArray();
            _items[Count++] = new KeyValuePair<TPriority, TValue>(priority, value);
            MinHeapifyUp(Count - 1);

#if BINARY_HEAP_DEBUG
            Console.WriteLine($"{nameof(Add)}: {ToString2()}");
#endif

            #region Local function

            void ResizeArray()
            {
                if (Count != _items.Length)
                    return;

                var newItems = new KeyValuePair<TPriority, TValue>[Count * 2 + 1];
                Array.Copy(_items, newItems, Count);
                _items = newItems;
            }

            #endregion
        }

        private void MinHeapifyUp(int start)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine(nameof(MinHeapifyUp));
#endif
            int current = start;
            int parent = (current - 1) / 2;
            while (current > 0 && Less(current, parent))
            {
                Swap(current, parent);
                current = parent;
                parent = (current - 1) / 2;
            }
        }

        /// <summary>
        /// Gets the minimum pair.
        /// </summary>
        /// <returns>The minimal pair.</returns>
        /// <exception cref="T:System.InvalidOperationException">The heap is empty.</exception>
        public KeyValuePair<TPriority, TValue> Minimum()
        {
            if (Count == 0)
                throw new InvalidOperationException("Heap is empty.");
            return _items[0];
        }

        /// <summary>
        /// Gets and removes the minimal pair.
        /// </summary>
        /// <returns>The minimal pair.</returns>
        /// <exception cref="T:System.InvalidOperationException">The heap is empty.</exception>
        public KeyValuePair<TPriority, TValue> RemoveMinimum()
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine(nameof(RemoveMinimum));
#endif

            if (Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            ++_version;

            // Shortcut for heap with 1 element.
            if (Count == 1)
                return _items[--Count];

            Swap(0, Count - 1);
            --Count;
            MinHeapifyDown(0);

            return _items[Count];
        }

        private void MinHeapifyDown(int index)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine(nameof(MinHeapifyDown));
#endif

            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;
                if (left < Count && Less(left, smallest))
                {
                    smallest = left;
                }
                if (right < Count && Less(right, smallest))
                {
                    smallest = right;
                }

                if (smallest == index)
                    break;

                Swap(smallest, index);
                index = smallest;
            }
        }

        /// <summary>
        /// Gets the index of the given <paramref name="value"/> in the heap.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Index of the value if found, otherwise -1.</returns>
        [Pure]
        public int IndexOf([CanBeNull] TValue value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(value, _items[i].Value))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Updates the priority of the given <paramref name="value"/> (or add it if not present).
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="value">The value.</param>
        public void Update([NotNull] TPriority priority, [NotNull] TValue value)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine($"{nameof(Update)}({priority}, {value})");
#endif

            // Find index
            int index = IndexOf(value);

            // If it exists, update, else add
            if (index > -1)
            {
                TPriority oldPriority = _items[index].Key;
                _items[index] = new KeyValuePair<TPriority, TValue>(priority, value);

                if (PriorityComparison(priority, oldPriority) > 0)
                {
                    MinHeapifyDown(index);
                }
                else if (PriorityComparison(priority, oldPriority) < 0)
                {
                    MinHeapifyUp(index);
                }
            }
            else
            {
                Add(priority, value);
            }
        }

        /// <summary>
        /// Updates the priority of the given <paramref name="value"/> if the new priority is lower
        /// than the current <paramref name="value"/> priority (or add it if not present).
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if the heap was updated, false otherwise.</returns>
        public bool MinimumUpdate([NotNull] TPriority priority, [NotNull] TValue value)
        {
            // Find index
            int index = IndexOf(value);
            if (index >= 0)
            {
                if (PriorityComparison(priority, _items[index].Key) <= 0)
                {
                    Update(priority, value);
                    return true;
                }

                return false;
            }

            // Not in collection
            Add(priority, value);
            return true;
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TPriority, TValue>>
        {
            private BinaryHeap<TPriority, TValue> _owner;

            private KeyValuePair<TPriority, TValue>[] _items;

            private readonly int _count;
            private readonly int _version;
            private int _index;

            public Enumerator([NotNull] BinaryHeap<TPriority, TValue> owner)
            {
                _owner = owner;
                _items = owner._items;
                _count = owner.Count;
                _version = owner._version;
                _index = -1;
            }

            public KeyValuePair<TPriority, TValue> Current
            {
                get
                {
                    if (_version != _owner._version)
                        throw new InvalidOperationException();
                    if (_index < 0 || _index == _count)
                        throw new InvalidOperationException();

                    Debug.Assert(_index <= _count);

                    return _items[_index];
                }
            }

            void IDisposable.Dispose()
            {
                _owner = null;
                _items = null;
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException();
                return ++_index < _count;
            }

            public void Reset()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException();
                _index = -1;
            }
        }

        #endregion

        /// <summary>
        /// Gets all heap values.
        /// </summary>
        /// <returns>Array of heap values.</returns>
        [Pure]
        [NotNull]
        public TValue[] ToArray()
        {
            var array = new TValue[Count];
            for (int i = 0; i < Count; ++i)
            {
                array[i] = _items[i].Value;
            }
            return array;
        }

        /// <summary>
        /// Gets all values with their priorities.
        /// </summary>
        /// <returns>Array of heap priorities and values.</returns>
        [Pure]
        [NotNull]
        public KeyValuePair<TPriority, TValue>[] ToPairsArray()
        {
            var array = new KeyValuePair<TPriority, TValue>[Count];
            Array.Copy(_items, 0, array, 0, Count);
            return array;
        }

        [NotNull]
        private string EntryToString(int i)
        {
            if (i < 0 || i >= Count)
                return "null";

            KeyValuePair<TPriority, TValue> kvp = _items[i];
            TPriority k = kvp.Key;
            TValue v = kvp.Value;

            return $"{k} {(v == null ? "null" : v.ToString())}";
        }

        /// <summary>
        /// Checks if this heap is consistent (fulfill indexing rule).
        /// </summary>
        /// <returns>True if the heap is consistent, false otherwise.</returns>
        [Pure]
        public bool IsConsistent()
        {
            int wrong = -1;

            for (int i = 0; i < Count; ++i)
            {
                int l = 2 * i + 1;
                int r = 2 * i + 2;
                if (l < Count && !LessOrEqual(i, l))
                {
                    wrong = i;
                }

                if (r < Count && !LessOrEqual(i, r))
                {
                    wrong = i;
                }
            }

            bool correct = wrong == -1;
            return correct;

            #region Local function

            bool LessOrEqual(int i, int j)
            {
                Debug.Assert(
                    i >= 0 && i < Count
                           && j >= 0 && j < Count
                           && i != j);

                return PriorityComparison(_items[i].Key, _items[j].Key) <= 0;
            }

            #endregion
        }

        /// <summary>
        /// Gets a string representation of this heap.
        /// </summary>
        /// <returns>String representation.</returns>
        [Pure]
        [NotNull]
        public string ToString2()
        {
            bool status = IsConsistent();
            return $"{(status ? Consistent : NotConsistent)}: {string.Join(", ", Enumerable.Range(0, _items.Length).Select(EntryToString).ToArray())}";
        }

        /// <summary>
        /// Gets a string tree representation of this heap.
        /// </summary>
        /// <returns>String representation.</returns>
        [Pure]
        [NotNull]
        public string ToStringTree()
        {
            bool status = IsConsistent();
            var str = new StringBuilder(status ? Consistent : NotConsistent);

            for (int i = 0; i < Count; ++i)
            {
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                str.Append(
                    $"{Environment.NewLine}index{i} {EntryToString(i)} -> {EntryToString(l)} and {EntryToString(r)}");
            }

            return str.ToString();
        }
    }
}