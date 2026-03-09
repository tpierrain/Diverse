using System.Collections.Generic;
using System.Linq;
using Diverse.Address;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the primitives related to <see cref="Person"/>, Email etc.
    /// </summary>
    [TestFixture]
    public class PersonFuzzerShould
    {
        [Test]
        public void Be_able_to_Fuzz_FirstNames_for_male()
        {
            var fuzzer = new Fuzzer(381025586);

            var firstNames = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                firstNames.Add( fuzzer.GenerateFirstName(Gender.Male));
            }

            Check.That(firstNames).ContainsExactly("Jensson", "Alexei", "Ranjit", "Mu", "Ricardo", "Achille", "Mitt", "Djiby", "Shui", "Gemi");
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

            Check.That(firstNames).ContainsExactly("Zuri", "Fleur", "Esther", "Shayna", "Nyofu", "Chiquita", "Pepita", "Nimeesha", "Qiao", "Jamila");
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

            Check.That(firstNames).ContainsExactly("Nhung", "Mugisa", "Nataniel", "Jarmaine", "Diễm Hạnh", "Riley", "Mehmet", "Aroha", "Evelyn", "Isis");
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

            Check.That(lastNames).ContainsExactly("Femi Keita", "Kai NQuoc", "Kamini Laghari", "Sekou Diop", "Latrell Sanchez", "Yael Madan", "Deloise Baker", "Loc Laghari", "Olamide Ibrahim", "Aminata Nwadike");
        }

        [Test]
        public void Be_able_to_Fuzz_Diverse_Persons()
        {
            var fuzzer = new Fuzzer(722752479);

            var persons = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                persons.Add(fuzzer.GeneratePerson());
            }

            Check.That(persons.Select(x => x.ToString()))
                .ContainsExactly(
                    "Mrs. Amber NGOMA (Female) angoma@yopmail.com (married - age: 20 years)\n90 quai Claude-Bernard\n93400 - Saint-Ouen\nFrance",
                    "Mrs. Aissatou BADU (Female) abadu@kolab.com (married - age: 28 years)\n76 rue Tristan Tzara\n56170 - Quiberon\nFrance",
                    "Mx. Marija DUPUY (NonBinary) marija.dupuy@yopmail.com (age: 21 years)\n173 quai Claude-Bernard\n44405 - Nantes\nFrance",
                    "Mr. Obi ZIDANE (Male) ozidane@yopmail.com (age: 25 years)\n260 bd de la Somme\n75011 - Paris\nFrance",
                    "Ms. Laurence PETIT (Female) lpetit@gmail.com (age: 20 years)\n45 rue de la Poste\n93400 - Saint-Ouen\nFrance",
                    "Ms. Trini SOLBERG (Female) trini.solberg@ibm.com (age: 41 years)\n248 rue de la r\u00e9sistance\n38626 - Grenoble\nFrance",
                    "Mx. Malik MWANGI (NonBinary) mmwangi@kolab.com (married - age: 27 years)\n311 1st Avenue\n77009 - Houston\nUsa",
                    "Ms. Goo ACHARYA (Female) gacharya@ibm.com (age: 82 years)\n49 Changjiang Binjiang Road\uff0cYuzhong District, Yu Zhong\n674110 - Lijiang\nChina",
                    "Mrs. Audrey RIZZO (Female) audrey.rizzo@microsoft.com (married - age: 29 years)\n224 bd des Arceaux\n33200 - Bordeaux\nFrance",
                    "Mrs. R\u00e9jane FERN\u00c1NDEZ (Female) rfernandez@ibm.com (married - age: 23 years)\n241 quai des Orf\u00e8vres\n34069 - Montpellier\nFrance");
        }

        [Test]
        public void Be_able_to_generate_email_from_scratch()
        {
            var fuzzer = new Fuzzer(1996243347);

            var email = fuzzer.GenerateEMail();
            var email2 = fuzzer.GenerateEMail();
            var email3 = fuzzer.GenerateEMail();

            Check.That(email).IsEqualTo("okal.ben-achour@yopmail.com");
            Check.That(email2).IsEqualTo("adubois@kolab.com");
            Check.That(email3).IsEqualTo("lucca.hendricks@shodo.io");
        }

        [Test]
        public void Generate_Email_with_dashes_instead_of_spaces_and_pure_ascii_char()
        {
            var fuzzer = new Fuzzer(40816378);

            var email = fuzzer.GenerateEMail("Saïd Ef", "Ben Achour");

            Check.That(email).IsEqualTo("said-ef.ben-achour@microsoft.com");
        }

        [Test]
        [Repeat(10000)]
        public void Generate_Age_between_18_and_97_years()
        {
            var fuzzer = new Fuzzer();

            var age = fuzzer.GenerateAge();

            Check.That(age)
                .IsAfter(17)
                .And.IsBefore(98);
        }

        [Test]
        public void Generate_Address_with_french_format()
        {
            var fuzzer = new Fuzzer();

            var person = fuzzer.GeneratePerson();

            Check.That(person.Address.StreetNumber).IsInstanceOf<string>().And.IsNotEmpty();
            Check.That(person.Address.StreetName).IsNotEmpty();
            Check.That(person.Address.Street).IsNotEmpty();
            Check.That(person.Address.City).IsNotEmpty();
            Check.That(person.Address.CountryLabel).IsNotEmpty();
        }
    }
}
