using NFluent;
using NUnit.Framework;

namespace Fuzzers.Tests
{
    [TestFixture]
    public class FuzzerShould
    {
        [Test]
        public void Log_when_Log_is_registered()
        {
            var fuzzer = new Fuzzers.Fuzzer();
            var generateInteger = fuzzer.GenerateInteger(3, 5);

            Check.That(generateInteger).IsBefore(5).And.IsAfter(3);
        }
    }
}
