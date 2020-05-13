using System;
using System.Collections;
using System.Drawing;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// GraphViz edge label.
    /// </summary>
    public class GraphvizEdgeLabel
    {
        /// <summary>
        /// Label angle.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelangle">See more</see>
        /// </summary>
        public double Angle { get; set; } = -25;

        /// <summary>
        /// Scaling factor from node.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labeldistance">See more</see>
        /// </summary>
        public double Distance { get; set; } = 1;

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
        public Font Font { get; set; }

        /// <summary>
        /// Font color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelfontcolor">See more</see>
        /// </summary>
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Label text.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:label">See more</see>
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Adds this edge label parameters to the given <paramref name="parameters"/> map.
        /// </summary>
        /// <param name="parameters">Parameter map to fill.</param>
        public void AddParameters([NotNull] IDictionary parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            if (Value != null)
            {
                parameters["label"] = Value;
                if (Angle != -25)
                {
                    parameters["labelangle"] = Angle;
                }
                if (Distance != 1)
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
                if (FontColor != Color.Black)
                {
                    parameters["fontcolor"] = FontColor;
                }
            }
        }
    }
}