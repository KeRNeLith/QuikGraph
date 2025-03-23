using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.AssertHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary> Base class for rooted algorithm tests. </summary>
    internal abstract class RootedAlgorithmTestsBase
    {
        /// <summary> Tests <see cref="RootedAlgorithmBase{TVertex,TGraph}.TryGetRootVertex"/> </summary>
        protected static void TryGetRootVertex_Test<TVertex, TGraph>(
            [NotNull] RootedAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            Assert.IsFalse(algorithm.TryGetRootVertex(out _));

            var vertex = new TVertex();
            algorithm.SetRootVertex(vertex);
            Assert.IsTrue(algorithm.TryGetRootVertex(out TVertex root));
            AssertEqual(vertex, root);
        }

        /// <summary> Checks if <see cref="RootedAlgorithmBase{TVertex,TGraph}.SetRootVertex"/> modifies and raises <see cref="RootedAlgorithmBase{TVertex,TGraph}.RootVertexChanged"/> </summary>
        /// <inheritdoc cref="RootedAlgorithmBase{TVertex,TGraph}.SetRootVertex"/>
        protected static void SetRootVertex_Test<TGraph>(
            [NotNull] RootedAlgorithmBase<int, TGraph> algorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            int rootVertexChangeCount = 0;
            algorithm.RootVertexChanged += (_, _) => ++rootVertexChangeCount;

            const int vertex1 = 0;
            algorithm.SetRootVertex(vertex1);
            Assert.AreEqual(1, rootVertexChangeCount);
            algorithm.TryGetRootVertex(out int root);
            Assert.AreEqual(vertex1, root);

            // Not changed
            algorithm.SetRootVertex(vertex1);
            Assert.AreEqual(1, rootVertexChangeCount);
            algorithm.TryGetRootVertex(out root);
            Assert.AreEqual(vertex1, root);

            const int vertex2 = 1;
            algorithm.SetRootVertex(vertex2);
            Assert.AreEqual(2, rootVertexChangeCount);
            algorithm.TryGetRootVertex(out root);
            Assert.AreEqual(vertex2, root);

            algorithm.SetRootVertex(vertex1);
            Assert.AreEqual(3, rootVertexChangeCount);

            algorithm.TryGetRootVertex(out root);
            Assert.AreEqual(vertex1, root);
        }

        /// <summary> <see cref="RootedAlgorithmBase{TVertex,TGraph}.SetRootVertex"/> Throws <see cref="ArgumentNullException"/> when  </summary>
        protected static void SetRootVertex_Null_Should_Throw_ArgumentNullException<TVertex, TGraph>(
            [NotNull] RootedAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : class
            where TGraph : IImplicitVertexSet<TVertex>
        {
            Assert.Throws<ArgumentNullException>(() => algorithm.SetRootVertex(null!));
        }

        /// <summary> Checks that <see cref="RootedAlgorithmBase{TVertex,TGraph}.ClearRootVertex"/> is idempotent and raises <see cref="RootedAlgorithmBase{TVertex,TGraph}.RootVertexChanged"/> </summary>
        protected static void ClearRootVertex_RaisesVertexChanged_Test<TVertex, TGraph>(
            [NotNull] RootedAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            int rootVertexChangeCount = 0;
            // ReSharper disable once AccessToModifiedClosure
            algorithm.RootVertexChanged += (_, _) => ++rootVertexChangeCount;

            algorithm.ClearRootVertex();
            Assert.AreEqual(0, rootVertexChangeCount);

            var vertex = new TVertex();
            algorithm.SetRootVertex(vertex);
            rootVertexChangeCount = 0;

            algorithm.ClearRootVertex();
            Assert.AreEqual(1, rootVertexChangeCount);

            algorithm.ClearRootVertex();
            Assert.AreEqual(1, rootVertexChangeCount);
        }

        protected static void ComputeWithoutRoot_ShouldNotThrow_Test<TGraph>(
            [NotNull] IMutableVertexSet<int> graph,
            [NotNull, InstantHandle] Func<RootedAlgorithmBase<int, TGraph>> createAlgorithm)
            where TGraph : IImplicitVertexSet<int>
        {
            RootedAlgorithmBase<int, TGraph> algorithm = createAlgorithm();
            Assert.DoesNotThrow(algorithm.Compute);

            graph.AddVertexRange( 1, 2 );
            algorithm = createAlgorithm();
            Assert.DoesNotThrow(algorithm.Compute);
        }

        protected static void ComputeWithoutRoot_Throws_Test<TVertex, TGraph>(
            [NotNull, InstantHandle] Func<RootedAlgorithmBase<TVertex, TGraph>> createAlgorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            RootedAlgorithmBase<TVertex, TGraph> algorithm = createAlgorithm();
            Assert.Throws<InvalidOperationException>(algorithm.Compute);

            // Source vertex set but not to a vertex in the graph
            algorithm = createAlgorithm();
            algorithm.SetRootVertex(new TVertex());
            Assert.Throws<Exception>(algorithm.Compute);
        }

        protected static void ComputeWithRoot_Test<TVertex, TGraph>(
            [NotNull] RootedAlgorithmBase<TVertex, TGraph> algorithm)
            where TVertex : new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            var vertex = new TVertex();
            Assert.DoesNotThrow(() => algorithm.Compute(vertex));
            Assert.IsTrue(algorithm.TryGetRootVertex(out TVertex root));
            AssertEqual(vertex, root);
        }

        protected static void ComputeWithUnknownRootOrNull_Throws_Test<TVertex, TGraph>(
            [NotNull, InstantHandle] Func<RootedAlgorithmBase<TVertex, TGraph>> createAlgorithm)
            where TVertex : class, new()
            where TGraph : IImplicitVertexSet<TVertex>
        {
            RootedAlgorithmBase<TVertex, TGraph> algorithm = createAlgorithm();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.Compute(null));
            Assert.IsFalse(algorithm.TryGetRootVertex(out _));

            // Vertex not in the graph
            algorithm = createAlgorithm();
            Assert.Throws<ArgumentException>(() => algorithm.Compute(new TVertex()));
        }
    }
}