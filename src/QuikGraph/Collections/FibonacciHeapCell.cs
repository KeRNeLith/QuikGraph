#if SUPPORTS_SERIALIZATION
using System;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Represents a cell that stores a value with its priority.
    /// </summary>
    /// <typeparam name="TPriority">Priority type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class FibonacciHeapCell<TPriority, TValue>
    {
        /// <summary>
        /// Determines if a node has had a child cut from it before.
        /// </summary>
        public bool Marked { get; internal set; }

        /// <summary>
        /// Determines the depth of a node.
        /// </summary>
        public int Degree { get; internal set; }

        /// <summary>
        /// Gets or sets the value priority.
        /// </summary>
        public TPriority Priority { get; internal set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public TValue Value { get; internal set; }

        /// <summary>
        /// Gets or sets the state removed of a cell.
        /// </summary>
        public bool Removed { get; internal set; }

        /// <summary>
        /// Parent cell.
        /// </summary>
        [CanBeNull]
        public FibonacciHeapCell<TPriority, TValue> Parent { get; internal set; }

        /// <summary>
        /// Children cells.
        /// </summary>
        [CanBeNull]
        public FibonacciHeapLinkedList<TPriority, TValue> Children { get; internal set; }

        /// <summary>
        /// Previous cell.
        /// </summary>
        [CanBeNull]
        public FibonacciHeapCell<TPriority, TValue> Previous { get; internal set; }

        /// <summary>
        /// Next cell.
        /// </summary>
        [CanBeNull]
        public FibonacciHeapCell<TPriority, TValue> Next { get; internal set; }

        /// <summary>
        /// Converts this cell to a <see cref="T:System.Collections.Generic.KeyValuePair{TPriority,TValue}"/>.
        /// </summary>
        /// <returns>A corresponding <see cref="T:System.Collections.Generic.KeyValuePair{TPriority,TValue}"/>.</returns>
        [Pure]
        public KeyValuePair<TPriority, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TPriority, TValue>(Priority, Value);
        }
    }
}