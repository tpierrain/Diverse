using System;

namespace Diverse
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz : IFuzzNumbers, IFuzzPersons, IFuzzDatesAndTime, IFuzzStrings, IFuzzTypes
    {
        /// <summary>
        /// Gets a <see cref="Random"/> instance to use if you want your extensible Fuzzer to be deterministic when providing a seed.
        /// </summary>
        Random Random { get; }

        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        bool HeadsOrTails();
    }
}