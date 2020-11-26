using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the primitives related to Dates.
    /// </summary>
    [TestFixture]
    public class FuzzerWithDatesShould
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

            Check.That(generatedDateTimes.Count).IsEqualTo(nbOfGeneration);
        }
    }
}