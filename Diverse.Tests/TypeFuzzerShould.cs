using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class TypeFuzzerShould
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
        public void Be_able_to_generate_diverse_values_for_every_property_of_an_object_with_getters_only_and_protected_base_class_constructor_involved()
        {
            var fuzzer = new Fuzzer(953064492);

            var player = fuzzer.GenerateInstanceOf<ChessPlayer>();

            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.ThatEnum(player.ChessLevel).IsEqualTo(ChessLevel.Expert);
            Check.That(player.Age).IsInstanceOf<int>().Which.IsNotEqualTo(0);
        }

        [Test]
        public void Be_able_to_generate_diverse_values_for_every_property_of_an_object_with_protected_constructor()
        {
            var fuzzer = new Fuzzer(23984398);

            var player = fuzzer.GenerateInstanceOf<Player>();

            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.That(player.Age).IsInstanceOf<int>().Which.IsNotEqualTo(0);
        }
    }
}