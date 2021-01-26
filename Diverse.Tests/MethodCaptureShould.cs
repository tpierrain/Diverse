using System.Collections.Generic;
using System.Reflection;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class MethodCaptureShould
    {
        [Test]
        public void Be_able_to_capture_a_generic_method()
        {
            var helperForTestingPurpose = new DummyHelperForTestingPurpose();

            var rateCodes = new[] { "PRBA", "LRBAMC", "AVG1", "PRBB", "LRBA", "BBSP", "PRBA2", "LRBA2MC", "PRPH", "PBCITE_tes", "PRPH2", "PRP2N2", "PR3NS1", "PBHS", "PBSNWH", "PBTHE", "PBVPOM", "PBZOO", "PHBO", "PHBPP", "PAB01", "PHB01", "PH3P", "LH3PMC", "CAMI", "FRCAMIF", "GENERICRATECODE", "PBGGG", "PBPPBRAZIL", "PBSENIOR" };

            var methodBase = helperForTestingPurpose.Capturable(rateCodes);
            var secondCallMethodBase = helperForTestingPurpose.Capturable(rateCodes);

            Check.That(secondCallMethodBase).IsEqualTo(methodBase);
        }
    }

    public class DummyHelperForTestingPurpose
    {
        public MethodBase Capturable<T>(IList<T> candidates)
        {

            return MethodCapture.CaptureCurrentMethod();
        }
    }

}