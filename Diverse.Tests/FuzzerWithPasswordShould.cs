using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the primitives related to password generation.
    /// </summary>
    [TestFixture]
    public class FuzzerWithPasswordShould
    {
        [TestCase(500)]
        public void GeneratePasswords_of_various_sizes(int attempts)
        {
            var fuzzer = new Fuzzer();

            var generatedPasswords = new List<string>();
            for (var i = 0; i < attempts; i++)
            {
                generatedPasswords.Add(fuzzer.GeneratePassword());
            }

            var generatedSizes = generatedPasswords.Select(p => p.Length).Distinct();
            Check.That(generatedSizes.Count()).IsStrictlyGreaterThan(2);
        }

        [TestCase(12, null)]
        [TestCase(7, null)]
        [TestCase(0, 1)]
        [TestCase(9, 13)]
        [TestCase(3, 24)]
        [TestCase(8, 12)]
        [TestCase(7, 16)]
        [TestCase(null, 8)]
        [TestCase(8, 15)]
        public void GeneratePasswords_of_various_sizes_respecting_the_min_and_max_size_specified(int? minSize, int? maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                var password = fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize);
                Check.That(password.Length >= minSize && password.Length <= maxSize);
            }
        }

        [TestCase(null, 7)]
        [TestCase(null, 8)]
        [TestCase(null, 12)]
        [TestCase(null, 18)]
        public void Take_7_as_minimum_size_when_GeneratePasswords_without_specifying_a_minSize(int? minSize, int? maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                var password = fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize);
                Check.That(password.Length <= 7);
            }
        }

        [TestCase(1, null)]
        [TestCase(12, null)]
        [TestCase(null, null)]
        public void Take_12_as_maximum_size_when_GeneratePasswords_without_specifying_a_maxSize(int? minSize, int? maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                var password = fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize);
                Check.That(password.Length <= 7);
            }
        }

        [TestCase(2, 1)]
        [TestCase(8, 7)]
        [TestCase(null, 3 /*default minSize is 7 */)]
        [TestCase(null, 6 /*default minSize is 7 */)]
        public void Throw_ArgumentOutOfRangeException_when_calling_GeneratePassword_specifying_a_maxSize_inferior_to_the_minSize(int? minSize, int? maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                Check.ThatCode(() => fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize))
                    .Throws<ArgumentOutOfRangeException>();
            }
        }

        [TestCase(2, 1)]
        [TestCase(7, 3)]
        [TestCase(8, 7)]
        [TestCase(13, null /*default maxSize is 12 */)]
        public void Throw_ArgumentOutOfRangeException_when_calling_GeneratePassword_specifying_a_minSize_superior_to_the_maxSize(int? minSize, int? maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                Check.ThatCode(() => fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize))
                    .Throws<ArgumentOutOfRangeException>();
            }
        }

        [TestCase(null, 1)]
        [TestCase(null, 2)]
        [TestCase(null, 3)]
        [TestCase(null, 4)]
        [TestCase(null, 5)]
        [TestCase(null, 6)]
        [TestCase(null, 7)]
        [TestCase(null, 8)]

        public void Downgrade_the_minSize_to_at_least_the_specified_maxSize_when_specifying_a_maxSize_below_the_default_minSize_of_7(int minSize, int maxSize)
        {
            var fuzzer = new Fuzzer();

            for (var i = 0; i < 200; i++)
            {
                var password = fuzzer.GeneratePassword(minSize: minSize, maxSize: maxSize);
                Check.That(password.Length <= maxSize);
            }
        }

        [Test]
        public void Contains_only_alphanumerical_characters_by_default()
        {
            var fuzzer = new Fuzzer();

            var password = fuzzer.GeneratePassword();
            
            Check.That(password).Not.Matches("[^A-Za-z0-9]+");
        }

        [Test]
        public void Contains_at_least_a_special_character()
        {
            var fuzzer = new Fuzzer();

            var password = fuzzer.GeneratePassword(includeSpecialCharacters: true);
            
            Check.That(password).Matches("[^A-Za-z0-9]+");
        }

        [Test]
        public void Contains_at_least_a_lowercase_character()
        {
            var fuzzer = new Fuzzer();

            var password = fuzzer.GeneratePassword(includeSpecialCharacters: true);
            
            Check.That(password).Matches("[a-z]+");
        }

        [Test]
        public void Contains_at_least_an_uppercase_character()
        {
            var fuzzer = new Fuzzer();

            var password = fuzzer.GeneratePassword(includeSpecialCharacters: true);
            
            Check.That(password).Matches("[A-Z]+");
        }

        [Test]
        public void Contains_at_least_a_number()
        {
            var fuzzer = new Fuzzer();

            var password = fuzzer.GeneratePassword(includeSpecialCharacters: true);
            
            Check.That(password).Matches("[0-9]+");
        }
    }
}