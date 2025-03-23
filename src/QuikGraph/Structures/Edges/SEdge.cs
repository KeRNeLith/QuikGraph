using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using QuikGraph.Constants;

namespace QuikGraph
{
    /// <summary>
    /// The default struct based <see cref="IEdge{TVertex}"/> implementation (it is by design an equatable edge) (directed edge).
    /// </summary>
    /// <inheritdoc />
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    [StructLayout(LayoutKind.Auto)]
    public class SEdge<TVertex> : IEdge<TVertex>, IEquatable<SEdge<TVertex>>
    {
        /// <summary>
        /// Initializes a new <see cref="SEdge{TVertex}"/> struct.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public SEdge([NotNull] TVertex source, [NotNull] TVertex target)
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
        public bool Equals(SEdge<TVertex> other) => other != null && Source.Equals(other.Source) && Target.Equals(other.Target);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as SEdge<TVertex>);

        /// <inheritdoc />
        public override int GetHashCode() => (Source.GetHashCode() << 1) ^ Target.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => string.Format(EdgeFormats.String, Source, Target);
    }
}