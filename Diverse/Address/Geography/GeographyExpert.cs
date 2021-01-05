using System.Collections.Generic;
using System.Linq;
using Diverse.Address.Geography.Countries;

// ReSharper disable StringLiteralTypo

namespace Diverse.Address.Geography
{
    /// <summary>
    /// Gathers all infos related to Geography (continent, countries, cities).
    /// </summary>
    /// <remarks>Note: All associations between cities, state/province/area and Countries are made here.</remarks>
    public static class GeographyExpert
    {
        private static string[] _cityNames;

        /// <summary>
        /// Gets the names of all the cities registered in the Diverse lib.
        /// </summary>
        public static string[] CityNames
        {
            get
            {
                if (_cityNames == null)
                {
                    _cityNames = CitiesOfTheWorld.Select(cc => cc.CityName).Distinct().ToArray();
                }

                return _cityNames;
            }

            set => _cityNames = value;
        }

        
        /// <summary>
        /// Gives an array with all the cities related to a <see cref="Country"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/>.</param>
        /// <returns>An array with all the cities related to a <see cref="Country"/>.</returns>
        public static string[] GiveMeCitiesOf(Country country)
        {
            return CitiesOfTheWorld
                .Where(cw => cw.Country == country)
                .Select(x => x.CityName)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Gives an array with all the <see cref="StateProvinceArea"/> related to a <see cref="Country"/>.
        /// /// </summary>
        /// <param name="country">The <see cref="Country"/>.</param>
        /// <returns>An array of all the <see cref="StateProvinceArea"/> registered for this <see cref="Country"/> in the Diverse lib.</returns>
        public static StateProvinceArea[] GiveMeStateProvinceAreaOf(Country country)
        {
            return CitiesOfTheWorld
                .Where(cw => cw.Country == country)
                .Select(x => x.StateProvinceArea)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Gets an array containing all the <see cref="CityWithRelatedInformation"/> registered in Diverse.
        /// </summary>
        public static CityWithRelatedInformation[] CitiesOfTheWorld
        {
            get
            {
                if (_citiesOfTheWorld == null)
                {
                    _citiesOfTheWorld = France.Cities
                        .Concat(China.Cities)
                        .Concat(UnitedStatesOfAmerica.Cities)
                        .ToArray();
                }
                
                return _citiesOfTheWorld;
            }
        }

        private static CityWithRelatedInformation[] _citiesOfTheWorld;

        private static readonly Dictionary<Country, string[]> _streetsOfCountries =
            new Dictionary<Country, string[]>
            {
                { Country.France, France.StreetNames },
                { Country.China, China.StreetNames },
                { Country.Usa, UnitedStatesOfAmerica.StreetNames },
            };

        /// <summary>
        /// Gives the <see cref="StateProvinceArea"/> of a given city name.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <returns>The <see cref="StateProvinceArea"/> where this city belongs.</returns>
        public static StateProvinceArea GiveMeStateProvinceAreaOf(string cityName)
        {
            return CitiesOfTheWorld.Single(c => c.CityName == cityName).StateProvinceArea;
        }

        /// <summary>
        /// Gives the <see cref="Country"/> of a given city name.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <returns>The <see cref="Country"/> where this city belongs.</returns>
        public static Country GiveMeCountryOf(string cityName)
        {
            return CitiesOfTheWorld.Single(c => c.CityName == cityName).Country;
        }

        /// <summary>
        /// Gives all the registered streets of a <see cref="Country"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/></param>
        /// <returns>All the streets of the provided <see cref="Country"/> that are registered in Diverse lib.</returns>
        public static string[] GiveMeStreetsOf(Country country)
        {
            return _streetsOfCountries[country];
        }

        /// <summary>
        /// Gives the ZipCode format registered for this city in Diverse.
        /// </summary>
        /// <param name="cityName">The name of the city you want to get the ZipCode format.</param>
        /// <returns>The ZipCode format registered for this city in Diverse.</returns>
        public static string GiveMeZipCodeFormatOf(string cityName)
        {
            return CitiesOfTheWorld.Single(c => c.CityName == cityName).ZipCodeFormat;
        }
    }
}