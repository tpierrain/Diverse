using Diverse.Address;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="Address"/>.
    /// </summary>
    public interface IFuzzAddress
    {
        /// <summary>
        /// Randomly generates an <see cref="Address"/>.
        /// </summary>
        /// <returns>The generated Address.</returns>
        Address.Address GenerateAddress();
    }
}