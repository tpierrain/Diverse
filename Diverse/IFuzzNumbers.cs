namespace Diverse
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    public interface IFuzzNumbers
    {
        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        int GenerateInteger(int minValue, int maxValue);

        /// <summary>
        /// Generates a random integer value.
        /// </summary>
        /// <returns>An integer value generated randomly.</returns>
        int GenerateInteger();

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        int GeneratePositiveInteger(int? maxValue = null);

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        decimal GeneratePositiveDecimal();

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <returns>A long value generated randomly.</returns>
        long GenerateLong();
    }
}