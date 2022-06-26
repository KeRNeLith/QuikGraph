using System;
using System.Collections;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Helpers;
using static QuikGraph.Graphviz.DotEscapers;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz edge extremity.
    /// </summary>
    public class GraphvizEdgeExtremity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizEdgeExtremity"/> class.
        /// </summary>
        /// <param name="isHead">Indicates if this edge extremity is the head.</param>
        public GraphvizEdgeExtremity(bool isHead)
        {
            IsHead = isHead;
            IsClipped = true;
        }

        /// <summary>
        /// Indicates if this extremity is edge head or tail.
        /// </summary>
        public bool IsHead { get; }

        /// <summary>
        /// Is edge extremity clipped to the boundaries of the target node?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:headclip">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:tailclip">See more</see>
        /// </summary>
        public bool IsClipped { get; set; }

        /// <summary>
        /// Indicates if label should be read as HTML or normal text. By default it is normal text.
        /// <see href="https://www.graphviz.org/doc/info/shapes.html#html">See more</see>
        /// </summary>
        public bool IsHtmlLabel { get; set; }

        /// <summary>
        /// Label.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:headlabel">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:taillabel">See more</see>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Tooltip.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:headtooltip">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:tailtooltip">See more</see>
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// URL.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:headURL">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:tailURL">See more</see>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Logical extremity of an edge.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:lhead">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:ltail">See more</see>
        /// </summary>
        public string Logical { get; set; }

        /// <summary>
        /// Identifier to group edge extremities with same identifier.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:samehead">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:sametail">See more</see>
        /// </summary>
        public string Same { get; set; }

        /// <summary>
        /// Adds this edge extremity parameters to the given <paramref name="parameters"/> map.
        /// </summary>
        /// <param name="parameters">Parameter map to fill.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public void AddParameters([NotNull] IDictionary parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            string extremity = IsHead ? "head" : "tail";
            if (Url != null)
            {
                parameters.Add(extremity + "URL", Url);
            }
            if (!IsClipped)
            {
                parameters.Add(extremity + "clip", IsClipped);
            }
            if (Label != null)
            {
                string labelKey = extremity + "label";
                if (IsHtmlLabel)
                {
                    parameters.Add(labelKey, new HtmlString(Label));
                }
                else
                {
                    parameters.Add(labelKey, Escape(Label));
                }
            }
            if (ToolTip != null)
            {
                parameters.Add(extremity + "tooltip", Escape(ToolTip));
            }
            if (Logical != null)
            {
                parameters.Add("l" + extremity, Logical);
            }
            if (Same != null)
            {
                parameters.Add("same" + extremity, Same);
            }
        }
    }
}