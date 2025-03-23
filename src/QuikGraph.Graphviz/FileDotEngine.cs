using System;
using System.IO;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Graphviz
{
    /// <summary>
    /// Default dot engine implementation, writes 'dot'-formatted code to disk.
    /// </summary>
    public sealed class FileDotEngine : IDotEngine
    {
        /// <inheritdoc />
        public string Run(GraphvizImageType imageType, string dot, string outputFilePath)
        {
            if (string.IsNullOrEmpty(dot))
                throw new ArgumentException("Dot content must not be null or empty.", nameof(dot));
            if (string.IsNullOrEmpty(outputFilePath))
                throw new ArgumentException("Output file path content must not be null or empty.", nameof(outputFilePath));

            string output = outputFilePath;
            if (!output.EndsWith(".dot", StringComparison.OrdinalIgnoreCase))
            {
                output += ".dot";
            }

            File.WriteAllText(output, dot);
            return output;
        }
    }
}