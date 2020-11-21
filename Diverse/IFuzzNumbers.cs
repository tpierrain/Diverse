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
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
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
        /// <returns>A positive integer value generated randomly.</returns>
        int GeneratePositiveInteger();

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        decimal GeneratePositiveDecimal();
    }
}