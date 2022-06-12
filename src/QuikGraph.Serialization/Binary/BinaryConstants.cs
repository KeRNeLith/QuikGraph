#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Resolvers;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// Constants related to binary serialization.
    /// </summary>
    [CLSCompliant(false)]
    public static class BinaryConstants
    {
        static BinaryConstants()
        {
            DefaultResolver = CompositeResolver.Create(
                QuikGraphFormatterResolver.Instance,
                // Finally use standard resolver
                StandardResolver.Instance);
            DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(DefaultResolver);
        }

        /// <summary>
        /// Default <see cref="IFormatterResolver"/> used for serialization with QuikGraph structures.
        /// </summary>
        [NotNull]
        public static IFormatterResolver DefaultResolver { get; }

        /// <summary>
        /// Default options used for serialization with QuikGraph structures.
        /// </summary>
        [NotNull]
        public static MessagePackSerializerOptions DefaultOptions { get; }
    }
}
#endif