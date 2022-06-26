using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Helpers;
using static QuikGraph.Graphviz.DotEscapers;
using static QuikGraph.Utils.MathUtils;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz edge.
    /// </summary>
    public class GraphvizEdge
    {
        /// <summary>
        /// Comment.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:comment">See more</see>
        /// </summary>
        public string Comment { get; set; }

        [NotNull]
        private GraphvizEdgeLabel _label = new GraphvizEdgeLabel();

        /// <summary>
        /// Label.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public GraphvizEdgeLabel Label
        {
            get => _label;
            set => _label = value ?? throw new ArgumentNullException(nameof(value));
        }

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
        /// Direction.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:dir">See more</see>
        /// </summary>
        public GraphvizEdgeDirection Direction { get; set; } = GraphvizEdgeDirection.Forward;

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

        [NotNull]
        private GraphvizEdgeExtremity _head = new GraphvizEdgeExtremity(true);

        /// <summary>
        /// Edge head.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">Set extremity is not corresponding to a head one.</exception>
        [NotNull]
        public GraphvizEdgeExtremity Head
        {
            get => _head;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (!value.IsHead)
                    throw new ArgumentException("Edge extremity must be a head extremity.");
                _head = value;
            }
        }

        /// <summary>
        /// Edge arrow.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:arrowhead">See more</see>
        /// </summary>
        public GraphvizArrow HeadArrow { get; set; }

        /// <summary>
        /// Head port.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:headport">See more</see>
        /// </summary>
        public string HeadPort { get; set; }

        [NotNull]
        private GraphvizEdgeExtremity _tail = new GraphvizEdgeExtremity(false);

        /// <summary>
        /// Tail.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">Set extremity is not corresponding to a tail one.</exception>
        [NotNull]
        public GraphvizEdgeExtremity Tail
        {
            get => _tail;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (value.IsHead)
                    throw new ArgumentException("Edge extremity must be a tail extremity.");
                _tail = value;
            }
        }

        /// <summary>
        /// Tail arrow.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:arrowtail">See more</see>
        /// </summary>
        public GraphvizArrow TailArrow { get; set; }

        /// <summary>
        /// Tail port.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:tailport">See more</see>
        /// </summary>
        public string TailPort { get; set; }

        /// <summary>
        /// Indicates if edge is constrained.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:constraint">See more</see>
        /// </summary>
        public bool IsConstrained { get; set; } = true;

        /// <summary>
        /// Indicates if edge is decorated.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:decorate">See more</see>
        /// </summary>
        public bool IsDecorated { get; set; }

        /// <summary>
        /// Layer.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:layer">See more</see>
        /// </summary>
        public GraphvizLayer Layer { get; set; }

        /// <summary>
        /// Stroke color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:color">See more</see>
        /// </summary>
        public GraphvizColor StrokeColor { get; set; } = GraphvizColor.Black;

        /// <summary>
        /// Edge style.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:style">See more</see>
        /// </summary>
        public GraphvizEdgeStyle Style { get; set; } = GraphvizEdgeStyle.Unspecified;

        /// <summary>
        /// Weight.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:weight">See more</see>
        /// </summary>
        public double Weight { get; set; } = 1;

        /// <summary>
        /// Length.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:len">See more</see>
        /// </summary>
        public int Length { get; set; } = 1;

        /// <summary>
        /// Minimal length.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:minlen">See more</see>
        /// </summary>
        public int MinLength { get; set; } = 1;

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

                    case GraphvizEdgeDirection direction:
                        builder.Append($"{pair.Key}={direction.ToString().ToLower()}");
                        continue;

                    case GraphvizEdgeStyle edgeStyle:
                        builder.Append($"{pair.Key}={edgeStyle.ToString().ToLower()}");
                        continue;

                    case GraphvizColor color:
                        builder.AppendFormat(
                            "{0}=\"#{1}{2}{3}{4}\"",
                            pair.Key,
                            color.R.ToString("x2").ToUpper(),
                            color.G.ToString("x2").ToUpper(),
                            color.B.ToString("x2").ToUpper(),
                            color.A.ToString("x2").ToUpper());
                        continue;

                    case HtmlString html:
                        builder.Append($"{pair.Key}=<{html.String}>");
                        continue;

                    default:
                        builder.Append($"{pair.Key}={pair.Value.ToString().ToLower()}");
                        break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts this edge to DOT.
        /// </summary>
        /// <returns>Edge as DOT.</returns>
        [Pure]
        [NotNull]
        public string ToDot()
        {
            var properties = new Dictionary<string, object>();
            if (Direction != GraphvizEdgeDirection.Forward)
            {
                properties["dir"] = Direction;
            }
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
            Head.AddParameters(properties);
            if (HeadArrow != null)
            {
                properties["arrowhead"] = HeadArrow.ToDot();
            }
            if (HeadPort != null)
            {
                properties["headport"] = EscapePort(HeadPort);
            }
            if (!IsConstrained)
            {
                properties["constraint"] = IsConstrained;
            }
            if (IsDecorated)
            {
                properties["decorate"] = IsDecorated;
            }
            Label.AddParameters(properties);
            if (Layer != null)
            {
                properties["layer"] = Layer.Name;
            }
            if (MinLength != 1)
            {
                properties["minlen"] = MinLength;
            }
            if (Length != 1)
            {
                properties["len"] = Length;
            }
            if (StrokeColor != GraphvizColor.Black)
            {
                properties["color"] = StrokeColor;
            }
            if (Style != GraphvizEdgeStyle.Unspecified)
            {
                properties["style"] = Style;
            }
            Tail.AddParameters(properties);
            if (TailArrow != null)
            {
                properties["arrowtail"] = TailArrow.ToDot();
            }
            if (TailPort != null)
            {
                properties["tailport"] = EscapePort(TailPort);
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
            if (!NearEqual(Weight, 1.0))
            {
                properties["weight"] = Weight;
            }
            return GenerateDot(properties);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToDot();
        }
    }
}