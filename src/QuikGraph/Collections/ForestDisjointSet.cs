using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Disjoint-set implementation with path compression and union-by-rank optimizations.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public class ForestDisjointSet<T> : IDisjointSet<T>
    {
#if DEBUG
        [DebuggerDisplay("{" + nameof(_id) + "}:{" + nameof(Rank) + "}->{" + nameof(Parent) + "}")]
#endif
        private sealed class Element
        {
#if DEBUG
            private readonly int _id;
            // ReSharper disable once StaticMemberInGenericType
            private static int _nextId;
#endif

            [CanBeNull]
            public Element Parent { get; set; }

            public int Rank { get; set; }

            [NotNull]
            public T Value { get; }

            public Element([NotNull] T value)
            {
                Debug.Assert(value != null);
#if DEBUG
                _id = _nextId++;
#endif
                Parent = null;
                Rank = 0;
                Value = value;
            }
        }

        [NotNull]
        private readonly Dictionary<T, Element> _elements;

        /// <summary>
        /// Initializes a new <see cref="ForestDisjointSet{T}"/> class.
        /// </summary>
        public ForestDisjointSet()
        {
            _elements = new Dictionary<T, Element>();
            SetCount = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="ForestDisjointSet{T}"/> class.
        /// </summary>
        /// <param name="capacity">Element capacity.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is negative or <see cref="F:int.MaxValue"/>.</exception>
        public ForestDisjointSet(int capacity)
        {
            if (capacity < 0 || capacity >= int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive nor max value.");

            _elements = new Dictionary<T, Element>(capacity);
            SetCount = 0;
        }

        #region IDisjointSet<T>

        /// <inheritdoc />
        public int SetCount { get; private set; }

        /// <inheritdoc />
        public int ElementCount => _elements.Count;

        /// <inheritdoc />
        public void MakeSet(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var element = new Element(value);
            _elements.Add(value, element);
            ++SetCount;
        }

        /// <inheritdoc />
        public T FindSet(T value)
        {
            return Find(_elements[value]).Value;
        }

        /// <inheritdoc />
        public bool AreInSameSet(T left, T right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return EqualityComparer<T>.Default.Equals(FindSet(left), FindSet(right));
        }

        /// <inheritdoc />
        public bool Union(T left, T right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return Union(_elements[left], _elements[right]);
        }

        /// <inheritdoc />
        public bool Contains(T value)
        {
            return _elements.ContainsKey(value);
        }

        #endregion

        [Pure]
        [NotNull]
        private static Element FindNoCompression([NotNull] Element element)
        {
            Debug.Assert(element != null);

            // Find root
            Element current = element;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            Debug.Assert(current != null);
            return current;
        }

        /// <summary>
        /// Finds the root parent element, and applies path compression.
        /// </summary>
        /// <param name="element">Element to search parent.</param>
        /// <returns>Root parent element.</returns>
        [NotNull]
        private static Element Find([NotNull] Element element)
        {
            Debug.Assert(element != null);

            Element root = FindNoCompression(element);
            CompressPath(element, root);

            Debug.Assert(root != null);
            return root;
        }

        private static void CompressPath([NotNull] Element element, [NotNull] Element root)
        {
            Debug.Assert(element != null);
            Debug.Assert(root != null);

            // Path compression
            Element current = element;
            while (current != root && current != null)
            {
                Element temp = current;
                current = current.Parent;
                temp.Parent = root;
            }
        }

        private bool Union([NotNull] Element left, [NotNull] Element right)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);

            // Shortcut when already unioned
            if (left == right)
                return false;

            Element leftRoot = Find(left);
            Element rightRoot = Find(right);

            // Union by rank
            if (leftRoot.Rank > rightRoot.Rank)
            {
                rightRoot.Parent = leftRoot;
            }
            else if (leftRoot.Rank < rightRoot.Rank)
            {
                leftRoot.Parent = rightRoot;
            }
            else if (leftRoot != rightRoot)
            {
                rightRoot.Parent = leftRoot;
                leftRoot.Rank += 1;
            }
            else
            {
                Debug.Assert(FindNoCompression(left) == FindNoCompression(right));
                return false; // Do not update the SetCount
            }

            --SetCount;

            Debug.Assert(FindNoCompression(left) == FindNoCompression(right));
            return true;
        }
    }
}