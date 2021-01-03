// ReSharper disable CommentTypo
namespace Diverse
{
    internal class ContextualizedCity
    {
        public string CityName { get; }
        public StateProvinceArea StateProvinceArea { get; }
        public Country Country { get; }
        public Continent Continent { get; }

        public ContextualizedCity(string cityName, StateProvinceArea stateProvinceArea,  Country country, Continent continent)
        {
            CityName = cityName;
            StateProvinceArea = stateProvinceArea;
            Country = country;
            Continent = continent;
        }
    }
}