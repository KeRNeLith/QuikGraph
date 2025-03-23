namespace QuikGraph.Tests
{
    /// <summary> Unit test categories. </summary>
    public static class TestCategories
    {
        /// <summary> Long-running unit tests. </summary>
        public const string LongRunning = nameof(LongRunning);

        /// <summary> Unit tests skipped by the CI. </summary>
        public const string CISkip = nameof(CISkip);

        /// <summary> Verbose unit tests (not really relevant to test a feature). </summary>
        public const string VerboseTest = nameof(VerboseTest);
    }
}