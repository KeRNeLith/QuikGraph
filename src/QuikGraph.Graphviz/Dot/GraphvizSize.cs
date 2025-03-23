using System;
using System.Diagnostics;
using System.Globalization;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif
using static QuikGraph.Utils.MathUtils;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz size (float).
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Width) + "}x{" + nameof(Height) + "}")]
    public struct GraphvizSizeF
#if SUPPORTS_SERIALIZATION
        : ISerializable
#endif
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
        /// Initializes a new <see cref="GraphvizSizeF"/> struct.
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

#if SUPPORTS_SERIALIZATION
        #region ISerializable

        private GraphvizSizeF(SerializationInfo info, StreamingContext context)
            : this(
                (float)info.GetValue("w", typeof(float)),
                (float)info.GetValue("h", typeof(float)))
        {
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("w", Width);
            info.AddValue("h", Height);
        }

        #endregion
#endif
    }

    /// <summary>
    /// Graphviz size.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Width) + "}x{" + nameof(Height) + "}")]
    public struct GraphvizSize
#if SUPPORTS_SERIALIZATION
        : ISerializable
#endif
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
        /// Initializes a new <see cref="GraphvizSize"/> struct.
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

#if SUPPORTS_SERIALIZATION
        #region ISerializable

        private GraphvizSize(SerializationInfo info, StreamingContext context)
            : this(
                (int)info.GetValue("w", typeof(int)),
                (int)info.GetValue("h", typeof(int)))
        {
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("w", Width);
            info.AddValue("h", Height);
        }

        #endregion
#endif
    }
}