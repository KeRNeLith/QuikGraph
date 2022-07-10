using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// An <see cref="ITermEdge{TVertex}"/> implementation that supports equality (directed edge).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    public class EquatableTermEdge<TVertex> : TermEdge<TVertex>, IEquatable<EquatableTermEdge<TVertex>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTermEdge{TVertex}"/> class
        /// using source/target vertices and zero terminals.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public EquatableTermEdge([NotNull] TVertex source, [NotNull] TVertex target)
            : base(source, target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTermEdge{TVertex}"/> class
        /// using source/target vertices and source/target terminals.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="sourceTerminal">The source terminal.</param>
        /// <param name="targetTerminal">The target terminal.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="sourceTerminal"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="targetTerminal"/> is negative.</exception>
        public EquatableTermEdge([NotNull] TVertex source, [NotNull] TVertex target, int sourceTerminal, int targetTerminal)
            : base(source, target, sourceTerminal, targetTerminal)
        {
        }

        /// <inheritdoc />
        public virtual bool Equals(EquatableTermEdge<TVertex> other)
        {
            if (other is null)
                return false;
            return EqualityComparer<TVertex>.Default.Equals(Source, other.Source)
                   && EqualityComparer<TVertex>.Default.Equals(Target, other.Target)
                   && SourceTerminal.Equals(other.SourceTerminal)
                   && TargetTerminal.Equals(other.TargetTerminal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableTermEdge<TVertex>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeHelpers.Combine(
                Source.GetHashCode(),
                Target.GetHashCode(),
                SourceTerminal.GetHashCode(),
                TargetTerminal.GetHashCode());
        }
    }
}