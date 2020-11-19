using NUnit.Framework;

namespace Fuzzers.Tests
{
    [SetUpFixture]
    public class AllTestFixtures
    {
        [OneTimeSetUp]
        public void Init()
        {
            Fuzzers.Fuzzer.Log = TestContext.WriteLine;
        }
    }
}