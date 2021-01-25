using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Diverse.Strings;
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
    public class NoDuplicationFuzzersShould
    {
        #region enums

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_MagnificentSeven_Enum()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = Enum.GetValues(typeof(MagnificentSeven)).Length;
            CheckThatNoDuplicationIsMadeWhileGenerating<MagnificentSeven>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateEnum<MagnificentSeven>();
            });
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_MagnificentSeven_Enum_when_instantiating_from_fuzzer()
        {
            var fuzzer = new Fuzzer();

            var duplicationFuzzer = fuzzer.GenerateNoDuplicationFuzzer();

            var maxNumberOfElements = Enum.GetValues(typeof(MagnificentSeven)).Length;
            CheckThatNoDuplicationIsMadeWhileGenerating<MagnificentSeven>(duplicationFuzzer, maxNumberOfElements, () =>
            {
                return duplicationFuzzer.GenerateEnum<MagnificentSeven>();
            });
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_The_good_the_bad_and_the_ugly_Enum()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = Enum.GetValues(typeof(TheGoodTheBadAndTheUgly)).Length;
            CheckThatNoDuplicationIsMadeWhileGenerating<TheGoodTheBadAndTheUgly>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateEnum<TheGoodTheBadAndTheUgly>();
            });
        }

        #endregion

        #region Guids

        [Test]
        //[Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_Guids()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = 100000;
            CheckThatNoDuplicationIsMadeWhileGenerating<Guid>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateGuid();
            });
        }

        #endregion

        #region Numbers

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_integers_within_a_range()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = 10;
            CheckThatNoDuplicationIsMadeWhileGenerating<int>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateInteger(0, maxNumberOfElements);
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_integers()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

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
            var fuzzer = new Fuzzer(noDuplication: true);

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
            var fuzzer = new Fuzzer(noDuplication: true);

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
        [TestCase(-1000, 1000)]
        [TestCase(-10000, 10000)]
        public void Be_able_to_provide_always_different_values_of_long_within_a_wide_range(int minValue, int maxValue)
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = (int)(maxValue - minValue + 1); // +1 since it is upper bound inclusive
            CheckThatNoDuplicationIsMadeWhileGenerating<long>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateLong(minValue, maxValue);
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_long_within_a_huge_range()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var minValue = long.MinValue;
            var maxValue = long.MaxValue;

            var maxNumberOfElements = 100000; // we won't test all possible long here! ;-)
            CheckThatNoDuplicationIsMadeWhileGenerating<long>(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateLong(minValue, maxValue);
            });
        }


        #endregion

        #region Collection

        [Test]
        public void Be_able_to_pick_always_different_values_from_a_list_of_string()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var candidates = new List<string>() { "un", "dos", "tres", "quatro", "cinquo", "seis" };

            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, candidates.Count, () =>
            {
                return fuzzer.PickOneFrom(candidates);
            });
        }

        [Test]
        public void Be_able_to_pick_always_different_values_from_a_list_of_int()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var candidates = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, candidates.Count, () =>
            {
                return fuzzer.PickOneFrom(candidates);
            });
        }

        [Test]
        public void Be_able_to_pick_always_different_values_from_a_medium_size_list_of_int()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var candidates = Enumerable.Range(-10000, 10000).ToArray();

            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, candidates.LongLength, () =>
            {
                return fuzzer.PickOneFrom(candidates);
            });
        }

        [Test]
        public void Be_able_to_pick_always_different_values_from_a_list_of_enum()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var candidates = new List<TheGoodTheBadAndTheUgly>() { TheGoodTheBadAndTheUgly.TheGood, TheGoodTheBadAndTheUgly.TheBad, TheGoodTheBadAndTheUgly.TheUgly };

            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, candidates.Count, () =>
            {
                return fuzzer.PickOneFrom(candidates);
            });
        }

        [Test]
        public void Be_able_to_pick_different_values_from_a_list_of_string()
        {
            var fuzzer = new Fuzzer(884871408);
            var rateCodes = new[] { "PRBA", "LRBAMC", "AVG1", "PRBB", "LRBA", "BBSP", "PRBA2", "LRBA2MC", "PRPH", "PBCITE_tes", "PRPH2", "PRP2N2", "PR3NS1", "PBHS", "PBSNWH", "PBTHE", "PBVPOM", "PBZOO", "PHBO", "PHBPP", "PAB01", "PHB01", "PH3P", "LH3PMC", "CAMI", "FRCAMIF", "GENERICRATECODE", "PBGGG", "PBPPBRAZIL", "PBSENIOR" };

            var noDupFuzzer = fuzzer.GenerateNoDuplicationFuzzer();

            CheckThatNoDuplicationIsMadeWhileGenerating(noDupFuzzer, rateCodes.Length, () =>
            {
                return noDupFuzzer.PickOneFrom(rateCodes);
            });
            
        }

        #endregion

        #region Persons

        [Test]
        public void Be_able_to_provide_always_different_values_of_FirstName()
        {
            var fuzzer = new Fuzzer(noDuplication: true);
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, 80, () =>
            {
                return fuzzer.GenerateFirstName();
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_LastName()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var firstName = fuzzer.GenerateFirstName();
            var continent = Locations.FindContinent(firstName);

            var maxNumberOfLastNamesForThisContinent = LastNames.PerContinent[continent].Length;

            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfLastNamesForThisContinent, () =>
            {
                return fuzzer.GenerateLastName(firstName);
            });
        }

        [Test]
        [Repeat(100)]
        public void Be_able_to_provide_always_different_values_of_Emails()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var firstName = fuzzer.GenerateFirstName();
            var lastName = fuzzer.GenerateLastName(firstName);

            var maxNumberOfElements = 16;
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateEMail(firstName, lastName);
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_Passwords()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = 100000;
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GeneratePassword();
            });
        }

        [Test]
        public void Be_able_to_provide_always_different_values_of_Age()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = 97 - 18;
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateAge();
            });
        }

        #endregion

        #region Strings

        [Test]
        [TestCase(Feeling.Negative)]
        [TestCase(Feeling.Positive)]
        public void Be_able_to_provide_always_different_values_of_Adjective(Feeling feeling)
        {
            var fuzzer = new Fuzzer(2023547856, noDuplication: true);

            var maxNumberOfAdjectives = Adjectives.PerFeeling[feeling].Distinct().Count();
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfAdjectives, () =>
            {
                var adjective = fuzzer.GenerateAdjective(feeling);
                TestContext.WriteLine(adjective);
                return adjective;
            });
        }

        #endregion

        #region Lorem

        [Test]
        [Repeat(5000)]
        public void Be_able_to_provide_always_different_Letters()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = LoremFuzzer.Alphabet.Length;
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateLetter();
            });
        }

        #endregion

        #region DateTimes

        [Test]
        public void Be_able_to_provide_always_different_values_of_DateTime()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var maxNumberOfElements = 10000;
            CheckThatNoDuplicationIsMadeWhileGenerating(fuzzer, maxNumberOfElements, () =>
            {
                return fuzzer.GenerateDateTime();
            });
        }

        #endregion


        
        [Test]
        public void Throw_DuplicationException_with_fix_explanation_when_number_of_attempts_is_too_low()
        {
            var fuzzer = new Fuzzer(707888174, noDuplication: true);

            // Make it fail on purpose
            fuzzer.MaxFailingAttemptsForNoDuplication = 1;

            Check.ThatCode(() =>
                {
                    const int maxValue = 50;
                    var moreAttemptsThanMaxValue = (maxValue + 2);
                    for (var i = 0; i < moreAttemptsThanMaxValue; i++)
                    {
                        var integer = fuzzer.GenerateInteger(0, maxValue);
                    }
                }).Throws<DuplicationException>()
                .AndWhichMessage()
                .StartsWith($"Couldn't find a non-already provided value of System.Int32 after {fuzzer.MaxFailingAttemptsForNoDuplication} attempts. Already provided values:").
                And.EndsWith($@". You can either:
- Generate a new specific fuzzer to ensure no duplication is provided for a sub-group of fuzzed values (anytime you want through the {nameof(IFuzz.GenerateNoDuplicationFuzzer)}() method of your current Fuzzer instance. E.g.: var tempFuzzer = fuzzer.{nameof(IFuzz.GenerateNoDuplicationFuzzer)}();)
- Increase the value of the {nameof(Fuzzer.MaxFailingAttemptsForNoDuplication)} property for your {nameof(IFuzz)} instance.");
        }

        private static void CheckThatNoDuplicationIsMadeWhileGenerating<T>(IFuzz fuzzer, long maxNumberOfElements, Func<T> fuzzingFunction)
        {
            var returnedElements = new HashSet<T>(); //T
            for (var i = 0; i < maxNumberOfElements; i++)
            {
                try
                {
                    var element = fuzzingFunction();
                    returnedElements.Add(element);
                    //TestContext.WriteLine(element.ToString());
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