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

        /// <summary> Gets all graph ML file paths, optionally <paramref name="filter"/>ed. </summary>
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
                    return filePaths.AsEnumerable();
                return filePaths.Where(filter);
            }
            throw new AssertionException("GraphML folder must exist.");
        }

        /// <summary> Creates an adjacency graph from the <paramref name="graphMLFilePath"/>. </summary>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<string, IEdge<string>> LoadGraph([NotNull] string graphMLFilePath)
        {
            var graph = new AdjacencyGraph<string, IEdge<string>>();
            using (var reader = new StreamReader(graphMLFilePath))
            {
                graph.DeserializeFromGraphML(
                    reader,
                    id => id,
                    (source, target, _) => Edge.Create(source, target));
            }
            graph.Id = Path.GetFileNameWithoutExtension(graphMLFilePath);
            return graph;
        }

        /// <summary> Creates a bidirectional graph from the <paramref name="graphMLFilePath"/>>. </summary>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<string, IEdge<string>> LoadBidirectionalGraph([NotNull] string graphMLFilePath)
        {
            var graph = new BidirectionalGraph<string, IEdge<string>>();
            using (var reader = new StreamReader(graphMLFilePath))
            {
                graph.DeserializeFromGraphML(
                    reader,
                    id => id,
                    (source, target, _) => Edge.Create(source, target));
            }
            graph.Id = Path.GetFileNameWithoutExtension(graphMLFilePath);
            return graph;
        }

        /// <summary>
        /// Creates an undirected graph from the given file.
        /// </summary>
        [Pure]
        [NotNull]
        public static UndirectedGraph<string, IEdge<string>> LoadUndirectedGraph([NotNull] string graphMLFilePath)
        {
            AdjacencyGraph<string, IEdge<string>> graph = LoadGraph(graphMLFilePath);
            var undirectedGraph = new UndirectedGraph<string, IEdge<string>>();
            undirectedGraph.AddVerticesAndEdgeRange(graph.Edges);
            undirectedGraph.Id = Path.GetFileNameWithoutExtension(graphMLFilePath);
            return undirectedGraph;
        }

        /// <summary> Creates adjacency graphs (filterable). </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<AdjacencyGraph<string, IEdge<string>>> GetAdjacencyGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new AdjacencyGraph<string, IEdge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadGraph(graphMLFilePath);
            }
        }

        /// <summary> Creates adjacency graphs. </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<AdjacencyGraph<string, IEdge<string>>> GetAdjacencyGraphs_All()
            => GetAdjacencyGraphsInternal();

        /// <summary> Creates adjacency graphs (version manageable with define for slow tests). </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<AdjacencyGraph<string, IEdge<string>>> GetAdjacencyGraphs_SlowTests(int rate = -1)
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

        /// <summary> Creates bidirectional graphs (filterable). </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<BidirectionalGraph<string, IEdge<string>>> GetBidirectionalGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new BidirectionalGraph<string, IEdge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadBidirectionalGraph(graphMLFilePath);
            }
        }

        /// <summary> Creates bidirectional graphs. </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<BidirectionalGraph<string, IEdge<string>>> GetBidirectionalGraphs_All() => GetBidirectionalGraphsInternal();

        /// <summary> Creates bidirectional graphs (version manageable with define for slow tests). </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<BidirectionalGraph<string, IEdge<string>>> GetBidirectionalGraphs_SlowTests(int rate = -1)
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

        /// <summary> Creates undirected graphs (filterable). </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<IUndirectedGraph<string, IEdge<string>>> GetUndirectedGraphsInternal(
            [CanBeNull, InstantHandle] Func<string, int, bool> filter = null)
        {
            yield return new UndirectedGraph<string, IEdge<string>>();
            foreach (string graphMLFilePath in GetGraphMLFilePaths(filter))
            {
                yield return LoadUndirectedGraph(graphMLFilePath);
            }
        }

        /// <summary> Creates undirected graphs. </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<IUndirectedGraph<string, IEdge<string>>> GetUndirectedGraphs_All()
            => GetUndirectedGraphsInternal();

        /// <summary>
        /// Creates undirected graphs (version manageable with define for slow tests).
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<IUndirectedGraph<string, IEdge<string>>> GetUndirectedGraphs_SlowTests(int rate = -1)
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