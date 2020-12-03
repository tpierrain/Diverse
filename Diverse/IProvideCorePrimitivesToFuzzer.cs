using System;

namespace Diverse
{
    /// <summary>
    /// Provides Core primitives for <see cref="Fuzzer"/> to be able to work.
    /// </summary>
    public interface IProvideCorePrimitivesToFuzzer
    {
        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        bool HeadsOrTails();
    }
}