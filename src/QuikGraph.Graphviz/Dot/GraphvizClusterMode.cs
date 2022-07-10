#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible cluster modes.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizClusterMode
    {
        /// <summary>
        /// Local.
        /// </summary>
        Local,

        /// <summary>
        /// Global.
        /// </summary>
        Global,

        /// <summary>
        /// None.
        /// </summary>
        None
    }
}