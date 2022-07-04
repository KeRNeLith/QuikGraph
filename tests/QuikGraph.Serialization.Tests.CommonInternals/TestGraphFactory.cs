//#define FULL_SLOW_TESTS_RUN

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Serialization;

namespace QuikGraph.Tests
{
    /// <summary>
    /// Factory to create graphs.
    /// </summary>
    internal static class TestGraphFactory
    {
        private const int SlowTestRate = 5;

        private static int GetSlowTestRate(int rate = -1)
        {
            if (rate > 0)
                return rate;
            return SlowTestRate;
        }

        /// <summary>
        /// Gets graph ML file paths.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<string> GetGraphMLFilePaths(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            string testPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "GraphML");
            if (Directory.Exists(testPath))
            {
                string[] filePaths = Directory.GetFiles(testPath, "g.*.graphml");
                if (filter is null)
                    return filePaths;
                return filePaths.Where(filter);
            }
            throw new AssertionException("GraphML folder must exist.");
        }

        /// <summary>
        /// Creates an adjacency graph from the given file.
        /// </summary>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<string, Edge<string>> LoadGraph([NotNull] string graphMLFilePath)
        {
            var graph = new AdjacencyGraph<string, Edge<string>>();
            using (var reader = new StreamReader(graphMLFilePath))
            {
                graph.DeserializeFromGraphML(
                    reader,
                    id => id,
                    (source, target, _) => new Edge<string>(source, target));
            }

            return graph;
        }

        /// <summary>
        /// Creates a bidirectional graph from the given file.
        /// </summary>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<string, Edge<string>> LoadBidirectionalGraph([NotNull] string graphMLFilePath)
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();
            using (var reader = new StreamReader(graphMLFilePath))
            {
                graph.DeserializeFromGraphML(
                    reader,
                    id => id,
                    (source, target, _) => new Edge<string>(source, target));
            }

            return graph;
        }

        /// <summary>
        /// Creates an undirected graph from the given file.
        /// </summary>
        [Pure]
        [NotNull]
        public static UndirectedGraph<string, Edge<string>> LoadUndirectedGraph([NotNull] string graphMLFilePath)
        {
            AdjacencyGraph<string, Edge<string>> graph = LoadGraph(graphMLFilePath);
            var undirectedGraph = new UndirectedGraph<string, Edge<string>>();
            undirectedGraph.AddVerticesAndEdgeRange(graph.Edges);
            return undirectedGraph;
        }

        /// <summary>
        /// Creates adjacency graphs (filterable).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<AdjacencyGraph<string, Edge<string>>> GetAdjacencyGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new AdjacencyGraph<string, Edge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadGraph(graphMLFilePath);
            }
        }

        /// <summary>
        /// Creates adjacency graphs.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<AdjacencyGraph<string, Edge<string>>> GetAdjacencyGraphs_All()
        {
            return GetAdjacencyGraphsInternal();
        }

        /// <summary>
        /// Creates adjacency graphs (version manageable with define for slow tests).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<AdjacencyGraph<string, Edge<string>>> GetAdjacencyGraphs_SlowTests(int rate = -1)
        {
#if !FULL_SLOW_TESTS_RUN
            int r = GetSlowTestRate(rate);
#endif
            return GetAdjacencyGraphsInternal(
#if !FULL_SLOW_TESTS_RUN
                // 1 over SlowTestRate
                (_, i) => i % r == 0
#endif
            );
        }

        /// <summary>
        /// Creates bidirectional graphs (filterable).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<BidirectionalGraph<string, Edge<string>>> GetBidirectionalGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new BidirectionalGraph<string, Edge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadBidirectionalGraph(graphMLFilePath);
            }
        }

        /// <summary>
        /// Creates bidirectional graphs.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<BidirectionalGraph<string, Edge<string>>> GetBidirectionalGraphs_All()
        {
            return GetBidirectionalGraphsInternal();
        }

        /// <summary>
        /// Creates bidirectional graphs (version manageable with define for slow tests).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<BidirectionalGraph<string, Edge<string>>> GetBidirectionalGraphs_SlowTests(int rate = -1)
        {
#if !FULL_SLOW_TESTS_RUN
            int r = GetSlowTestRate(rate);
#endif
            return GetBidirectionalGraphsInternal(
#if !FULL_SLOW_TESTS_RUN
                // 1 over SlowTestRate
                (_, i) => i % r == 0
#endif
            );
        }

        /// <summary>
        /// Creates undirected graphs (filterable).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<UndirectedGraph<string, Edge<string>>> GetUndirectedGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new UndirectedGraph<string, Edge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadUndirectedGraph(graphMLFilePath);
            }
        }

        /// <summary>
        /// Creates undirected graphs.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<UndirectedGraph<string, Edge<string>>> GetUndirectedGraphs_All()
        {
            return GetUndirectedGraphsInternal();
        }

        /// <summary>
        /// Creates undirected graphs (version manageable with define for slow tests).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<UndirectedGraph<string, Edge<string>>> GetUndirectedGraphs_SlowTests(int rate = -1)
        {
#if !FULL_SLOW_TESTS_RUN
            int r = GetSlowTestRate(rate);
#endif
            return GetUndirectedGraphsInternal(
#if !FULL_SLOW_TESTS_RUN
                // 1 over SlowTestRate
                (_, i) => i % r == 0
#endif
            );
        }
    }
}