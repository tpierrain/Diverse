namespace Diverse.Address
{
    /// <summary>
    /// International Telecommunication Union (ITU) Calling code for a Country.
    /// </summary>
    public class CountryCallingCode
    {
        /// <summary>
        /// Prefix for this ITU calling code.
        /// </summary>
        public string Prefix { get; }
        
        /// <summary>
        /// Code for the Country.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Instantiates a <see cref="CountryCallingCode"/>.
        /// </summary>
        /// <param name="code">Code of the country.</param>
        /// <param name="prefix">Prefix for this Country ITU.</param>
        public CountryCallingCode(int code, string prefix = "+")
        {
            Code = code;
            Prefix = prefix;
        }

        /// <summary>
        /// Returns the string description of a <see cref="CountryCallingCode"/>.
        /// </summary>
        /// <returns>The string description of a <see cref="CountryCallingCode"/>.</returns>
        public override string ToString()
        {
            return $"{Prefix}{Code}";
        }
    }
}