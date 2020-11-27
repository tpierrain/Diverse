using System;

namespace Diverse
{
    /// <summary>
    /// Fuzz Dates and Time.
    /// </summary>
    public interface IFuzzDatesAndTime
    {
        /// <summary>
        /// Generates a random <see cref="DateTime"/>.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value generated randomly.</returns>
        DateTime GenerateDateTime();

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minValue">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <param name="maxValue">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        DateTime GenerateDateTimeBetween(DateTime minValue, DateTime maxValue);

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minValue">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <param name="maxValue">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        DateTime GenerateDateTimeBetween(string minDate, string maxDate);
    }
}