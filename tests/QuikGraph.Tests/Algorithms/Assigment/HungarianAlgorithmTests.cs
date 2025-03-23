using System;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms.Assignment;

namespace QuikGraph.Tests.Algorithms.Assignment
{
    /// <summary>Tests for <see cref="HungarianAlgorithm"/>.</summary>
    [TestFixture]
    internal sealed class HungarianAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            int[,] costs = new int[0,0];
            var algorithm = new HungarianAlgorithm(costs);
            Assert.IsNull(algorithm.AgentByTaskNo);

            costs = new[,]
            {
                { 1, 2, 3 },
                { 1, 2, 3 },
            };
            algorithm = new HungarianAlgorithm(costs);
            Assert.IsNull(algorithm.AgentByTaskNo);
            algorithm.Compute();
            Assert.IsNotNull(algorithm.AgentByTaskNo);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new HungarianAlgorithm(null));
        }

        [Test]
        public void SimpleAssignment()
        {
            int[,] matrix =
            {
                { 1, 2, 3 },
                { 3, 3, 3 },
                { 3, 3, 2 }
            };
            var algorithm = new HungarianAlgorithm(matrix);
            int[] tasks = algorithm.Compute();

            Assert.AreEqual(0, tasks[0]);
            Assert.AreEqual(1, tasks[1]);
            Assert.AreEqual(2, tasks[2]);
        }

        static readonly int[,] _Cost =
        {
                { 82, 83, 69, 92 },
                { 77, 37, 49, 92 },
                { 11, 69,  5, 86 },
                {  8,  9, 98, 23 }
            };

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// J = Job | W = Worker
        ///     J1  J2  J3  J4
        /// W1  82  83  69  92
        /// W2  77  37  49  92
        /// W3  11  69  5   86
        /// W4  8   9   98  23
        /// </remarks>
        public static int[,] GetSampleMatrix() => (int[,])_Cost.Clone();

        [Test]
        public void JobAssignment()
        {
            int[,] cost = GetSampleMatrix();
            var algorithm = new HungarianAlgorithm(cost);
            algorithm.Compute();

            Assert.IsNotNull(algorithm.AgentByTaskNo);
            int[] tasks = algorithm.AgentByTaskNo;
            Assert.AreEqual(2, tasks[0]); // J1 to be done by W3
            Assert.AreEqual(1, tasks[1]); // J2 to be done by W2
            Assert.AreEqual(0, tasks[2]); // J3 to be done by W1
            Assert.AreEqual(3, tasks[3]); // J4 to be done by W4
        }

        [Test]
        public void SimpleAssignmentIterations()
        {
            int[,] matrix =
            {
                { 1, 2, 3 },
                { 3, 3, 3 },
                { 3, 3, 2 }
            };
            var algorithm = new HungarianAlgorithm(matrix);
            HungarianIteration[] iterations = algorithm.GetIterations().ToArray();

            int[] tasks = algorithm.AgentByTaskNo;
            Assert.AreEqual(0, tasks[0]);
            Assert.AreEqual(1, tasks[1]);
            Assert.AreEqual(2, tasks[2]);

            Assert.AreEqual(3, iterations.Length);
            Assert.That(
                new[]
                {
                    new[,] { { 0, 1, 2 }, { 0, 0, 0 }, { 1, 1, 0 } },
                    new[,] { { 0, 1, 2 }, { 0, 0, 0 }, { 1, 1, 0 } },
                    new[,] { { 0, 1, 2 }, { 0, 0, 0 }, { 1, 1, 0 } }
                }, Is.EqualTo(iterations.Select(iteration => iteration.Matrix)));
            Assert.AreEqual(
                new[]
                {
                    new[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                    new[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                    new[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }
                },
                iterations.Select(iteration => iteration.Mask));
            CollectionAssert.AreEqual(
                new[]
                {
                    new[] { false, false, false },
                    [false, false, false],
                    [false, false, false]
                },
                iterations.Select(iteration => iteration.RowsCovered));
            CollectionAssert.AreEqual(
                new[]
                {
                    new[] { false, false, false },
                    [true,  true,  true],
                    [true,  true,  true]
                },
                iterations.Select(iteration => iteration.ColumnsCovered));
            CollectionAssert.AreEqual(
                new[]
                {
                    HungarianAlgorithm.Steps.Init,
                    HungarianAlgorithm.Steps.Step1,
                    HungarianAlgorithm.Steps.End
                },
                iterations.Select(iteration => iteration.Step));
        }

        [Test]
        public void JobAssignmentIterations()
        {
            int[,] matrix = GetSampleMatrix();
            var algorithm = new HungarianAlgorithm(matrix);
            HungarianIteration[] iterations = algorithm.GetIterations().ToArray();

            Assert.IsNotNull(algorithm.AgentByTaskNo);
            int[] tasks = algorithm.AgentByTaskNo;
            Assert.AreEqual(2, tasks[0]); // J1 to be done by W3
            Assert.AreEqual(1, tasks[1]); // J2 to be done by W2
            Assert.AreEqual(0, tasks[2]); // J3 to be done by W1
            Assert.AreEqual(3, tasks[3]); // J4 to be done by W4

            Assert.AreEqual(11, iterations.Length);
            Assert.AreEqual(
                new[]
                {
                    new[,] { { 13, 14, 0, 23 }, { 40, 0, 12, 55 }, { 6, 64, 0, 81 }, { 0, 1, 90, 15 } },
                    new[,] { { 13, 14, 0, 23 }, { 40, 0, 12, 55 }, { 6, 64, 0, 81 }, { 0, 1, 90, 15 } },
                    new[,] { { 13, 14, 0, 23 }, { 40, 0, 12, 55 }, { 6, 64, 0, 81 }, { 0, 1, 90, 15 } },
                    new[,] { { 13, 14, 0,  8 }, { 40, 0, 12, 40 }, { 6, 64, 0, 66 }, { 0, 1, 90,  0 } },
                    new[,] { { 13, 14, 0,  8 }, { 40, 0, 12, 40 }, { 6, 64, 0, 66 }, { 0, 1, 90,  0 } },
                    new[,] { { 13, 14, 0,  8 }, { 40, 0, 12, 40 }, { 6, 64, 0, 66 }, { 0, 1, 90,  0 } },
                    new[,] { {  7, 14, 0,  2 }, { 34, 0, 12, 34 }, { 0, 64, 0, 60 }, { 0, 7, 96,  0 } },
                    new[,] { {  7, 14, 0,  2 }, { 34, 0, 12, 34 }, { 0, 64, 0, 60 }, { 0, 7, 96,  0 } },
                    new[,] { {  7, 14, 0,  2 }, { 34, 0, 12, 34 }, { 0, 64, 0, 60 }, { 0, 7, 96,  0 } },
                    new[,] { {  7, 14, 0,  2 }, { 34, 0, 12, 34 }, { 0, 64, 0, 60 }, { 0, 7, 96,  0 } },
                    new[,] { {  7, 14, 0,  2 }, { 34, 0, 12, 34 }, { 0, 64, 0, 60 }, { 0, 7, 96,  0 } }
                },
                iterations.Select(iteration => iteration.Matrix));
            Assert.AreEqual(
                new[]
                {
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 0 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 0 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 0 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 0 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 2 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 2 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 1, 0, 0, 2 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 2, 0, 0, 0 }, { 1, 0, 0, 2 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 1 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 1 } },
                    new[,] { { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 1 } }
                },
                iterations.Select(iteration => iteration.Mask));
            CollectionAssert.AreEqual(
                new bool[][]
                {
                    [false, false, false, false],
                    [false, false, false, false],
                    [false, false, false, false],
                    [false, false, false, false],
                    [false, false, false, true],
                    [false, false, false, true],
                    [false, false, false, true],
                    [false, false, false, true],
                    [false, false, false, false],
                    [false, false, false, false],
                    [false, false, false, false]
                },
                iterations.Select(iteration => iteration.RowsCovered));
            CollectionAssert.AreEqual(
                new bool[][]
                {
                    [false, false, false, false],
                    [true,  true,  true,  false],
                    [true,  true,  true,  false],
                    [true,  true,  true,  false],
                    [false, true,  true,  false],
                    [false, true,  true,  false],
                    [false, true,  true,  false],
                    [false, true,  true,  false],
                    [false, false, false, false],
                    [true,  true,  true,  true],
                    [true,  true,  true,  true]
                },
                iterations.Select(iteration => iteration.ColumnsCovered));
            CollectionAssert.AreEqual(
                new[]
                {
                    HungarianAlgorithm.Steps.Init,
                    HungarianAlgorithm.Steps.Step1,
                    HungarianAlgorithm.Steps.Step2,
                    HungarianAlgorithm.Steps.Step4,
                    HungarianAlgorithm.Steps.Step2,
                    HungarianAlgorithm.Steps.Step2,
                    HungarianAlgorithm.Steps.Step4,
                    HungarianAlgorithm.Steps.Step2,
                    HungarianAlgorithm.Steps.Step3,
                    HungarianAlgorithm.Steps.Step1,
                    HungarianAlgorithm.Steps.End
                },
                iterations.Select(iteration => iteration.Step));
        }

    }
}