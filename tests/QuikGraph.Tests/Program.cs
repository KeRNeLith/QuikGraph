using QuikGraph.Algorithms.Assignment;

namespace QuikGraph.Tests
{
    public static class Program
    {
        public static void Main()
        {
            HabrTransformTests.TestHabrTransform(new double[,] {
                { 82, 83, 69, 92 },
                { 77, 37, 49, 92 },
                { 11, 69,  5, 86 },
                {  8,  9, 98, 23 }
            });
        }
    }
}
