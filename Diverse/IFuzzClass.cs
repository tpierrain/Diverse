namespace Diverse
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    public interface IFuzzClass
    {
        /// <summary>
        /// Generates an instance of T for your tests
        /// </summary>
        /// <returns>An instance with string properties set to random strings</returns>
        T GenerateInstance<T>();
    }
}