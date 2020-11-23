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
            var fuzzer = new Fuzzer(541936699);

            var persons = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                persons.Add(fuzzer.GenerateAPerson());
            }

            Check.That(persons.Select(x => x.ToString()))
                .ContainsExactly(
                    "Ms. Valeria DENILSON (Female) vdenilson@microsoft.com (is married: False - age: 62 yrs)", 
                    "Ms. Kirsten BREKKE (Female) kbrekke@gmail.com (is married: False - age: 76 yrs)", 
                    "Mr. John BRAND (Male) john.brand@gmail.com (is married: True - age: 86 yrs)", 
                    "Mx. Ashok KHATRI (NonBinary) ashok.khatri@yahoo.fr (is married: True - age: 32 yrs)", 
                    "Ms. Fatima SELASSIE (Female) fatima.selassie@aol.com (is married: False - age: 68 yrs)", 
                    "Mx. Demba ADOMAKO (NonBinary) demba.adomako@gmail.com (is married: False - age: 34 yrs)", 
                    "Mrs. Erika MADSEN (Female) emadsen@gmail.com (is married: True - age: 24 yrs)", 
                    "Ms. Antje JOHNSON (Female) antje.johnson@protonmail.com (is married: False - age: 37 yrs)", 
                    "Ms. Isabella AMBRÍZ (Female) isabella.ambriz@microsoft.com (is married: False - age: 60 yrs)", 
                    "Mr. Arjun YOON (Male) ayoon@42skillz.com (is married: False - age: 53 yrs)");
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
