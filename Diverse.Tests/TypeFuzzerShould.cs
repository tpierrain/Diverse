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

        [Test]
        public void Be_able_to_Fuzz_an_enumerable_of_5_elements_when_fuzzing_a_Type_containing_an_Enumerable_of_something()
        {
            var fuzzer = new Fuzzer(977324123);

            var player = fuzzer.GenerateInstanceOf<ChessPlayer>();

            var aggregatedFavOpponent = player.FavoriteOpponent;
            Check.That(aggregatedFavOpponent).IsNotNull();
            Check.That(aggregatedFavOpponent.LastName).IsNotEmpty();
            Check.That(aggregatedFavOpponent.FirstName).IsNotEmpty();
            Check.That(aggregatedFavOpponent.FirstName).IsNotEmpty();
            Check.That(aggregatedFavOpponent.CurrentClub.Name).IsEqualTo("Aylan");
            Check.ThatEnum(aggregatedFavOpponent.CurrentClub.Country).IsEqualTo(Country.Ukraine);

            Check.That(player.FormerClubs).HasSize(5);
        }

        [Test]
        public void Fuzz_via_Properties_when_the_Constructor_is_empty_and_public_or_private_Setters_exist()
        {
            var fuzzer = new Fuzzer(345766738);

            var instance = fuzzer.GenerateInstanceOf<TypeWithoutConstructor>();
            
            Check.That(instance.ModifiableSecret).IsNotEmpty(); // because we Fuzz properties with public setter
            Check.That(instance.FavoriteNumber).IsNotEqualTo(0); // because we fuzz properties with private setter
        }

        [Test]
        public void Not_Fuzz_via_Properties_when_the_Constructor_is_empty_and_public_or_private_Setters_DO_NOT_not_exist()
        {
            var fuzzer = new Fuzzer(345766738);

            var instance = fuzzer.GenerateInstanceOf<TypeWithoutConstructor>();

            Check.That(instance.ConsultableSecret).IsNull(); // because we don't Fuzz properties with no setter
            Check.That(instance.Name).IsNull(); // because we don't fuzz properties with no setter
        }
    }
}