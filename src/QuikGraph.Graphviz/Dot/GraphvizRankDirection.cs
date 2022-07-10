#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Enumeration of possible rank directions.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizRankDirection
    {
        /// <summary>
        /// Left to right.
        /// </summary>
        LR,

        /// <summary>
        /// Top to bottom.
        /// </summary>
        TB
    }
}