#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal static class GraphMLResourceResolver
    {
        /// <summary>
        /// Gets the GraphML resource with given <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Resource stream.</returns>
        [Pure]
        [NotNull]
        public static Stream GetResource([NotNull] string resourceName)
        {
            Stream resourceStream = typeof(GraphMLResourceResolver).Assembly
                .GetManifestResourceStream(
                    typeof(GraphMLResourceResolver),
                    $@"GraphML.{resourceName}");

            Debug.Assert(resourceStream != null);
            return resourceStream;
        }
    }
}
#endif