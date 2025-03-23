using System;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Constants;

namespace QuikGraph
{
    /// <summary>
    /// The default <see cref="ITermEdge{TVertex}"/> implementation (directed edge).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    public class TermEdge<TVertex> : ITermEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="TermEdge{TVertex}"/> class
        /// using source/target vertices and zero terminals.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public TermEdge([NotNull] TVertex source, [NotNull] TVertex target)
            : this(source, target, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="TermEdge{TVertex}"/> class
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
        public TermEdge([NotNull] TVertex source, [NotNull] TVertex target, int sourceTerminal, int targetTerminal)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (sourceTerminal < 0)
                throw new ArgumentException("Must not be negative", nameof(sourceTerminal));
            if (targetTerminal < 0)
                throw new ArgumentException("Must not be negative.", nameof(targetTerminal));

            Source = source;
            Target = target;
            SourceTerminal = sourceTerminal;
            TargetTerminal = targetTerminal;
        }

        /// <inheritdoc />
        public TVertex Source { get; }

        /// <inheritdoc />
        public TVertex Target { get; }

        /// <inheritdoc />
        public int SourceTerminal { get; }

        /// <inheritdoc />
        public int TargetTerminal { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(EdgeFormats.Terminal, Source, SourceTerminal, Target, TargetTerminal);
        }
    }
}