#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible edge styles.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizEdgeStyle
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Invisible.
        /// </summary>
        Invis,

        /// <summary>
        /// Dashed.
        /// </summary>
        Dashed,

        /// <summary>
        /// Dotted.
        /// </summary>
        Dotted,

        /// <summary>
        /// Bold.
        /// </summary>
        Bold,

        /// <summary>
        /// Solid.
        /// </summary>
        Solid
    }
}