using System;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Represents a color.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public partial struct GraphvizColor : IEquatable<GraphvizColor>
#if SUPPORTS_SERIALIZATION
        , ISerializable
#endif
    {
        /// <summary>
        /// Initializes a new <see cref="GraphvizColor"/> struct.
        /// </summary>
        /// <param name="a">Alpha channel value.</param>
        /// <param name="r">Red channel value.</param>
        /// <param name="g">Green channel value.</param>
        /// <param name="b">Blue channel value.</param>
        public GraphvizColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Alpha channel value.
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Red channel value.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// Green channel value.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// Blue channel value.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// Indicates whether both <see cref="GraphvizColor"/> are equal.
        /// </summary>
        /// <param name="color1">First <see cref="GraphvizColor"/> to compare.</param>
        /// <param name="color2">Second <see cref="GraphvizColor"/> to compare.</param>
        /// <returns>True if both <see cref="GraphvizColor"/> are equal, otherwise false.</returns>
        public static bool operator ==(GraphvizColor color1, GraphvizColor color2)
        {
            return color1.Equals(color2);
        }

        /// <summary>
        /// Indicates whether both <see cref="GraphvizColor"/> are not equal.
        /// </summary>
        /// <param name="color1">First <see cref="GraphvizColor"/> to compare.</param>
        /// <param name="color2">Second <see cref="GraphvizColor"/> to compare.</param>
        /// <returns>True if both <see cref="GraphvizColor"/> are equal, otherwise false.</returns>
        public static bool operator !=(GraphvizColor color1, GraphvizColor color2)
        {
            return !(color1 == color2);
        }

        /// <inheritdoc />
        public bool Equals(GraphvizColor other)
        {
            return A == other.A && R == other.R && G == other.G && B == other.B;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is GraphvizColor color)
                return Equals(color);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return A << 24 | R << 16 | G << 8 | B;
        }

#if SUPPORTS_SERIALIZATION
        #region ISerializable

        private GraphvizColor(SerializationInfo info, StreamingContext context)
            : this(
                (byte)info.GetValue("a", typeof(byte)),
                (byte)info.GetValue("r", typeof(byte)),
                (byte)info.GetValue("g", typeof(byte)),
                (byte)info.GetValue("b", typeof(byte)))
        {
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("a", A);
            info.AddValue("r", R);
            info.AddValue("g", G);
            info.AddValue("b", B);
        }

        #endregion
#endif
    }
}