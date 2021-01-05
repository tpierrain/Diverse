using Country = Diverse.Address.Geography.Country;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="Address"/>.
    /// </summary>
    public interface IFuzzAddress
    {
        /// <summary>
        /// Randomly generates an <see cref="Address.Address"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/> of the address to generate.</param>
        /// <returns>The generated Address.</returns>
        Address.Address GenerateAddress(Country? country = null);
    }
}