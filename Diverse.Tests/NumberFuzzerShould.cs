using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the primitives related to Numbers.
    /// </summary>
    [TestFixture]
    public class NumberFuzzerShould
    {
        [TestCase(500)]
        public void GeneratePositiveInteger_with_an_inclusive_upper_bound(int attempts)
        {
            var fuzzer = new Fuzzer();

            var maxValue = 3;

            var generatedPositiveNumbers = new List<int>();
            for (var i = 0; i < attempts; i++)
            {
                generatedPositiveNumbers.Add(fuzzer.GeneratePositiveInteger(maxValue));
            }

            Check.That(generatedPositiveNumbers.Any(n => n == 3)).IsTrue();
            Check.That(generatedPositiveNumbers.Any(n => n > 3)).IsFalse();
        }

        [TestCase(500)]
        public void GenerateIntegers_with_an_inclusive_upper_bound(int attempts)
        {
            var fuzzer = new Fuzzer();

            var maxValue = 3;

            var generatedPositiveNumbers = new List<int>();
            for (var i = 0; i < attempts; i++)
            {
                generatedPositiveNumbers.Add(fuzzer.GenerateInteger(-2, maxValue));
            }

            Check.That(generatedPositiveNumbers.Any(n => n == 3)).IsTrue();
            Check.That(generatedPositiveNumbers.Any(n => n > 3)).IsFalse();
        }

        [Test]
        public void GenerateLong()
        {
            
        }

    }
}