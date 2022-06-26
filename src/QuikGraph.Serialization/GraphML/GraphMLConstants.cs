#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal static class GraphMLConstants
    {
        /// <summary>
        /// Graph ML XML namespace.
        /// </summary>
        [NotNull]
        public const string GraphMLNamespace = "http://graphml.graphdrawing.org/xmlns";
    }
}
#endif