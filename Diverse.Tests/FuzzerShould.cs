using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about some intrinsic behaviours of the <see cref="Fuzzer"/>.
    /// </summary>
    [TestFixture]
    public class FuzzerShould
    {
        [Test]
        [Repeat(200)]
        public void Be_able_to_avoid_providing_twice_the_same_MagnificentSeven_Enum_value_when_specified()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var returnedElements = new HashSet<MagnificentSeven>();

            var numberOfMagnificent = 7;
            for (var i = 0; i < numberOfMagnificent; i++)
            {
                var aMagnificent = fuzzer.GenerateEnum<MagnificentSeven>();
                TestContext.WriteLine(aMagnificent.ToString());
                returnedElements.Add(aMagnificent);
            }

            Check.That(returnedElements).HasSize(numberOfMagnificent);
        }

        [Test]
        [Repeat(200)]
        public void Be_able_to_avoid_providing_twice_the_same_The_good_the_bad_and_the_ugly_Enum_value_when_specified()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var returnedElements = new HashSet<TheGoodTheBadAndTheUgly>();

            var numberOfPersonas = 3;
            for (var i = 0; i < numberOfPersonas; i++)
            {
                var someone = fuzzer.GenerateEnum<TheGoodTheBadAndTheUgly>();
                TestContext.WriteLine(someone.ToString());
                returnedElements.Add(someone);
            }

            Check.That(returnedElements).HasSize(numberOfPersonas);
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