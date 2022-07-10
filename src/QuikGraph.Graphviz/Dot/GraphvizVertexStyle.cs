#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible vertex styles.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizVertexStyle
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Filled.
        /// </summary>
        Filled,

        /// <summary>
        /// Diagonals.
        /// </summary>
        Diagonals,

        /// <summary>
        /// Rounded.
        /// </summary>
        Rounded,

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