using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// GraphViz graph.
    /// </summary>
    public class GraphvizGraph
    {
        /// <summary>
        /// Graph name.
        /// </summary>
        public string Name { get; set; } = "G";

        /// <summary>
        /// Comment.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:comment">See more</see>
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// URL.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:URL">See more</see>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Background color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:bgcolor">See more</see>
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// Cluster rank mode.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:clusterrank">See more</see>
        /// </summary>
        public GraphvizClusterMode ClusterRank { get; set; } = GraphvizClusterMode.Local;

        /// <summary>
        /// Font.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontname">See more</see> or
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontsize">See more</see>
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Font color.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontcolor">See more</see>
        /// </summary>
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Graph should be centered?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:center">See more</see>
        /// </summary>
        public bool IsCentered { get; set; }

        /// <summary>
        /// Graph is compound?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:compound">See more</see>
        /// </summary>
        public bool IsCompounded { get; set; }

        /// <summary>
        /// Graph is concentrated?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:concentrate">See more</see>
        /// </summary>
        public bool IsConcentrated { get; set; }

        /// <summary>
        /// Graph is landscape?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#dd:orientation">See more</see>
        /// </summary>
        public bool IsLandscape { get; set; }

        /// <summary>
        /// Graph should be normalized?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:normalize">See more</see>
        /// </summary>
        public bool IsNormalized { get; set; }

        /// <summary>
        /// Should run crossing minimization?
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:remincross">See more</see>
        /// </summary>
        public bool IsReMinCross { get; set; }

        /// <summary>
        /// Label.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:label">See more</see>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label justification.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labeljust">See more</see>
        /// </summary>
        public GraphvizLabelJustification LabelJustification { get; set; } = GraphvizLabelJustification.C;

        /// <summary>
        /// Label location.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:labelloc">See more</see>
        /// </summary>
        public GraphvizLabelLocation LabelLocation { get; set; } = GraphvizLabelLocation.B;

        /// <summary>
        /// Layers.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:layers">See more</see>
        /// </summary>
        [NotNull, ItemNotNull]
        public GraphvizLayerCollection Layers { get; } = new GraphvizLayerCollection();

        /// <summary>
        /// Crossing minimization improvement tries limit.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:mclimit">See more</see>
        /// </summary>
        public double McLimit { get; set; } = 1;

        /// <summary>
        /// Node separation.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:nodesep">See more</see>
        /// </summary>
        public double NodeSeparation { get; set; } = 0.25;

        /// <summary>
        /// Iterations limit for simplex applications.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:nslimit">See more</see>
        /// </summary>
        public int NsLimit { get; set; } = -1;

        /// <summary>
        /// Iterations limit for simplex applications.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:nslimit1">See more</see>
        /// </summary>
        public int NsLimit1 { get; set; } = -1;

        /// <summary>
        /// Output order.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:outputorder">See more</see>
        /// </summary>
        public GraphvizOutputMode OutputOrder { get; set; } = GraphvizOutputMode.BreadthFirst;

        /// <summary>
        /// Page direction.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:pagedir">See more</see>
        /// </summary>
        public GraphvizPageDirection PageDirection { get; set; } = GraphvizPageDirection.BL;

        /// <summary>
        /// Page size.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:page">See more</see>
        /// </summary>
        public SizeF PageSize { get; set; } = new SizeF(0, 0);

        /// <summary>
        /// Quantum.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:quantum">See more</see>
        /// </summary>
        public double Quantum { get; set; }

        /// <summary>
        /// Rank direction.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:rankdir">See more</see>
        /// </summary>
        public GraphvizRankDirection RankDirection { get; set; } = GraphvizRankDirection.TB;

        /// <summary>
        /// Rank separation.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:ranksep">See more</see>
        /// </summary>
        public double RankSeparation { get; set; } = 0.5;

        /// <summary>
        /// Aspect ratio.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:ratio">See more</see>
        /// </summary>
        public GraphvizRatioMode Ratio { get; set; } = GraphvizRatioMode.Auto;

        /// <summary>
        /// Resolution.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:resolution">See more</see>
        /// </summary>
        public double Resolution { get; set; } = 0.96;

        /// <summary>
        /// Graph rotation.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:rotate">See more</see>
        /// </summary>
        public int Rotate { get; set; }

        /// <summary>
        /// Sample points.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:samplepoints">See more</see>
        /// </summary>
        public int SamplePoints { get; set; } = 8;

        /// <summary>
        /// Search size.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:searchsize">See more</see>
        /// </summary>
        public int SearchSize { get; set; } = 30;

        /// <summary>
        /// Size.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:size">See more</see>
        /// </summary>
        public SizeF Size { get; set; } = new SizeF(0, 0);

        /// <summary>
        /// Stylesheet.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:stylesheet">See more</see>
        /// </summary>
        public string StyleSheet { get; set; }

        [Pure]
        [NotNull]
        internal string GenerateDot([NotNull] Dictionary<string, object> properties)
        {
            var entries = new List<string>(properties.Count);
            foreach (KeyValuePair<string, object> pair in properties)
            {
                switch (pair.Value)
                {
                    case string strValue:
                        entries.Add($"{pair.Key}=\"{strValue}\"");
                        continue;
                    
                    case Color color:
                        entries.Add(
                            $"{pair.Key}=\"#{color.R.ToString("x2").ToUpper()}{color.G.ToString("x2").ToUpper()}{color.B.ToString("x2").ToUpper()}{color.A.ToString("x2").ToUpper()}\"");
                        continue;
                    
                    case GraphvizRankDirection _:
                    case GraphvizPageDirection _:
                        entries.Add($"{pair.Key}={pair.Value}");
                        continue;
                    default:
                        entries.Add($" {pair.Key}={pair.Value.ToString().ToLower()}");
                        break;
                }
            }

            string result = string.Join(";", entries);
            result = entries.Count > 1 ? result + ";" : result;

            return result;
        }

        /// <summary>
        /// Converts this graph to DOT.
        /// </summary>
        /// <returns>Graph as DOT.</returns>
        [Pure]
        [NotNull]
        public string ToDot()
        {
            var properties = new Dictionary<string, object>();
            if (Url != null)
            {
                properties["URL"] = Url;
            }
            if (BackgroundColor != Color.White)
            {
                properties["bgcolor"] = BackgroundColor;
            }
            if (IsCentered)
            {
                properties["center"] = true;
            }
            if (ClusterRank != GraphvizClusterMode.Local)
            {
                properties["clusterrank"] = ClusterRank.ToString().ToLower();
            }
            if (Comment != null)
            {
                properties["comment"] = Comment;
            }
            if (IsCompounded)
            {
                properties["compound"] = IsCompounded;
            }
            if (IsConcentrated)
            {
                properties["concentrate"] = IsConcentrated;
            }
            if (Font != null)
            {
                properties["fontname"] = Font.Name;
                properties["fontsize"] = Font.SizeInPoints;
            }
            if (FontColor != Color.Black)
            {
                properties["fontcolor"] = FontColor;
            }
            if (Label != null)
            {
                properties["label"] = Label;
            }
            if (LabelJustification != GraphvizLabelJustification.C)
            {
                properties["labeljust"] = LabelJustification.ToString().ToLower();
            }
            if (LabelLocation != GraphvizLabelLocation.B)
            {
                properties["labelloc"] = LabelLocation.ToString().ToLower();
            }
            if (Layers.Count != 0)
            {
                properties["layers"] = Layers.ToDot();
            }
            if (McLimit != 1)
            {
                properties["mclimit"] = McLimit;
            }
            if (NodeSeparation != 0.25)
            {
                properties["nodesep"] = NodeSeparation;
            }
            if (IsNormalized)
            {
                properties["normalize"] = IsNormalized;
            }
            if (NsLimit > 0)
            {
                properties["nslimit"] = NsLimit;
            }
            if (NsLimit1 > 0)
            {
                properties["nslimit1"] = NsLimit1;
            }
            if (OutputOrder != GraphvizOutputMode.BreadthFirst)
            {
                properties["outputorder"] = OutputOrder.ToString().ToLower();
            }
            if (!PageSize.IsEmpty)
            {
                properties["page"] = string.Format("{0},{1}", PageSize.Width, PageSize.Height);
            }
            if (PageDirection != GraphvizPageDirection.BL)
            {
                properties["pagedir"] = PageDirection.ToString().ToLower();
            }
            if (Quantum > 0)
            {
                properties["quantum"] = Quantum;
            }
            if (RankSeparation != 0.5)
            {
                properties["ranksep"] = RankSeparation;
            }
            if (Ratio != GraphvizRatioMode.Auto)
            {
                properties["ratio"] = Ratio.ToString().ToLower();
            }
            if (IsReMinCross)
            {
                properties["remincross"] = IsReMinCross;
            }
            if (Resolution != 0.96)
            {
                properties["resolution"] = Resolution;
            }
            if (Rotate != 0)
            {
                properties["rotate"] = Rotate;
            }
            else if (IsLandscape)
            {
                properties["orientation"] = "[1L]*";
            }
            if (SamplePoints != 8)
            {
                properties["samplepoints"] = SamplePoints;
            }
            if (SearchSize != 30)
            {
                properties["searchsize"] = SearchSize;
            }
            if (!Size.IsEmpty)
            {
                properties["size"] = string.Format("{0},{1}", Size.Width, Size.Height);
            }
            if (StyleSheet != null)
            {
                properties["stylesheet"] = StyleSheet;
            }
            if (RankDirection != GraphvizRankDirection.TB)
            {
                properties["rankdir"] = RankDirection;
            }

            return this.GenerateDot(properties);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToDot();
        }
    }
}