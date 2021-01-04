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
        public void Be_able_to_generate_proper_address_for_the_Country_of(Country country)
        {
            var fuzzer = new Fuzzer();

            var address = fuzzer.GenerateAddress(country);
            TestContext.WriteLine(address);

            Check.That(address.StreetNumber).IsNotEmpty();
            Check.That(address.Street).IsNotEmpty();

            Check.That(Geography.GiveMeStreetsOf(country)).Contains(address.StreetName);
            Check.That(Geography.GiveMeCitiesOf(country)).Contains(address.City);
            Check.That(Geography.GiveMeStateProvinceAreaOf(country)).Contains(address.StateProvinceArea);
            Check.ThatEnum(address.Country).IsEqualTo(country);

            Check.That(address.CountryLabel).IsNotEmpty();
        }
    }
}