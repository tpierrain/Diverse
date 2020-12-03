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
    }
}