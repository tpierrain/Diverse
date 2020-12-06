using System;

namespace Diverse.Numbers
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    public class NumberFuzzer : IFuzzNumbers
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="NumberFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public NumberFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger(int minValue, int maxValue)
        {
            // Adjust the inclusiveness of the Fuzzer API to the exclusiveness of the Random API.
            maxValue = (maxValue == int.MaxValue) ? maxValue : maxValue + 1;
            
            return _fuzzer.Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random integer value.
        /// </summary>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger()
        {
            return GenerateInteger(int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        public int GeneratePositiveInteger(int? maxValue = null)
        {
            maxValue = maxValue ?? int.MaxValue;

            return GenerateInteger(0, maxValue.Value);
        }

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        public decimal GeneratePositiveDecimal()
        {
            return Convert.ToDecimal(GenerateInteger(0, int.MaxValue));
        }

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <returns>A long value generated randomly.</returns>
        public long GenerateLong()
        {
            // found here: https://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way

            var min = long.MinValue;
            var max = long.MaxValue;
            var buf = new byte[8];
            _fuzzer.Random.NextBytes(buf);

            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}