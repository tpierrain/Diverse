using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Diverse.Address;
using Diverse.Address.Geography;
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
        private readonly IFuzzAddress _addressFuzzer;
        private readonly IFuzzPersons _personFuzzer;
        private readonly IFuzzDatesAndTime _dateTimeFuzzer;
        private readonly IFuzzTypes _typeFuzzer;
        private readonly IFuzzGuid _guidFuzzer;
        private readonly IFuzzFromCollections _collectionFuzzer;
        

        // For NoDuplication mode
        private const int MaxFailingAttemptsForNoDuplicationDefaultValue = 100;
        private const int MaxRangeSizeAllowedForMemoizationDefaultValue = 1000000;
        private readonly Memoizer _memoizer = new Memoizer();
        private IFuzz _sideEffectFreeFuzzer;
        

        /// <summary>
        /// internal Fuzzer instance to be used by the various lambdas
        /// related to the NoDuplication mode (i.e. when the option is
        /// set to <b>true</b>).
        ///
        /// Ironically, the NoDuplication mode of this Fuzzer needs
        /// to use another fuzzer instance for the Lastchance mode (i.e. when the
        /// MaxFailingAttemptsForNoDuplicationDefaultValue has been
        /// reached).
        ///
        /// E.g.: if you call the <see cref="GenerateAge"/> method on a Fuzzer with
        /// NoDuplication set to <b>true</b> in a situation where the
        /// <see cref="MaxFailingAttemptsForNoDuplicationDefaultValue"/>
        /// was not enough to find another original value, the lastChance lambda
        /// of the <see cref="GenerateWithoutDuplication{T}"/> method will be called.
        ///
        /// In that case, since the last chance lambda of the <see cref="GenerateAge"/>
        /// method is using the <see cref="IFuzzNumbers.GenerateInteger"/> method,
        /// we need to avoid <see cref="StackOverflowException"/> by using
        /// a <see cref="SideEffectFreeSafeFuzzer"/> instance in that specific case
        /// (in all lastChance lambdas actually).
        /// </summary>
        private IFuzz SideEffectFreeSafeFuzzer => _sideEffectFreeFuzzer ?? (_sideEffectFreeFuzzer = new Fuzzer(this.Seed, noDuplication: false));

        /// <summary>
        /// Gets or sets the max number of attempts the Fuzzer should make in order to generate
        /// a not already provided value when <see cref="NoDuplication"/> mode
        /// is enabled (via constructor).
        /// </summary>
        public int MaxFailingAttemptsForNoDuplication { get; set; } =
            MaxFailingAttemptsForNoDuplicationDefaultValue;

        /// <summary>
        /// Gets or sets the maximum number of entries to be memoized if
        /// <see cref="NoDuplication"/> mode is enabled (via constructor).
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
        public bool NoDuplication { get; set; }

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
        /// <param name="noDuplication"><b>true</b> if you do not want the Fuzzer to provide you twice the same result for every fuzzing method type, <b>false</b> otherwise.</param>
        public Fuzzer(int? seed = null, string name = null, bool? noDuplication = false)
        {
            var seedWasProvided = seed.HasValue;

            seed = seed ??
                   new Random().Next(); // the seed is not specified? pick a random one for this Fuzzer instance.
            Seed = seed.Value;

            _internalRandom = new Random(seed.Value);

            name = name ?? GenerateFuzzerName();
            Name = name;

            noDuplication = noDuplication ?? false;
            NoDuplication = noDuplication.Value;

            LogSeedAndTestInformations(seed.Value, seedWasProvided, name);

            // Instantiates implementation types for the various Fuzzer
            _loremFuzzer = new LoremFuzzer(this);
            _stringFuzzer = new StringFuzzer(this);
            _numberFuzzer = new NumberFuzzer(this);
            _addressFuzzer = new AddressFuzzer(this);
            _personFuzzer = new PersonFuzzer(this);
            _dateTimeFuzzer = new DateTimeFuzzer(this);
            _typeFuzzer = new TypeFuzzer(this);
            _guidFuzzer = new GuidFuzzer(this);
            _collectionFuzzer = new CollectionFuzzer(this);
        }

        #region Core

        /// <summary>
        /// Returns a <see cref="IFuzz"/> instance that you can use to generate always different values from now
        /// (i.e. values that will be generated by this very specific <see cref="IFuzz"/> instance only).
        /// In other word, a <see cref="IFuzz"/> instance that will never return twice the same value (whatever the method called).
        /// </summary>
        /// <returns>A <see cref="IFuzz"/> instance that will never return twice the same value (whatever the method called).</returns>
        public IFuzz GenerateNoDuplicationFuzzer()
        {
            return new Fuzzer(Seed, noDuplication: true);
        }

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
        /// Methods to be used when <see cref="NoDuplication"/> is set to <b>true</b>
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
        ///     The maximum number of calls to the <paramref name="standardGenerationFunction"/>
        ///     we should try before we fall-back and call the
        ///     <paramref name="lastChanceGenerationFunction"/> lambda.
        /// </param>
        /// <param name="standardGenerationFunction">
        ///     The function to use in order to generate the thing(s) we want.
        ///     It should be the same function that the one we call for the cases
        ///     where <see cref="NoDuplication"/> is set to <b>false</b>.
        /// </param>
        /// <param name="lastChanceGenerationFunction">
        ///     The function to use in order to generate the thing(s) we want when
        ///     all the <paramref name="standardGenerationFunction"/> attempts have failed.
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
                            Func<IFuzz, T> standardGenerationFunction,
                            Func<IFuzz, SortedSet<object>, Maybe<T>> lastChanceGenerationFunction = null)
        {
            var memoizerKey = new MemoizerKey(currentMethod, argumentsHashCode);

            var maybe = TryGetNonAlreadyProvidedValuesWithRegularGenerationFunction<T>(memoizerKey, out var alreadyProvidedValues, standardGenerationFunction, maxFailingAttemptsBeforeLastChanceFunctionIsCalled);

            if (!maybe.HasItem)
            {
                if (lastChanceGenerationFunction != null)
                {
                    // last attempt, we randomly pick the missing bits from the memoizer
                    maybe = lastChanceGenerationFunction(SideEffectFreeSafeFuzzer, _memoizer.GetAlreadyProvidedValues(memoizerKey));
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(minValue, maxValue),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateInteger(minValue, maxValue),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindNotAlreadyProvidedInteger(alreadyProvidedSortedSet, minValue.Value, maxValue.Value, safeFuzzer));
            }

            return _numberFuzzer.GenerateInteger(minValue, maxValue);
        }
 
        private static Maybe<int> LastChanceToFindNotAlreadyProvidedInteger(SortedSet<object> alreadyProvidedValues, int? minValue, int? maxValue, IFuzz fuzzer)
        {
            minValue = minValue ?? int.MinValue;
            maxValue = maxValue ?? int.MaxValue;

            var allPossibleValues = Enumerable.Range(minValue.Value, maxValue.Value).ToArray();

            var remainingCandidates = allPossibleValues.Except<int>(alreadyProvidedValues.Cast<int>()).ToArray();

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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(maxValue),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GeneratePositiveInteger(maxValue));
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
            if (NoDuplication)
            {
                // We will only memoize if the range is not too wide
                var uRange = NumberExtensions.ComputeRange(minValue, maxValue);

                if (uRange <= MaxRangeSizeAllowedForMemoization)
                {
                    return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(minValue, maxValue),
                        maxFailingAttemptsBeforeLastChanceFunctionIsCalled:
                        MaxFailingAttemptsForNoDuplication,
                        standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateLong(minValue, maxValue),
                        lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindNotAlreadyProvidedLong(ref minValue, ref maxValue, alreadyProvidedSortedSet, safeFuzzer));
                }
            }

            return _numberFuzzer.GenerateLong(minValue, maxValue);
        }

        private static Maybe<long> LastChanceToFindNotAlreadyProvidedLong(ref long? minValue, ref long? maxValue, SortedSet<object> alreadyProvidedValues, IFuzz fuzzer)
        {
            minValue = minValue ?? long.MinValue;
            maxValue = maxValue ?? long.MaxValue;

            var allPossibleValues = GenerateAllPossibleOptions(minValue.Value, maxValue.Value);
            
            var remainingCandidates = allPossibleValues.Except<long>(alreadyProvidedValues.Cast<long>()).ToArray();
            
            if (remainingCandidates.Any())
            {
                var index = fuzzer.GenerateInteger(0, remainingCandidates.Length - 1);
                var randomRemainingNumber = remainingCandidates[index];

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

        #region Address

        /// <summary>
        /// Randomly generates an <see cref="Address"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/> of the address to generate.</param>
        /// <returns>The generated Address.</returns>
        public Address.Address GenerateAddress(Country? country = null)
        {
            return _addressFuzzer.GenerateAddress(country);
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(gender),
                    MaxFailingAttemptsForNoDuplication, 
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateFirstName(gender));
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(firstName),
                    MaxFailingAttemptsForNoDuplication, 
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateLastName(firstName),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindLastName(firstName, alreadyProvidedSortedSet, safeFuzzer));
            }

            return _personFuzzer.GenerateLastName(firstName);
        }

        private static Maybe<string> LastChanceToFindLastName(string firstName, SortedSet<object> alreadyProvidedValues, IFuzz fuzzer)
        {
            var continent = Locations.FindContinent(firstName);
            var allPossibleValues = LastNames.PerContinent[continent];

            var remainingCandidates = allPossibleValues.Except(alreadyProvidedValues.Cast<string>()).ToArray();

            if (remainingCandidates.Any())
            {
                var lastName = fuzzer.PickOneFrom(remainingCandidates);
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (fuzzerWithDuplicationAllowed) => fuzzerWithDuplicationAllowed.GenerateAge(),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindAge(alreadyProvidedSortedSet, 18, 97, safeFuzzer));
            }

            return _personFuzzer.GenerateAge();
        }

        private static Maybe<int> LastChanceToFindAge(SortedSet<object> alreadyProvidedValues, int minAge, int maxAge, IFuzz fuzzer)
        {
            var allPossibleValues = Enumerable.Range(minAge, maxAge - minAge).ToArray();

            var remainingCandidates = allPossibleValues.Except(alreadyProvidedValues.Cast<int>()).ToArray();

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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(firstName, lastName),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateEMail(firstName, lastName));
            }

            return _personFuzzer.GenerateEMail(firstName, lastName);
        }

        /// <summary>
        /// Generates a password following some common rules asked on the internet.
        /// </summary>
        /// <returns>The generated password</returns>
        public string GeneratePassword(int? minSize = null, int? maxSize = null, bool? includeSpecialCharacters = null)
        {
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(),
                    HashArguments(minSize, maxSize, includeSpecialCharacters),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GeneratePassword(minSize, maxSize, includeSpecialCharacters));
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication<T>(CaptureCurrentMethod(), HashArguments(candidates),
                    MaxFailingAttemptsForNoDuplication, 
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.PickOneFrom(candidates),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindNotAlreadyPickedValue(alreadyProvidedSortedSet, candidates, safeFuzzer));
            }

            return _collectionFuzzer.PickOneFrom(candidates);
        }

        private static Maybe<T> LastChanceToFindNotAlreadyPickedValue<T>(SortedSet<object> alreadyProvidedValues, IList<T> candidates, IFuzz fuzzer)
        {
            var allPossibleValues = candidates.ToArray();

            var remainingCandidates = allPossibleValues.Except<T>(alreadyProvidedValues.Cast<T>()).ToArray();

            if (remainingCandidates.Any())
            {
                var index = fuzzer.GenerateInteger(0, remainingCandidates.Length - 1);
                var pickOneFrom = remainingCandidates[index];

                return new Maybe<T>(pickOneFrom);
            }

            return new Maybe<T>();
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(feeling),
                    MaxFailingAttemptsForNoDuplication, 
                    standardGenerationFunction: safeFuzzer => safeFuzzer.GenerateAdjective(feeling),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindAdjective(feeling, alreadyProvidedSortedSet, safeFuzzer));
            }

            return _stringFuzzer.GenerateAdjective(feeling);
        }

        private static Maybe<string> LastChanceToFindAdjective(Feeling? feeling, SortedSet<object> alreadyProvidedValues, IFuzz fuzzer)
        {
            var allPossibleValues = Adjectives.PerFeeling[feeling.Value];

            var remainingCandidates = allPossibleValues.Except(alreadyProvidedValues.Cast<string>()).ToArray();

            if (remainingCandidates.Any())
            {
                var adjective = fuzzer.PickOneFrom(remainingCandidates);
                return new Maybe<string>(adjective);
            }

            return new Maybe<string>();
        }

        /// <summary>
        /// Generates a string from a given 'diverse' format (# for a single digit number, X for upper-cased letter, x for lower-cased letter). 
        /// </summary>
        /// <param name="diverseFormat">The 'diverse' format to use (# for a single digit number, X for upper-cased letter, x for lower-cased letter).</param>
        /// <returns>A randomly generated string following the 'diverse' format.</returns>
        public string GenerateStringFromPattern(string diverseFormat)
        {
            return _stringFuzzer.GenerateStringFromPattern(diverseFormat);
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication<T>(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsForNoDuplication, 
                    (safeFuzzer) => safeFuzzer.GenerateEnum<T>());
            }

            return _typeFuzzer.GenerateEnum<T>();
        }

        #endregion

        #region LoremFuzzer

        /// <summary>
        /// Generates random latin words.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="NoDuplication"/> mode.</remarks>
        /// <param name="number">(optional) Number of words to generate.</param>
        /// <returns>The generated latin words.</returns>
        public IEnumerable<string> GenerateWords(int? number = null)
        {
            return _loremFuzzer.GenerateWords(number);
        }

        /// <summary>
        /// Generate a sentence in latin.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="NoDuplication"/> mode.</remarks>
        /// <param name="nbOfWords">(optional) Number of words for this sentence.</param>
        /// <returns>The generated sentence in latin.</returns>
        public string GenerateSentence(int? nbOfWords = null)
        {
            return _loremFuzzer.GenerateSentence(nbOfWords);
        }

        /// <summary>
        /// Generates a paragraph in latin.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="NoDuplication"/> mode.</remarks>
        /// <param name="nbOfSentences">(optional) Number of sentences for this paragraph.</param>
        /// <returns>The generated paragraph in latin.</returns>
        public string GenerateParagraph(int? nbOfSentences = null)
        {
            return _loremFuzzer.GenerateParagraph(nbOfSentences);
        }

        /// <summary>
        /// Generates a collection of paragraphs. 
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="NoDuplication"/> mode.</remarks>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The collection of paragraphs.</returns>
        public IEnumerable<string> GenerateParagraphs(int? nbOfParagraphs = null)
        {
            return _loremFuzzer.GenerateParagraphs(nbOfParagraphs);
        }

        /// <summary>
        /// Generates a text in latin with a specified number of paragraphs.
        /// </summary>
        /// <remarks>This method won't be affected by the <see cref="NoDuplication"/> mode.</remarks>
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
            if (NoDuplication)
            {
                return GenerateWithoutDuplication(CaptureCurrentMethod(), HashArguments(),
                    MaxFailingAttemptsForNoDuplication,
                    standardGenerationFunction: (safeFuzzer) => safeFuzzer.GenerateLetter(),
                    lastChanceGenerationFunction: (safeFuzzer, alreadyProvidedSortedSet) => LastChanceToFindLetter(alreadyProvidedSortedSet, safeFuzzer));
            }

            return _loremFuzzer.GenerateLetter();
        }

        private static Maybe<char> LastChanceToFindLetter(SortedSet<object> alreadyProvidedValues, IFuzz fuzzer)
        {
            var allPossibleValues = LoremFuzzer.Alphabet;

            var remainingCandidates = allPossibleValues.Except(alreadyProvidedValues.Cast<char>()).ToArray();
            
            if (remainingCandidates.Any())
            {
                var letter = fuzzer.PickOneFrom<char>(remainingCandidates);
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