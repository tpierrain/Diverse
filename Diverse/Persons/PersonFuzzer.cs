using System;
using System.Text;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="Person"/>.
    /// </summary>
    internal class PersonFuzzer : IFuzzPersons
    {
        private readonly IFuzz _fuzzer;

        private static readonly char[] _specialCharacters = "+-_$%£&!?*$€'|[]()".ToCharArray();
        private static readonly char[] _upperCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] _lowerCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] _numericCharacters = "0123456789".ToCharArray();

        /// <summary>
        /// Instantiates a <see cref="PersonFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public PersonFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
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
                var isFemale = _fuzzer.HeadsOrTails();
                firstNameCandidates = isFemale ? Female.FirstNames : Male.FirstNames;
            }
            else
            {
                firstNameCandidates = gender == Gender.Female ? Female.FirstNames : Male.FirstNames;
            }

            return _fuzzer.PickOneFrom(firstNameCandidates);
        }

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="firstName">The first name of this person.</param>
        /// <returns>A 'Diverse' last name.</returns>
        public string GenerateLastName(string firstName)
        {
            var continent = Locations.FindContinent(firstName);
            var lastNames = LastNames.PerContinent[continent];
            
            return _fuzzer.PickOneFrom(lastNames);
        }

        /// <summary>
        /// Generates a 'Diverse' <see cref="Person"/> (i.e. from all around the world and different cultures). 
        /// </summary>
        /// <param name="gender">The (optional) <see cref="Gender"/> of this <see cref="Person"/></param>
        /// <returns>A 'Diverse' <see cref="Person"/> instance.</returns>
        public Person GeneratePerson(Gender? gender = null)
        {
            gender = gender ?? PickAGenderRandomly();

            var firstName = GenerateFirstName(gender);
            var lastName = GenerateLastName(firstName);
            var eMail = GenerateEMail(firstName, lastName);
            var isMarried = _fuzzer.HeadsOrTails();

            var age = GenerateAge();

            return new Person(firstName, lastName, gender.Value, eMail, isMarried, age);
        }

        /// <summary>
        /// Generates the number of year to be associated with a person.
        /// </summary>
        /// <returns>The number of year to be associated with a person.</returns>
        public int GenerateAge()
        {
            var minValue = 18;
            var maxValue = 97;

            if (_fuzzer.HeadsOrTails())
            {
                return _fuzzer.GenerateInteger(minValue, 28);
            }

            if (_fuzzer.HeadsOrTails())
            {
                return _fuzzer.GenerateInteger(28, 42);
            }

            if (_fuzzer.HeadsOrTails())
            {
                return _fuzzer.GenerateInteger(minValue, 42);
            }

            return _fuzzer.GenerateInteger(42, maxValue);
        }

        private Gender? PickAGenderRandomly()
        {
            Gender? gender;
            var isFemale = _fuzzer.HeadsOrTails();
            if (isFemale)
            {
                gender = Gender.Female;
            }
            else
            {
                var isNonBinary = _fuzzer.HeadsOrTails();
                gender = isNonBinary ? Gender.NonBinary : Gender.Male;
            }

            return gender;
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

            var domainName = _fuzzer.PickOneFrom(Tech.EmailDomainNames);

            if (_fuzzer.HeadsOrTails())
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

            var passwordSize = _fuzzer.Random.Next(minimumSize, maximumSize + 1);

            var pwd = new StringBuilder(passwordSize);
            for (var i = 0; i < passwordSize; i++)
            {
                if ((i == 0 || i == 10) && (includeSpecialCharacters.HasValue && includeSpecialCharacters.Value))
                {
                    pwd.Append(_specialCharacters[_fuzzer.Random.Next(0, _specialCharacters.Length)]);
                    continue;
                }

                if (i == 4 || i == 14)
                {
                    pwd.Append(_upperCharacters[_fuzzer.Random.Next(1, 26)]);
                    continue;
                }

                if (i == 6 || i == 13)
                {
                    pwd.Append(_numericCharacters[_fuzzer.Random.Next(4, 10)]);
                    continue;
                }

                if (i == 3 || i == 9)
                {
                    pwd.Append(_numericCharacters[_fuzzer.Random.Next(1, 5)]);
                    continue;
                }

                // by default
                pwd.Append(_lowerCharacters[_fuzzer.Random.Next(1, 26)]);
            }

            return pwd.ToString();
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
            validFormat = validFormat.RemoveDiacritics();

            return validFormat;
        }
    }
}