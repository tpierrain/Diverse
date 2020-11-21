using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// Note: this test fixture has lot of tests using a specific seed provided to the Fuzzer instance.
    /// This is not representative on how to use <see cref="Fuzzer"/> instances in your code
    /// base (i.e. without fixing a seed in order to go full random), but made
    /// for deterministic results instead.
    /// </summary>
    [TestFixture]
    public class FuzzerShould
    {
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
        public void Be_able_to_Fuzz_FirstNames_for_male()
        {
            var fuzzer = new Fuzzer(381025586);

            var firstNames = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                firstNames.Add( fuzzer.GenerateFirstName(Gender.Male));
            }

            Check.That(firstNames).ContainsExactly("Khaled", "Lukas", "Aylan", "Espen", "Jerry", "Mehmet", "Alexei", "Djibril", "Enok", "Hoà");
        }

        [Test]
        public void Be_able_to_Fuzz_FirstNames_for_female()
        {
            var fuzzer = new Fuzzer(1834164083);

            var firstNames = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                firstNames.Add(fuzzer.GenerateFirstName(Gender.Female));
            }

            Check.That(firstNames).ContainsExactly("Keisha", "Jung-sook", "Liv", "Lin", "Laila", "Marie", "Christine", "Makeba", "Mette", "Nichelle");
        }

        [Test]
        public void Be_able_to_Fuzz_FirstNames_for_every_genders()
        {
            var fuzzer = new Fuzzer(1340516487);

            var firstNames = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                firstNames.Add(fuzzer.GenerateFirstName());
            }

            Check.That(firstNames).ContainsExactly("Zayneb", "Koffi", "Jian", "David", "Jasmine", "Héctor", "Vinh", "Aata", "Noëlla", "Bintu");
        }

        [Test]
        public void Be_able_to_Fuzz_LastName_for_every_FirstName()
        {
            var fuzzer = new Fuzzer(2000210944);

            var lastNames = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var firstName = fuzzer.GenerateFirstName();
                lastNames.Add($"{firstName} {fuzzer.GenerateLastName(firstName)}");
            }

            Check.That(lastNames).ContainsExactly("Maëla Di Maria", "Drell Thompson", "Ida Johnsen", "Demba Belkhodja", "Liam Brown", "Maja Eriksen", "Li Wú", "Oluf Johnsen", "Mahdi Smah", "Bintu Kasongo");
        }

        [Test]
        public void Be_able_to_Fuzz_Persons()
        {
            var fuzzer = new Fuzzer(1320843396);

            var persons = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                persons.Add(fuzzer.GenerateAPerson());
            }

            Check.That(persons.Select(x => x.ToString()))
                .ContainsExactly(
                    "Ms. Nakeisha BEN ACHOUR (Female) nben-achour@kolab.com (is married: False - age: 92 yrs)", 
                    "Mrs. Bayla CHÉN (Female) bchen@aol.com (is married: True - age: 26 yrs)", 
                    "Mx. Julien SCHNEIDER (NonBinary) julien.schneider@aol.com (is married: True - age: 65 yrs)", 
                    "Ms. Chan HAN (Female) chan.han@louvre-hotels.com (is married: False - age: 34 yrs)", 
                    "Mrs. Nora BEN AYED (Female) nora.ben-ayed@aol.com (is married: True - age: 82 yrs)", 
                    "Mrs. Ja-kyung CHÉN (Female) ja-kyung.chen@yopmail.com (is married: True - age: 61 yrs)", 
                    "Mr. Sergio RODRÍGUEZ (Male) srodriguez@yahoo.fr (is married: False - age: 65 yrs)", 
                    "Mx. Esteban ROJAS (NonBinary) esteban.rojas@louvre-hotels.com (is married: False - age: 75 yrs)", 
                    "Ms. Eve MÜLLER (Female) emuller@microsoft.com (is married: False - age: 85 yrs)", 
                    "Ms. Solveig IVERSEN (Female) siversen@ibm.com (is married: False - age: 43 yrs)");
        }

        [Test]
        public void Be_able_to_generate_email_from_scratch()
        {
            var fuzzer = new Fuzzer(1996243347);

            var email = fuzzer.GenerateEMail();
            var email2 = fuzzer.GenerateEMail();
            var email3 = fuzzer.GenerateEMail();

            Check.That(email).IsEqualTo("mamadou.chedjou@aol.com");
            Check.That(email2).IsEqualTo("eesposito@gmail.com");
            Check.That(email3).IsEqualTo("dimitri.dupuy@yopmail.com");
        }

        [Test]
        public void Generate_Email_with_dashes_instead_of_spaces_and_pure_ascii_char()
        {
            var fuzzer = new Fuzzer(40816378);

            var email = fuzzer.GenerateEMail("Saïd Ef", "Ben Achour");

            Check.That(email).IsEqualTo("said-ef.ben-achour@kolab.com");
        }
    }

}
