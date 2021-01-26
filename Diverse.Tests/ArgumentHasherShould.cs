using System.Collections.Generic;
using System.Reflection;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class ArgumentHasherShould
    {
        [Test]
        public void Provide_the_same_hash_for_two_identical_Lists()
        {
            var rateCodes = new List<string> { "PRBA", "LRBAMC", "AVG1", "PRBB", "LRBA", "BBSP", "PRBA2", "LRBA2MC", "PRPH", "PBCITE_tes", "PRPH2", "PRP2N2", "PR3NS1", "PBHS", "PBSNWH", "PBTHE", "PBVPOM", "PBZOO", "PHBO", "PHBPP", "PAB01", "PHB01", "PH3P", "LH3PMC", "CAMI", "FRCAMIF", "GENERICRATECODE", "PBGGG", "PBPPBRAZIL", "PBSENIOR" };
            var identicalRateCodes = new List<string> { "PRBA", "LRBAMC", "AVG1", "PRBB", "LRBA", "BBSP", "PRBA2", "LRBA2MC", "PRPH", "PBCITE_tes", "PRPH2", "PRP2N2", "PR3NS1", "PBHS", "PBSNWH", "PBTHE", "PBVPOM", "PBZOO", "PHBO", "PHBPP", "PAB01", "PHB01", "PH3P", "LH3PMC", "CAMI", "FRCAMIF", "GENERICRATECODE", "PBGGG", "PBPPBRAZIL", "PBSENIOR" };

            var firstHash = ArgumentHasher.HashArguments(rateCodes);
            var secondHash = ArgumentHasher.HashArguments(identicalRateCodes);

            Check.That(secondHash).IsEqualTo(firstHash);
        }
    }
}