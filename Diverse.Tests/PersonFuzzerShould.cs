using System.Collections.Generic;
using System.Linq;
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

            Check.That(firstNames).ContainsExactly("Nzinga", "Guri", "Lan", "Giovanna", "Kwame", "Valeria", "Dhalia", "Kione", "Xian", "Zayneb");
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

            Check.That(firstNames).ContainsExactly("Goo", "Uélé", "Katsuki", "Enrico", "Jung-sook", "Atahualpa", "Soske", "Manua", "Maffalda", "Aminata");
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

            Check.That(lastNames).ContainsExactly("Itxaro González", "Josh Walker", "Yue Laghari", "Olamide Diop", "Kevin Sanchez", "Bao Madan", "Nichelle Baker", "Xing Xing Laghari", "Simba Ibrahim", "Abeba Nwadike");
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
                    "Mrs. Zahara NGOMA (Female) zngoma@yopmail.com (married - age: 20 years)", 
                    "Ms. Brendette HAUGLAND (Female) bhaugland@protonmail.com (age: 33 years)", 
                    "Ms. Imogen WILLIAMS (Female) imogen.williams@protonmail.com (age: 41 years)", 
                    "Mrs. Khadija DROGBA (Female) khadija.drogba@protonmail.com (married - age: 31 years)", 
                    "Mr. Samuel MÜLLER (Male) smuller@kolab.com (age: 27 years)", 
                    "Mx. Gojko BERNARD (NonBinary) gojko.bernard@ibm.com (age: 34 years)", 
                    "Ms. Zhang KIM (Female) zkim@kolab.com (age: 75 years)", 
                    "Mr. Samuel CONTI (Male) samuel.conti@42skillz.com (married - age: 25 years)", 
                    "Mrs. Eun-Kyung BAK (Female) eun-kyung.bak@ibm.com (married - age: 32 years)", 
                    "Mx. Okal MENSAH (NonBinary) omensah@yopmail.com (married - age: 28 years)");
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
    }
}
