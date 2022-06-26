using System;
using System.Diagnostics;
using System.Globalization;
using static QuikGraph.Utils.MathUtils;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz size (float).
    /// </summary>
    [DebuggerDisplay("{" + nameof(Width) + "}x{" + nameof(Height) + "}")]
    public struct GraphvizSizeF
    {
        /// <summary>
        /// Width.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:width">See more</see>
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// Height.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:height">See more</see>
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// Indicates if this size is empty.
        /// </summary>
        public bool IsEmpty =>  IsZero(Width) || IsZero(Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizSizeF"/> struct.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="width"/> and/or <paramref name="height"/> is negative.</exception>
        public GraphvizSizeF(float width, float height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException("Width and height must be positive or 0.");

            Width = width;
            Height = height;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}x{1}", Width, Height);
        }
    }

    /// <summary>
    /// Graphviz size.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Width) + "}x{" + nameof(Height) + "}")]
    public struct GraphvizSize
    {
        /// <summary>
        /// Width.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:width">See more</see>
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:height">See more</see>
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Indicates if this size is empty.
        /// </summary>
        public bool IsEmpty => Width == 0 || Height == 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizSize"/> struct.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="width"/> and/or <paramref name="height"/> is negative.</exception>
        public GraphvizSize(int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("Width and height must be positive or 0.");

            Width = width;
            Height = height;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}x{1}", Width, Height);
        }
    }
}