#if SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;

namespace System.Collections.Generic
{
#if NET45
#elif NET40_OR_GREATER
    /// <summary> Represents a strongly-typed, read-only collection of elements. </summary>
    /// <remarks>Re-declaration of the corresponding .net Interface</remarks>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
    }

    /// <summary> Represents a strongly-typed, read-only List of elements. </summary>
    /// <remarks>Re-declaration of the corresponding .net Interface</remarks>
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        /// <summary> Gets the element at Index <paramref name="i"/>. </summary>
        T this[int i] { get; }
    }
#elif NET35_OR_GREATER
    /// <summary> Represents a strongly-typed, read-only collection of elements. </summary>
    /// <remarks>Re-declaration of the corresponding .net Interface</remarks>
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
    }

    /// <summary> Represents a strongly-typed, read-only List of elements. </summary>
    /// <remarks>Re-declaration of the corresponding .net Interface</remarks>
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        /// <summary> Gets the element at Index <paramref name="i"/>. </summary>
        T this[int i] { get; }
    }
#else //NET35_OR_GREATER
#endif //NET35_OR_GREATER
}

namespace QuikGraph.Collections
{
    /// <summary> A mutable, cloneable list of edges. </summary>
    public interface IEdgeList<TEdge> : IList<TEdge>, IReadOnlyList<TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
        /// <summary> Trims excess allocated space. </summary>
        void TrimExcess();

        /// <summary> Returns the number of items in this list </summary>
        /// <remarks>Re-declaration to resolve the Ambiguity</remarks>
        new int Count { get; }

        /// <summary> Returns the number of items in this list </summary>
        /// <remarks>Re-declaration to resolve the Ambiguity</remarks>
        new TEdge this[int i] { get; }

        /// <summary> Gets a clone of this list. </summary>
        /// <returns>Cloned list.</returns>
        [Pure]
        [NotNull]
#if SUPPORTS_CLONEABLE
        new
#endif
        IEdgeList<TEdge> Clone();
    }
}