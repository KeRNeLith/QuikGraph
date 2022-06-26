using System.Text;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz arrow.
    /// <see href="https://www.graphviz.org/doc/info/arrows.html">See more</see>
    /// </summary>
    public class GraphvizArrow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizArrow"/> class.
        /// </summary>
        /// <param name="shape">Arrow shape.</param>
        public GraphvizArrow(GraphvizArrowShape shape)
        {
            Shape = shape;
            Clipping = GraphvizArrowClipping.None;
            Filling = GraphvizArrowFilling.Close;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizArrow"/> class.
        /// </summary>
        /// <param name="shape">Arrow shape.</param>
        /// <param name="clipping">Arrow clipping.</param>
        /// <param name="filling">Arrow filling.</param>
        public GraphvizArrow(GraphvizArrowShape shape, GraphvizArrowClipping clipping, GraphvizArrowFilling filling)
        {
            Shape = shape;
            Clipping = clipping;
            Filling = filling;
        }

        /// <summary>
        /// Arrow shape.
        /// </summary>
        public GraphvizArrowShape Shape { get; set; }

        /// <summary>
        /// Arrow clipping.
        /// </summary>
        public GraphvizArrowClipping Clipping { get; set; }

        /// <summary>
        /// Arrow filling.
        /// </summary>
        public GraphvizArrowFilling Filling { get; set; }

        /// <summary>
        /// Converts this arrow to DOT.
        /// </summary>
        /// <returns>Arrow as DOT.</returns>
        [Pure]
        [NotNull]
        public string ToDot()
        {
            var builder = new StringBuilder();

            if (Filling == GraphvizArrowFilling.Open
                && SupportOpen())
            {
                builder.Append('o');
            }

            switch (Clipping)
            {
                case GraphvizArrowClipping.Left when SupportClipping():
                    builder.Append('l');
                    break;

                case GraphvizArrowClipping.Right when SupportClipping():
                    builder.Append('r');
                    break;
            }

            builder.Append(Shape.ToString().ToLower());

            return builder.ToString();

            #region Local functions

            bool SupportOpen()
            {
                return Shape == GraphvizArrowShape.Box
                       || Shape == GraphvizArrowShape.Diamond
                       || Shape == GraphvizArrowShape.Dot
                       || Shape == GraphvizArrowShape.Inv
                       || Shape == GraphvizArrowShape.Normal;
            }

            bool SupportClipping()
            {
                return Shape == GraphvizArrowShape.Box
                       || Shape == GraphvizArrowShape.Crow
                       || Shape == GraphvizArrowShape.Diamond
                       || Shape == GraphvizArrowShape.Inv
                       || Shape == GraphvizArrowShape.Normal
                       || Shape == GraphvizArrowShape.Tee
                       || Shape == GraphvizArrowShape.Vee
                       || Shape == GraphvizArrowShape.Curve
                       || Shape == GraphvizArrowShape.ICurve;
            }

            #endregion
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToDot();
        }
    }
}