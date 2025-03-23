#if !SUPPORTS_SORTEDSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using JetBrains.Annotations;
using QuikGraph.Utils;

namespace QuikGraph.Collections
{
    /// <summary>
    /// This interface provides methods for implementing sets, which are collections
    /// that have unique elements and specific operations.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public interface ISet<T> : ICollection<T>
    {
    }

    internal delegate bool TreeWalkPredicate<T>(SortedSet<T>.Node node);

    internal enum TreeRotation
    {
        LeftRotation = 1,
        RightRotation = 2,
        RightLeftRotation = 3,
        LeftRightRotation = 4
    }

    /// <summary>
    /// Represents a collection of objects that is maintained in sorted order.
    /// Container used when standard collection is not available in target version.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public class SortedSet<T> : ISet<T>, ICollection, ISerializable, IDeserializationCallback, IReadOnlyCollection<T>
    {
        #region Local variables/Constants

        private Node _root;
        private int _version;
        private object _syncRoot;

        private const string ComparerName = "Comparer";
        private const string CountName = "Count";
        private const string ItemsName = "Items";
        private const string VersionName = "Version";
        // Needed for enumerator
        private const string TreeName = "Tree";
        private const string NodeValueName = "Item";
        private const string EnumStartName = "EnumStarted";
        private const string ReverseName = "Reverse";
        private const string EnumVersionName = "EnumVersion";

        internal const int StackAllocThreshold = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="SortedSet{T}"/> class.
        /// </summary>
        public SortedSet()
        {
            Comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new <see cref="SortedSet{T}"/> class
        /// using the given <paramref name="comparer"/>, or <see cref="Comparer{T}.Default"/> otherwise.
        /// </summary>
        /// <param name="comparer"><see cref="Comparer{T}"/> to use.</param>
        public SortedSet([CanBeNull] IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new <see cref="SortedSet{T}"/> class
        /// with given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">Initial elements to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public SortedSet([NotNull, ItemCanBeNull] IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="SortedSet{T}"/> class
        /// with given <paramref name="collection"/>.
        /// Uses the given <paramref name="comparer"/>, or <see cref="Comparer{T}.Default"/> otherwise.
        /// </summary>
        /// <param name="collection">Initial elements to add.</param>
        /// <param name="comparer"><see cref="Comparer{T}"/> to use.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public SortedSet([NotNull, ItemCanBeNull] IEnumerable<T> collection, [CanBeNull] IComparer<T> comparer)
            : this(comparer)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            // These are explicit type checks in the mould of HashSet. It would have worked better
            // with something like an ISorted<T> (we could make this work for SortedList.Keys etc)
            if (collection is SortedSet<T> baseSortedSet && AreComparersEqual(this, baseSortedSet))
            {
                // Breadth first traversal to recreate nodes
                if (baseSortedSet.Count == 0)
                {
                    Count = 0;
                    _version = 0;
                    _root = null;
                    return;
                }

                // Pre order way to replicate nodes
                var theirStack = new Stack<Node>(2 * Log2(baseSortedSet.Count) + 2);
                var myStack = new Stack<Node>(2 * Log2(baseSortedSet.Count) + 2);
                Node theirCurrent = baseSortedSet._root;
                Node myCurrent = theirCurrent is null ? null : new Node(theirCurrent.Item, theirCurrent.IsRed);
                _root = myCurrent;
                while (theirCurrent != null)
                {
                    theirStack.Push(theirCurrent);
                    myStack.Push(myCurrent);
                    // ReSharper disable once PossibleNullReferenceException
                    myCurrent.Left = theirCurrent.Left is null ? null : new Node(theirCurrent.Left.Item, theirCurrent.Left.IsRed);
                    theirCurrent = theirCurrent.Left;
                    myCurrent = myCurrent.Left;
                }

                while (theirStack.Count != 0)
                {
                    theirCurrent = theirStack.Pop();
                    myCurrent = myStack.Pop();
                    Node theirRight = theirCurrent.Right;
                    Node myRight = null;
                    if (theirRight != null)
                    {
                        myRight = new Node(theirRight.Item, theirRight.IsRed);
                    }
                    myCurrent.Right = myRight;

                    while (theirRight != null)
                    {
                        theirStack.Push(theirRight);
                        myStack.Push(myRight);
                        // ReSharper disable once PossibleNullReferenceException
                        myRight.Left = theirRight.Left is null ? null : new Node(theirRight.Left.Item, theirRight.Left.IsRed);
                        theirRight = theirRight.Left;
                        myRight = myRight.Left;
                    }
                }

                Count = baseSortedSet.Count;
                _version = 0;
            }
            else
            {
                // As it stands, you're doing an NlogN sort of the collection
                var els = new List<T>(collection);
                els.Sort(Comparer);
                for (int i = 1; i < els.Count; ++i)
                {
                    if (Comparer.Compare(els[i], els[i - 1]) == 0)
                    {
                        els.RemoveAt(i);
                        --i;
                    }
                }

                _root = ConstructRootFromSortedArray(els.ToArray(), 0, els.Count - 1, null);
                Count = els.Count;
                _version = 0;
            }
        }


#if SUPPORTS_SERIALIZATION

        private SerializationInfo _serializationInfo; // A temporary variable which we need during deserialization.

        /// <summary>
        /// Constructor used during runtime serialization.
        /// </summary>
        protected SortedSet(SerializationInfo info, StreamingContext context)
        {
            _serializationInfo = info;
        }
#endif

        #endregion

        #region Bulk Operation Helpers

        private void AddAllElements([NotNull, ItemCanBeNull] IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                if (!Contains(item))
                    Add(item);
            }
        }

        private void RemoveAllElements([NotNull, ItemCanBeNull] IEnumerable<T> collection)
        {
            T min = Min;
            T max = Max;
            foreach (T item in collection)
            {
                if (!(Comparer.Compare(item, min) < 0 || Comparer.Compare(item, max) > 0) && Contains(item))
                    Remove(item);
            }
        }

        //
        // Do a in order walk on tree and calls the delegate for each node.
        // If the action delegate returns false, stop the walk.
        // 
        // Return true if the entire tree has been walked. 
        // Otherwise returns false.
        //
        // Allows for the change in traversal direction. Reverse visits nodes in descending order 
        private bool InOrderTreeWalk([NotNull] TreeWalkPredicate<T> action, bool reverse = false)
        {
            if (_root is null)
                return true;

            // The maximum height of a red-black tree is 2*lg(n+1).
            // See page 264 of "Introduction to algorithms" by Thomas H. Cormen
            // note: this should be logbase2, but since the stack grows itself, we 
            // don't want the extra cost
            var stack = new Stack<Node>(2 * Log2(Count + 1));
            Node current = _root;
            while (current != null)
            {
                stack.Push(current);
                current = reverse ? current.Right : current.Left;
            }

            while (stack.Count != 0)
            {
                current = stack.Pop();
                if (!action(current))
                    return false;

                Node node = reverse ? current.Left : current.Right;
                while (node != null)
                {
                    stack.Push(node);
                    node = reverse ? node.Right : node.Left;
                }
            }

            return true;
        }

        //
        // Do a left to right breadth first walk on tree and 
        // calls the delegate for each node.
        // If the action delegate returns false, stop the walk.
        // 
        // Return true if the entire tree has been walked. 
        // Otherwise returns false.
        //
        private bool BreadthFirstTreeWalk([NotNull] TreeWalkPredicate<T> action)
        {
            if (_root is null)
                return true;

            var processQueue = new List<Node> { _root };
            while (processQueue.Count != 0)
            {
                Node current = processQueue[0];
                processQueue.RemoveAt(0);
                if (!action(current))
                    return false;

                if (current.Left != null)
                {
                    processQueue.Add(current.Left);
                }
                if (current.Right != null)
                {
                    processQueue.Add(current.Right);
                }
            }

            return true;
        }

        #endregion

        #region Properties

        /// <inheritdoc cref="ICollection.Count"/>
        public int Count { get; private set; }

        /// <summary>
        /// Element comparer.
        /// </summary>
        public IComparer<T> Comparer { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot is null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }
        
        #endregion

        #region Subclass helpers

        //virtual function for subclass that needs to do range checks
        internal virtual bool IsWithinRange(T item)
        {
            return true;

        }

        #endregion

        #region ICollection<T>

        /// <summary>
        /// Add the value <paramref name="item"/> to the tree, returns true if added, false if duplicate 
        /// </summary>
        /// <param name="item">Item to be added.</param> 
        public bool Add([CanBeNull] T item)
        {
            return AddIfNotPresent(item);
        }

        void ICollection<T>.Add(T item)
        {
            AddIfNotPresent(item);
        }

        /// <summary>
        /// Adds <paramref name="item"/> to the tree if not already present. Returns true if value was successfully added         
        /// or false if it is a duplicate.
        /// </summary>
        internal virtual bool AddIfNotPresent([CanBeNull] T item)
        {
            if (_root is null)
            {
                // Empty tree
                _root = new Node(item, false);
                Count = 1;
                ++_version;
                return true;
            }

            // Search for a node at bottom to insert the new node.
            // If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
            // We split 4-nodes along the search path.
            Node current = _root;
            Node parent = null;
            Node grandParent = null;
            Node greatGrandParent = null;

            // Even if we don't actually add to the set, we may be altering its structure (by doing rotations
            // and such). so update version to disable any enumerators/subsets working on it
            ++_version;

            int order = 0;
            while (current != null)
            {
                order = Comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    // We could have changed root node to red during the search process.
                    // We need to set it to black before we return.
                    _root.IsRed = false;
                    return false;
                }

                // Split a 4-node into two 2-nodes
                if (Is4Node(current))
                {
                    Split4Node(current);
                    // We could have introduced two consecutive red nodes after split. Fix that by rotation.
                    if (IsRed(parent))
                    {
                        InsertionBalance(current, ref parent, grandParent, greatGrandParent);
                    }
                }

                greatGrandParent = grandParent;
                grandParent = parent;
                parent = current;
                current = order < 0 ? current.Left : current.Right;
            }

            Debug.Assert(parent != null, "Parent node cannot be null here!");
            // Ready to insert the new node
            var node = new Node(item);
            if (order > 0)
            {
                parent.Right = node;
            }
            else
            {
                parent.Left = node;
            }

            // The new node will be red, so we will need to adjust the colors if parent node is also red
            if (parent.IsRed)
            {
                InsertionBalance(node, ref parent, grandParent, greatGrandParent);
            }

            // Root node is always black
            _root.IsRed = false;
            ++Count;
            return true;
        }

        /// <summary>
        /// Remove the <paramref name="item"/> from this <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if successfully removed, false otherwise.</returns>
        public bool Remove(T item)
        {
            return DoRemove(item); // hack so it can be made non-virtual
        }

        internal virtual bool DoRemove([CanBeNull] T item)
        {
            if (_root is null)
                return false;

            // Search for a node and then find its successor.
            // Then copy the item from the successor to the matching node and delete the successor.
            // If a node doesn't have a successor, we can replace it with its left child (if not empty.)
            // or delete the matching node.
            // 
            // In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
            // Following code will make sure the node on the path is not a 2 Node. 

            //even if we don't actually remove from the set, we may be altering its structure (by doing rotations
            //and such). so update version to disable any enumerators/subsets working on it
            ++_version;

            Node current = _root;
            Node parent = null;
            Node grandParent = null;
            Node match = null;
            Node parentOfMatch = null;
            bool foundMatch = false;
            while (current != null)
            {
                if (Is2Node(current))
                {
                    // Fix up 2-Node
                    if (parent is null)
                    {
                        // Current is root. Mark it as red
                        current.IsRed = true;
                    }
                    else
                    {
                        Node sibling = GetSibling(current, parent);
                        if (sibling.IsRed)
                        {
                            // If parent is a 3-node, flip the orientation of the red link. 
                            // We can achieve this by a single rotation        
                            // This case is converted to one of other cased below.
                            Debug.Assert(!parent.IsRed, "parent must be a black node!");
                            if (parent.Right == sibling)
                            {
                                RotateLeft(parent);
                            }
                            else
                            {
                                RotateRight(parent);
                            }

                            parent.IsRed = true;
                            sibling.IsRed = false; // Parent's color
                                                   // Sibling becomes child of grandParent or root after rotation. Update link from grandParent or root
                            ReplaceChildOfNodeOrRoot(grandParent, parent, sibling);
                            // Sibling will become grandParent of current node 
                            grandParent = sibling;
                            if (parent == match)
                            {
                                parentOfMatch = sibling;
                            }

                            // Update sibling, this is necessary for following processing
                            sibling = parent.Left == current ? parent.Right : parent.Left;
                        }
                        Debug.Assert(sibling != null && sibling.IsRed == false, "sibling must not be null and it must be black!");

                        if (Is2Node(sibling))
                        {
                            Merge2Nodes(parent, current, sibling);
                        }
                        else
                        {
                            // Current is a 2-node and sibling is either a 3-node or a 4-node.
                            // We can change the color of current to red by some rotation.
                            TreeRotation rotation = RotationNeeded(parent, current, sibling);
                            Node newGrandParent = null;
                            switch (rotation)
                            {
                                case TreeRotation.RightRotation:
                                    Debug.Assert(parent.Left == sibling, "sibling must be left child of parent!");
                                    Debug.Assert(sibling.Left.IsRed, "Left child of sibling must be red!");
                                    sibling.Left.IsRed = false;
                                    newGrandParent = RotateRight(parent);
                                    break;
                                case TreeRotation.LeftRotation:
                                    Debug.Assert(parent.Right == sibling, "sibling must be left child of parent!");
                                    Debug.Assert(sibling.Right.IsRed, "Right child of sibling must be red!");
                                    sibling.Right.IsRed = false;
                                    newGrandParent = RotateLeft(parent);
                                    break;

                                case TreeRotation.RightLeftRotation:
                                    Debug.Assert(parent.Right == sibling, "sibling must be left child of parent!");
                                    Debug.Assert(sibling.Left.IsRed, "Left child of sibling must be red!");
                                    newGrandParent = RotateRightLeft(parent);
                                    break;

                                case TreeRotation.LeftRightRotation:
                                    Debug.Assert(parent.Left == sibling, "sibling must be left child of parent!");
                                    Debug.Assert(sibling.Right.IsRed, "Right child of sibling must be red!");
                                    newGrandParent = RotateLeftRight(parent);
                                    break;
                            }

                            // ReSharper disable once PossibleNullReferenceException
                            newGrandParent.IsRed = parent.IsRed;
                            parent.IsRed = false;
                            current.IsRed = true;
                            ReplaceChildOfNodeOrRoot(grandParent, parent, newGrandParent);
                            if (parent == match)
                            {
                                parentOfMatch = newGrandParent;
                            }
                        }
                    }
                }

                // We don't need to compare any more once we found the match
                int order = foundMatch ? -1 : Comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    // Save the matching node
                    foundMatch = true;
                    match = current;
                    parentOfMatch = parent;
                }

                grandParent = parent;
                parent = current;

                if (order < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right; // Continue the search in right sub tree after we find a match
                }
            }

            // Move successor to the matching node position and replace links
            if (match != null)
            {
                ReplaceNode(match, parentOfMatch, parent, grandParent);
                --Count;
            }

            if (_root != null)
            {
                _root.IsRed = false;
            }

            return foundMatch;
        }

        /// <inheritdoc />
        public virtual void Clear()
        {
            _root = null;
            Count = 0;
            ++_version;
        }

        /// <inheritdoc />
        public virtual bool Contains(T item)
        {
            return FindNode(item) != null;
        }

        /// <summary>
        /// Copies the content of this <see cref="SortedSet{T}"/> to the given array.
        /// </summary>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0, Count);
        }

        /// <summary>
        /// Copies the content of this <see cref="SortedSet{T}"/> to the given array.
        /// Starts at given <paramref name="index"/>.
        /// </summary>
        public void CopyTo(T[] array, int index)
        {
            CopyTo(array, index, Count);
        }

        /// <summary>
        /// Copies the content of this <see cref="SortedSet{T}"/> to the given array.
        /// Starts at given <paramref name="index"/> and copies <paramref name="count"/> elements.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is too small.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        public void CopyTo([NotNull] T[] array, int index, int count)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            // Will array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (index > array.Length || count > array.Length - index)
                throw new ArgumentException("Too small array.");
            // upper bound
            count += index;

            InOrderTreeWalk(node =>
            {
                if (index >= count)
                {
                    return false;
                }

                array[index++] = node.Item;
                return true;
            });
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException("Rank higher than one not supported.", nameof(array.Rank));
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("Non 0 lower bound.");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length - index < Count)
                throw new ArgumentException("Too small array.");

            if (array is T[] castArray)
            {
                CopyTo(castArray, index);
            }
            else
            {
                object[] objects = array as object[];
                if (objects is null)
                    throw new ArgumentException("Invalid array type.");

                try
                {
                    InOrderTreeWalk(node =>
                    {
                        objects[index++] = node.Item;
                        return true;
                    });
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type.");
                }
            }
        }

        #endregion

        #region IEnumerable<T>

        private Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Tree Specific Operations

        private static Node GetSibling([CanBeNull] Node node, [NotNull] Node parent)
        {
            if (parent.Left == node)
                return parent.Right;
            return parent.Left;
        }

        // After calling InsertionBalance, we need to make sure current and parent up-to-date.
        // It doesn't matter if we keep grandParent and greatGrantParent up-to-date 
        // because we won't need to split again in the next node.
        // By the time we need to split again, everything will be correctly set.
        private void InsertionBalance(
            [CanBeNull] Node current,
            ref Node parent,
            [NotNull] Node grandParent,
            Node greatGrandParent)
        {
            Debug.Assert(grandParent != null, "Grand parent cannot be null here!");
            bool parentIsOnRight = grandParent.Right == parent;
            bool currentIsOnRight = parent.Right == current;

            Node newChildOfGreatGrandParent;
            if (parentIsOnRight == currentIsOnRight)
            {
                // Same orientation, single rotation
                newChildOfGreatGrandParent = currentIsOnRight
                    ? RotateLeft(grandParent)
                    : RotateRight(grandParent);
            }
            else
            {
                // Different orientation, double rotation
                newChildOfGreatGrandParent = currentIsOnRight
                    ? RotateLeftRight(grandParent)
                    : RotateRightLeft(grandParent);
                // Current node now becomes the child of greatgrandparent 
                parent = greatGrandParent;
            }
            // grand parent will become a child of either parent of current.
            grandParent.IsRed = true;
            newChildOfGreatGrandParent.IsRed = false;

            ReplaceChildOfNodeOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
        }

        private static bool Is2Node([NotNull] Node node)
        {
            Debug.Assert(node != null, "node cannot be null!");
            return IsBlack(node) && IsNullOrBlack(node.Left) && IsNullOrBlack(node.Right);
        }

        private static bool Is4Node([NotNull] Node node)
        {
            return IsRed(node.Left) && IsRed(node.Right);
        }

        private static bool IsBlack([CanBeNull] Node node)
        {
            return node != null && !node.IsRed;
        }

        private static bool IsNullOrBlack([CanBeNull] Node node)
        {
            return node is null || !node.IsRed;
        }

        private static bool IsRed([CanBeNull] Node node)
        {
            return node != null && node.IsRed;
        }

        private static void Merge2Nodes([NotNull] Node parent, [NotNull] Node child1, [NotNull] Node child2)
        {
            Debug.Assert(IsRed(parent), "parent must be red.");
            // Combing two 2-nodes into a 4-node
            parent.IsRed = false;
            child1.IsRed = true;
            child2.IsRed = true;
        }

        // Replace the child of a parent node. 
        // If the parent node is null, replace the root.
        private void ReplaceChildOfNodeOrRoot([CanBeNull] Node parent, Node child, Node newChild)
        {
            if (parent != null)
            {
                if (parent.Left == child)
                {
                    parent.Left = newChild;
                }
                else
                {
                    parent.Right = newChild;
                }
            }
            else
            {
                _root = newChild;
            }
        }

        // Replace the matching node with its successor.
        private void ReplaceNode(
            [NotNull] Node match,
            [CanBeNull] Node parentOfMatch,
            [NotNull] Node successor,
            [NotNull] Node parentOfSuccessor)
        {
            if (successor == match)
            {
                // This node has no successor, should only happen if right child of matching node is null.
                Debug.Assert(match.Right is null, "Right child must be null!");
                successor = match.Left;
            }
            else
            {
                Debug.Assert(parentOfSuccessor != null, "parent of successor cannot be null!");
                Debug.Assert(successor.Left is null, "Left child of successor must be null!");
                Debug.Assert(successor.Right is null && successor.IsRed || successor.Right.IsRed && !successor.IsRed, "Successor must be in valid state.");
                if (successor.Right != null)
                {
                    successor.Right.IsRed = false;
                }

                if (parentOfSuccessor != match)
                {
                    // Detach successor from its parent and set its right child
                    parentOfSuccessor.Left = successor.Right;
                    successor.Right = match.Right;
                }

                successor.Left = match.Left;
            }

            if (successor != null)
            {
                successor.IsRed = match.IsRed;
            }

            ReplaceChildOfNodeOrRoot(parentOfMatch, match, successor);

        }

        [CanBeNull]
        private Node FindNode([CanBeNull] T item)
        {
            Node current = _root;
            while (current != null)
            {
                int order = Comparer.Compare(item, current.Item);
                if (order == 0)
                    return current;
                current = order < 0 ? current.Left : current.Right;
            }

            return null;
        }

        // used for bithelpers. Note that this implementation is completely different 
        // from the Subset's. The two should not be mixed. This indexes as if the tree were an array.
        // http://en.wikipedia.org/wiki/Binary_Tree#Methods_for_storing_binary_trees
        private int InternalIndexOf([CanBeNull] T item)
        {
            Node current = _root;
            int count = 0;
            while (current != null)
            {
                int order = Comparer.Compare(item, current.Item);
                if (order == 0)
                    return count;

                current = order < 0 ? current.Left : current.Right;
                count = order < 0 ? 2 * count + 1 : 2 * count + 2;
            }

            return -1;
        }

        private static Node RotateLeft([NotNull] Node node)
        {
            Node x = node.Right;
            node.Right = x.Left;
            x.Left = node;
            return x;
        }

        private static Node RotateLeftRight([NotNull] Node node)
        {
            Node child = node.Left;
            Node grandChild = child.Right;

            node.Left = grandChild.Right;
            grandChild.Right = node;
            child.Right = grandChild.Left;
            grandChild.Left = child;
            return grandChild;
        }

        private static Node RotateRight([NotNull] Node node)
        {
            Node x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            return x;
        }

        private static Node RotateRightLeft([NotNull] Node node)
        {
            Node child = node.Right;
            Node grandChild = child.Left;

            node.Right = grandChild.Left;
            grandChild.Left = node;
            child.Left = grandChild.Right;
            grandChild.Right = child;
            return grandChild;
        }

        /// <summary>
        /// Testing counter that can track rotations.
        /// </summary>
        private static TreeRotation RotationNeeded([NotNull] Node parent, [NotNull] Node current, [NotNull] Node sibling)
        {
            Debug.Assert(IsRed(sibling.Left) || IsRed(sibling.Right), "sibling must have at least one red child");
            if (IsRed(sibling.Left))
            {
                if (parent.Left == current)
                {
                    return TreeRotation.RightLeftRotation;
                }
                return TreeRotation.RightRotation;
            }

            if (parent.Left == current)
            {
                return TreeRotation.LeftRotation;
            }
            return TreeRotation.LeftRightRotation;
        }

        // This is a little frustrating because we can't support more sorted structures
        private static bool AreComparersEqual(
            [NotNull, ItemCanBeNull] SortedSet<T> set1,
            [NotNull, ItemCanBeNull] SortedSet<T> set2)
        {
            return set1.Comparer.Equals(set2.Comparer);
        }

        private static void Split4Node([NotNull] Node node)
        {
            node.IsRed = true;
            node.Left.IsRed = false;
            node.Right.IsRed = false;
        }

        #endregion

        #region ISet

        /// <summary>
        /// Transforms this set into its union with the <see cref="IEnumerable{T}"/> <paramref name="other"/>
        /// Attempts to insert each element and rejects it if it exists.
        /// </summary>
        /// <remarks>
        /// The caller object is important as <see cref="UnionWith"/> uses the Comparator
        /// associated with THIS to check equality.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public void UnionWith([NotNull, ItemCanBeNull] IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            var s = other as SortedSet<T>;
            if (s != null && Count == 0)
            {
                var dummy = new SortedSet<T>(s, Comparer);
                _root = dummy._root;
                Count = dummy.Count;
                ++_version;
                return;
            }

            if (s != null && AreComparersEqual(this, s) && s.Count > Count / 2)
            {
                // This actually hurts if N is much greater than M the /2 is arbitrary
                // First do a merge sort to an array.
                var merged = new T[s.Count + Count];
                int c = 0;
                Enumerator mine = GetEnumerator();
                Enumerator theirs = s.GetEnumerator();
                bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
                while (!mineEnded && !theirsEnded)
                {
                    int comp = Comparer.Compare(mine.Current, theirs.Current);
                    if (comp < 0)
                    {
                        merged[c++] = mine.Current;
                        mineEnded = !mine.MoveNext();
                    }
                    else if (comp == 0)
                    {
                        merged[c++] = theirs.Current;
                        mineEnded = !mine.MoveNext();
                        theirsEnded = !theirs.MoveNext();
                    }
                    else
                    {
                        merged[c++] = theirs.Current;
                        theirsEnded = !theirs.MoveNext();
                    }
                }

                if (!mineEnded || !theirsEnded)
                {
                    Enumerator remaining = mineEnded ? theirs : mine;
                    do
                    {
                        merged[c++] = remaining.Current;
                    } while (remaining.MoveNext());
                }

                // Now merged has all c elements

                // Safe to GC the root, we have all the elements
                _root = null;

                _root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
                Count = c;
                ++_version;
            }
            else
            {
                AddAllElements(other);
            }
        }

        private static Node ConstructRootFromSortedArray(
            [NotNull, ItemCanBeNull] IList<T> arr,
            int startIndex,
            int endIndex,
            [CanBeNull] Node redNode)
        {
            // What does this do?
            // You're given a sorted array... say 1 2 3 4 5 6 
            // 2 cases:
            //    If there are odd # of elements, pick the middle element (in this case 4), and compute
            //    its left and right branches
            //    If there are even # of elements, pick the left middle element, save the right middle element
            //    and call the function on the rest
            //    1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
            //    now add 4 as a red node to the lowest element on the right branch
            //             3                       3
            //         1       5       ->     1        5
            //           2       6             2     4   6            
            //    As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
            //    Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
            //    down to the bottom


            // The iterative way to do this ends up wasting more space than it saves in stack frames (at
            // least in what i tried)
            // so we're doing this recursively
            // base cases are described below
            int size = endIndex - startIndex + 1;
            if (size == 0)
                return null;

            Node root;
            if (size == 1)
            {
                root = new Node(arr[startIndex], false);
                if (redNode != null)
                {
                    root.Left = redNode;
                }
            }
            else if (size == 2)
            {
                root = new Node(arr[startIndex], false)
                {
                    Right = new Node(arr[endIndex], false)
                    {
                        IsRed = true
                    }
                };

                if (redNode != null)
                {
                    root.Left = redNode;
                }
            }
            else if (size == 3)
            {
                root = new Node(arr[startIndex + 1], false)
                {
                    Left = new Node(arr[startIndex], false),
                    Right = new Node(arr[endIndex], false)
                };

                if (redNode != null)
                {
                    root.Left.Left = redNode;
                }
            }
            else
            {
                int midpt = (startIndex + endIndex) / 2;
                root = new Node(arr[midpt], false)
                {
                    Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode),
                    Right = size % 2 == 0
                        ? ConstructRootFromSortedArray(arr, midpt + 2, endIndex, new Node(arr[midpt + 1], true))
                        : ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null)
                };
            }

            return root;
        }

        /// <summary>
        /// Transforms this set into its intersection with the <see cref="IEnumerable{T}"/> <paramref name="other"/>.
        /// </summary>
        /// <remarks>
        /// The caller object is important as <see cref="IntersectWith"/> uses the Comparator
        /// associated with THIS to check equality.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public void IntersectWith([NotNull, ItemCanBeNull] IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (Count == 0)
                return;

            // HashSet<T> optimizations can't be done until equality comparers and comparers are related
            // Technically, this would work as well with an ISorted<T>

            // Only let this happen if i am also a SortedSet, not a SubSet
            if (other is SortedSet<T> s && AreComparersEqual(this, s))
            {
                // First do a merge sort to an array.
                var merged = new T[Count];
                int c = 0;
                using (Enumerator mine = GetEnumerator())
                using (Enumerator theirs = s.GetEnumerator())
                {
                    bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
                    T max = Max;

                    while (!mineEnded && !theirsEnded && Comparer.Compare(theirs.Current, max) <= 0)
                    {
                        int comp = Comparer.Compare(mine.Current, theirs.Current);
                        if (comp < 0)
                        {
                            mineEnded = !mine.MoveNext();
                        }
                        else if (comp == 0)
                        {
                            merged[c++] = theirs.Current;
                            mineEnded = !mine.MoveNext();
                            theirsEnded = !theirs.MoveNext();
                        }
                        else
                        {
                            theirsEnded = !theirs.MoveNext();
                        }
                    }
                }

                // Now merged has all c elements

                // Safe to GC the root, we  have all the elements
                _root = null;

                _root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
                Count = c;
                ++_version;
            }
            else
            {
                IntersectWithEnumerable(other);
            }
        }

        private void IntersectWithEnumerable([NotNull, ItemCanBeNull] IEnumerable<T> other)
        {
            var toSave = new List<T>(Count);
            foreach (T item in other)
            {
                if (Contains(item))
                {
                    toSave.Add(item);
                    Remove(item);
                }
            }

            Clear();
            AddAllElements(toSave);
        }

        /// <summary>
        /// Transforms this set into its complement with the <see cref="IEnumerable{T}"/> <paramref name="other"/>.
        /// </summary>
        /// <remarks>
        /// The caller object is important as <see cref="ExceptWith"/> uses the Comparator
        /// associated with THIS to check equality.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public void ExceptWith([NotNull, ItemCanBeNull] IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (Count == 0)
                return;

            if (other == this)
            {
                Clear();
                return;
            }

            if (other is SortedSet<T> asSorted && AreComparersEqual(this, asSorted))
            {
                // Outside range, no point doing anything               
                if (!(Comparer.Compare(asSorted.Max, Min) < 0 || Comparer.Compare(asSorted.Min, Max) > 0))
                {
                    T min = Min;
                    T max = Max;
                    foreach (T item in other)
                    {
                        if (Comparer.Compare(item, min) < 0)
                            continue;
                        if (Comparer.Compare(item, max) > 0)
                            break;
                        Remove(item);
                    }
                }
            }
            else
            {
                RemoveAllElements(other);
            }
        }

        /// <summary>
        /// Checks whether this Tree has all elements in common with <see cref="IEnumerable{T}"/> <paramref name="other"/>.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public bool SetEquals([NotNull, ItemCanBeNull] IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (other is SortedSet<T> asSorted && AreComparersEqual(this, asSorted))
            {
                using (IEnumerator<T> mine = GetEnumerator())
                using (IEnumerator<T> theirs = asSorted.GetEnumerator())
                {
                    bool mineEnded = !mine.MoveNext();
                    bool theirsEnded = !theirs.MoveNext();
                    while (!mineEnded && !theirsEnded)
                    {
                        if (Comparer.Compare(mine.Current, theirs.Current) != 0)
                            return false;

                        mineEnded = !mine.MoveNext();
                        theirsEnded = !theirs.MoveNext();
                    }

                    return mineEnded && theirsEnded;
                }
            }

            // Worst case: mark every element in my set and see if i've counted all
            // O(N) by size of other
            ElementCount result = CheckUniqueAndUnfoundElements(other, true);
            return result.UniqueCount == Count && result.UnfoundCount == 0;
        }

        /// <summary>
        /// This works similar to HashSet's CheckUniqueAndUnfound (description below), except that the bit
        /// array maps differently than in the HashSet. We can only use this for the bulk boolean checks.
        /// 
        /// Determines counts that can be used to determine equality, subset, and superset. This
        /// is only used when other is an IEnumerable and not a HashSet. If other is a HashSet
        /// these properties can be checked faster without use of marking because we can assume 
        /// other has no duplicates.
        /// 
        /// The following count checks are performed by callers:
        /// 1. Equals: checks if unfoundCount = 0 and uniqueFoundCount = Count; i.e. everything 
        /// in other is in this and everything in this is in other
        /// 2. Subset: checks if unfoundCount >= 0 and uniqueFoundCount = Count; i.e. other may
        /// have elements not in this and everything in this is in other
        /// 3. Proper subset: checks if unfoundCount > 0 and uniqueFoundCount = Count; i.e
        /// other must have at least one element not in this and everything in this is in other
        /// 4. Proper superset: checks if unfound count = 0 and uniqueFoundCount strictly less
        /// than Count; i.e. everything in other was in this and this had at least one element
        /// not contained in other.
        /// 
        /// An earlier implementation used delegates to perform these checks rather than returning
        /// an ElementCount struct; however this was changed due to the perf overhead of delegates.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="returnIfUnfound">Allows us to finish faster for equals and proper superset
        /// because unfoundCount must be 0.</param>
        [SecurityCritical]
        private unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount result;

            // Need special case in case this has no elements. 
            if (Count == 0)
            {
                int numElementsInOther = 0;
                foreach (T _ in other)
                {
                    ++numElementsInOther;
                    // Break right away, all we want to know is whether other has 0 or 1 elements
                    break;
                }
                result.UniqueCount = 0;
                result.UnfoundCount = numElementsInOther;
                return result;
            }

            int originalLastIndex = Count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            BitHelper bitHelper;
            if (intArrayLength <= StackAllocThreshold)
            {
                int* bitArrayPtr = stackalloc int[intArrayLength];
                bitHelper = new BitHelper(bitArrayPtr, intArrayLength);
            }
            else
            {
                int[] bitArray = new int[intArrayLength];
                bitHelper = new BitHelper(bitArray, intArrayLength);
            }

            // Count of items in other not found in this
            int unfoundCount = 0;
            // Count of unique items in other found in this
            int uniqueFoundCount = 0;

            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    if (!bitHelper.IsMarked(index))
                    {
                        // Item hasn't been seen yet
                        bitHelper.MarkBit(index);
                        ++uniqueFoundCount;
                    }
                }
                else
                {
                    ++unfoundCount;
                    if (returnIfUnfound)
                        break;
                }
            }

            result.UniqueCount = uniqueFoundCount;
            result.UnfoundCount = unfoundCount;
            return result;
        }

        /// <summary>
        /// Removes elements matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Predicate to match to removed elements.</param>
        /// <returns>Number of removed elements.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveWhere([NotNull] Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var matches = new List<T>(Count);
            BreadthFirstTreeWalk(n =>
            {
                if (predicate(n.Item))
                {
                    matches.Add(n.Item);
                }

                return true;
            });

            // Reverse breadth first to (try to) incur low cost
            int actuallyRemoved = 0;
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                if (Remove(matches[i]))
                {
                    ++actuallyRemoved;
                }
            }

            return actuallyRemoved;
        }

        #endregion

        #region ISorted

        /// <summary>
        /// Minimal value.
        /// </summary>
        public T Min
        {
            get
            {
                var ret = default(T);
                InOrderTreeWalk(n =>
                {
                    ret = n.Item;
                    return false;
                });
                return ret;
            }
        }

        /// <summary>
        /// Maximal value.
        /// </summary>
        public T Max
        {
            get
            {
                var ret = default(T);
                InOrderTreeWalk(n =>
                {
                    ret = n.Item;
                    return false;
                }, true);
                return ret;
            }
        }

        /// <summary>
        /// Reverses the order of this set.
        /// </summary>
        [NotNull, ItemCanBeNull]
        public IEnumerable<T> Reverse()
        {
            var e = new Enumerator(this, true);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        #endregion

        #region Serialization methods

#if SUPPORTS_SERIALIZATION

        // LinkDemand here is unnecessary as this is a methodimpl and linkdemand from the interface should suffice
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info);
        }

        private void GetObjectData(SerializationInfo info)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(CountName, Count); //This is the length of the bucket array.
            info.AddValue(ComparerName, Comparer, typeof(IComparer<T>));
            info.AddValue(VersionName, _version);

            if (_root != null)
            {
                var items = new T[Count];
                CopyTo(items, 0);
                info.AddValue(ItemsName, items, typeof(T[]));
            }
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            OnDeserialization();
        }

        private void OnDeserialization()
        {
            if (Comparer != null)
            {
                return; // Somebody had a dependency on this class and fixed us up before the ObjectManager got to it.
            }

            if (_serializationInfo is null)
                throw new SerializationException("No information to deserialize SortedSet.");

            Comparer = (IComparer<T>)_serializationInfo.GetValue(ComparerName, typeof(IComparer<T>));
            int savedCount = _serializationInfo.GetInt32(CountName);

            if (savedCount != 0)
            {
                var items = (T[])_serializationInfo.GetValue(ItemsName, typeof(T[]));
                if (items is null)
                    throw new SerializationException("Missing serialized values.");

                foreach (T item in items)
                {
                    Add(item);
                }
            }

            _version = _serializationInfo.GetInt32(VersionName);
            if (Count != savedCount)
                throw new SerializationException("Serialized count mismatch acutal count.");

            _serializationInfo = null;
        }
#endif

        #endregion

        #region Helper Classes

        internal sealed class Node
        {
            public bool IsRed;
            public T Item;
            public Node Left;
            public Node Right;

            public Node(T item)
            {
                // The default color will be red, we never need to create a black node directly.                
                Item = item;
                IsRed = true;
            }

            public Node(T item, bool isRed)
            {
                // The default color will be red, we never need to create a black node directly.                
                Item = item;
                IsRed = isRed;
            }
        }

        /// <inheritdoc cref="IEnumerable{T}" />
        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
#if SUPPORTS_SERIALIZATION
        [Serializable]
        public struct Enumerator : IEnumerator<T>, ISerializable, IDeserializationCallback
        {
#else
        public struct Enumerator : IEnumerator<T>
        {
#endif
            private SortedSet<T> _tree;
            private int _version;

            private Stack<Node> _stack;
            private Node _current;

            [NotNull]
            private static readonly Node DummyNode = new Node(default(T));

            private bool _reverse;

#if SUPPORTS_SERIALIZATION
            private readonly SerializationInfo _sInfo;
#endif
            internal Enumerator([NotNull] SortedSet<T> set)
            {
                _tree = set;
                _version = _tree._version;

                // 2lg(n + 1) is the maximum height
                _stack = new Stack<Node>(2 * Log2(set.Count + 1));
                _current = null;
                _reverse = false;
#if SUPPORTS_SERIALIZATION
                _sInfo = null;
#endif
                Initialize();
            }

            internal Enumerator([NotNull] SortedSet<T> set, bool reverse)
            {
                _tree = set;
                _version = _tree._version;

                // 2lg(n + 1) is the maximum height
                _stack = new Stack<Node>(2 * Log2(set.Count + 1));
                _current = null;
                _reverse = reverse;
#if SUPPORTS_SERIALIZATION
                _sInfo = null;
#endif
                Initialize();
            }

#if SUPPORTS_SERIALIZATION
            private Enumerator(SerializationInfo info, StreamingContext context)
            {
                _tree = null;
                _version = -1;
                _current = null;
                _reverse = false;
                _stack = null;
                _sInfo = info;
            }

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                GetObjectData(info);
            }

            private void GetObjectData(SerializationInfo info)
            {
                if (info is null)
                    throw new ArgumentNullException(nameof(info));

                info.AddValue(TreeName, _tree, typeof(SortedSet<T>));
                info.AddValue(EnumVersionName, _version);
                info.AddValue(ReverseName, _reverse);
                info.AddValue(EnumStartName, !NotStartedOrEnded);
                info.AddValue(NodeValueName, _current is null ? DummyNode.Item : _current.Item, typeof(T));
            }

            void IDeserializationCallback.OnDeserialization(object sender)
            {
                OnDeserialization();
            }

            private void OnDeserialization()
            {
                if (_sInfo is null)
                    throw new SerializationException("No information to deserialize SortedSet Enumerator.");

                _tree = (SortedSet<T>)_sInfo.GetValue(TreeName, typeof(SortedSet<T>));
                _version = _sInfo.GetInt32(EnumVersionName);
                _reverse = _sInfo.GetBoolean(ReverseName);
                bool enumStarted = _sInfo.GetBoolean(EnumStartName);
                _stack = new Stack<Node>(2 * Log2(_tree.Count + 1));
                _current = null;
                if (enumStarted)
                {
                    var item = (T)_sInfo.GetValue(NodeValueName, typeof(T));
                    Initialize();
                    // Go until it reaches the value we want
                    while (MoveNext())
                    {
                        if (_tree.Comparer.Compare(Current, item) == 0)
                            break;
                    }
                }
            }
#endif

            private void Initialize()
            {
                _current = null;
                Node node = _tree._root;
                while (node != null)
                {
                    Node next = _reverse ? node.Right : node.Left;
                    Node other = _reverse ? node.Left : node.Right;
                    if (_tree.IsWithinRange(node.Item))
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (next is null || !_tree.IsWithinRange(next.Item))
                    {
                        node = other;
                    }
                    else
                    {
                        node = next;
                    }
                }
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_version != _tree._version)
                    throw new InvalidOperationException("Collection was modified while enumerating it.");

                if (_stack.Count == 0)
                {
                    _current = null;
                    return false;
                }

                _current = _stack.Pop();
                Node node = _reverse ? _current.Left : _current.Right;
                while (node != null)
                {
                    Node next = _reverse ? node.Right : node.Left;
                    Node other = _reverse ? node.Left : node.Right;
                    if (_tree.IsWithinRange(node.Item))
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (other is null || !_tree.IsWithinRange(other.Item))
                    {
                        node = next;
                    }
                    else
                    {
                        node = other;
                    }
                }
                return true;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }

            /// <inheritdoc />
            public T Current => _current != null ? _current.Item : default(T);

            object IEnumerator.Current
            {
                get
                {
                    if (_current is null)
                        throw new InvalidOperationException("No current item.");
                    return _current.Item;
                }
            }

            internal bool NotStartedOrEnded => _current is null;

            internal void Reset()
            {
                if (_version != _tree._version)
                    throw new InvalidOperationException("Collection was modified while enumerating it.");

                _stack.Clear();
                Initialize();
            }

            void IEnumerator.Reset()
            {
                Reset();
            }
        }

        private struct ElementCount
        {
            internal int UniqueCount;
            internal int UnfoundCount;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Searches the set for a given value and returns the equal value it finds, if any.
        /// </summary>
        /// <param name="equalValue">The value to search for.</param>
        /// <param name="actualValue">The value from the set that the search found, or the default value of <typeparamref name="T"/> when the search yielded no match.</param>
        /// <returns>A value indicating whether the search was successful.</returns>
        /// <remarks>
        /// This can be useful when you want to reuse a previously stored reference instead of 
        /// a newly constructed one (so that more sharing of references can occur) or to look up
        /// a value that has more complete data than the value you currently have, although their
        /// comparer functions indicate they are equal.
        /// </remarks>
        public bool TryGetValue([CanBeNull] T equalValue, out T actualValue)
        {
            Node node = FindNode(equalValue);
            if (node != null)
            {
                actualValue = node.Item;
                return true;
            }
            actualValue = default(T);
            return false;
        }

        // Used for set checking operations (using enumerables) that rely on counting
        private static int Log2(int value)
        {
            int c = 0;
            while (value > 0)
            {
                ++c;
                value >>= 1;
            }
            return c;
        }

        #endregion
    }
}
#endif