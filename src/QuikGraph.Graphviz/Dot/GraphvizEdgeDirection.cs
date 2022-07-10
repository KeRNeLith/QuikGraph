#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible edge directions.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizEdgeDirection
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Forward.
        /// </summary>
        Forward,

        /// <summary>
        /// Backward.
        /// </summary>
        Back,

        /// <summary>
        /// Both directions.
        /// </summary>
        Both
    }
}