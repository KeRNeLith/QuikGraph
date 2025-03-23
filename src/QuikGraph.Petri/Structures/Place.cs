using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
#if REQUIRE_STRING_COMPATIBILITY
using static QuikGraph.Utils.StringUtils;
#endif

namespace QuikGraph.Petri
{
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    internal sealed class Place<TToken> : IPlace<TToken>
    {
        /// <summary>
        /// Initializes a new <see cref="Place{TToken}"/> class.
        /// </summary>
        /// <param name="name">Place name.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public Place([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IList<TToken> Marking { get; } = new List<TToken>();

        /// <inheritdoc />
        public string ToStringWithMarking()
        {
            var builder = new StringBuilder();
            builder.Append(ToString());

            if (Marking.Count > 0)
            {
                builder.AppendLine();
                builder.Append("\t");
            }

            builder.Append(
#if REQUIRE_STRING_COMPATIBILITY
                Join(
#else
                string.Join(
#endif
                    Environment.NewLine + "\t", Marking.Select(token => token.GetType().Name)));

            return builder.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"P({Name}|{Marking.Count})";
        }
    }
}