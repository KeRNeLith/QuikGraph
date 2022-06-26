using System.Collections.Generic;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using System.Text;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Helpers;
using static QuikGraph.Graphviz.DotEscapers;
using static QuikGraph.Utils.MathUtils;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz vertex.
    /// </summary>
    public class GraphvizVertex
    {
        /// <summary>
        /// Position.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:pos">See more</see>
        /// </summary>
        public GraphvizPoint Position { get; set; }

        /// <summary>
        /// Comment.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:comment">See more</see>
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Indicates if label should be read as HTML or normal text. By default it is normal text.
        /// <see href="https://www.graphviz.org/doc/info/shapes.html#html">See more</see>
        /// </summary>
        public bool IsHtmlLabel { get; set; }

        /// <summary>
        /// Label.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:label">See more</see>
        /// </summary>
        /// <remarks>
        /// If <see cref="Shape"/> is defined to <see cref="GraphvizVertexShape.Record"/>, you can
        /// use <see cref="Record"/> structure to generate a string label. Note also that if <see cref="Label"/>
        /// is set, it has the priority over <see cref="Record"/> so rather than using <see cref="Record"/>
        /// to generate the label you can define it using your own way in the <see cref="Label"/>.
        /// The only constraint will be to generate a fully valid record string.
        /// </remarks>
        public string Label { get; set; }

        /// <summary>
        /// Tooltip.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:tooltip">See more</see>
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// URL.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:URL">See more</see>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Distortion.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:distortion">See more</see>
        /// </summary>
        public double Distortion { get; set; }

        /// <summary>
        /// Filling color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fillcolor">See more</see>
        /// </summary>
        public GraphvizColor FillColor { get; set; } = GraphvizColor.White;

        /// <summary>
        /// Font.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontname">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontsize">See more</see>
        /// </summary>
        public GraphvizFont Font { get; set; }

        /// <summary>
        /// Font color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontcolor">See more</see>
        /// </summary>
        public GraphvizColor FontColor { get; set; } = GraphvizColor.Black;

        /// <summary>
        /// Pen width.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:penwidth">See more</see>
        /// </summary>
        public double PenWidth { get; set; } = 1.0;

        /// <summary>
        /// Vertex group.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:group">See more</see>
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Vertex layer.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:layer">See more</see>
        /// </summary>
        public GraphvizLayer Layer { get; set; }

        /// <summary>
        /// Orientation.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:orientation">See more</see>
        /// </summary>
        public double Orientation { get; set; }

        /// <summary>
        /// Peripheries.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:peripheries">See more</see>
        /// </summary>
        public int Peripheries { get; set; } = -1;

        /// <summary>
        /// Indicates if it is a regular vertex.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:regular">See more</see>
        /// </summary>
        public bool Regular { get; set; }

        /// <summary>
        /// Record info.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:label">See more</see>
        /// </summary>
        public GraphvizRecord Record { get; set; } = new GraphvizRecord();

        /// <summary>
        /// Vertex shape.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:shape">See more</see>
        /// </summary>
        public GraphvizVertexShape Shape { get; set; } = GraphvizVertexShape.Unspecified;

        /// <summary>
        /// Vertex sides.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:sides">See more</see>
        /// </summary>
        public int Sides { get; set; } = 4;

        /// <summary>
        /// Vertex size.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:width">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:height">See more</see>
        /// </summary>
        public GraphvizSizeF Size { get; set; } = new GraphvizSizeF(0.0f, 0.0f);

        /// <summary>
        /// Fixed size.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fixedsize">See more</see>
        /// </summary>
        public bool FixedSize { get; set; }

        /// <summary>
        /// Skew.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:skew">See more</see>
        /// </summary>
        public double Skew { get; set; }

        /// <summary>
        /// Stroke color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:color">See more</see>
        /// </summary>
        public GraphvizColor StrokeColor { get; set; } = GraphvizColor.Black;

        /// <summary>
        /// Vertex style.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:style">See more</see>
        /// </summary>
        public GraphvizVertexStyle Style { get; set; } = GraphvizVertexStyle.Unspecified;

        /// <summary>
        /// Z index.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:z">See more</see>
        /// </summary>
        public double Z { get; set; } = -1;

        [Pure]
        [NotNull]
        internal string GenerateDot([NotNull] Dictionary<string, object> properties)
        {
            var builder = new StringBuilder();

            bool flag = false;
            foreach (KeyValuePair<string, object> pair in properties)
            {
                if (flag)
                {
                    builder.Append(", ");
                }
                else
                {
                    flag = true;
                }

                switch (pair.Value)
                {
                    case string strValue:
                        builder.Append($"{pair.Key}=\"{strValue}\"");
                        continue;

                    case float floatValue:
                        builder.Append($"{pair.Key}={floatValue.ToInvariantString()}");
                        continue;

                    case double doubleValue:
                        builder.Append($"{pair.Key}={doubleValue.ToInvariantString()}");
                        continue;

                    case GraphvizVertexShape shape:
                        builder.Append($"{pair.Key}={shape.ToString().ToLower()}");
                        continue;

                    case GraphvizVertexStyle style:
                        builder.Append($"{pair.Key}={style.ToString().ToLower()}");
                        continue;

                    case GraphvizColor color:
                        {
                            builder.AppendFormat(
                                "{0}=\"#{1}{2}{3}{4}\"",
                                pair.Key,
                                color.R.ToString("x2").ToUpper(),
                                color.G.ToString("x2").ToUpper(),
                                color.B.ToString("x2").ToUpper(),
                                color.A.ToString("x2").ToUpper());
                            continue;
                        }

                    case HtmlString html:
                        builder.Append($"{pair.Key}=<{html.String}>");
                        continue;

                    case GraphvizRecord record:
                        builder.Append($"{pair.Key}=\"{record.ToDot()}\"");
                        continue;

                    default:
                        builder.Append($"{pair.Key}={pair.Value.ToString().ToLower()}");
                        break;
                }
            }

            return builder.ToString();
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void TextRelatedPropertiesToDot([NotNull] IDictionary<string, object> properties)
        {
            if (Font != null)
            {
                properties["fontname"] = Font.Name;
                properties["fontsize"] = Font.SizeInPoints;
            }

            if (FontColor != GraphvizColor.Black)
            {
                properties["fontcolor"] = FontColor;
            }

            if (!NearEqual(PenWidth, 1.0))
            {
                properties["penwidth"] = PenWidth;
            }

            if (ToolTip != null)
            {
                properties["tooltip"] = Escape(ToolTip);
            }

            if (Comment != null)
            {
                properties["comment"] = Escape(Comment);
            }

            if (Url != null)
            {
                properties["URL"] = Url;
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void LabelOrRecordToDot(
            [NotNull] IDictionary<string, object> properties,
            GraphvizVertexShape shape)
        {
            if (shape == GraphvizVertexShape.Record)
            {
                // Priority to label to allow custom record generation process
                if (!string.IsNullOrEmpty(Label))
                {
                    properties["label"] = Label;
                }
                else if (Record != null && Record.Cells.Count > 0)
                {
                    properties["label"] = Record;
                }
            }
            else if (!string.IsNullOrEmpty(Label))
            {
                properties["label"] = IsHtmlLabel
                    ? (object)new HtmlString(Label)
                    : Escape(Label);
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ShapeRelatedPropertiesToDot(
            [CanBeNull] GraphvizVertex commonFormat,
            [NotNull] IDictionary<string, object> properties)
        {
            if (Shape != GraphvizVertexShape.Unspecified)
            {
                properties["shape"] = Shape;
            }

            GraphvizVertexShape shape = Shape == GraphvizVertexShape.Unspecified && commonFormat != null
                ? commonFormat.Shape
                : Shape;

            LabelOrRecordToDot(properties, shape);

            if (shape == GraphvizVertexShape.Polygon)
            {
                if (Sides != 0)
                {
                    properties["sides"] = Sides;
                }

                if (!IsZero(Skew))
                {
                    properties["skew"] = Skew;
                }

                if (!IsZero(Distortion))
                {
                    properties["distortion"] = Distortion;
                }
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void LayoutRelatedPropertiesToDot([NotNull] IDictionary<string, object> properties)
        {
            if (FixedSize)
            {
                properties["fixedsize"] = true;
                if (Size.Height > 0f)
                {
                    properties["height"] = Size.Height;
                }

                if (Size.Width > 0f)
                {
                    properties["width"] = Size.Width;
                }
            }

            if (Style != GraphvizVertexStyle.Unspecified)
            {
                properties["style"] = Style;
            }

            if (StrokeColor != GraphvizColor.Black)
            {
                properties["color"] = StrokeColor;
            }

            if (FillColor != GraphvizColor.White)
            {
                properties["fillcolor"] = FillColor;
            }

            if (Orientation > 0)
            {
                properties["orientation"] = Orientation;
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void PositionRelatedProperties([NotNull] IDictionary<string, object> properties)
        {
            if (Z > 0)
            {
                properties["z"] = Z;
            }

            if (Position != null)
            {
                properties["pos"] = $"{Position.X},{Position.Y}!";
            }
        }

        /// <summary>
        /// Converts this vertex to DOT.
        /// </summary>
        /// <param name="commonFormat">Common vertex format to apply.</param>
        /// <returns>Vertex as DOT.</returns>
        [Pure]
        [NotNull]
        internal string InternalToDot([CanBeNull] GraphvizVertex commonFormat = null)
        {
            var properties = new Dictionary<string, object>();
            TextRelatedPropertiesToDot(properties);
            ShapeRelatedPropertiesToDot(commonFormat, properties);
            LayoutRelatedPropertiesToDot(properties);
            if (Regular)
            {
                properties["regular"] = Regular;
            }
            if (Group != null)
            {
                properties["group"] = Group;
            }
            if (Layer != null)
            {
                properties["layer"] = Layer.Name;
            }
            if (Peripheries >= 0)
            {
                properties["peripheries"] = Peripheries;
            }
            PositionRelatedProperties(properties);

            return GenerateDot(properties);
        }

        /// <summary>
        /// Converts this vertex to DOT.
        /// </summary>
        /// <returns>Vertex as DOT.</returns>
        [Pure]
        [NotNull]
        public string ToDot()
        {
            return InternalToDot();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToDot();
        }
    }
}