#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible spline types.
    /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:splines">See details</see>
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizSplineType
    {
        /// <summary>
        /// Edges drawn as splines routed by around nodes.
        /// </summary>
        Spline,

        /// <summary>
        /// No edge drawn.
        /// </summary>
        None,

        /// <summary>
        /// Edges drawn with line segments.
        /// </summary>
        Line,

        /// <summary>
        /// Edges drawn as polylines.
        /// </summary>
        Polyline,

        /// <summary>
        /// Edges drawn as curved arcs.
        /// </summary>
        Curved,

        /// <summary>
        /// Edges drawn with orthogonal lines.
        /// </summary>
        Ortho
    }
}