using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="Person"/>.
    /// </summary>
    public class PersonFuzzer : IFuzzPersons
    {
        private readonly IProvideCorePrimitivesToFuzzer _fuzzerPrimitives;

        private static char[] _specialCharacters = "+-_$%£&!?*$€'|[]()".ToCharArray();
        private static char[] _upperCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static char[] _lowerCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static char[] _numericCharacters = "0123456789".ToCharArray();

        private IFuzzNumbers _numberFuzzer;

        /// <summary>
        /// Instantiates a <see cref="PersonFuzzer"/>.
        /// </summary>
        /// <param name="fuzzerPrimitives">Instance of <see cref="IProvideCorePrimitivesToFuzzer"/> to use.</param>
        /// <param name="numberFuzzer">Instance of <see cref="IFuzzNumbers"/> to use.</param>
        public PersonFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives, IFuzzNumbers numberFuzzer)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
            _numberFuzzer = numberFuzzer;
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
                var isFemale = _fuzzerPrimitives.HeadsOrTails();
                firstNameCandidates = isFemale ? Female.FirstNames : Male.FirstNames;
            }
            else
            {
                firstNameCandidates = gender == Gender.Female ? Female.FirstNames : Male.FirstNames;
            }

            var randomLocaleIndex = _fuzzerPrimitives.Random.Next(0, firstNameCandidates.Length);

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

            var randomLocaleIndex = _fuzzerPrimitives.Random.Next(0, lastNames.Length);

            return lastNames[randomLocaleIndex];
        }

        /// <summary>
        /// Generates a 'Diverse' <see cref="Person"/> (i.e. from all around the world and different cultures). 
        /// </summary>
        /// <param name="gender">The (optional) <see cref="Gender"/> of this <see cref="Person"/></param>
        /// <returns>A 'Diverse' <see cref="Person"/> instance.</returns>
        public Person GeneratePerson(Gender? gender = null)
        {
            if (gender == null)
            {
                var isFemale = _fuzzerPrimitives.HeadsOrTails();
                if (isFemale)
                {
                    gender = Gender.Female;
                }
                else
                {
                    var isNonBinary = _fuzzerPrimitives.HeadsOrTails();
                    gender = isNonBinary ? Gender.NonBinary : Gender.Male;
                }
            }

            var firstName = GenerateFirstName(gender);
            var lastName = GenerateLastName(firstName);
            var eMail = GenerateEMail(firstName, lastName);
            var isMarried = _fuzzerPrimitives.HeadsOrTails();
            var age = _numberFuzzer.GenerateInteger(18, 97);

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

            var domainNames = new string[]
            {
                "kolab.com", "protonmail.com", "gmail.com", "yahoo.fr", "42skillz.com", "gmail.com", "ibm.com",
                "gmail.com", "yopmail.com", "microsoft.com", "gmail.com", "aol.com"
            };
            var index = _fuzzerPrimitives.Random.Next(0, domainNames.Length);

            var domainName = domainNames[index];


            if (_fuzzerPrimitives.HeadsOrTails())
            {
                var shortVersion = $"{firstName.Substring(0, 1)}{lastName}@{domainName}".ToLower();
                shortVersion = TransformIntoValidEmailFormat(shortVersion);
                return shortVersion;
            }

            var longVersion = $"{firstName}.{lastName}@{domainName}".ToLower();
            longVersion = TransformIntoValidEmailFormat(longVersion);
            return longVersion;
        }

        /// <summary>
        /// Generates a password following some common rules asked on the internet.
        /// </summary>
        /// <returns>The generated password</returns>
        public string GeneratePassword(int? minSize = null, int? maxSize = null, bool? includeSpecialCharacters = null)
        {
            var defaultMinSize = 7;
            var defaultMaxSize = 12;

            var minimumSize = minSize ?? defaultMinSize;
            var maximumSize = maxSize ?? defaultMaxSize;

            CheckGuardMinAndMaximumSizes(minSize, maxSize, minimumSize, maximumSize, defaultMinSize, defaultMaxSize);

            var passwordSize = _fuzzerPrimitives.Random.Next(minimumSize, maximumSize + 1);

            var pwd = new StringBuilder(passwordSize);
            for (var i = 0; i < passwordSize; i++)
            {
                if ((i == 0 || i == 10) && (includeSpecialCharacters.HasValue && includeSpecialCharacters.Value))
                {
                    pwd.Append(_specialCharacters[_fuzzerPrimitives.Random.Next(0, _specialCharacters.Length)]);
                    continue;
                }

                if (i == 4 || i == 14)
                {
                    pwd.Append(_upperCharacters[_fuzzerPrimitives.Random.Next(1, 26)]);
                    continue;
                }

                if (i == 6 || i == 13)
                {
                    pwd.Append(_numericCharacters[_fuzzerPrimitives.Random.Next(4, 10)]);
                    continue;
                }

                if (i == 3 || i == 9)
                {
                    pwd.Append(_numericCharacters[_fuzzerPrimitives.Random.Next(1, 5)]);
                    continue;
                }

                // by default
                pwd.Append(_lowerCharacters[_fuzzerPrimitives.Random.Next(1, 26)]);
            }

            return pwd.ToString();
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

        private static void CheckGuardMinAndMaximumSizes(int? minSize, int? maxSize, int minimumSize, int maximumSize, int defaultMinSize, int defaultMaxSize)
        {
            if (minimumSize > maximumSize)
            {
                var parameterName = minSize == null ? "maxSize" : "minSize";
                if (minSize.HasValue && maxSize.HasValue)
                {
                    parameterName = "maxSize";
                }

                throw new ArgumentOutOfRangeException(parameterName,
                    $"maxSize ({maximumSize}) can't be inferior to minSize({minimumSize}). Specify 2 values if you don't want to use the default values of the library (i.e.: [{defaultMinSize}, {defaultMaxSize}]).");
            }
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

    }
}