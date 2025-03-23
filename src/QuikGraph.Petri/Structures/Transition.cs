using System;
using JetBrains.Annotations;

namespace QuikGraph.Petri
{
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    internal sealed class Transition<TToken> : ITransition<TToken>
    {
        /// <summary>
        /// Initializes a new <see cref="Transition{Token}"/> class.
        /// </summary>
        /// <param name="name">Transition name.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public Transition([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <inheritdoc />
        public string Name { get; }

        [NotNull]
        private IConditionExpression<TToken> _condition = new AlwaysTrueConditionExpression<TToken>();

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        public IConditionExpression<TToken> Condition
        {
            get => _condition;
            set => _condition = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"T({Name})";
        }
    }
}