using System;

namespace Diverse.Address
{
    /// <summary>
    /// Address of a <see cref="Person"/>.
    /// </summary>
    public class Address
    {
        private string _street;

        /// <summary>
        /// Gets or sets the number for this apartment/condo/house in the street.
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>
        /// Gets or sets the street name.
        /// </summary>
        public string StreetName { get; set; }

        /// <summary>
        /// Gets or sets the Zip code.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the street part of the address. By default, this includes the <see cref="StreetNumber"/>
        /// and the <see cref="StreetName"/>.
        /// </summary>
        public string Street
        {
            get
            {
                if (_street == null)
                {
                    return $"{StreetNumber} {StreetName}";
                }

                return _street;
            }
            set => _street = value;
        }

        /// <summary>
        /// Gets or sets the City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the State/Province/Area.
        /// </summary>
        public StateProvinceArea StateProvinceArea { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string CountryLabel { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// Instantiates an <see cref="Address"/>.
        /// </summary>
        /// <param name="streetNumber">The street number</param>
        /// <param name="streetName">The street name</param>
        /// <param name="city">The city</param>
        /// <param name="zipCode">The zip code</param>
        /// <param name="stateProvinceArea">The <see cref="StateProvinceArea"/></param>
        /// <param name="country">The <see cref="Country"/></param>
        public Address(string streetNumber, string streetName, string city, string zipCode, StateProvinceArea stateProvinceArea, Country country)
        {
            StreetName = streetName;
            City = city;
            ZipCode = zipCode;
            StateProvinceArea = stateProvinceArea;
            Country = country;
            CountryLabel = country.ToString();
            StreetNumber = streetNumber;
        }

        /// <summary>
        /// Returns the string description of a <see cref="Address"/>.
        /// </summary>
        /// <returns>The string description of a <see cref="Address"/>.</returns>
        public override string ToString()
        {
            return $"{Street}{Environment.NewLine}{ZipCode} - {City}{Environment.NewLine}{Country}";
        }
    }
}