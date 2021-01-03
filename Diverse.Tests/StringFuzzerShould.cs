using System.Collections.Generic;
using Diverse.Strings;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class StringFuzzerShould
    {
        [Test]
        public void Be_able_to_generate_a_random_adjective()
        {
            var fuzzer = new Fuzzer(438709238);

            var randomString = fuzzer.GenerateAdjective();

            Check.That(randomString).IsEqualTo("upbeat");
        }

        [Test]
        public void Be_able_to_generate_a_positive_adjective()
        {
            var fuzzer = new Fuzzer(43930430);

            var positiveAdjectives = new List<string>();

            for(var i = 0; i < 3; i++)
            {
                positiveAdjectives.Add(fuzzer.GenerateAdjective(Feeling.Positive));
            }

            Check.That(positiveAdjectives).ContainsExactly("gifted", "sharp", "peaceful");
        }

        [Test]
        [Repeat(1000)]
        public void Generate_random_string_from_pattern()
        {
            var fuzzer = new Fuzzer();

            var value = fuzzer.GenerateFromPattern("X#A02").ToCharArray();

            Check.That(value[0]).IsALetter();
            Check.That(value[1]).IsADigit();
            Check.That(value[2]).IsEqualTo('A');
            Check.That(value[3]).IsEqualTo('0');
            Check.That(value[4]).IsEqualTo('2');
        }
    }
}