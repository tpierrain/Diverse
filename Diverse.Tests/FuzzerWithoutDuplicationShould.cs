using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NFluent;
using NFluent.ApiChecks;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about some intrinsic behaviours of the <see cref="Fuzzer"/>.
    /// Note: we use <see cref="RepeatAttribute"/> only for every test that has a small number of combinations (e.g. 10 or enum values or...).
    /// </summary>
    [TestFixture]
    [SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
    public class FuzzerWithoutDuplicationShould
    {
        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_MagnificentSeven_Enum()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = Enum.GetValues(typeof(MagnificentSeven)).Length;
            CheckThatNoDuplicationIsMadeWhileGenerating<MagnificentSeven>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateEnum<MagnificentSeven>();
            });
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_The_good_the_bad_and_the_ugly_Enum()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = Enum.GetValues(typeof(TheGoodTheBadAndTheUgly)).Length;
            CheckThatNoDuplicationIsMadeWhileGenerating<TheGoodTheBadAndTheUgly>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateEnum<TheGoodTheBadAndTheUgly>();
            });
        }

        [Test]
        //[Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_Guids()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = 100000;
            CheckThatNoDuplicationIsMadeWhileGenerating<Guid>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateGuid();
            });
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_integers_within_a_range()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = 10;
            CheckThatNoDuplicationIsMadeWhileGenerating<int>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateInteger(0, maxNumberOfElements);
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_integers()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = 1000;
            CheckThatNoDuplicationIsMadeWhileGenerating<int>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateInteger();
            });
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_positive_integers()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var maxNumberOfElements = 10;
            CheckThatNoDuplicationIsMadeWhileGenerating<int>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GeneratePositiveInteger(10);
            });
        }

        [Test]
        [Repeat(100)]
        public void Be_able_to_provide_always_different_values_of_long_within_a_small_range()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var minValue = 0;
            var maxValue = 3;
            var maxNumberOfElements = (int)(maxValue - minValue + 1); // +1 since it is upper bound inclusive
            CheckThatNoDuplicationIsMadeWhileGenerating<long>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateLong(minValue, maxValue);
            });
        }

        [Test]
        [TestCase(-1, 10)]
        [TestCase(-1, 1)]
        [TestCase(-5, 5)]
        [TestCase(0, 600)]
        public void Be_able_to_provide_always_different_values_of_long_within_a_wide_range(int minValue, int maxValue)
        {
            var fuzzer = new Fuzzer(/*624013454,*/ avoidDuplication: true);

            var maxNumberOfElements = (int)(maxValue - minValue + 1); // +1 since it is upper bound inclusive
            CheckThatNoDuplicationIsMadeWhileGenerating<long>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateLong(minValue, maxValue);
            });
        }

        [Test]
        public void Throw_DuplicationException_with_fix_explanation_when_number_of_attempts_is_too_low()
        {
            var fuzzer = new Fuzzer(707888174, avoidDuplication: true);
            fuzzer.MaxAttemptsToFindNotAlreadyProvidedValue = 1;

            Check.ThatCode(() =>
                {
                    const int maxValue = 10;
                    for (var i = 0; i < maxValue; i++)
                    {
                        var integer = fuzzer.GenerateInteger(0, maxValue);
                    }
                }).Throws<DuplicationException>()
                .AndWhichMessage()
                .StartsWith($"Couldn't find a non-already provided value of System.Int32 after {fuzzer.MaxAttemptsToFindNotAlreadyProvidedValue} attempts. Already provided values:").
                And.EndsWith($". In your case, try to increase the value of the {nameof(Fuzzer.MaxAttemptsToFindNotAlreadyProvidedValue)} property for your Fuzzer.");
        }

        private static void CheckThatNoDuplicationIsMadeWhileGenerating<T>(Fuzzer fuzzer, int maxNumberOfElements, Func<T> fuzzingFunction)
        {
            var returnedElements = new HashSet<T>(); //T
            for (var i = 0; i < maxNumberOfElements; i++)
            {
                try
                {
                    var element = fuzzingFunction();
                    returnedElements.Add(element);
                    // TestContext.WriteLine(element.ToString());
                }
                catch (DuplicationException) { }
            }

            Check.WithCustomMessage("The fuzzer was not able to generate the maximum number of expected entries")
                .That(returnedElements).HasSize(maxNumberOfElements);
        }
    }

    /// <summary>
    /// Name of the Good, the Bad and the Ugly (https://en.wikipedia.org/wiki/The_Good,_the_Bad_and_the_Ugly)
    /// </summary>
    public enum TheGoodTheBadAndTheUgly
    {
        TheGood,
        TheBad,
        TheUgly
    }

    /// <summary>
    /// Name of the magnificent seven (https://en.wikipedia.org/wiki/The_Magnificent_Seven_(2016_film))
    /// </summary>
    public enum MagnificentSeven
    {
        SamChisolm,
        JoshFaraday,
        GoodnightRobicheaux,
        JackHorne,
        BillyRocks,
        Vasquez,
        RedHarvest
    }
}