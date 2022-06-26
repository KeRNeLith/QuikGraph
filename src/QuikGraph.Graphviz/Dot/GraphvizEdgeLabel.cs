using System;
using System.Collections;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Helpers;
using static QuikGraph.Graphviz.DotEscapers;
using static QuikGraph.Utils.MathUtils;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz edge label.
    /// </summary>
    public class GraphvizEdgeLabel
    {
        /// <summary>
        /// Label angle.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelangle">See more</see>
        /// </summary>
        public double Angle { get; set; } = -25.0;

        /// <summary>
        /// Scaling factor from node.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labeldistance">See more</see>
        /// </summary>
        public double Distance { get; set; } = 1.0;

        /// <summary>
        /// Floating label.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelfloat">See more</see>
        /// </summary>
        public bool Float { get; set; } = true;

        /// <summary>
        /// Font.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontname">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontsize">See more</see>
        /// </summary>
        public GraphvizFont Font { get; set; }

        /// <summary>
        /// Font color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelfontcolor">See more</see>
        /// </summary>
        public GraphvizColor FontColor { get; set; } = GraphvizColor.Black;

        /// <summary>
        /// Indicates if label should be read as HTML or normal text. By default it is normal text.
        /// <see href="https://www.graphviz.org/doc/info/shapes.html#html">See more</see>
        /// </summary>
        public bool IsHtmlLabel { get; set; }

        /// <summary>
        /// Label text.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:label">See more</see>
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Adds this edge label parameters to the given <paramref name="parameters"/> map.
        /// </summary>
        /// <param name="parameters">Parameter map to fill.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public void AddParameters([NotNull] IDictionary parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            if (Value != null)
            {
                if (IsHtmlLabel)
                {
                    parameters["label"] = new HtmlString(Value);
                }
                else
                {
                    parameters["label"] = Escape(Value);
                }
                if (!NearEqual(Angle, -25.0))
                {
                    parameters["labelangle"] = Angle;
                }
                if (!NearEqual(Distance, 1))
                {
                    parameters["labeldistance"] = Distance;
                }
                if (!Float)
                {
                    parameters["labelfloat"] = Float;
                }
                if (Font != null)
                {
                    parameters["labelfontname"] = Font.Name;
                    parameters["labelfontsize"] = Font.SizeInPoints;
                }
                if (FontColor != GraphvizColor.Black)
                {
                    parameters["labelfontcolor"] = FontColor;
                }
            }
        }
    }
}