#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible arrow shapes.
    /// <see href="https://www.graphviz.org/doc/info/arrows.html">See more</see>
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum GraphvizArrowShape
    {
        /// <summary>
        /// Box.
        /// </summary>
        Box,

        /// <summary>
        /// Crow.
        /// </summary>
        Crow,

        /// <summary>
        /// Diamond.
        /// </summary>
        Diamond,

        /// <summary>
        /// Dot.
        /// </summary>
        Dot,

        /// <summary>
        /// Inv.
        /// </summary>
        Inv,

        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Tee.
        /// </summary>
        Tee,

        /// <summary>
        /// Vee.
        /// </summary>
        Vee,

        /// <summary>
        /// Curve.
        /// </summary>
        Curve,

        /// <summary>
        /// ICurve.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ICurve
    }
}