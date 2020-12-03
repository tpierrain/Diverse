using System;

namespace Diverse
{
    /// <summary>
    /// Provides Core primitives for <see cref="Fuzzer"/> to be able to work.
    /// </summary>
    public interface IProvideCorePrimitivesToFuzzer
    {
        /// <summary>
        /// Gets a <see cref="Random"/> instance to use if you want your Fuzzer to be deterministic when providing a seed.
        /// </summary>
        Random Random { get; }

        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        bool HeadsOrTails();
    }
}