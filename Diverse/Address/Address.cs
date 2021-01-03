namespace Diverse.Address
{
    /// <summary>
    /// Address of a <see cref="Person"/>.
    /// </summary>
    public abstract class Address
    {
        private string _street;

        /// <summary>
        /// Gets the <see cref="AddressFormat"/> of the <see cref="Address"/>.
        /// </summary>
        public AddressFormat Format { get; }

        /// <summary>
        /// Gets or sets the number for this apartment/condo/house in the street.
        /// </summary>
        public int? StreetNumber { get; set; }

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
        public string StateProvinceArea { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Instantiates an <see cref="Address"/>.
        /// </summary>
        /// <param name="format">The <see cref="AddressFormat"/> for this Address.</param>
        /// <param name="countryCode">The <see cref="CountryCallingCode"/></param>
        protected Address(AddressFormat format)
        {
            Format = format;
        }
    }
}