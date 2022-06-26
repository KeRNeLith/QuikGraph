namespace QuikGraph.Graphviz.Dot
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Enumeration of possible page directions.
    /// </summary>
    public enum GraphvizPageDirection
    {
        /// <summary>
        /// Major order: Bottom to Top, Minor order: Left to Right.
        /// </summary>
        BL,

        /// <summary>
        /// Major order: Bottom to Top, Minor order: Right to Left.
        /// </summary>
        BR,

        /// <summary>
        /// Major order: Top to Bottom, Minor order: Left to Right.
        /// </summary>
        TL,

        /// <summary>
        /// Major order: Top to Bottom, Minor order: Right to Left.
        /// </summary>
        TR,

        /// <summary>
        /// Major order: Right to Left, Minor order: Bottom to Top.
        /// </summary>
        RB,

        /// <summary>
        /// Major order: Right to Left, Minor order: Top to Bottom.
        /// </summary>
        RT,

        /// <summary>
        /// Major order: Left to Right, Minor order: Bottom to Top.
        /// </summary>
        LB,

        /// <summary>
        /// Major order: Left to Right, Minor order: Top to Bottom.
        /// </summary>
        LT
    }
}