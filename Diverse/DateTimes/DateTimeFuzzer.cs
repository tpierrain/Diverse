using System;
using System.Globalization;

namespace Diverse.DateTimes
{
    /// <summary>
    /// Fuzz <see cref="DateTime"/>.
    /// </summary>
    public class DateTimeFuzzer : IFuzzDatesAndTime
    {
        private IProvideCorePrimitivesToFuzzer _fuzzerPrimitives;
        
        private readonly IFuzzNumbers _numberFuzzer;

        /// <summary>
        /// Instantiates a <see cref="DateTimeFuzzer"/>.
        /// </summary>
        /// <param name="fuzzerPrimitives">Instance of <see cref="IProvideCorePrimitivesToFuzzer"/> to use.</param>
        /// <param name="numberFuzzer">Instance of <see cref="IFuzzNumbers"/> to use.</param>
        public DateTimeFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives, IFuzzNumbers numberFuzzer)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
            _numberFuzzer = numberFuzzer;
        }

        /// <summary>
        /// Generates a random <see cref="DateTime"/>.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value generated randomly.</returns>
        public DateTime GenerateDateTime()
        {
            return GenerateDateTimeBetween(DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minValue">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <param name="maxValue">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        public DateTime GenerateDateTimeBetween(DateTime minValue, DateTime maxValue)
        {
            var nbDays = (maxValue - minValue).Days;

            var midInterval = (minValue.AddDays(nbDays/2));

            var maxDaysAllowedBefore = (midInterval - minValue).Days;
            var maxDaysAllowedAfter = (maxValue - midInterval).Days;
            var maxDays = Math.Min(maxDaysAllowedBefore, maxDaysAllowedAfter);

            return midInterval.AddDays(_numberFuzzer.GenerateInteger(-maxDays, maxDays));
        }

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minDate">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <param name="maxDate">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        public DateTime GenerateDateTimeBetween(string minDate, string maxDate)
        {
            var minDateOk = DateTime.TryParseExact(minDate, "yyyy/MM/dd", null, DateTimeStyles.None,  out var minDateTime);
            var maxDateOk = DateTime.TryParseExact(maxDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var maxDateTime);

            if (!minDateOk || !maxDateOk)
            {
                ThrowProperArgumentException(minDateOk, minDate, maxDateOk, maxDate);
            }

            return GenerateDateTimeBetween(minDateTime, maxDateTime);
        }

        private static void ThrowProperArgumentException(bool minDateOk, string minDate, bool maxDateOk, string maxDate)
        {
            string message;

            if (!minDateOk && !maxDateOk)
            {
                message =
                    $"Min and Max dates are missing or incorrect. minDate: '{minDate}' maxDate: '{maxDate}'. minDate and maxDate should follow the pattern: yyyy/MM/dd";
            }
            else
            {
                var paramName = string.Empty;
                var incorrectValue = string.Empty;

                if (!minDateOk)
                {
                    paramName = nameof(minDate);
                    incorrectValue = minDate;
                }

                if (!maxDateOk)
                {
                    paramName = nameof(maxDate);
                    incorrectValue = maxDate;
                }

                message =
                    $"{paramName} is missing or incorrect. {paramName}: '{incorrectValue}'. {paramName} should follow the pattern: yyyy/MM/dd";
            }

            throw new ArgumentException(message);
        }
    }
}