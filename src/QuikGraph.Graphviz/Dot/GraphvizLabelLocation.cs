#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of label locations.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizLabelLocation
    {
        /// <summary>
        /// Top.
        /// </summary>
        T,

        /// <summary>
        /// Bottom.
        /// </summary>
        B
    }
}