using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Diverse.DateTimes;
using Diverse.Numbers;
using Diverse.Strings;

namespace Diverse
{
    /// <summary>
    /// Allows to generate lots of combination of things. <see cref="Fuzzer"/> are very useful to detect hard coded values in our implementations.
    /// Note: you can instantiate another Deterministic Fuzzer by providing it the Seed you want to reuse.
    /// </summary>
    public class Fuzzer : IFuzz
    {
        private readonly Random _internalRandom;

        private readonly IFuzzStrings _stringFuzzer;
        private readonly IFuzzNumbers _numberFuzzer;
        private readonly IFuzzPersons _personFuzzer;
        private readonly IFuzzDatesAndTime _dateTimeFuzzer;
        private readonly IFuzzTypes _typeFuzzer;

        /// <summary>
        /// Generates a DefaultSeed. Important to keep a trace of the used seed so that we can reproduce a failing situation with <see cref="Fuzzer"/> involved.
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// Gets the name of this <see cref="Fuzzer"/> instance.
        /// </summary>
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
        /// Sets the way you want to log or receive what the <see cref="Fuzzer"/> has to say about every generated seeds used for every fuzzer instance and test.
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

            // Instantiates implementation types for the various Fuzzer
            _stringFuzzer = new StringFuzzer(this);
            _numberFuzzer = new NumberFuzzer(this);
            _personFuzzer = new PersonFuzzer(this);
            _dateTimeFuzzer = new DateTimeFuzzer(this);
            _typeFuzzer = new TypeFuzzer(this);
        }

        private static void LogSeedAndTestInformations(int seed, bool seedWasProvided, string fuzzerName)
        {
            var testName = FindTheNameOfTheTestInvolved();

            if (Log == null)
            {
                throw new FuzzerException(BuildErrorMessageForMissingLogRegistration());
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

        private static string BuildErrorMessageForMissingLogRegistration()
        {
            var message = @"You must register (at least once) a log handler in your Test project for the Diverse library to be able to publish all the seeds used for every test (which is a prerequisite for deterministic test runs afterward).
The only thing you have to do is to set a value for the static " + $"{nameof(Log)} property of the {nameof(Fuzzer)} type." + @"

The best location for this call is within a unique AllFixturesSetup class.
e.g.: with NUnit:

using NUnit.Framework;

namespace YouNameSpaceHere.Tests
{
    [SetUpFixture]
    public class AllTestFixtures
    {
        [OneTimeSetUp]
        public void Init()
        {
            "+ $"{nameof(Fuzzer)}.{nameof(Log)} = TestContext.WriteLine;" + @"
        }
    }
}

";
            return message;
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

        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        public bool HeadsOrTails()
        {
            return InternalRandom.Next(0, 2) == 1;
        }

        #region NumberFuzzer

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger(int minValue, int maxValue)
        {
            return _numberFuzzer.GenerateInteger(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random integer value.
        /// </summary>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger()
        {
            return _numberFuzzer.GenerateInteger();
        }

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        public int GeneratePositiveInteger(int? maxValue = null)
        {
            return _numberFuzzer.GeneratePositiveInteger(maxValue);
        }

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        public decimal GeneratePositiveDecimal()
        {
            return _numberFuzzer.GeneratePositiveDecimal();
        }

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <returns>A long value generated randomly.</returns>
        public long GenerateLong()
        {
            return _numberFuzzer.GenerateLong();
        }

        #endregion

        #region PersonFuzzer

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="gender">The <see cref="Gender"/> to be used as indication (optional).</param>
        /// <returns>A 'Diverse' first name.</returns>
        public string GenerateFirstName(Gender? gender = null)
        {
            return _personFuzzer.GenerateFirstName(gender);
        }

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="firstName">The first name of this person.</param>
        /// <returns>A 'Diverse' last name.</returns>
        public string GenerateLastName(string firstName)
        {
            return _personFuzzer.GenerateLastName(firstName);
        }

        /// <summary>
        /// Generates a 'Diverse' <see cref="Person"/> (i.e. from all around the world and different cultures). 
        /// </summary>
        /// <param name="gender">The (optional) <see cref="Gender"/> of this <see cref="Person"/></param>
        /// <returns>A 'Diverse' <see cref="Person"/> instance.</returns>
        public Person GeneratePerson(Gender? gender = null)
        {
            return _personFuzzer.GeneratePerson(gender);
        }

        /// <summary>
        /// Generates a random Email.
        /// </summary>
        /// <param name="firstName">The (optional) first name for this Email</param>
        /// <param name="lastName">The (option) last name for this Email.</param>
        /// <returns>A random Email.</returns>
        public string GenerateEMail(string firstName = null, string lastName = null)
        {
            return _personFuzzer.GenerateEMail(firstName, lastName);
        }

        /// <summary>
        /// Generates a password following some common rules asked on the internet.
        /// </summary>
        /// <returns>The generated password</returns>
        public string GeneratePassword(int? minSize = null, int? maxSize = null, bool? includeSpecialCharacters = null)
        {
            return _personFuzzer.GeneratePassword(minSize, maxSize, includeSpecialCharacters);
        }

        #endregion

        #region DateTimeFuzzer

        /// <summary>
        /// Generates a random <see cref="DateTime"/>.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value generated randomly.</returns>
        public DateTime GenerateDateTime()
        {
            return _dateTimeFuzzer.GenerateDateTime();
        }

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minValue">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <param name="maxValue">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        public DateTime GenerateDateTimeBetween(DateTime minValue, DateTime maxValue)
        {
            return _dateTimeFuzzer.GenerateDateTimeBetween(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random <see cref="DateTime"/> in a Time Range.
        /// </summary>
        /// <param name="minDate">The minimum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <param name="maxDate">The maximum inclusive boundary of the Time Range for this <see cref="DateTime"/> generation, specified as a yyyy/MM/dd string.</param>
        /// <returns>A <see cref="DateTime"/> instance between the min and the max inclusive boundaries.</returns>
        public DateTime GenerateDateTimeBetween(string minDate, string maxDate)
        {
            return _dateTimeFuzzer.GenerateDateTimeBetween(minDate, maxDate);
        }

        #endregion

        #region StringFuzzer

        /// <summary>
        /// Generates a random adjective based on a feeling.
        /// </summary>
        /// <param name="feeling">The expected feeling of the adjective</param>
        /// <returns>An adjective based on a particular feeling or random one if not provided</returns>
        public string GenerateAdjective(Feeling? feeling = null)
        {
            return _stringFuzzer.GenerateAdjective(feeling);
        }

        #endregion

        #region TypeFuzzer

        /// <summary>
        /// Generates an instance of a type T.
        /// </summary>
        /// <returns>An instance of type T with some fuzzed properties.</returns>
        public T GenerateInstanceOf<T>()
        {
            return _typeFuzzer.GenerateInstanceOf<T>();
        }

        /// <summary>
        /// Generates an instance of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Enum"/></typeparam>
        /// <returns>An random value of the specified <see cref="Enum"/> type.</returns>
        public T GenerateEnum<T>()
        {
            return _typeFuzzer.GenerateEnum<T>();
        }

        #endregion
    }
}
