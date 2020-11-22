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
    public class FuzzerWithPersonsShould
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

            Check.That(firstNames).ContainsExactly("Einar", "Rajiv", "Abraham", "Olle", "George", "Elias", "Anton", "Djibril", "Gjurd", "Jin");
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

            Check.That(firstNames).ContainsExactly("Malika", "Akshara", "My Lê", "Eve", "Nzinga", "Ella", "Zoe", "Nichelle", "Mây", "Noûr");
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

            Check.That(firstNames).ContainsExactly("Kyeol", "Koffi", "Ba-Thong", "Mikhail", "Johanna", "Diego", "Zhen", "Aata", "Ximena", "Jemila");
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

            Check.That(lastNames).ContainsExactly("Beatriz González", "Drell Chen", "Mei Laghari", "Mamadou Diop", "David Sanchez", "Xuân Madan", "Tanja Brandolini", "Bao Laghari", "Demba Ibrahim", "Jemila Nwadike");
        }

        [Test]
        public void Be_able_to_Fuzz_Diverse_Persons()
        {
            var fuzzer = new Fuzzer(1320843396);
            //var fuzzer = new Fuzzer();

            var persons = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                persons.Add(fuzzer.GenerateAPerson());
            }

            Check.That(persons.Select(x => x.ToString()))
                .ContainsExactly("Ms. Fatima BA (Female) fba@gmail.com (is married: False - age: 93 yrs)", 
                    "Mrs. Sofie MOREAU (Female) smoreau@microsoft.com (is married: True - age: 26 yrs)", 
                    "Mx. Jeremie MATEUDI (NonBinary) jeremie.mateudi@yopmail.com (is married: True - age: 66 yrs)", 
                    "Ms. Marija DUPONT (Female) marija.dupont@yahoo.fr (is married: False - age: 34 yrs)", 
                    "Mrs. Mériem MWANGI (Female) meriem.mwangi@yopmail.com (is married: True - age: 83 yrs)", 
                    "Mrs. Esther CHAKRABARTI (Female) esther.chakrabarti@ibm.com (is married: True - age: 62 yrs)", 
                    "Mr. Javier MUÑOZ (Male) jmunoz@gmail.com (is married: False - age: 66 yrs)", 
                    "Mx. Alejandro QUISPE (NonBinary) alejandro.quispe@yahoo.fr (is married: False - age: 76 yrs)", 
                    "Ms. Francesca ARELLANO (Female) farellano@ibm.com (is married: False - age: 85 yrs)", 
                    "Ms. Ji-yeon WANG (Female) jwang@42skillz.com (is married: False - age: 43 yrs)");
        }

        [Test]
        public void Be_able_to_generate_email_from_scratch()
        {
            var fuzzer = new Fuzzer(1996243347);

            var email = fuzzer.GenerateEMail();
            var email2 = fuzzer.GenerateEMail();
            var email3 = fuzzer.GenerateEMail();

            Check.That(email).IsEqualTo("atef.ben-achour@yopmail.com");
            Check.That(email2).IsEqualTo("abak@protonmail.com");
            Check.That(email3).IsEqualTo("wolfgang.hendricks@gmail.com");
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
