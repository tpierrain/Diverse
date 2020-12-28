using System;

namespace Diverse
{
    /// <summary>
    /// <see cref="Guid"/> Fuzzer.
    /// </summary>
    internal class GuidFuzzer : IFuzzGuid
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiate a <see cref="GuidFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public GuidFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates a random <see cref="Guid"/>
        /// </summary>
        /// <returns>A random <see cref="Guid"/>.</returns>
        public Guid GenerateGuid()
        {
            var guid = new byte[16];
            _fuzzer.Random.NextBytes(guid);

            return new Guid(guid);
        }
    }
}