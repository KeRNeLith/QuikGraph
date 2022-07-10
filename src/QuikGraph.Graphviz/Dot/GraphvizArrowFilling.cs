#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible arrow fillings.
    /// <see href="https://www.graphviz.org/doc/info/arrows.html">See more</see>
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizArrowFilling
    {
        /// <summary>
        /// Use a closed (filled) version of the shape.
        /// </summary>
        Close,

        /// <summary>
        /// Use an open (non-filled) version of the shape.
        /// </summary>
        Open
    }
}