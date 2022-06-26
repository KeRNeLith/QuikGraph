namespace QuikGraph
{
    /// <summary>
    /// Colors used in vertex coloring algorithms.
    /// </summary>
    public enum GraphColor : byte
    {
        /// <summary>
        /// Usually initial color.
        /// </summary>
        White = 0,

        /// <summary>
        /// Usually intermediate color.
        /// </summary>
        Gray,

        /// <summary>
        /// Usually finished color.
        /// </summary>
        Black
    }
}