using System;

namespace Diverse
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz : IProvideCorePrimitivesToFuzzer, IFuzzNumbers, IFuzzPersons, IFuzzDatesAndTime, IFuzzStrings, IFuzzTypes
    {
        /// <summary>
        /// Gets a <see cref="Random"/> instance to use if you want your extensible Fuzzer to be deterministic when providing a seed.
        /// </summary>
        new Random Random { get; }
    }
}