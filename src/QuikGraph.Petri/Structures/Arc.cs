using System;
using JetBrains.Annotations;

namespace QuikGraph.Petri
{
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    internal sealed class Arc<TToken> : Edge<IPetriVertex>, IArc<TToken>
    {
        /// <summary>
        /// Initializes a new <see cref="Arc{TToken}"/> class.
        /// </summary>
        /// <param name="place">Place (Source).</param>
        /// <param name="transition">Transition (Target).</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="place"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transition"/> is <see langword="null"/>.</exception>
        public Arc([NotNull] IPlace<TToken> place, [NotNull] ITransition<TToken> transition)
            : base(place, transition)
        {
            Place = place;
            Transition = transition;
            IsInputArc = true;
        }

        /// <summary>
        /// Initializes a new <see cref="Arc{TToken}"/> class.
        /// </summary>
        /// <param name="transition">Transition (Source).</param>
        /// <param name="place">Place (Target).</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transition"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="place"/> is <see langword="null"/>.</exception>
        public Arc([NotNull] ITransition<TToken> transition, [NotNull] IPlace<TToken> place)
            : base(place, transition)
        {
            Place = place;
            Transition = transition;
            IsInputArc = false;
        }

        /// <inheritdoc />
        public bool IsInputArc { get; }

        /// <inheritdoc />
        public IPlace<TToken> Place { get; }

        /// <inheritdoc />
        public ITransition<TToken> Transition { get; }

        [NotNull]
        private IExpression<TToken> _annotation = new IdentityExpression<TToken>();

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        public IExpression<TToken> Annotation
        {
            get => _annotation;
            set => _annotation = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsInputArc
                ? $"{Place} -> {Transition}"
                : $"{Transition} -> {Place}";
        }
    }
}