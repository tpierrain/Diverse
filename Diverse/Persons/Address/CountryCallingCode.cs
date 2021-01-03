namespace Diverse
{
    /// <summary>
    /// International Telecommunication Union (ITU) Calling code for a Country.
    /// </summary>
    public class CountryCallingCode
    {
        public string Prefix { get; }
        
        public int Code { get; }

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