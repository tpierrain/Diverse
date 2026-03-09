using System;
using System.Collections.Generic;
using System.Linq;
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
            var fuzzer = new Fuzzer();

            var instance = fuzzer.GenerateInstanceOf<PropertyOfAllTypes>();

            Check.That(instance).IsNotNull();
            Check.That(instance.Name).IsNotEmpty();
            Check.That(instance.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
            Check.That(instance.Gender).IsInstanceOf<Gender>();
            Check.That(instance.FavoriteNumbers).HasSize(5);
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
            var fuzzer = new Fuzzer();

            var player = fuzzer.GenerateInstanceOf<PlayerWithProtectedConstructor>();

            Check.That(player).IsNotNull();
            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.That(player.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
        }

        [Test]
        public void Be_able_to_Fuzz_a_Type_with_getters_only_and_some_public_constructors_with_a_protected_base_class_constructor_involved()
        {
            var fuzzer = new Fuzzer();

            var player = fuzzer.GenerateInstanceOf<ChessPlayerWithPublicConstructor>();

            Check.That(player).IsNotNull();
            Check.That(player.LastName).IsNotEmpty();
            Check.That(player.FirstName).IsNotEmpty();
            Check.That(player.ChessLevel).IsInstanceOf<ChessLevel>();
            Check.That(player.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
        }

        [Test]
        public void Be_able_to_Fuzz_the_Properties_of_a_Type_even_when_the_Constructor_is_empty_as_soon_as_they_have_a_public_or_a_private_Setter()
        {
            var fuzzer = new Fuzzer();

            var instance = fuzzer.GenerateInstanceOf<TypeWithPrivateEmptyConstructorOnly>();

            Check.That(instance.ModifiableSecret).IsNotEmpty(); // because we Fuzz properties with public setter
            Check.That(instance.FavoriteNumber).IsInstanceOf<int>().Which.IsNotEqualTo(0); // because we fuzz properties with private setter
        }

        [Test]
        public void Not_be_able_to_Fuzz_the_setterLess_properties_of_a_Type_when_the_Constructor_is_empty()
        {
            var fuzzer = new Fuzzer();

            var instance = fuzzer.GenerateInstanceOf<TypeWithPrivateEmptyConstructorOnly>();

            Check.That(instance.ConsultableSecret).IsNull(); // because we don't Fuzz properties with no setter
            Check.That(instance.Name).IsNull(); // because we don't fuzz properties with no setter
        }

        [Test]
        public void Be_able_to_Fuzz_an_aggregated_complex_type_and_a_collection_of_complex_types()
        {
            var fuzzer = new Fuzzer();

            var player = fuzzer.GenerateInstanceOf<ChessPlayerWithPublicConstructor>();

            var aggregatedFavOpponent = player.FavoriteOpponent;
            Check.That(aggregatedFavOpponent).IsNotNull();
            Check.That(aggregatedFavOpponent.LastName).IsNotEmpty();
            Check.That(aggregatedFavOpponent.FirstName).IsNotEmpty();
            Check.That(aggregatedFavOpponent.CurrentClub).IsNotNull();
            Check.That(aggregatedFavOpponent.CurrentClub.Name).IsNotEmpty();

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
        public void Generate_semantically_coherent_UserProfile_with_email_age_and_password()
        {
            var fuzzer = new Fuzzer();

            var user = fuzzer.GenerateInstanceOf<UserProfile>();

            Check.That(user).IsNotNull();
            Check.That(user.FirstName).IsNotEmpty();
            Check.That(user.LastName).IsNotEmpty();
            Check.That(user.Email).Contains("@");
            Check.That(user.Password).IsNotEmpty();
            Check.That(user.Password.Length).IsStrictlyGreaterThan(5);
            Check.That(user.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
        }

        [Test]
        public void Generate_semantically_coherent_OrderWithNamedParameters()
        {
            var fuzzer = new Fuzzer();

            var order = fuzzer.GenerateInstanceOf<OrderWithNamedParameters>();

            Check.That(order).IsNotNull();
            Check.That(order.Description).Contains(" "); // sentence with multiple words
            Check.That(order.CustomerEmail).Contains("@");
            Check.That(order.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
            Check.That(order.Price).IsStrictlyGreaterThan(0m).And.IsStrictlyLessThan(10000m);
        }

        [Test]
        public void Generate_semantically_coherent_properties_via_setters()
        {
            var fuzzer = new Fuzzer();

            var instance = fuzzer.GenerateInstanceOf<TypeWithSetterBasedNamedProperties>();

            Check.That(instance).IsNotNull();
            Check.That(instance.FirstName).IsNotEmpty();
            Check.That(instance.LastName).IsNotEmpty();
            Check.That(instance.Email).Contains("@");
            Check.That(instance.Description).Contains(" ");
            Check.That(instance.Age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
            Check.That(instance.TotalAmount).IsStrictlyGreaterThan(0m).And.IsStrictlyLessThan(10000m);
        }

        [Test]
        public void Be_able_to_Fuzz_Self_Referencing_Types_Aggregating_a_collection_of_the_same_Type()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Birthday).IsNotEqualTo(default(DateTime));
            // Friends collection should be non-empty (reduced size due to cycle detection, but not zero at top level)
            Check.That(result.Friends).IsNotNull();
            var friendsList = result.Friends.ToList();
            Check.That(friendsList).Not.IsEmpty();
            // Each friend should have an empty Friends collection (cycle detected at depth 2)
            foreach (var friend in friendsList)
            {
                Check.That(friend).IsNotNull();
                Check.That(friend.Name).IsNotEmpty();
                Check.That(friend.Friends.ToList()).IsEmpty();
            }
        }
    }
}
