using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Diverse.Collections;
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
        private readonly IFuzzLorem _loremFuzzer;
        private readonly IFuzzNumbers _numberFuzzer;
        private readonly IFuzzPersons _personFuzzer;
        private readonly IFuzzDatesAndTime _dateTimeFuzzer;
        private readonly IFuzzTypes _typeFuzzer;
        private readonly IFuzzGuid _guidFuzzer;
        private readonly IFuzzFromCollections _collectionFuzzer;

        // For AvoidDuplication mode
        private const int MaxFailingAttemptsToFindNotAlreadyProvidedValueDefaultValue = 100;
        private const int MaxRangeSizeAllowedForMemoizationDefaultValue = 1000000;
        private readonly Memoizer _memoizer = new Memoizer();
        private IFuzz _sideEffectFreeFuzzer;

        /// <summary>
        /// internal Fuzzer instance to be used by the various lambdas
        /// related to the AvoidDuplication mode (i.e. when the option is
        /// set to <b>true</b>).
        ///
        /// Ironically, the AvoidDuplication mode of this Fuzzer needs
        /// to use another fuzzer instance for the Lastchance mode (i.e. when the
        /// MaxFailingAttemptsToFindNotAlreadyProvidedValueDefaultValue has been
        /// reached).
        ///
        /// E.g.: if you call the <see cref="GenerateAge"/> method on a Fuzzer with
        /// AvoidDuplication set to <b>true</b> in a situation where the
        /// <see cref="MaxFailingAttemptsToFindNotAlreadyProvidedValueDefaultValue"/>
        /// was not enough to find another original value, the lastChance lambda
        /// of the <see cref="GenerateWithoutDuplication"/> method will be called.
        ///
        /// In that case, since the last chance lambda of the <see cref="GenerateAge"/>
        /// method is using the <see cref="IFuzzNumbers.GenerateInteger"/> method,
        /// we need to avoid <see cref="StackOverflowException"/> by using
        /// a <see cref="SideEffectFreeSafeFuzzer"/> instance in that specific case
        /// (in all lastChance lambdas actually).
        /// </summary>
        private IFuzz SideEffectFreeSafeFuzzer => _sideEffectFreeFuzzer ?? (_sideEffectFreeFuzzer = new Fuzzer(this.Seed, avoidDuplication: false));

        /// <summary>
        /// Gets or sets the max number of attempts the Fuzzer should make in order to generate
        /// a not already provided value when <see cref="AvoidDuplication"/> mode
        /// is enabled (via constructor).
        /// </summary>
        public int MaxFailingAttemptsToFindNotAlreadyProvidedValue { get; set; } =
            MaxFailingAttemptsToFindNotAlreadyProvidedValueDefaultValue;

        /// <summary>
        /// Gets or sets the maximum number of entries to be memoized if
        /// <see cref="AvoidDuplication"/> mode is enabled (via constructor).
        /// </summary>
        public ulong MaxRangeSizeAllowedForMemoization { get; set; } = MaxRangeSizeAllowedForMemoizationDefaultValue;

        /// <summary>
        /// Generates a DefaultSeed. Important to keep a trace of the used seed so that we can reproduce a failing situation with <see cref="Fuzzer"/> involved.
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// Gets the name of this <see cref="Fuzzer"/> instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets of sets a value indicating whether the <see cref="Fuzzer"/> should avoid providing twice the same value or not.
        /// </summary>
        public bool AvoidDuplication { get; set; }

        /// <summary>
        /// Gets the Random instance to be used when we want to create a new extension method for the <see cref="Fuzzer"/>.
        /// <remarks>The use of explicit interface implementation for this property is made on purpose in order to hide this internal mechanic details from the Fuzzer end-user code.</remarks>
        /// </summary>
        Random IFuzz.Random => _internalRandom;

        /// <summary>
        /// Gives easy access to the <see cref="IFuzz.Random"/> explicit implementation.
        /// </summary>
        private Random InternalRandom => ((IFuzz) this).Random;

        /// <summary>
        /// Sets the way you want to log or receive what the <see cref="Fuzzer"/> has to say about every generated seeds used for every fuzzer instance and test.
        /// </summary>
        public static Action<string> Log { get; set; }

        /// <summary>
        /// Instantiates a <see cref="Fuzzer"/>.
        /// </summary>
        /// <param name="seed">The seed if you want to reuse in order to reproduce the very same conditions of another (failing) test.</param>
        /// <param name="name">The name you want to specify for this <see cref="Fuzzer"/> instance (useful for debuging purpose).</param>
        /// <param name="avoidDuplication"><b>true</b> if you do not want the Fuzzer to provide you twice the same result for every fuzzing method type, <b>false</b> otherwise.</param>
        public Fuzzer(int? seed = null, string name = null, bool? avoidDuplication = false)
        {
            var seedWasProvided = seed.HasValue;

            seed = seed ??
                   new Random().Next(); // the seed is not specified? pick a random one for this Fuzzer instance.
            Seed = seed.Value;

            _internalRandom = new Random(seed.Value);

            name = name ?? GenerateFuzzerName();
            Name = name;

            avoidDuplication = avoidDuplication ?? false;
            AvoidDuplication = avoidDuplication.Value;

            LogSeedAndTestInformations(seed.Value, seedWasProvided, name);

            // Instantiates implementation types for the various Fuzzer
            _loremFuzzer = new LoremFuzzer(this);
            _stringFuzzer = new StringFuzzer(this);
            _numberFuzzer = new NumberFuzzer(this);
            _personFuzzer = new PersonFuzzer(this);
            _dateTimeFuzzer = new DateTimeFuzzer(this);
            _typeFuzzer = new TypeFuzzer(this);
            _guidFuzzer = new GuidFuzzer(this);
            _collectionFuzzer = new CollectionFuzzer(this);
        }

        #region Core

        private static void LogSeedAndTestInformations(int seed, bool seedWasProvided, string fuzzerName)
        {
            var testName = FindTheNameOfTheTestInvolved();

            if (Log == null)
            {
                throw new FuzzerException(BuildErrorMessageForMissingLogRegistration());
            }

            Log(
                $"----------------------------------------------------------------------------------------------------------------------");
            if (seedWasProvided)
            {
                Log($"--- Fuzzer (\"{fuzzerName}\") instantiated from a provided seed ({seed})");
                Log($"--- from the test: {testName}()");
            }
            else
            {
                Log($"--- Fuzzer (\"{fuzzerName}\") instantiated with the seed ({seed})");
                Log($"--- from the test: {testName}()");
                Log(
                    $"--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions");
            }

            Log(
                $"----------------------------------------------------------------------------------------------------------------------");
        }

        private static string BuildErrorMessageForMissingLogRegistration()
        {
            var message =
                @"You must register (at least once) a log handler in your Test project for the Diverse library to be able to publish all the seeds used for every test (which is a prerequisite for deterministic test runs afterward).
The only thing you have to do is to set a value for the static " +
                $"{nameof(Log)} property of the {nameof(Fuzzer)} type." + @"

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
            " + $"{nameof(Fuzzer)}.{nameof(Log)} = TestContext.WriteLine;" + @"
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

                var testMethod = stackTrace.GetFrames().Select(sf => sf.GetMethod()).First(IsATestMethod);

                testName = $"{testMethod.DeclaringType.Name}.{testMethod.Name}";
            }
            catch
            {
            }

            return testName;
        }

        private static bool IsATestMethod(MethodBase mb)
        {
            var attributeTypes = mb.CustomAttributes.Select(c => c.AttributeType);

            var hasACustomAttributeOfTypeTest = attributeTypes.Any(y =>
                (y.Name == "TestAttribute" || y.Name == "TestCaseAttribute" || y.Name == "Fact"));

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

        /// <summary>
        /// Methods to be used when <see cref="AvoidDuplication"/> is set to <b>true</b>
        /// for any fuzzing method of this <see cref="Fuzzer"/> instance.
        /// It encapsulates the logic of various attempts and retries before
        /// falling back to a very specific <paramref name="lastChanceGenerationFunction"/>
        /// lambda associated to the considered fuzzing method.
        /// </summary>
        /// <typeparam name="T">Type to be fuzzed/generated</typeparam>
        /// <param name="currentMethod">
        ///     The current Method calling us (e.g.: <see cref="GenerateAge"/>).
        ///     Used for memoization purpose.
        /// </param>
        /// <param name="argumentsHashCode">
        ///     A hash for the current method call arguments. Used for memoization purpose.
        /// </param>
        /// <param name="maxFailingAttemptsBeforeLastChanceFunctionIsCalled">
        ///     The maximum number of calls to the <paramref name="regularGenerationFunction"/>
        ///     we should try before we fall-back and call the
        ///     <paramref name="lastChanceGenerationFunction"/> lambda.
        /// </param>
        /// <param name="regularGenerationFunction">
        ///     The function to use in order to generate the thing(s) we want.
        ///     It should be the same function that the one we call for the cases
        ///     where <see cref="AvoidDuplication"/> is set to <b>false</b>.
        /// </param>
        /// <param name="lastChanceGenerationFunction">
        ///     The function to use in order to generate the thing(s) we want when
        ///     all the <paramref name="regularGenerationFunction"/> attempts have failed.
        ///     To do our job, we receive:
        ///         - A <see cref="SortedSet{T}"/> instance with all the previously
        ///           generated values
        /// 
        ///         - A side-effect free <see cref="IFuzz"/> instance to use if needed.
        ///           (one should not use the current instance of <see cref="Fuzzer"/>
        ///           to do the job since it may have side-effects
        ///           and lead to <see cref="StackOverflowException"/>)).
        /// </param>
        /// <returns>The thing(s) we want to generate.</returns>
        private T GenerateWithoutDuplication<T>(MethodBase currentMethod, int argumentsHashCode,
                            int maxFailingAttemptsBeforeLastChanceFunctionIsCalled, 
                            Func<IFuzz, T> regularGenerationFunction,
                            Func<SortedSet<object>, Maybe<T>> lastChanceGenerationFunction = null)
        {
            var memoizerKey = new MemoizerKey(currentMethod, argumentsHashCode);

            var maybe = TryGetNonAlreadyProvidedValuesWithRegularGenerationFunction<T>(memoizerKey, out var alreadyProvidedValues,
                regularGenerationFunction, maxFailingAttemptsBeforeLastChanceFunctionIsCalled);

            if (!maybe.HasItem)
            {
                if (lastChanceGenerationFunction != null)
                {
                    // last attempt, we randomly pick the missing bits from the memoizer
                    maybe = lastChanceGenerationFunction(_memoizer.GetAlreadyProvidedValues(memoizerKey));
                }

                if (!maybe.HasItem)
                {
                    throw new DuplicationException(typeof(T), maxFailingAttemptsBeforeLastChanceFunctionIsCalled, alreadyProvidedValues);
                }
            }

            alreadyProvidedValues.Add(maybe.Item);
            return maybe.Item;
        }

        private Maybe<T> TryGetNonAlreadyProvidedValuesWithRegularGenerationFunction<T>(MemoizerKey memoizerKey, 
                                    out SortedSet<object> alreadyProvidedValues, 
                                    Func<IFuzz, T> generationFunction, 
                                    int maxFailingAttempts)
        {
            alreadyProvidedValues = _memoizer.GetAlreadyProvidedValues(memoizerKey);

            var maybe = GenerateNotAlreadyProvidedValue<T>(alreadyProvidedValues, maxFailingAttempts, generationFunction);

            return maybe;
        }

        private int HashArguments(params object[] parameters)
        {
            var hash = 17;
            foreach (var parameter in parameters)
            {
                var parameterHashCode = parameter?.GetHashCode() ?? 17;
                hash = hash * 23 + parameterHashCode;
            }

            return hash;
        }

        private Maybe<T> GenerateNotAlreadyProvidedValue<T>(ISet<object> alreadyProvidedValues, int maxAttempts, Func<IFuzz, T> generationFunction)
        {
            T result = default(T);
            for (var i = 0; i < maxAttempts; i++)
            {
                result = generationFunction(SideEffectFreeSafeFuzzer);

                if (!alreadyProvidedValues.Contains(result))
                {
                    return new Maybe<T>(result);
                }
            }

            return new Maybe<T>();
        }

        #endregion

        #region NumberFuzzer

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger(int? minValue = null, int? maxValue = null)
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(minValue, maxValue),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    (safeFuzzer) => safeFuzzer.GenerateInteger(minValue, maxValue),
                    alreadyProvidedValues => LastChanceToFindNotAlreadyProvidedInteger(alreadyProvidedValues,
                        minValue.Value, maxValue.Value, _collectionFuzzer));
            }

            return _numberFuzzer.GenerateInteger(minValue, maxValue);
        }
 
        private static Maybe<int> LastChanceToFindNotAlreadyProvidedInteger(SortedSet<object> alreadyProvidedValues,
            int? minValue, int? maxValue, IFuzzFromCollections fuzzer)
        {
            minValue = minValue ?? int.MinValue;
            maxValue = maxValue ?? int.MaxValue;

            var allPossibleValues = Enumerable.Range(minValue.Value, maxValue.Value).ToArray();
            var remainingCandidates =
                allPossibleValues.Where(number => !alreadyProvidedValues.Contains(number)).ToList();

            if (remainingCandidates.Any())
            {
                var pickOneFrom = fuzzer.PickOneFrom<int>(remainingCandidates);
                return new Maybe<int>(pickOneFrom);
            }

            return new Maybe<int>();
        }


        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        public int GeneratePositiveInteger(int? maxValue = null)
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(maxValue),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    (safeFuzzer) => safeFuzzer.GeneratePositiveInteger(maxValue));
            }

            return _numberFuzzer.GeneratePositiveInteger(maxValue);
        }

        /// <summary>
        /// Generates a random decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive upper bound of the random number returned.</param>
        /// <returns>A decimal value generated randomly.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref>.</exception>
        public decimal GenerateDecimal(decimal? minValue = null, decimal? maxValue = null)
        {
            // No need to memoize decimals here since it is very unlikely that the lib generate twice the same decimal
            return _numberFuzzer.GenerateDecimal(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive positive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive positive upper bound of the random number returned.</param>
        /// <returns>A positive decimal value generated randomly.</returns>
        public decimal GeneratePositiveDecimal(decimal? minValue = null, decimal? maxValue = null)
        {
            // No need to memoize decimals here since it is very unlikely that the lib generate twice the same decimal
            return _numberFuzzer.GeneratePositiveDecimal(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A long value generated randomly.</returns>
        public long GenerateLong(long? minValue = null, long? maxValue = null)
        {
            if (AvoidDuplication)
            {
                // We will only memoize if the range is not too wide
                var uRange = NumberExtensions.ComputeRange(minValue, maxValue);

                if (uRange <= MaxRangeSizeAllowedForMemoization)
                {
                    return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(minValue, maxValue),
                        maxFailingAttemptsBeforeLastChanceFunctionIsCalled:
                        MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                        regularGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateLong(minValue, maxValue),
                        lastChanceGenerationFunction: (alreadyProvidedSortedSet) =>
                            LastChanceToFindNotAlreadyProvidedLong(ref minValue, ref maxValue, alreadyProvidedSortedSet,
                                this));
                }
            }

            return _numberFuzzer.GenerateLong(minValue, maxValue);
        }

        private static Maybe<long> LastChanceToFindNotAlreadyProvidedLong(ref long? minValue, ref long? maxValue, SortedSet<object> alreadyProvidedSortedSet, IFuzz fuzzer)
        {
            minValue = minValue ?? long.MinValue;
            maxValue = maxValue ?? long.MaxValue;

            var allPossibleOptions = GenerateAllPossibleOptions(minValue.Value, maxValue.Value);

            var remainingNumbers = allPossibleOptions.Where(n => !alreadyProvidedSortedSet.Contains(n)).ToArray();

            if (remainingNumbers.Any())
            {
                var index = fuzzer.GenerateInteger(0, remainingNumbers.Length - 1);
                var randomRemainingNumber = remainingNumbers[index];

                return new Maybe<long>(randomRemainingNumber);
            }

            return new Maybe<long>();
        }

        private static SortedSet<long> GenerateAllPossibleOptions(long min, long max)
        {
            var allPossibleOptions = new SortedSet<long>();
            for (var i = min; i < max + 1; i++)
            {
                allPossibleOptions.Add(i);
            }

            return allPossibleOptions;
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
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(gender),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue, 
                    (safeFuzzer) => safeFuzzer.GenerateFirstName(gender));
            }

            return _personFuzzer.GenerateFirstName(gender);
        }

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="firstName">The first name of this person.</param>
        /// <returns>A 'Diverse' last name.</returns>
        public string GenerateLastName(string firstName)
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(firstName),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue, (safeFuzzer) =>
                    {
                        return safeFuzzer.GenerateLastName(firstName);
                    },
                    (alreadyProvidedSortedSet) => LastChanceToFindLastName(firstName, alreadyProvidedSortedSet, this));
            }

            return _personFuzzer.GenerateLastName(firstName);
        }

        private static Maybe<string> LastChanceToFindLastName(string firstName, SortedSet<object> alreadyProvidedLastNames, IFuzz fuzzer)
        {
            var continent = Locations.FindContinent(firstName);
            var allPossibleOptions = LastNames.PerContinent[continent];

            var remainingLastNames = allPossibleOptions.Where(n => !alreadyProvidedLastNames.Contains(n)).ToArray();

            if (remainingLastNames.Any())
            {
                var lastName = fuzzer.PickOneFrom(remainingLastNames);
                return new Maybe<string>(lastName);
            }

            return new Maybe<string>();
        }

        /// <summary>
        /// Generates the number of year to be associated with a person.
        /// </summary>
        /// <returns>The number of year to be associated with a person.</returns>
        public int GenerateAge()
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    regularGenerationFunction: (fuzzerWithDuplicationAllowed) => fuzzerWithDuplicationAllowed.GenerateAge(),
                    lastChanceGenerationFunction: (alreadyProvidedValues) =>
                        LastChanceToFindAge(alreadyProvidedValues, 18, 97, _collectionFuzzer));
            }

            return _personFuzzer.GenerateAge();
        }

        private static Maybe<int> LastChanceToFindAge(SortedSet<object> alreadyProvidedValues, int minAge, int maxAge,
            IFuzzFromCollections fuzzer)
        {
            var allPossibleValues = Enumerable.Range(minAge, maxAge - minAge).ToArray();
            var remainingCandidates = allPossibleValues.Where(age => !alreadyProvidedValues.Contains(age)).ToList();

            if (remainingCandidates.Any())
            {
                var pickOneFrom = fuzzer.PickOneFrom<int>(remainingCandidates);
                return new Maybe<int>(pickOneFrom);
            }

            return new Maybe<int>();
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
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(firstName, lastName),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    (safeFuzzer) => safeFuzzer.GenerateEMail(firstName, lastName));
            }

            return _personFuzzer.GenerateEMail(firstName, lastName);
        }

        /// <summary>
        /// Generates a password following some common rules asked on the internet.
        /// </summary>
        /// <returns>The generated password</returns>
        public string GeneratePassword(int? minSize = null, int? maxSize = null, bool? includeSpecialCharacters = null)
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(),
                    HashArguments(minSize, maxSize, includeSpecialCharacters),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    (safeFuzzer) =>
                    {
                        return safeFuzzer.GeneratePassword(minSize, maxSize, includeSpecialCharacters);
                    });
            }

            return _personFuzzer.GeneratePassword(minSize, maxSize, includeSpecialCharacters);
        }

        #endregion

        #region CollectionFuzzer

        /// <summary>
        /// Randomly pick one element from a given collection.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns>One of the elements from the candidates collection.</returns>
        public T PickOneFrom<T>(IList<T> candidates)
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication<T>(CaptureCurrentMethod(), HashArguments(candidates),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue, 
                    (safeFuzzer) =>
                    {
                        return safeFuzzer.PickOneFrom(candidates);
                    });
            }

            return _collectionFuzzer.PickOneFrom(candidates);
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
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(feeling),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue, 
                    (safeFuzzer) => safeFuzzer.GenerateAdjective(feeling),
                    alreadyProvidedSortedSet => LastChanceToFindAdjective(feeling, alreadyProvidedSortedSet, this));
            }

            return _stringFuzzer.GenerateAdjective(feeling);
        }

        private static Maybe<string> LastChanceToFindAdjective(Feeling? feeling,
            SortedSet<object> alreadyProvidedLastNames, IFuzz fuzzer)
        {
            var remainingAdjectives = Adjectives.PerFeeling[feeling.Value]
                .Where(n => !alreadyProvidedLastNames.Contains(n)).ToArray();

            if (remainingAdjectives.Any())
            {
                var adjective = fuzzer.PickOneFrom(remainingAdjectives);
                return new Maybe<string>(adjective);
            }

            return new Maybe<string>();
        }

        #endregion

        #region GuidFuzzer

        /// <summary>
        /// Generates a random <see cref="Guid"/>
        /// </summary>
        /// <returns>A random <see cref="Guid"/>.</returns>
        public Guid GenerateGuid()
        {
            // No need to memoize Guids here since it is very unlikely that the lib generate twice the same value
            return _guidFuzzer.GenerateGuid();
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
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication<T>(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue, 
                    (safeFuzzer) => safeFuzzer.GenerateEnum<T>());
            }

            return _typeFuzzer.GenerateEnum<T>();
        }

        #endregion

        #region LoremFuzzer

        /// <summary>
        /// Generates random latin words.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="AvoidDuplication"/> mode.</remarks>
        /// <param name="number">(optional) Number of words to generate.</param>
        /// <returns>The generated latin words.</returns>
        public IEnumerable<string> GenerateWords(int? number = null)
        {
            return _loremFuzzer.GenerateWords(number);
        }

        /// <summary>
        /// Generate a sentence in latin.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="AvoidDuplication"/> mode.</remarks>
        /// <param name="nbOfWords">(optional) Number of words for this sentence.</param>
        /// <returns>The generated sentence in latin.</returns>
        public string GenerateSentence(int? nbOfWords = null)
        {
            return _loremFuzzer.GenerateSentence(nbOfWords);
        }

        /// <summary>
        /// Generates a paragraph in latin.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="AvoidDuplication"/> mode.</remarks>
        /// <param name="nbOfSentences">(optional) Number of sentences for this paragraph.</param>
        /// <returns>The generated paragraph in latin.</returns>
        public string GenerateParagraph(int? nbOfSentences = null)
        {
            return _loremFuzzer.GenerateParagraph(nbOfSentences);
        }

        /// <summary>
        /// Generates a collection of paragraphs. 
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="AvoidDuplication"/> mode.</remarks>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The collection of paragraphs.</returns>
        public IEnumerable<string> GenerateParagraphs(int? nbOfParagraphs = null)
        {
            return _loremFuzzer.GenerateParagraphs(nbOfParagraphs);
        }

        /// <summary>
        /// Generates a text in latin with a specified number of paragraphs.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="AvoidDuplication"/> mode.</remarks>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The generated text in latin.</returns>
        public string GenerateText(int? nbOfParagraphs = null)
        {
            return _loremFuzzer.GenerateText(nbOfParagraphs);
        }

        /// <summary>
        /// Generates a random letter.
        /// </summary>
        /// <returns>The generated letter.</returns>
        public char GenerateLetter()
        {
            if (AvoidDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsToFindNotAlreadyProvidedValue,
                    (safeFuzzer) => safeFuzzer.GenerateLetter(),
                    alreadyProvidedSortedSet => LastChanceToFindLetter(alreadyProvidedSortedSet, this));
            }

            return _loremFuzzer.GenerateLetter();
        }

        private Maybe<char> LastChanceToFindLetter(SortedSet<object> alreadyProvidedSortedSet, IFuzz fuzzer)
        {
            var allPossibleValues = LoremFuzzer.Alphabet;
            var alreadyProvidedLetters = alreadyProvidedSortedSet.Cast<char>();

            var remainingOptions = allPossibleValues.Except(alreadyProvidedLetters).ToList();

            if (remainingOptions.Any())
            {
                var letter = fuzzer.PickOneFrom<char>(remainingOptions);
                return new Maybe<char>(letter);
            }

            return new Maybe<char>();
        }

        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        private MethodBase CaptureCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);


            return sf.GetMethod();
        }
    }
}