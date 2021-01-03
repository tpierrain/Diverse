using Diverse.Address;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class AddressFuzzerShould
    {
        [TestCase(Country.France)]
        [TestCase(Country.China)]
        public void Be_able_to_generate_addressForTheCountryOf(Country country)
        {
            var fuzzer = new Fuzzer();

            var address = fuzzer.GenerateAddress(country);
            TestContext.WriteLine(address);

            Check.That(address.StreetNumber).IsInstanceOf<int?>().And.IsNotEqualTo(0);
            Check.That(address.StreetName).IsNotEmpty();
            Check.That(address.Street).IsNotEmpty();
            Check.That(address.City).IsNotEmpty();
            Check.That(address.StateProvinceArea).IsNotEmpty();
            Check.That(address.CountryLabel).IsNotEmpty();
            Check.ThatEnum(address.Country).IsEqualTo(country);
        }
    }
}