using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms.GraphPartition;
#if !SUPPORTS_SORTEDSET
using QuikGraph.Collections;
#endif
using static QuikGraph.Tests.TestHelpers;

namespace QuikGraph.Tests.Algorithms.GraphPartitioning
{
    /// <summary>
    /// Tests for <see cref="KernighanLinAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class KernighanLinAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            UndirectedGraph<int, TaggedUndirectedEdge<int, double>> graph = new (), nullGraph = null;
            var algorithm = graph.CreateKernighanLinAlgorithm(42);
            algorithm.AssertAlgorithmState(graph);
            Assert.AreEqual(default(Partition<int>), algorithm.Partition);

            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateKernighanLinAlgorithm(42));
        }

        [Test]
        public void GraphPartitioningSimpleGraph()
        {
            var edges = new List<TaggedUndirectedEdge<int, double>>
            {
                new TaggedUndirectedEdge<int,double>(0, 1, 100),
                new TaggedUndirectedEdge<int,double>(1, 2, 20),
                new TaggedUndirectedEdge<int,double>(2, 3, 10),
                new TaggedUndirectedEdge<int,double>(1, 3, 50)
            };

            UndirectedGraph<int, TaggedUndirectedEdge<int, double>> graph = CreateUndirectedGraph<int, TaggedUndirectedEdge<int, double>>(edges);

            var algorithm = graph.CreateKernighanLinAlgorithm(1);
            algorithm.Compute();

            var setA = new SortedSet<int>();
            var setB = new SortedSet<int>();
            setA.Add(0);
            setA.Add(1);
            setB.Add(3);
            setB.Add(2);
            var expected = new Partition<int>(setA, setB, 3);
            Assert.IsTrue(PartitionHelpers.AreEquivalent(expected, algorithm.Partition));
        }

        [Test]
        public void GraphPartitioningSimpleGraph2()
        {
            var edges = new List<TaggedUndirectedEdge<int, double>>
            {
                new TaggedUndirectedEdge<int, double>(0, 1, 1),
                new TaggedUndirectedEdge<int, double>(1, 2, 1),
                new TaggedUndirectedEdge<int, double>(2, 3, 1),
                new TaggedUndirectedEdge<int, double>(3, 4, 1),
                new TaggedUndirectedEdge<int, double>(0, 4, 1),
                new TaggedUndirectedEdge<int, double>(1, 4, 1)
            };

            UndirectedGraph<int, TaggedUndirectedEdge<int, double>> graph = CreateUndirectedGraph<int, TaggedUndirectedEdge<int, double>>(edges);

            var algorithm = graph.CreateKernighanLinAlgorithm(1);
            algorithm.Compute();

            var setA = new SortedSet<int>();
            var setB = new SortedSet<int>();
            for (int i = 0; i < 5; ++i)
            {
                if (i == 2 || i == 3)
                    setA.Add(i);
                else
                    setB.Add(i);
            }

            var expected = new Partition<int>(setA, setB, 3);
            Assert.IsTrue(PartitionHelpers.AreEquivalent(expected, algorithm.Partition));
        }

        /*
         9   10        11
         *    *         *
         *    *         *
         6****7*********8
         *    **       **
         *    *  *   *  *  
         *    *    *    *
         *    *  *   *  *
         *    **       **
         3****4*********5
              *         
              *         
         0****1*********2
        */
        [Test]
        public void GraphPartitioningSimpleGraph3()
        {
            var edges = new List<TaggedUndirectedEdge<int, double>>
            {
                new TaggedUndirectedEdge<int, double>(0, 1, 1),
                new TaggedUndirectedEdge<int, double>(1, 2, 50),
                new TaggedUndirectedEdge<int, double>(1, 4, 5),
                new TaggedUndirectedEdge<int, double>(3, 4, 1),
                new TaggedUndirectedEdge<int, double>(3, 6, 10),
                new TaggedUndirectedEdge<int, double>(4, 5, 1),
                new TaggedUndirectedEdge<int, double>(4, 7, 25),
                new TaggedUndirectedEdge<int, double>(4, 8, 100),
                new TaggedUndirectedEdge<int, double>(5, 7, 1),
                new TaggedUndirectedEdge<int, double>(5, 8, 3),
                new TaggedUndirectedEdge<int, double>(6, 7, 1),
                new TaggedUndirectedEdge<int, double>(6, 9, 2),
                new TaggedUndirectedEdge<int, double>(7, 8, 1),
                new TaggedUndirectedEdge<int, double>(7, 10, 5),
                new TaggedUndirectedEdge<int, double>(8, 11, 1)
            };

            UndirectedGraph<int, TaggedUndirectedEdge<int, double>> graph = CreateUndirectedGraph<int, TaggedUndirectedEdge<int, double>>(edges);

            var algorithm = graph.CreateKernighanLinAlgorithm(1);
            algorithm.Compute();

            var setA = new SortedSet<int>();
            var setB = new SortedSet<int>();
            for (int i = 0; i < 12; i++)
            {
                if (i < 4 || i == 6 || i == 9)
                    setB.Add(i);
                else
                    setA.Add(i);
            }

            var expected = new Partition<int>(setA, setB, 3);
            Assert.IsTrue(PartitionHelpers.AreEquivalent(expected, algorithm.Partition));
        }
    }
}