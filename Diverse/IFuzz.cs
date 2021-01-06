using System;

namespace Diverse
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz : IFuzzNumbers, IFuzzPersons, IFuzzDatesAndTime, IFuzzStrings, IFuzzTypes, IFuzzGuid, IFuzzFromCollections, IFuzzLorem, IFuzzAddress
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

        /// <summary>
        /// Gets of sets a value indicating whether the <see cref="Fuzzer"/> should avoid providing twice the same value or not.
        /// </summary>
        bool AvoidDuplication { get; set; }

        /// <summary>
        /// Gets a <see cref="IFuzz"/> instance that will not return twice the same value (whatever the method called).
        /// </summary>
        /// <returns></returns>
        IFuzz GetFuzzerProvidingNoDuplication();
    }
}