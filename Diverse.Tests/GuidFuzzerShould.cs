using System;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class GuidFuzzerShould
    {
        [Test]
        public void Generate_random_but_deterministic_Guid()
        {
            var fuzzer = new Fuzzer(1898737139);
            var guid  = fuzzer.GenerateGuid();
            Check.That(guid).IsEqualTo(Guid.Parse("4b730026-135c-a175-888f-2fdda993e237"));

            guid = fuzzer.GenerateGuid();
            Check.That(guid).IsEqualTo(Guid.Parse("9a8471f9-0a1d-ccb7-f58a-e690f92ff5b2"));
        }
    }
}