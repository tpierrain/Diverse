using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Fuzzers
{
    /// <summary>
    /// Allows to generate lots of combination of things. <see cref="Fuzzer"/> are very useful to detect hard coded values in our implementations.
    /// Note: you can instantiate another Deterministic Fuzzer by providing it the Seed you want to reuse.
    /// </summary>
    public class Fuzzer : IFuzz
    {
        /// <summary>
        /// Generate a DefaultSeed. Important to keep a trace of the used seed so that we can reproduce a failing situation with <see cref="Fuzzer"/> involved.
        /// </summary>
        public int Seed { get; }

        public string Name { get; }

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

            DumpSeedIntoTestContext(seed.Value, seedWasProvided, name);
        }

        public decimal GeneratePositiveDecimal(int? seed = null)
        {
            return Convert.ToDecimal(GenerateInteger(0, int.MaxValue));
        }

        public int GenerateInteger(int minValue, int maxValue)
        {
            return InternalRandom.Next(minValue, maxValue);
        }

        public int GenerateInteger()
        {
            return GenerateInteger(int.MinValue, int.MaxValue);
        }

        public int GeneratePositiveInteger()
        {
            return GenerateInteger(0, int.MaxValue);
        }

        public static Action<string> Log { get; set; }

        private static void DumpSeedIntoTestContext(int seed, bool seedWasProvided, string fuzzerName)
        {
            var testName = GetNameOfTheTestInvolved();

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

        private static string GetNameOfTheTestInvolved()
        {
            var testName = "(not found)";
            try
            {
                var stackTrace = new StackTrace();

                var testMethod = stackTrace.GetFrames()
                    .Select(sf => sf.GetMethod())
                    .First(mb => IsATestMethod(mb));

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