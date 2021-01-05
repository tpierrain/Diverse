// ReSharper disable CommentTypo
namespace Diverse.Address.Geography
{
    /// <summary>
    /// Information related to a City.
    /// </summary>
    public class CityWithRelatedInformation
    {
        /// <summary>
        /// Gets the name of the city
        /// </summary>
        public string CityName { get; }
        
        /// <summary>
        /// Gets the format to generate Zip code that is realistic with this city (used by <see cref="IFuzzStrings.GenerateStringFromPattern"/>).
        /// </summary>
        public string ZipCodeFormat { get; }
        
        /// <summary>
        /// Gets the State/Province/Area of this City.
        /// </summary>
        public StateProvinceArea StateProvinceArea { get; }
        
        /// <summary>
        /// Gets the <see cref="Country"/> of this City.
        /// </summary>
        public Country Country { get; }
        
        /// <summary>
        /// Gets the <see cref="Continent"/> of this City.
        /// </summary>
        public Continent Continent { get; }

        /// <summary>
        /// Instantiates a <see cref="CityWithRelatedInformation"/>.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <param name="stateProvinceArea">The State/Province/Area of the city.</param>
        /// <param name="country">The <see cref="Country"/> of this City.</param>
        /// <param name="continent">The <see cref="Continent"/> of this City.</param>
        /// <param name="zipCodeFormat">The format to be used in order to generate Zip code that is realistic with this city (used by <see cref="IFuzzStrings.GenerateStringFromPattern"/>).</param>
        public CityWithRelatedInformation(string cityName, StateProvinceArea stateProvinceArea, Country country, Continent continent, string zipCodeFormat = null)
        {
            CityName = cityName;
            ZipCodeFormat = zipCodeFormat ?? "######";
            StateProvinceArea = stateProvinceArea;
            Country = country;
            Continent = continent;
        }
    }
}