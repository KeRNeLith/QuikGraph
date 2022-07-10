#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible output modes.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizOutputMode
    {
        /// <summary>
        /// Breadth first.
        /// </summary>
        BreadthFirst,

        /// <summary>
        /// Nodes first.
        /// </summary>
        NodesFirst,

        /// <summary>
        /// Edges first.
        /// </summary>
        EdgesFirst
    }
}