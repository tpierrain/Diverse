using System.Collections.Generic;
using System.Linq;
using Diverse.Strings;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class ParameterNameMatcherShould
    {
        private static readonly string[] AllFirstNames =
            Male.FirstNames.Concat(Female.FirstNames).ToArray();

        private static readonly string[] AllLastNames =
            LastNames.PerContinent.Values.SelectMany(names => names).ToArray();

        private static readonly string[] AllEmailDomains =
            Tech.EmailDomainNames;

        private static readonly string[] AllLatinWords =
            Latin.Words;

        [Test]
        public void Match_firstName_to_a_known_first_name_from_the_lib()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = (string)matcher.TryGenerateFromName("firstName", typeof(string), context);

            Check.That(result).IsOneOf(AllFirstNames);
        }

        [Test]
        public void Match_lastName_to_a_known_last_name_coherent_with_firstName_continent()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var firstName = (string)matcher.TryGenerateFromName("firstName", typeof(string), context);
            context.Store("firstName", firstName);

            var lastName = (string)matcher.TryGenerateFromName("lastName", typeof(string), context);

            Check.That(lastName).IsOneOf(AllLastNames);

            // Verify continent coherence: the lastName belongs to the same continent as the firstName
            var firstNameContinent = Locations.FindContinent(firstName);
            var lastNamesForContinent = LastNames.PerContinent[firstNameContinent];
            Check.That(lastName).IsOneOf(lastNamesForContinent);
        }

        [Test]
        public void Match_lastName_to_a_known_last_name_even_without_firstName_in_context()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var lastName = (string)matcher.TryGenerateFromName("lastName", typeof(string), context);

            Check.That(lastName).IsOneOf(AllLastNames);
        }

        [Test]
        public void Match_email_to_a_valid_email_using_known_domain()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var email = (string)matcher.TryGenerateFromName("email", typeof(string), context);

            Check.That(email).IsNotNull().And.Contains("@");
            var domain = email.Split('@')[1];
            Check.That(domain).IsOneOf(AllEmailDomains);
        }

        [Test]
        public void Match_customerEmail_to_a_valid_email_using_known_domain()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var email = (string)matcher.TryGenerateFromName("customerEmail", typeof(string), context);

            Check.That(email).IsNotNull().And.Contains("@");
            var domain = email.Split('@')[1];
            Check.That(domain).IsOneOf(AllEmailDomains);
        }

        [Test]
        public void Match_password_to_a_string_with_valid_length_range()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var password = (string)matcher.TryGenerateFromName("password", typeof(string), context);

            Check.That(password).IsNotNull();
            // GeneratePassword uses default minSize=7, maxSize=12
            Check.That(password.Length).IsStrictlyGreaterThan(6).And.IsStrictlyLessThan(13);
        }

        [Test]
        public void Match_age_as_int_to_a_realistic_age()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("age", typeof(int), context);

            Check.That(result).IsNotNull().And.IsInstanceOf<int>();
            var age = (int)result;
            // GenerateAge produces values between 18 and 97
            Check.That(age).IsStrictlyGreaterThan(17).And.IsStrictlyLessThan(98);
        }

        [Test]
        public void Match_description_to_a_sentence_made_of_known_latin_words()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var sentence = (string)matcher.TryGenerateFromName("description", typeof(string), context);

            Check.That(sentence).IsNotNull();
            // GenerateSentence capitalizes the first word and adds a trailing period
            CheckThatAllWordsAreKnownLatinWords(sentence);
        }

        [Test]
        public void Match_text_to_a_sentence_made_of_known_latin_words()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var sentence = (string)matcher.TryGenerateFromName("text", typeof(string), context);

            Check.That(sentence).IsNotNull();
            CheckThatAllWordsAreKnownLatinWords(sentence);
        }

        [Test]
        public void Match_price_as_decimal_to_a_bounded_positive_value()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("price", typeof(decimal), context);

            Check.That(result).IsNotNull().And.IsInstanceOf<decimal>();
            var price = (decimal)result;
            Check.That(price).IsStrictlyGreaterThan(0m).And.IsStrictlyLessThan(10000m);
        }

        [Test]
        public void Match_totalAmount_as_decimal_to_a_bounded_positive_value()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("totalAmount", typeof(decimal), context);

            Check.That(result).IsNotNull().And.IsInstanceOf<decimal>();
            var amount = (decimal)result;
            Check.That(amount).IsStrictlyGreaterThan(0m).And.IsStrictlyLessThan(10000m);
        }

        [Test]
        public void Not_match_age_when_type_is_string()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("age", typeof(string), context);

            Check.That(result).IsNull();
        }

        [Test]
        public void Not_match_price_when_type_is_string()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("price", typeof(string), context);

            Check.That(result).IsNull();
        }

        [Test]
        public void Be_case_insensitive_and_return_a_known_first_name()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = (string)matcher.TryGenerateFromName("FirstName", typeof(string), context);

            Check.That(result).IsOneOf(AllFirstNames);
        }

        [Test]
        public void Match_composed_names_like_playerName_to_a_known_first_name()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = (string)matcher.TryGenerateFromName("playerName", typeof(string), context);

            Check.That(result).IsOneOf(AllFirstNames);
        }

        [Test]
        public void Return_null_when_no_pattern_matches()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("foo", typeof(string), context);

            Check.That(result).IsNull();
        }

        [Test]
        public void Return_null_for_unknown_int_parameter_names()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName("count", typeof(int), context);

            Check.That(result).IsNull();
        }

        [Test]
        public void Return_null_for_null_name()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = matcher.TryGenerateFromName(null, typeof(string), context);

            Check.That(result).IsNull();
        }

        [Test]
        public void Match_surname_to_a_known_last_name()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var result = (string)matcher.TryGenerateFromName("surname", typeof(string), context);

            Check.That(result).IsOneOf(AllLastNames);
        }

        [Test]
        public void Match_pwd_to_a_string_with_valid_length_range()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var password = (string)matcher.TryGenerateFromName("pwd", typeof(string), context);

            Check.That(password).IsNotNull();
            Check.That(password.Length).IsStrictlyGreaterThan(6).And.IsStrictlyLessThan(13);
        }

        [Test]
        public void Match_title_to_a_sentence_made_of_known_latin_words()
        {
            var fuzzer = new Fuzzer();
            var matcher = new ParameterNameMatcher(fuzzer);
            var context = new ConstructorFuzzingContext();

            var sentence = (string)matcher.TryGenerateFromName("title", typeof(string), context);

            Check.That(sentence).IsNotNull();
            CheckThatAllWordsAreKnownLatinWords(sentence);
        }

        /// <summary>
        /// Verifies that every word in a generated sentence comes from <see cref="Latin.Words"/>.
        /// GenerateSentence capitalizes the first word and appends a trailing period to the last one.
        /// </summary>
        private static void CheckThatAllWordsAreKnownLatinWords(string sentence)
        {
            var words = sentence.Split(' ');
            Check.That(words.Length).IsStrictlyGreaterThan(1);

            foreach (var word in words)
            {
                var normalized = word.TrimEnd('.').ToLowerInvariant();
                Check.That(normalized).IsOneOf(AllLatinWords);
            }
        }
    }
}
