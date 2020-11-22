using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the deterministic capabilities of the <see cref="Fuzzer"/>.
    /// <remarks>
    ///     This test fixture has lot of tests using a specific seed provided to
    ///     the Fuzzer instance.
    /// 
    ///     This is not representative on how to use <see cref="Fuzzer"/> instances in your code
    ///     base (i.e. without fixing a seed in order to go full random), but made
    ///     for deterministic results instead.
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerWithItsOwnDeterministicCapabilitiesShould
    {
        [Test]
        public void Be_able_to_expose_the_Seed_we_provide_but_also_the_one_we_did_not_provide()
        {
            var providedSeed = 428;
            var fuzzer = new Fuzzer(428);

            Check.That(fuzzer.Seed).IsEqualTo(providedSeed);

            var otherFuzzer = new Fuzzer();
            Check.That(fuzzer.Seed).IsNotEqualTo(otherFuzzer.Seed);
        }

        [Test]
        public void Be_Deterministic_when_specifying_an_existing_seed()
        {
            var seed = 1226354269;
            var fuzzer = new Fuzzer(seed);

            var fuzzedIntegers = new List<int>();
            for (var i = 0; i < 10; i++)
            {
                fuzzedIntegers.Add(fuzzer.GeneratePositiveInteger());
            }

            var fuzzedDecimal = fuzzer.GeneratePositiveDecimal();

            Check.That(fuzzedIntegers).ContainsExactly(33828652, 221134346, 1868176041, 1437724735, 1202622988, 974525956, 1605572379, 1127364048, 1453698000, 141079432);
            Check.That(fuzzedDecimal).IsEqualTo(720612366m);
        }

        [Test]
        public void Provide_different_values_when_using_different_Fuzzer_instances()
        {
            var deterministicFuzzer = new Fuzzer(1226354269, "first");
            var randomFuzzer = new Fuzzer(name: "second");
            var anotherRandomFuzzer = new Fuzzer(name: "third");

            var deterministicInteger = deterministicFuzzer.GeneratePositiveInteger();
            var randomInteger = randomFuzzer.GeneratePositiveInteger();
            var anotherRandomInteger = anotherRandomFuzzer.GeneratePositiveInteger();

            Check.That(deterministicInteger).IsEqualTo(33828652);
            Check.That(deterministicInteger).IsNotEqualTo(randomInteger).And.IsNotEqualTo(anotherRandomInteger);
            Check.That(randomInteger).IsNotEqualTo(anotherRandomInteger);
        }

        [Test]
        public void Be_Deterministic_when_specifying_an_existing_seed_whatever_the_specified_name_of_the_fuzzer()
        {
            var seed = 1226354269;
            var fuzzer = new Fuzzer(seed);
            var fuzzerWithSameFeedButDifferentName = new Fuzzer(seed, "Monte-Cristo");

            var valueFuzzer = fuzzer.GenerateInteger();
            var valueFuzzerWithNameSpecified = fuzzerWithSameFeedButDifferentName.GenerateInteger();

            Check.That(fuzzerWithSameFeedButDifferentName.Name).IsNotEqualTo(fuzzer.Name);
            Check.That(valueFuzzerWithNameSpecified).IsEqualTo(valueFuzzer);
        }
    }
}