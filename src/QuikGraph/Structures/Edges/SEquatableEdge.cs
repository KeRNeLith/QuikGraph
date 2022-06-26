using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using QuikGraph.Constants;

namespace QuikGraph
{
    /// <summary>
    /// A struct based <see cref="IEdge{TVertex}"/> implementation that supports equality (directed edge).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    [StructLayout(LayoutKind.Auto)]
    public struct SEquatableEdge<TVertex> : IEdge<TVertex>, IEquatable<SEquatableEdge<TVertex>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SEquatableEdge{TVertex}"/> struct.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public SEquatableEdge([NotNull] TVertex source, [NotNull] TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
        }

        /// <inheritdoc />
        public TVertex Source { get; }

        /// <inheritdoc />
        public TVertex Target { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SEquatableEdge<TVertex> edge
                   && Equals(edge);
        }

        /// <inheritdoc />
        public bool Equals(SEquatableEdge<TVertex> other)
        {
            return EqualityComparer<TVertex>.Default.Equals(Source, other.Source)
                   && EqualityComparer<TVertex>.Default.Equals(Target, other.Target);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable ConstantConditionalAccessQualifier
            // ReSharper disable ConstantNullCoalescingCondition
            // Justification: Because of struct default constructor
            return HashCodeHelpers.Combine(
                Source?.GetHashCode() ?? 0,
                Target?.GetHashCode() ?? 0);
            // ReSharper restore ConstantNullCoalescingCondition
            // ReSharper restore ConstantConditionalAccessQualifier
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(EdgeConstants.EdgeFormatString, Source, Target);
        }
    }
}