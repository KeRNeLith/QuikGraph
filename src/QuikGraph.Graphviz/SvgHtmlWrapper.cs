using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Graphviz
{
    /// <summary>
    /// Helpers to related to SVG and HTML.
    /// </summary>
    public static class SvgHtmlWrapper
    {
        /// <summary>
        /// Creates an HTML file containing the given <paramref name="svgFilePath"/>.
        /// </summary>
        /// <param name="size">Image size.</param>
        /// <param name="svgFilePath">SVG file path.</param>
        /// <returns>Dumped HTML file path.</returns>
        [Pure]
        [NotNull]
        public static string DumpHtml(GraphvizSize size, [NotNull] string svgFilePath)
        {
            if (svgFilePath is null)
                throw new ArgumentNullException(nameof(svgFilePath));

            string outputFile = $"{svgFilePath}.html";
#if SUPPORTS_STREAM_FULL_FEATURES
            using (var html = new StreamWriter(outputFile))
#else
            var fileWriter = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            using (var html = new StreamWriter(fileWriter))
#endif
            {
                html.WriteLine("<html>");
                html.WriteLine("<body>");
                html.WriteLine($"<object data=\"{svgFilePath}\" type=\"image/svg+xml\" width=\"{size.Width}\" height=\"{size.Height}\">");
                html.WriteLine($"  <embed src=\"{svgFilePath}\" type=\"image/svg+xml\" width=\"{size.Width}\" height=\"{size.Height}\" />");
                html.WriteLine("If you see this, you need to install a SVG viewer");
                html.WriteLine("</object>");
                html.WriteLine("</body>");
                html.WriteLine("</html>");
            }

            return outputFile;
        }

        /// <summary>
        /// Creates an HTML file that wraps the given <paramref name="svgFilePath"/>.
        /// </summary>
        /// <param name="svgFilePath">SVG file path.</param>
        /// <returns>HTML file path.</returns>
        [Pure]
        [NotNull]
        public static string WrapSvg([NotNull] string svgFilePath)
        {
#if SUPPORTS_STREAM_FULL_FEATURES
            using (var reader = new StreamReader(svgFilePath))
            {
                GraphvizSize size = ParseSize(reader.ReadToEnd());
                reader.Close();
                return DumpHtml(size, svgFilePath);
            }
#else
            GraphvizSize size;
            var fileReader = new FileStream(svgFilePath, FileMode.Open, FileAccess.Read);
            using (var reader = new StreamReader(fileReader))
            {
                size = ParseSize(reader.ReadToEnd());
            }

            return DumpHtml(size, svgFilePath);
#endif
        }

        [NotNull]
        private const string WidthGroupName = "Width";

        [NotNull]
        private const string HeightGroupName = "Height";

        [NotNull]
        private static readonly Regex SizeRegex = new Regex(
            $@"<\s*svg.*width\s*=\s*""\s*(?<{WidthGroupName}>\d+)\s*(px|)\s*"".*height\s*=\s*""\s*(?<{HeightGroupName}>\d+)\s*(px|)\s*""",
            RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.Compiled
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            , TimeSpan.FromSeconds(10));
#else
            );
#endif

        /// <summary>
        /// Parses the size of an SVG.
        /// </summary>
        /// <param name="svg">SVG content.</param>
        /// <returns>SVG size.</returns>
        [Pure]
        public static GraphvizSize ParseSize([NotNull] string svg)
        {
            Match match = SizeRegex.Match(svg);
            if (!match.Success)
                return new GraphvizSize(400, 400);
            
            int size = int.Parse(match.Groups[WidthGroupName].Value);
            int height = int.Parse(match.Groups[HeightGroupName].Value);
            return new GraphvizSize(size, height);
        }
    }
}