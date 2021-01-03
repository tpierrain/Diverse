using System;

namespace Diverse.Address
{
    /// <summary>
    /// French address (including how it is ToString() etc).
    /// </summary>
    public class FrenchAddress : Address
    {
        public FrenchAddress(int streetNumber, string streetName, string city, string zipCode, StateProvinceArea stateProvinceArea, Country country) : base(AddressFormat.French)
        {
            StreetName = streetName;
            City = city;
            ZipCode = zipCode;
            StateProvinceArea = stateProvinceArea.ToString();
            Country = country.ToString();
            StreetNumber = streetNumber;
        }

        public override string ToString()
        {
            return $"{Street}{Environment.NewLine}{ZipCode} - {City}{Environment.NewLine}{Country}";
        }
    }
}