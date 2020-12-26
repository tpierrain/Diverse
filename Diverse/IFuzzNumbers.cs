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
        int GenerateInteger(int? minValue = null, int? maxValue = null);

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        int GeneratePositiveInteger(int? maxValue = null);

        /// <summary>
        /// Generates a random decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive upper bound of the random number returned.</param>
        /// <returns>A decimal value generated randomly.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref>.</exception>
        decimal GenerateDecimal(decimal? minValue = null, decimal? maxValue = null);

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive positive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive positive upper bound of the random number returned.</param>
        /// <returns>A positive decimal value generated randomly.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref>.</exception>
        decimal GeneratePositiveDecimal(decimal? minValue = null, decimal? maxValue = null);

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A long value generated randomly.</returns>
        long GenerateLong(long? minValue = null, long? maxValue = null);
    }
}