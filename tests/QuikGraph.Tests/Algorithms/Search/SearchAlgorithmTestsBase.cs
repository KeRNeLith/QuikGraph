using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.AssertHelpers;

namespace QuikGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Base class for search algorithms.
    /// </summary>
    internal abstract class SearchAlgorithmTestsBase : RootedAlgorithmTestsBase
    {
        #region Test helpers

        protected static void TryGetTargetVertex_Test<TVertex, TGraph>(
            [NotNull] RootedSearchAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            Assert.IsFalse(algorithm.TryGetTargetVertex(out _));

            var vertex = new TVertex();
            algorithm.SetTargetVertex(vertex);
            Assert.IsTrue(algorithm.TryGetTargetVertex(out TVertex target));
            AssertEqual(vertex, target);
        }

        protected static void SetTargetVertex_Test<TGraph>(
            [NotNull] RootedSearchAlgorithmBase<int, TGraph> algorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            int targetVertexChangeCount = 0;
            algorithm.TargetVertexChanged += (_, _) => ++targetVertexChangeCount;

            const int vertex1 = 0;
            algorithm.SetTargetVertex(vertex1);
            Assert.AreEqual(1, targetVertexChangeCount);
            algorithm.TryGetTargetVertex(out int target);
            Assert.AreEqual(vertex1, target);

            // Not changed
            algorithm.SetTargetVertex(vertex1);
            Assert.AreEqual(1, targetVertexChangeCount);
            algorithm.TryGetTargetVertex(out target);
            Assert.AreEqual(vertex1, target);

            const int vertex2 = 1;
            algorithm.SetTargetVertex(vertex2);
            Assert.AreEqual(2, targetVertexChangeCount);
            algorithm.TryGetTargetVertex(out target);
            Assert.AreEqual(vertex2, target);

            algorithm.SetTargetVertex(vertex1);
            Assert.AreEqual(3, targetVertexChangeCount);
            algorithm.TryGetTargetVertex(out target);
            Assert.AreEqual(vertex1, target);
        }

        protected static void SetTargetVertex_Throws_Test<TVertex, TGraph>(
            [NotNull] RootedSearchAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : class
            where TGraph : IImplicitVertexSet<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.SetTargetVertex(null));
        }

        protected static void ClearTargetVertex_Test<TVertex, TGraph>(
            [NotNull] RootedSearchAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            int targetVertexChangeCount = 0;
            // ReSharper disable once AccessToModifiedClosure
            algorithm.TargetVertexChanged += (_, _) => ++targetVertexChangeCount;

            algorithm.ClearTargetVertex();
            Assert.AreEqual(0, targetVertexChangeCount);

            var vertex = new TVertex();
            SetTargetVertex(vertex);
            algorithm.ClearTargetVertex();
            Assert.AreEqual(1, targetVertexChangeCount);

            algorithm.ClearTargetVertex();
            Assert.AreEqual(1, targetVertexChangeCount);

            #region Local function

            void SetTargetVertex(TVertex v)
            {
                algorithm.SetTargetVertex(v);
                targetVertexChangeCount = 0;
            }

            #endregion
        }

        protected static void ComputeWithoutRoot_Throws_Test<TGraph>(
            [NotNull] IMutableVertexSet<int> graph,
            [NotNull, InstantHandle] Func<RootedSearchAlgorithmBase<int, TGraph>> createAlgorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            RootedSearchAlgorithmBase<int, TGraph> algorithm = createAlgorithm();
            Assert.Throws<InvalidOperationException>(algorithm.Compute);

            // Source (and target) vertex set but not to a vertex in the graph
            const int vertex1 = 1;
            algorithm = createAlgorithm();
            algorithm.SetRootVertex(vertex1);
            algorithm.SetTargetVertex(vertex1);
            Assert.Throws<Exception>(algorithm.Compute);

            const int vertex2 = 2;
            graph.AddVertex(vertex1);
            algorithm = createAlgorithm();
            algorithm.SetRootVertex(vertex1);
            algorithm.SetTargetVertex(vertex2);
            Assert.Throws<Exception>(algorithm.Compute);
        }

        protected static void ComputeWithRootAndTarget_Test<TGraph>(
            [NotNull] RootedSearchAlgorithmBase<int, TGraph> algorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            const int start = 0;
            const int end = 1;
            Assert.DoesNotThrow(() => algorithm.Compute(start, end));
            Assert.IsTrue(algorithm.TryGetRootVertex(out int root));
            Assert.IsTrue(algorithm.TryGetTargetVertex(out int target));
            AssertEqual(start, root);
            AssertEqual(end, target);
        }

        protected static void ComputeWithRootAndTarget_Throws_Test<TGraph>(
            [NotNull] IMutableVertexSet<int> graph,
            [NotNull] RootedSearchAlgorithmBase<int, TGraph> algorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            const int start = 1;
            const int end = 2;

            Assert.Throws<ArgumentException>(() => algorithm.Compute(start));
            graph.AddVertex(start);

            Assert.Throws<InvalidOperationException>(() => algorithm.Compute(start));

            Assert.Throws<ArgumentException>(() => algorithm.Compute(start, end));
        }

        protected static void ComputeWithRootAndTarget_Throws_Test<TVertex, TGraph>(
            [NotNull] RootedSearchAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : class, new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            var start = new TVertex();
            var end = new TVertex();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(start, null));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, end));
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion
    }
}