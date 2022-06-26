#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// Formatter resolver for serialization using MessagePack with QuikGraph structures.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class QuikGraphFormatterResolver : IFormatterResolver
    {
        #region Singleton management

        private QuikGraphFormatterResolver()
        {
        }

        /// <summary>
        /// Gets the resolver instance.
        /// </summary>
        /// <remarks>Should be a singleton.</remarks>
        public static QuikGraphFormatterResolver Instance => InstanceHandler.InternalInstance;

        private static class InstanceHandler
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static InstanceHandler()
            {
            }

            internal static readonly QuikGraphFormatterResolver InternalInstance = new QuikGraphFormatterResolver();
        }

        #endregion

        /// <inheritdoc />
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        // GetFormatter<T>'s get cost should be minimized so use type cache.
        private static class FormatterCache<T>
        {
            [CanBeNull]
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                // Generic's static constructor should be minimized for reduce type generation size!
                // use outer helper method
                Formatter = (IMessagePackFormatter<T>)FormatterResolverCache.GetFormatter(typeof(T));
            }
        }
    }
}
#endif