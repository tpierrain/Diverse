using System;
using System.Collections.Generic;
using System.Globalization;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the primitives related to Dates.
    /// </summary>
    [TestFixture]
    public class DateTimeFuzzerShould
    {
        [Test]
        public void GenerateDifferentDates()
        {
            var fuzzer = new Fuzzer();

            var generatedDateTimes = new HashSet<DateTime>();
            var nbOfGeneration = 1000;
            for (var i = 0; i < nbOfGeneration; i++)
            {
                generatedDateTimes.Add(fuzzer.GenerateDateTime());
            }

            CheckThatGeneratedDatesAreDiverseAMinimum(generatedDateTimes, nbOfGeneration, 90);
        }

        [Test]
        public void GenerateDifferentDatesBetween()
        {
            var minDate = new DateTime(2020, 3, 28);
            var maxDate = new DateTime(2020, 11, 1);

            var fuzzer = new Fuzzer();

            var generated = fuzzer.GenerateDateTimeBetween(minDate, maxDate);

            Check.That(generated).IsAfterOrEqualTo(minDate).And.IsBeforeOrEqualTo(maxDate);
        }

        [TestCase("", "2020/11/01")]
        [TestCase(null, "2020/01/01")]
        [TestCase("2000/31/01", "2020/01/01")]
        [TestCase("portnaouaq", "2020/01/01")]
        public void ThrowsArgumentException_when_calling_GenerateDateTimeBetween_with_empty_or_incorrect_minDate(string minDate, string maxDate)
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() => fuzzer.GenerateDateTimeBetween(minDate, maxDate)).Throws<ArgumentException>()
                .WithMessage($"minDate is missing or incorrect. minDate: '{minDate}'. minDate should follow the pattern: yyyy/MM/dd");
        }

        [TestCase("2020/11/01", "")]
        [TestCase("2020/01/01", null)]
        [TestCase("2020/01/01", "2000/31/01")]
        [TestCase("2020/01/01", "portnaouaq")]
        public void ThrowsArgumentException_when_calling_GenerateDateTimeBetween_with_empty_or_incorrect_maxDate(string minDate, string maxDate)
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() => fuzzer.GenerateDateTimeBetween(minDate, maxDate)).Throws<ArgumentException>()
                .WithMessage($"maxDate is missing or incorrect. maxDate: '{maxDate}'. maxDate should follow the pattern: yyyy/MM/dd");
        }

        [TestCase(null, "")]
        [TestCase("portnaouaq", null)]
        [TestCase("", "2000/31/01")]
        [TestCase("2000 / 31 / 01", "portnaouaq")]
        public void ThrowsArgumentException_when_calling_GenerateDateTimeBetween_with_two_empty_or_incorrect_dateRanges(string minDate, string maxDate)
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() => fuzzer.GenerateDateTimeBetween(minDate, maxDate)).Throws<ArgumentException>()
                .WithMessage($"Min and Max dates are missing or incorrect. minDate: '{minDate}' maxDate: '{maxDate}'. minDate and maxDate should follow the pattern: yyyy/MM/dd");
        }

        [TestCase("1974/06/08", "2020/11/01")]
        [TestCase("2000/01/01", "2020/01/01")]
        public void GenerateDifferentDatesBetween(string minDate, string maxDate)
        {
            var fuzzer = new Fuzzer();

            var generatedDateTimes = new HashSet<DateTime>();
            var nbOfGeneration = 1000;
            for (var i = 0; i < nbOfGeneration; i++)
            {
                generatedDateTimes.Add(fuzzer.GenerateDateTimeBetween(minDate, maxDate));
            }

            CheckThatGeneratedDatesAreDiverseAMinimum(generatedDateTimes, nbOfGeneration, 90);

            // Check that every generated date is between our min and max boundaries
            CheckThatEveryDateTimeBelongsToTheInclusiveTimeRange(generatedDateTimes, minDate, maxDate);
        }

        [Test]
        public void GenerateDateTimeBetweenUsingInclusiveBoundaries()
        {
            var fuzzer = new Fuzzer();
            var minDate = new DateTime(1074, 6,8);
            var maxDate = minDate;

            var generateDateTime = fuzzer.GenerateDateTimeBetween(minDate, maxDate);

            Check.That(generateDateTime).IsEqualTo(maxDate);
        }

        //[Test]
        //public void GenerateDateTimeLastYear()
        //{
        //    // TBD
        //
        //    var fuzzer = new Fuzzer();

        //    var generateDateTimeLastYear = fuzzer.GenerateDateTimeLastYear();

        //    CheckThatDateTimeBelongsToTheInclusiveTimeRange(generateDateTimeLastYear, DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1));
        //}

        #region helpers

        private static void CheckThatGeneratedDatesAreDiverseAMinimum(HashSet<DateTime> generatedDateTimes, int nbOfGeneration, int percentage)
        {
            // Check that generated dates are diverse
            var actualPercentageOfDifferentDateTimes = generatedDateTimes.Count * 100 / nbOfGeneration;
            Check.That(actualPercentageOfDifferentDateTimes).IsStrictlyGreaterThan(percentage);
        }

        private static void CheckThatEveryDateTimeBelongsToTheInclusiveTimeRange(HashSet<DateTime> generatedDateTimes,
            string minDate, string maxDate)
        {
            var minDateOk = DateTime.TryParseExact(minDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var minDateTime);
            var maxDateOk = DateTime.TryParseExact(maxDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var maxDateTime);

            foreach (var generatedDateTime in generatedDateTimes)
            {
                Check.That(generatedDateTime).IsBefore(maxDateTime.AddDays(1)).And.IsAfter(minDateTime.AddDays(-1));
            }
        }

        private static void CheckThatDateTimeBelongsToTheInclusiveTimeRange(DateTime generatedDateTime, string minDate,
            string maxDate)
        {
            var minDateOk = DateTime.TryParseExact(minDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var minDateTime);
            var maxDateOk = DateTime.TryParseExact(maxDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var maxDateTime);

            Check.That(generatedDateTime).IsBefore(maxDateTime.AddDays(1)).And.IsAfter(minDateTime.AddDays(-1));
        }

        #endregion
    }
}