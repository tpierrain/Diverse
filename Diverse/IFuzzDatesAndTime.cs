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
    }
}