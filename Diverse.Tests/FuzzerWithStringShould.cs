using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class FuzzerWithStringShould
    {
        [Test]
        public void Be_able_to_generate_a_random_adjective()
        {
            Fuzzer fuzzer = new Fuzzer(438709238);

            string randomString = fuzzer.GenerateString();

            Check.That(randomString).IsEqualTo("upbeat");
        }

        [Test]
        public void Be_able_to_generate_a_positive_adjective()
        {
            Fuzzer fuzzer = new Fuzzer(43930430);

            List<string> positiveAdjectives = new List<string>();

            for(int i = 0; i < 3; i++)
            {
                positiveAdjectives.Add(fuzzer.GenerateString(Feeling.Positive));
            }

            Check.That(positiveAdjectives).ContainsExactly("gifted", "sharp", "peaceful");
        }
    }
}