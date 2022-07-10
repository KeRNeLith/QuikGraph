#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz point.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class GraphvizPoint
    {
        /// <summary>
        /// X.
        /// </summary>
        public int X { get; }
        
        /// <summary>
        /// Y.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizPoint"/> class.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        public GraphvizPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}