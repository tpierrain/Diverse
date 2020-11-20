using NUnit.Framework;

namespace Diverse.Tests
{
    [SetUpFixture]
    public class AllTestFixtures
    {
        [OneTimeSetUp]
        public void Init()
        {
            Fuzzer.Log = TestContext.WriteLine;
        }
    }
}