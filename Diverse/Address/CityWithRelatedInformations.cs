// ReSharper disable CommentTypo
namespace Diverse.Address
{
    internal class CityWithRelatedInformations
    {
        public string CityName { get; }
        public string ZipCodeFormat { get; }
        public StateProvinceArea StateProvinceArea { get; }
        public Country Country { get; }
        public Continent Continent { get; }

        public CityWithRelatedInformations(string cityName, StateProvinceArea stateProvinceArea,
            Country country, Continent continent, string zipCodeFormat = null)
        {
            CityName = cityName;
            ZipCodeFormat = zipCodeFormat ?? "######";
            StateProvinceArea = stateProvinceArea;
            Country = country;
            Continent = continent;
        }
    }
}