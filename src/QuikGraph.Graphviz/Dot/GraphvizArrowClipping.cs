namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible arrow clippings.
    /// <see href="https://www.graphviz.org/doc/info/arrows.html">See more</see>
    /// </summary>
    public enum GraphvizArrowClipping
    {
        /// <summary>
        /// No clipping.
        /// </summary>
        None,

        /// <summary>
        /// Clip the shape, leaving only the part to the left of the edge.
        /// </summary>
        Left,

        /// <summary>
        /// Clip the shape, leaving only the part to the right of the edge.
        /// </summary>
        Right
    }
}