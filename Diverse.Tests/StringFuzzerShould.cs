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
        [Repeat(100)]
        public void Generate_random_string_replacing_x_with_lowerCaseLetter()
        {
            var fuzzer = new Fuzzer();

            var chars = fuzzer.GenerateStringFromPattern("x").ToCharArray();

            Check.That(chars[0]).IsALetter();
            Check.That(char.IsUpper(chars[0])).IsFalse();
        }

        [Test]
        [Repeat(100)]
        public void Generate_random_string_replacing_X_with_UpperCaseLetter()
        {
            var fuzzer = new Fuzzer();

            var chars = fuzzer.GenerateStringFromPattern("X").ToCharArray();

            Check.That(chars[0]).IsALetter();
            Check.That(char.IsUpper(chars[0])).IsTrue();
        }

        [Test]
        [Repeat(100)]
        public void Generate_random_string_replacing_SharpSign_with_a_single_digit_number()
        {
            var fuzzer = new Fuzzer();

            var chars = fuzzer.GenerateStringFromPattern("#").ToCharArray();

            Check.That(chars[0]).IsADigit();
        }

        [Test]
        [Repeat(1000)]
        public void Generate_random_string_from_pattern()
        {
            var fuzzer = new Fuzzer();

            var value = fuzzer.GenerateStringFromPattern("X#A02x"); // U3A02k
            TestContext.WriteLine(value);
            var chars = value.ToCharArray();

            Check.That(chars[0]).IsALetter();
            Check.That(char.IsUpper(chars[0])).IsTrue();

            Check.That(chars[1]).IsADigit();
            
            Check.That(chars[2]).IsEqualTo('A');
            Check.That(chars[3]).IsEqualTo('0');
            Check.That(chars[4]).IsEqualTo('2');

            Check.That(chars[5]).IsALetter();
            Check.That(char.IsUpper(chars[5])).IsFalse();
        }
    }
}