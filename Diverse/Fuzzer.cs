using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Diverse
{
    /// <summary>
    /// Allows to generate lots of combination of things. <see cref="Fuzzer"/> are very useful to detect hard coded values in our implementations.
    /// Note: you can instantiate another Deterministic Fuzzer by providing it the Seed you want to reuse.
    /// </summary>
    public class Fuzzer : IFuzz
    {
        /// <summary>
        /// Generates a DefaultSeed. Important to keep a trace of the used seed so that we can reproduce a failing situation with <see cref="Fuzzer"/> involved.
        /// </summary>
        public int Seed { get; }

        public string Name { get; }

        /// <summary>
        /// Exposes the Random instance to be used when we want to create a new extension method for the <see cref="Fuzzer"/>.
        /// <remarks>The use of explicit interface implementation for this property hide this internal mechanic details from the Fuzzer end-users ;-)</remarks>
        /// </summary>
        Random IFuzz.Random { get; set; } 

        /// <summary>
        /// Gives easy access to the <see cref="IFuzz.Random"/> explicit implementation.
        /// </summary>
        private Random InternalRandom => ((IFuzz)this).Random;

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

            ((IFuzz)this).Random = new Random(seed.Value);

            name = name ?? GenerateFuzzerName();
            Name = name;

            LogSeedAndTestInformations(seed.Value, seedWasProvided, name);
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
        /// Generates a random integer value between a min and a max value.
        /// </summary>
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
        /// Sets the way you want to log or receive what the <see cref="Fuzzer"/> has to say about every generated seeds used for every fuzzer instance & test.
        /// </summary>
        public static Action<string> Log { get; set; }


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
    }
}