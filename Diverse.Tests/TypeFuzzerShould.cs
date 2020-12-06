using System;
using System.Collections.Generic;
using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class TypeFuzzerShould
    {
        [Test]
        public void Be_able_to_Fuzz_a_Type_aggregating_a_bunch_of_various_type_of_Properties()
        {
            var fuzzer = new Fuzzer(140481483);

            var instance = fuzzer.GenerateInstanceOf<PropertyOfAllTypes>();

            Check.That(instance.Name).IsNotEmpty();
            Check.That(instance.Age).IsInstanceOf<int>().Which.IsNotZero();
            Check.That(instance.Gender).IsNotEqualTo(default(Gender));
            Check.That(instance.FavoriteNumbers).HasSize(5).And.Not.Contains(0);
            Check.That(instance.Birthday).IsNotEqualTo(default(DateTime));
        }

        [Test]
        public void Be_able_to_Fuzz_enum_values()
        {
            var fuzzer = new Fuzzer(1277808677);
            var ingredient = fuzzer.GenerateEnum<Ingredient>();

            Check.ThatEnum(ingredient).IsEqualTo(Ingredient.Lettuce);

            fuzzer = new Fuzzer(805686996);
            var otherIngredient = fuzzer.GenerateEnum<Ingredient>();

            Check.ThatEnum(otherIngredient).IsEqualTo(Ingredient.Chocolate);
        }

        [Test]
        public void Be_able_to_Fuzz_a_Type_with_a_protected_constructor()
        {
            var fuzzer = new Fuzzer(23984398);

            var player = fuzzer.GenerateInstanceOf<PlayerWithProtectedConstructor>();

            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.That(player.Age).IsInstanceOf<int>().Which.IsNotEqualTo(0);
        }

        [Test]
        public void Be_able_to_Fuzz_a_Type_with_getters_only_and_some_public_constructors_with_a_protected_base_class_constructor_involved()
        {
            var fuzzer = new Fuzzer(953064492);

            var player = fuzzer.GenerateInstanceOf<ChessPlayerWithPublicConstructor>();

            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.ThatEnum(player.ChessLevel).IsEqualTo(ChessLevel.Expert);
            Check.That(player.Age).IsInstanceOf<int>().Which.IsNotEqualTo(0);
        }
        
        [Test]
        public void Be_able_to_Fuzz_the_Properties_of_a_Type_even_when_the_Constructor_is_empty_as_soon_as_they_have_a_public_or_a_private_Setter()
        {
            var fuzzer = new Fuzzer(345766738);

            var instance = fuzzer.GenerateInstanceOf<TypeWithPrivateEmptyConstructorOnly>();
            
            Check.That(instance.ModifiableSecret).IsNotEmpty(); // because we Fuzz properties with public setter
            Check.That(instance.FavoriteNumber).IsInstanceOf<int>().Which.IsNotEqualTo(0); // because we fuzz properties with private setter
        }

        [Test]
        public void Not_be_able_to_Fuzz_the_setterLess_properties_of_a_Type_when_the_Constructor_is_empty()
        {
            var fuzzer = new Fuzzer(345766738);

            var instance = fuzzer.GenerateInstanceOf<TypeWithPrivateEmptyConstructorOnly>();

            Check.That(instance.ConsultableSecret).IsNull(); // because we don't Fuzz properties with no setter
            Check.That(instance.Name).IsNull(); // because we don't fuzz properties with no setter
        }

        [Test]
        public void Be_able_to_Fuzz_an_enumerable_with_5_elements_when_Fuzzing_a_Type_containing_an_Enumerable_of_something()
        {
            var fuzzer = new Fuzzer(977324123);

            var player = fuzzer.GenerateInstanceOf<ChessPlayerWithPublicConstructor>();

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
        public void Be_able_to_GenerateInstanceOf_int()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<int>();
            Check.That(result).IsInstanceOf<int>().And.IsNotZero();
        }

        [Test]
        public void Be_able_to_GenerateInstanceOf_long()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<long>();
            Check.That(result).IsInstanceOf<long>().And.IsNotZero();
        }

        [Test]
        public void Be_able_to_GenerateInstanceOf_decimal()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<decimal>();
            Check.That(result).IsInstanceOf<decimal>().And.IsNotZero();
        }

        [Test]
        public void Be_able_to_GenerateInstanceOf_IEnumerable_of_int()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<IEnumerable<int>>();
            Check.That(result).HasSize(5);
        }
        
        [Test]
        public void Be_able_to_GenerateInstanceOf_DateTime()
        {
            var fuzzer = new Fuzzer();
            
            var result = fuzzer.GenerateInstanceOf<DateTime>();
            Check.That(result).IsNotEqualTo(default(DateTime));
        }

        [Test]
        public void Be_able_to_GenerateInstanceOf_Bool()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<bool>();
            Check.That(result).IsInstanceOf<bool>();
        }

        [Test]
        [Ignore("Because it timeouts NCrunch for the moment due to excessive recursion. Time to find another solution than recursion.")]
        public void Be_able_to_Fuzz_Self_Referencing_Types_Aggregating_a_collection_of_the_same_Type()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            Check.That(result.Friends).HasSize(5);
        }
    }
}