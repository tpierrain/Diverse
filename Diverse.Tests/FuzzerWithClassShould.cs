using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class FuzzerWithClassShould
    {
        [Test]
        public void Be_able_to_generate_a_diverse_firstname_for_any_object_containing_a_firstname_property()
        {
            var fuzzer = new Fuzzer(23984398);

            var dummyClass = fuzzer.GenerateInstance<Player>();

            Check.That(dummyClass.FirstName).IsEqualTo("Kevin");
        }

        [Test]
        public void Be_able_to_generate_a_diverse_lastname_for_any_object_containing_a_lastname_property()
        {
            var fuzzer = new Fuzzer(23984398);

            var dummyClass = fuzzer.GenerateInstance<Player>();

            Check.That(dummyClass.LastName).IsEqualTo("Fantasia");
        }
    }

    internal class Player
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}