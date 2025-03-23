namespace QuikGraph.Algorithms.Assignment
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <inheritdoc cref="ModifyMatrixSquare(double[,])"/>
    public static class HabrTransformTests
    {
        public static IEnumerable<TestCaseData> TestMatrices() {
            yield return new TestCaseData(new double[,] {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            })
            { TestName = "3"};
            yield return new TestCaseData(new double[,] {
                { 82, 83, 69, 92 },
                { 77, 37, 49, 92 },
                { 11, 69,  5, 86 },
                {  8,  9, 98, 23 }
            })
            { TestName = "4"};
            //yield return HungarianAlgorithmTests.GetSampleMatrix();
        }

        [TestCaseSource(nameof(TestMatrices))]
        public static void TestHabrTransform(double[,] matrix)
        {
            Console.WriteLine("\nOriginal Matrix:");
            matrix.PrintMatrix();
            double[,] clone1 = (double[,]) matrix.Clone();
            matrix.ModifyMatrixSquare();
            Assert.AreNotEqual(clone1, matrix);

            Console.WriteLine("\nHabr-Transformed Matrix:");
            matrix.PrintMatrix();
            double[,] clone2 = (double[,]) matrix.Clone();
            matrix.ModifyMatrixSquare();
            Assert.AreEqual(clone2, matrix);

            Console.WriteLine("\n2*Habr-Transformed Matrix:");
            matrix.PrintMatrix();
        }
    }
}