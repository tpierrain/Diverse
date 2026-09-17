using System;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Mimics xUnit's ITestOutputHelper when it is used outside of an active test
    /// (e.g. from a test class constructor, or once the test that owned it has ended):
    /// it throws an <see cref="InvalidOperationException"/> instead of writing.
    /// <remarks>
    ///     This is the whole point of https://github.com/tpierrain/Diverse/issues/11, reproduced here
    ///     without taking any dependency on xUnit (Diverse is a zero-dependency library, and this
    ///     test project is a NUnit one).
    /// </remarks>
    /// </summary>
    public class TestOutputHelperOutsideOfAnActiveTestStub
    {
        public TestOutputHelperOutsideOfAnActiveTestStub()
        {
            // Cached once, so that the very same delegate instance is registered every time.
            WriteLine = line =>
            {
                CallCount++;
                throw new InvalidOperationException("There is no currently active test.");
            };
        }

        /// <summary>
        /// Gets the failing sink to register (the equivalent of testOutputHelper.WriteLine).
        /// </summary>
        public Action<string> WriteLine { get; }

        /// <summary>
        /// Gets the number of times the sink has been called (it throws every single time).
        /// </summary>
        public int CallCount { get; private set; }
    }
}
