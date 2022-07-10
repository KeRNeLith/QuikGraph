using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Constants;
using System.Collections.Generic;

namespace QuikGraph
{
    /// <summary>
    /// The default implementation of an <see cref="IEdge{TVertex}"/> that supports tagging (struct) (directed edge).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TTag">Tag type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}:{" + nameof(Tag) + "}")]
    public struct SEquatableTaggedEdge<TVertex, TTag> : IEdge<TVertex>, ITagged<TTag>, IEquatable<SEquatableTaggedEdge<TVertex, TTag>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SEquatableTaggedEdge{TVertex, TTag}"/> struct.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="tag">Edge tag.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public SEquatableTaggedEdge([NotNull] TVertex source, [NotNull] TVertex target, [CanBeNull] TTag tag)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
            _tag = tag;
            TagChanged = null;
        }

        /// <inheritdoc />
        public TVertex Source { get; }

        /// <inheritdoc />
        public TVertex Target { get; }

        /// <inheritdoc />
        public event EventHandler TagChanged;

        /// <summary>
        /// Event invoker for <see cref="TagChanged"/> event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        private void OnTagChanged([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            TagChanged?.Invoke(this, args);
        }

        private TTag _tag;

        /// <inheritdoc />
        public TTag Tag
        {
            get => _tag;
            set
            {
                if (EqualityComparer<TTag>.Default.Equals(_tag, value))
                    return;

                _tag = value;
                OnTagChanged(EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SEquatableTaggedEdge<TVertex, TTag> edge
                   && Equals(edge);
        }

        /// <inheritdoc />
        public bool Equals(SEquatableTaggedEdge<TVertex, TTag> other)
        {
            return EqualityComparer<TVertex>.Default.Equals(Source, other.Source)
                   && EqualityComparer<TVertex>.Default.Equals(Target, other.Target);
        }

        /// <summary>
        /// Indicates whether both <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> are equal.
        /// </summary>
        /// <param name="edge1">First <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> to compare.</param>
        /// <param name="edge2">Second <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> to compare.</param>
        /// <returns>True if both <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> are equal, otherwise false.</returns>
        public static bool operator ==(SEquatableTaggedEdge<TVertex, TTag> edge1, SEquatableTaggedEdge<TVertex, TTag> edge2)
        {
            return edge1.Equals(edge2);
        }

        /// <summary>
        /// Indicates whether both <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> are not equal.
        /// </summary>
        /// <param name="edge1">First <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> to compare.</param>
        /// <param name="edge2">Second <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> to compare.</param>
        /// <returns>True if both <see cref="SEquatableTaggedEdge{TVertex,TTag}"/> are not equal, otherwise false.</returns>
        public static bool operator !=(SEquatableTaggedEdge<TVertex, TTag> edge1, SEquatableTaggedEdge<TVertex, TTag> edge2)
        {
            return !(edge1 == edge2);
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
            return string.Format(EdgeConstants.TaggedEdgeFormatString, Source, Target, Tag?.ToString() ?? "no tag");
        }
    }
}