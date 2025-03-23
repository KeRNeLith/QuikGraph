using JetBrains.Annotations;

namespace QuikGraph.Constants
{
    /// <summary> Format Strings for edges. </summary>
    internal static class EdgeFormats
    {
        /// <summary>Edge string formatting.</summary>
        [NotNull]
        public const string String = "{0} -> {1}";

        /// <summary>Edge terminals string formatting. </summary>
        [NotNull]
        public const string Terminal = "{0} ({1}) -> {2} ({3})";

        /// <summary>Edge string formatting (with tag). </summary>
        [NotNull]
        public const string Tagged = "{0} -> {1} ({2})";

        /// <summary>Undirected edge string formatting. </summary>
        [NotNull]
        public const string Undirected = "{0} <-> {1}";

        /// <summary>Undirected edge string formatting (with tag). </summary>
        [NotNull]
        public const string TaggedUndirected = "{0} <-> {1} ({2})";
    }
}