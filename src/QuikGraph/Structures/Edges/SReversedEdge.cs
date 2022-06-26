using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// The default struct based reversed <see cref="IEdge{TVertex}"/> implementation.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Original edge type.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{" + nameof(Source) + "}<-{" + nameof(Target) + "}")]
    public struct SReversedEdge<TVertex, TEdge> : IEdge<TVertex>, IEquatable<SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SReversedEdge{TVertex, TEdge}"/> struct.
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

        /// <summary>
        /// Indicates whether both <see cref="SReversedEdge{TVertex,TEdge}"/> are equal.
        /// </summary>
        /// <param name="edge1">First <see cref="SReversedEdge{TVertex,TEdge}"/> to compare.</param>
        /// <param name="edge2">Second <see cref="SReversedEdge{TVertex,TEdge}"/> to compare.</param>
        /// <returns>True if both <see cref="SReversedEdge{TVertex,TEdge}"/> are equal, otherwise false.</returns>
        public static bool operator ==(SReversedEdge<TVertex, TEdge> edge1, SReversedEdge<TVertex, TEdge> edge2)
        {
            return edge1.Equals(edge2);
        }

        /// <summary>
        /// Indicates whether both <see cref="SReversedEdge{TVertex,TEdge}"/> are not equal.
        /// </summary>
        /// <param name="edge1">First <see cref="SReversedEdge{TVertex,TEdge}"/> to compare.</param>
        /// <param name="edge2">Second <see cref="SReversedEdge{TVertex,TEdge}"/> to compare.</param>
        /// <returns>True if both <see cref="SReversedEdge{TVertex,TEdge}"/> are not equal, otherwise false.</returns>
        public static bool operator !=(SReversedEdge<TVertex, TEdge> edge1, SReversedEdge<TVertex, TEdge> edge2)
        {
            return !(edge1 == edge2);
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