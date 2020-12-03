using System;

namespace Diverse.Numbers
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    public class NumberFuzzer : IFuzzNumbers
    {
        private readonly IProvideCorePrimitivesToFuzzer _fuzzerPrimitives;

        /// <summary>
        /// Instantiates a <see cref="NumberFuzzer"/>.
        /// </summary>
        /// <param name="fuzzerPrimitives">Instance of <see cref="IProvideCorePrimitivesToFuzzer"/> to use.</param>
        public NumberFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
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
            
            return _fuzzerPrimitives.Random.Next(minValue, maxValue);
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
    }
}