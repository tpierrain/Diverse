using System;
using System.Collections.Generic;
using System.Linq;
using Diverse.Address;
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
        [Repeat(100)]
        public void GeneratePositiveDecimal_respecting_the_specified_very_tight_range()
        {
            var fuzzer = new Fuzzer();

            var minValue = 1.1074696394971609377302661681m;
            var maxValue = 1.1074696394971609377302661682m;
            var number = fuzzer.GeneratePositiveDecimal(minValue, maxValue);

            Check.WithCustomMessage($"number: {number} should be lower or equal to maxValue: {maxValue}")
                .That(number <= maxValue).IsTrue();
            
            Check.WithCustomMessage($"number: {number} should be greater or equal to minValue: {minValue}")
                .That(number >= minValue).IsTrue();
        }

        [Test]
        [Repeat(10000)]
        public void Generate_Decimal_respecting_the_specified_ranges_within_42()
        {
            var fuzzer = new Fuzzer();

            var minValue = 42.30266168232m;
            var maxValue = 42.9999999999999m;
            var number = fuzzer.GenerateDecimal(minValue, maxValue);

            TestContext.WriteLine(number);

            Check.WithCustomMessage($"number: {number} should be lower or equal to maxValue: {maxValue}")
                .That(number <= maxValue).IsTrue();

            Check.WithCustomMessage($"number: {number} should be greater or equal to minValue: {minValue}")
                .That(number >= minValue).IsTrue();
        }

        [Test]
        [Repeat(10000)]
        public void Generate_Decimal_respecting_the_specified_ranges()
        {
            var fuzzer = new Fuzzer();

            var minValue = -23023456564.3332323234m;
            var maxValue = 7777777099232.999m;
            var number = fuzzer.GenerateDecimal(minValue, maxValue);

            Check.WithCustomMessage($"number: {number} should be lower or equal to maxValue: {maxValue}")
                .That(number <= maxValue).IsTrue();

            Check.WithCustomMessage($"number: {number} should be greater or equal to minValue: {minValue}")
                .That(number >= minValue).IsTrue();
        }

        [Test]
        public void GeneratePositiveDecimal_throws_Exception_when_maxValue_is_greater_minValue()
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() =>
            {
                var positiveDecimal = fuzzer.GeneratePositiveDecimal(10, 1);
            }).Throws<ArgumentOutOfRangeException>()
                .WithMessage($"Specified argument was out of the range of valid values. (Parameter 'maxValue should be greater than minValue. minValue: 10 - maxValue: 1')");
        }

        [Test]
        public void GeneratePositiveDecimal_throws_Exception_when_minValue_is_negative()
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() =>
                {
                    var positiveDecimal = fuzzer.GeneratePositiveDecimal(-1, 2);
                }).Throws<ArgumentOutOfRangeException>()
                .WithMessage($"minValue should be positive. minValue: -1 (Parameter 'minValue')");
        }

        [Test]
        public void Repl()
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 50; i++)
            {
                var generated = fuzzer.GenerateAddress(Country.Usa);

                TestContext.WriteLine(generated);
            }
        }
    }
}