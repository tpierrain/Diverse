namespace Diverse
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

        public Address GenerateAddress()
        {
            var country = Country.France;

            var streetNumber = _fuzzer.GenerateInteger(1, 390);
            var streetName = _fuzzer.PickOneFrom(Geography.GiveMeStreetsOf(country));
            var city = _fuzzer.PickOneFrom(Geography.GiveMeCitiesOf(country));
            var zipCode = _fuzzer.GenerateInteger(13000, 75020).ToString();
            var stateProvinceArea = Geography.GiveMeStateProvinceAreaOf(city);
            //var country = Geography.GiveMeCountryOf(city);
           
            var address = new FrenchAddress(streetNumber, streetName, city, zipCode, stateProvinceArea, country);

            return address;
        }
    }
}