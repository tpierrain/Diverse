using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Diverse
{
    /// <summary>
    /// Allows to generate lots of combination of things. <see cref="Fuzzer"/> are very useful to detect hard coded values in our implementations.
    /// Note: you can instantiate another Deterministic Fuzzer by providing it the Seed you want to reuse.
    /// </summary>
    public class Fuzzer : IFuzz
    {
        private Random _internalRandom;

        /// <summary>
        /// Generates a DefaultSeed. Important to keep a trace of the used seed so that we can reproduce a failing situation with <see cref="Fuzzer"/> involved.
        /// </summary>
        public int Seed { get; }
        public string Name { get; }

        /// <summary>
        /// Gets the Random instance to be used when we want to create a new extension method for the <see cref="Fuzzer"/>.
        /// <remarks>The use of explicit interface implementation for this property is made on purpose in order to hide this internal mechanic details from the Fuzzer end-user code.</remarks>
        /// </summary>
        Random IFuzz.Random => _internalRandom;

        /// <summary>
        /// Gives easy access to the <see cref="IFuzz.Random"/> explicit implementation.
        /// </summary>
        private Random InternalRandom => ((IFuzz)this).Random;

        /// <summary>
        /// Sets the way you want to log or receive what the <see cref="Fuzzer"/> has to say about every generated seeds used for every fuzzer instance & test.
        /// </summary>
        public static Action<string> Log { get; set; }

        /// <summary>
        /// Instantiates a <see cref="Fuzzer"/>.
        /// </summary>
        /// <param name="seed">The seed if you want to reuse in order to reproduce the very same conditions of another (failing) test.</param>
        /// <param name="name">The name you want to specify for this <see cref="Fuzzer"/> instance (useful for debuging purpose).</param>
        public Fuzzer(int? seed = null, string name = null)
        {
            var seedWasProvided = seed.HasValue;

            seed = seed ?? new Random().Next(); // the seed is not specified? pick a random one for this Fuzzer instance.
            Seed = seed.Value;

            _internalRandom = new Random(seed.Value);

            name = name ?? GenerateFuzzerName();
            Name = name;

            LogSeedAndTestInformations(seed.Value, seedWasProvided, name);
        }

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger(int minValue, int maxValue)
        {
            return InternalRandom.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random integer value.
        /// </summary>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger()
        {
            return GenerateInteger(int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <returns>A positive integer value generated randomly.</returns>
        public int GeneratePositiveInteger()
        {
            return GenerateInteger(0, int.MaxValue);
        }

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        public decimal GeneratePositiveDecimal()
        {
            return Convert.ToDecimal(GenerateInteger(0, int.MaxValue));
        }

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="gender">The <see cref="Gender"/> to be used as indication (optional).</param>
        /// <returns>A 'Diverse' first name.</returns>
        public string GenerateFirstName(Gender? gender = null)
        {
            string[] firstNameCandidates;
            if (gender == null)
            {
                var isFemale = HeadsOrTails();
                firstNameCandidates = isFemale ? Female.FirstNames : Male.FirstNames;
            }
            else
            {
                firstNameCandidates = gender == Gender.Female ? Female.FirstNames : Male.FirstNames;
            }

            var randomLocaleIndex = InternalRandom.Next(0, firstNameCandidates.Length);

            return firstNameCandidates[randomLocaleIndex];
        }

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="firstName">The first name of this person.</param>
        /// <returns>A 'Diverse' last name.</returns>
        public string GenerateLastName(string firstName)
        {
            Continent continent = FindContinent(firstName);

            var lastNames = LastNames.PerContinent[continent];

            var randomLocaleIndex = InternalRandom.Next(0, lastNames.Length);

            return lastNames[randomLocaleIndex];
        }

        /// <summary>
        /// Generates a 'Diverse' <see cref="Person"/> (i.e. from all around the world and different cultures). 
        /// </summary>
        /// <param name="gender">The (optional) <see cref="Gender"/> of this <see cref="Person"/></param>
        /// <returns>A 'Diverse' <see cref="Person"/> instance.</returns>
        public Person GenerateAPerson(Gender? gender = null)
        {
            if (gender == null)
            {
                var isFemale = HeadsOrTails();
                if (isFemale)
                {
                    gender = Gender.Female;
                }
                else
                {
                    var isNonBinary = HeadsOrTails();
                    gender = isNonBinary ? Gender.NonBinary : Gender.Male;
                }
            }

            var firstName = GenerateFirstName(gender);
            var lastName = GenerateLastName(firstName);
            var eMail = GenerateEMail(firstName, lastName);
            var isMarried = HeadsOrTails();
            var age = GenerateInteger(18, 97);

            return new Person(firstName, lastName, gender.Value, eMail, isMarried, age);
        }

        /// <summary>
        /// Generates a random Email.
        /// </summary>
        /// <param name="firstName">The (optional) first name for this Email</param>
        /// <param name="lastName">The (option) last name for this Email.</param>
        /// <returns>A random Email.</returns>
        public string GenerateEMail(string firstName = null, string lastName = null)
        {
            if (firstName == null)
            {
                firstName = GenerateFirstName();
            }

            if (lastName == null)
            {
                lastName = GenerateLastName(firstName);
            }

            var domainNames = new string[] { "gmail.com", "yahoo.fr", "louvre-hotels.com", "ibm.com", "yopmail.com", "microsoft.com", "aol.com", "kolab.com", "protonmail.com" };
            var index = InternalRandom.Next(0, domainNames.Length);

            var domainName = domainNames[index];


            if (HeadsOrTails())
            {
                var shortVersion = $"{firstName.Substring(0, 1)}{lastName}@{domainName}".ToLower();
                shortVersion = TransformIntoValidEmailFormat(shortVersion);
                return shortVersion;
            }

            var longVersion = $"{firstName}.{lastName}@{domainName}".ToLower();
            longVersion = TransformIntoValidEmailFormat(longVersion);
            return longVersion;
        }


        private static void LogSeedAndTestInformations(int seed, bool seedWasProvided, string fuzzerName)
        {
            var testName = FindTheNameOfTheTestInvolved();

            if (Log == null)
            {
                throw new FuzzerException($"You must (at least once) set a value for the static {nameof(Log)} property of the {nameof(Fuzzer)} type in order to be able to retrieve the seeds used for each one of your Fuzzer/Tests (which is a prerequisite for deterministic test runs). Suggested value: ex. Fuzzers.Log = TestContext.WriteLine; in the [OneTimeSetUp] initialization method of your [SetUpFixture] class.");
            }

            Log($"----------------------------------------------------------------------------------------------------------------------");
            if (seedWasProvided)
            {
                Log($"--- Fuzzer (\"{fuzzerName}\") instantiated from a provided seed ({seed})");
                Log($"--- from the test: {testName}()");
            }
            else
            {
                Log($"--- Fuzzer (\"{fuzzerName}\") instantiated with the seed ({seed})");
                Log($"--- from the test: {testName}()");
                Log($"--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions");
            }

            Log($"----------------------------------------------------------------------------------------------------------------------");
        }

        private static string FindTheNameOfTheTestInvolved()
        {
            var testName = "(not found)";
            try
            {
                var stackTrace = new StackTrace();

                var testMethod = stackTrace.GetFrames()
                    .Select(sf => sf.GetMethod())
                    .First(IsATestMethod);

                testName = $"{testMethod.DeclaringType.Name}.{testMethod.Name}";
            }
            catch
            { }

            return testName;
        }
        private static bool IsATestMethod(MethodBase mb)
        {
            var attributeTypes = mb.CustomAttributes.Select(c => c.AttributeType);

            var hasACustomAttributeOfTypeTest = attributeTypes.Any(y => (y.Name == "TestAttribute" || y.Name == "TestCaseAttribute" || y.Name == "Fact"));

            if (hasACustomAttributeOfTypeTest)
            {
                return true;
            }

            return hasACustomAttributeOfTypeTest;
        }

        private string GenerateFuzzerName(bool upperCased = true)
        {
            // We are explicitly not using the Random field here to prevent from doing side effects on the deterministic fuzzer instances (depending on whether or not we specified a name)
            var index = new Random().Next(0, 1500);

            return $"fuzzer{index}";
        }

        private static Continent FindContinent(string firstName)
        {
            Continent continent;
            var contextualizedFirstName = Male.ContextualizedFirstNames.FirstOrDefault(c => c.FirstName == firstName);
            if (contextualizedFirstName != null)
            {
                continent = contextualizedFirstName.Origin;
            }
            else
            {
                contextualizedFirstName = Female.ContextualizedFirstNames.FirstOrDefault(c => c.FirstName == firstName);
                if (contextualizedFirstName != null)
                {
                    continent = contextualizedFirstName.Origin;
                }
                else
                {
                    continent = Continent.Africa;
                }
            }

            return continent;
        }

        private string TransformIntoValidEmailFormat(string eMail)
        {
            var validFormat = eMail.Replace(' ', '-');
            validFormat = RemoveDiacritics(validFormat);

            return validFormat;
        }

        // from https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        public bool HeadsOrTails()
        {
            return InternalRandom.Next(0, 2) == 1;
        }
    }
}