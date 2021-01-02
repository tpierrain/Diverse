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

            Check.That(firstNames).ContainsExactly("Olle", "Wolfgang", "Yannick", "Jian", "Dwade", "Aadesh", "Boyko", "Sékou", "Hagan", "Kim");
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

            Check.That(firstNames).ContainsExactly("Nzinga", "Usha", "Wen", "Ulrike", "Zuri", "Camelia", "Valentina", "Ekundayo", "Mei", "Mériem");
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

            Check.That(firstNames).ContainsExactly("Dae", "Uélé", "Soske", "Arsene", "Ja-kyung", "João", "Fumihiro", "Manua", "Javiera", "Aminata");
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

            Check.That(lastNames).ContainsExactly("Racquel González", "Josh Walker", "Chen Laghari", "Olamide Diop", "Jacob Sanchez", "Li Mei Madan", "Julie Baker", "Shing Laghari", "Kimoni Ibrahim", "Abeba Nwadike");
        }

        [Test]
        public void Be_able_to_Fuzz_Diverse_Persons()
        {
            var fuzzer = new Fuzzer(541936699);

            var persons = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                persons.Add(fuzzer.GeneratePerson());
            }

            Check.That(persons.Select(x => x.ToString()))
                .ContainsExactly(
                    "Ms. Yanamarie DENILSON (Female) ydenilson@microsoft.com (age: 25 years)",
                    "Mx. Milen MURAZ (NonBinary) mmuraz@protonmail.com (age: 38 years)", 
                    "Mr. Pietro PETIT (Male) ppetit@yahoo.fr (married - age: 87 years)", 
                    "Mx. Jalil SELASSIE (NonBinary) jalil.selassie@aol.com (age: 19 years)", 
                    "Ms. Aida ADOMAKO (Female) aida.adomako@protonmail.com (age: 31 years)", 
                    "Mrs. Samarah SUZUKI (Female) samarah.suzuki@microsoft.com (married - age: 27 years)", 
                    "Mr. Yu Jie MADAN (Male) ymadan@ibm.com (married - age: 18 years)", 
                    "Mx. Mahavira CHANDRA (NonBinary) mchandra@42skillz.com (age: 19 years)", 
                    "Mr. Markus TUNE (Male) markus.tune@protonmail.com (married - age: 23 years)", 
                    "Mrs. Alexis BOONE (Female) alexis.boone@yopmail.com (married - age: 26 years)");
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
            Check.That(email3).IsEqualTo("julien.hendricks@42skillz.com");
        }

        [Test]
        public void Generate_Email_with_dashes_instead_of_spaces_and_pure_ascii_char()
        {
            var fuzzer = new Fuzzer(40816378);

            var email = fuzzer.GenerateEMail("Saïd Ef", "Ben Achour");

            Check.That(email).IsEqualTo("said-ef.ben-achour@microsoft.com");
        }
    }
}
