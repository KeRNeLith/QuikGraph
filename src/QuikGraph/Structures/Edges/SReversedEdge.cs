using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Extension Methods for <see cref="SReversedEdge{TVertex,TEdge}"/> </summary>
    /// <inheritdoc cref="CreateReversedEdge{TVertex,TEdge}(TEdge)"/>
    public static class SReversedEdge
    {
        /// <summary> Creates a <see cref="SReversedEdge{TVertex,TEdge}"/> from <paramref name="edge"/> </summary>
        [CanBeNull]
        public static SReversedEdge<TVertex, TEdge>? CreateReversedEdge<TVertex, TEdge>([CanBeNull] this TEdge edge) where TEdge : IEdge<TVertex>
        {
            if (edge == null)
            {
                return null;
            }

            return new SReversedEdge<TVertex, TEdge>(edge);
        }

        /// <inheritdoc cref="CreateReversedEdge{TVertex,TEdge}(TEdge)"/>
        public static SReversedEdge<TVertex, TEdge>? CreateReversedEdge<TVertex, TEdge>(this TEdge? edge) where TEdge : struct, IEdge<TVertex>
        {
            if (edge is null)
            {
                return null;
            }

            return new SReversedEdge<TVertex, TEdge>(edge.Value);
        }
    }

    /// <summary>
    /// The default struct based reversed <see cref="IEdge{TVertex}"/> implementation.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{" + nameof(Source) + "}<-{" + nameof(Target) + "}")]
    public struct SReversedEdge<TVertex, TEdge> : IEdge<TVertex>, IEquatable<SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="SReversedEdge{TVertex, TEdge}"/> struct.
        /// </summary>
        /// <param name="originalEdge">Original edge.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="originalEdge"/> is <see langword="null"/>.</exception>
        public SReversedEdge([NotNull] TEdge originalEdge)
        {
            if (originalEdge == null)
                throw new ArgumentNullException(nameof(originalEdge));

            OriginalEdge = originalEdge;
        }

        /// <summary>
        /// Original edge.
        /// </summary>
        [NotNull]
        public TEdge OriginalEdge { get; }

        /// <inheritdoc />
        public TVertex Source => OriginalEdge.Target;

        /// <inheritdoc />
        public TVertex Target => OriginalEdge.Source;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SReversedEdge<TVertex, TEdge> reversedEdge
                   && Equals(reversedEdge);
        }

        /// <inheritdoc />
        public bool Equals(SReversedEdge<TVertex, TEdge> other)
        {
            return EqualityComparer<TEdge>.Default.Equals(OriginalEdge, other.OriginalEdge);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // Justification: Because of struct default constructor
            return (OriginalEdge != null ? OriginalEdge.GetHashCode() : 0) ^ 16777619;
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"R({OriginalEdge})";
        }
    }
}