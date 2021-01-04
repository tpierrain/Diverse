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

            Check.That(firstNames).ContainsExactly("Bjorn", "Jean-Pierre", "Abhimanyu", "Cheng", "Jarmaine", "Akshay", "Milen", "Sékou", "Tuve", "Hwang");
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

            Check.That(firstNames).ContainsExactly("Nakeisha", "Guri", "Chan", "Chiara", "Kwame", "Valeria", "Dhalia", "Kione", "Xian", "Zayneb");
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

            Check.That(firstNames).ContainsExactly("Hei-Ran", "Uélé", "Katsuki", "Enrico", "Jung-sook", "Atahualpa", "Soske", "Manua", "Maffalda", "Aminata");
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

            Check.That(lastNames).ContainsExactly("Itxaro González", "Josh Walker", "Qiao Laghari", "Olamide Diop", "Kevin Sanchez", "Bao Madan", "Nichelle Baker", "Xing Xing Laghari", "Simba Ibrahim", "Abeba Nwadike");
        }

        [Test]
        [Ignore("To be restored once v1 is sealed.")]
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
                    "Mrs. Zahara NGOMA (Female) zngoma@yopmail.com (married - age: 20 years)", "Mx. Mohammed TESFAYE (NonBinary) mohammed.tesfaye@kolab.com (age: 24 years)", "Ms. Ella DAVIS (Female) ella.davis@protonmail.com (age: 27 years)", "Mx. Akmal KASONGO (NonBinary) akmal.kasongo@aol.com (age: 23 years)", "Mx. Shing KHATRI (NonBinary) shing.khatri@aol.com (married - age: 20 years)", "Ms. Jolanta BOTELHO (Female) jolanta.botelho@gmail.com (age: 23 years)", "Mx. Dzigbode DIOP (NonBinary) dzigbode.diop@gmail.com (age: 31 years)", "Mrs. Ishani LǏ (Female) ili@yahoo.fr (married - age: 24 years)", "Mrs. Gioconda GOMES (Female) gioconda.gomes@ibm.com (married - age: 19 years)", "Ms. Manutea FORSYTHE (Female) mforsythe@protonmail.com (age: 18 years)");
        }

        [Test]
        public void Be_able_to_generate_email_from_scratch()
        {
            var fuzzer = new Fuzzer(1996243347);

            var email = fuzzer.GenerateEMail();
            var email2 = fuzzer.GenerateEMail();
            var email3 = fuzzer.GenerateEMail();

            Check.That(email).IsEqualTo("sekou.ben-achour@yopmail.com");
            Check.That(email2).IsEqualTo("jdubois@kolab.com");
            Check.That(email3).IsEqualTo("maxime.hendricks@42skillz.com");
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
