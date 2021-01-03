using Diverse.Strings;

namespace Diverse
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    public interface IFuzzStrings
    {
        /// <summary>
        /// Generates a random adjective based on a feeling.
        /// </summary>
        /// <param name="feeling">The expected feeling of the adjective</param>
        /// <returns>An adjective based on a particular feeling or random one if not provided</returns>
        string GenerateAdjective(Feeling? feeling);

        /// <summary>
        /// Generates a string from a given 'diverse' format.
        /// </summary>
        /// <param name="diverseFormat">The 'diverse' format to use.</param>
        /// <returns>A randomly generated string followin the 'diverse' format.</returns>
        string GenerateStringFromPattern(string diverseFormat);
    }
}