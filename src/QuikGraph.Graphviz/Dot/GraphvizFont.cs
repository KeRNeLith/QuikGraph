using System;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz font.
    /// </summary>
    public sealed class GraphvizFont
    {
        /// <summary>
        /// Font name.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontname">See more</see>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Font size (in points).
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontsize">See more</see>
        /// </summary>
        public float SizeInPoints { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizFont"/> class.
        /// </summary>
        /// <param name="name">Font name.</param>
        /// <param name="sizeInPoints">Font size.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="sizeInPoints"/> is negative or equal to 0.</exception>
        public GraphvizFont([NotNull] string name, float sizeInPoints)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Font name cannot be null or empty.", nameof(name));
            if (sizeInPoints <= 0)
                throw new ArgumentOutOfRangeException(nameof(sizeInPoints), "Size must be positive.");

            Name = name;
            SizeInPoints = sizeInPoints;
        }
    }
}