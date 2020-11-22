using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class FuzzerThatIsExtensibleShould
    {
        [Test]
        public void Be_able_to_have_extension_methods()
        {
            var fuzzer = new Fuzzer(1245650948);

            // we have access to all our extension methods on our Fuzzer
            // (here: GenerateAVerySpecificAge())
            Age age = fuzzer.GenerateAVerySpecificAge();

            Check.That(age.Confidential).IsTrue();
            Check.That(age.Years).IsEqualTo(59);
        }
    }

    public static class FuzzerExtensions
    {
        public static Age GenerateAVerySpecificAge(this IFuzz fuzzer)
        {
            // Here, we can have access to all the existing methods 
            // exposed by the IFuzz interface
            var years = fuzzer.GeneratePositiveInteger(97);

            // or this one (very useful)
            var isConfidential = fuzzer.HeadsOrTails();

            // For very specific needs, you have to use the
            // Random property of the Fuzzer
            var aDoubleForInstance = fuzzer.Random.NextDouble();

            return new Age(years, isConfidential);
        }
    }

    /// <summary>
    /// A dummy type to show how to extend <see cref="IFuzz"/> with extension methods.
    /// </summary>
    public class Age
    {
        public Age(int years, bool isConfidential)
        {
            Confidential = isConfidential;
            Years = years;
        }

        public bool Confidential { get; }
        public int Years { get; }
    }
}