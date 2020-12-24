using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about some intrinsic behaviours of the <see cref="Fuzzer"/>.
    /// </summary>
    [TestFixture]
    public class FuzzerWithoutDuplicationShould
    {
        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_MagnificentSeven_Enum()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var returnedElements = new HashSet<MagnificentSeven>();
            var numberOfMagnificent = Enum.GetValues(typeof(MagnificentSeven)).Length;
            for (var i = 0; i < numberOfMagnificent; i++)
            {
                var aMagnificent = fuzzer.GenerateEnum<MagnificentSeven>();
                returnedElements.Add(aMagnificent);
                TestContext.WriteLine(aMagnificent.ToString());
            }

            Check.That(returnedElements).HasSize(numberOfMagnificent);
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_The_good_the_bad_and_the_ugly_Enum()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var returnedElements = new HashSet<TheGoodTheBadAndTheUgly>();
            var numberOfPersonas = Enum.GetValues(typeof(TheGoodTheBadAndTheUgly)).Length;
            for (var i = 0; i < numberOfPersonas; i++)
            {
                var someone = fuzzer.GenerateEnum<TheGoodTheBadAndTheUgly>();
                returnedElements.Add(someone);
                TestContext.WriteLine(someone.ToString());
            }

            Check.That(returnedElements).HasSize(numberOfPersonas);
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_provide_always_different_values_of_integers_within_a_range()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var generatedIntegers = new HashSet<int>();
            const int maxValue = 10;
            for (var i = 0; i < maxValue; i++)
            {
                var integer = fuzzer.GenerateInteger(0, maxValue);
                generatedIntegers.Add(integer);
                TestContext.WriteLine(integer.ToString());
            }

            Check.That(generatedIntegers).HasSize(maxValue);
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