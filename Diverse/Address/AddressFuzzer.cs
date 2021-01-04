namespace Diverse.Address
{
    /// <summary>
    /// Fuzz <see cref="Address"/>.
    /// </summary>
    internal class AddressFuzzer : IFuzzAddress
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="AddressFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public AddressFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Randomly generates an <see cref="Address"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/> of the address to generate.</param>
        /// <returns>The generated Address.</returns>
        public Address GenerateAddress(Country? country = null)
        {
            country = country ?? Country.France;

            var streetNumber = _fuzzer.GenerateInteger(1, 390);
            var streetName = _fuzzer.PickOneFrom(Geography.GiveMeStreetsOf(country.Value));
            var city = _fuzzer.PickOneFrom(Geography.GiveMeCitiesOf(country.Value));

            var zipCode = _fuzzer.GenerateStringFromPattern(Geography.GiveMeZipCodeFormatOf(city));
            var stateProvinceArea = Geography.GiveMeStateProvinceAreaOf(city);

            var address = new Address(streetNumber.ToString(), streetName, city, zipCode, stateProvinceArea, country.Value);

            return address;
        }
    }
}