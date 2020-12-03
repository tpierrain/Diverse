using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class FuzzerWithTypesShould
    {
        [Test]
        public void Be_able_to_generate_random_enum_values()
        {
            var fuzzer = new Fuzzer(1277808677);
            var ingredient = fuzzer.GenerateEnum<Ingredient>();

            Check.ThatEnum(ingredient).IsEqualTo(Ingredient.Lettuce);

            fuzzer = new Fuzzer(805686996);
            var otherIngredient = fuzzer.GenerateEnum<Ingredient>();

            Check.ThatEnum(otherIngredient).IsEqualTo(Ingredient.Chocolate);
        }

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