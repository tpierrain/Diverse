namespace Diverse
{
    /// <summary>
    /// Fuzz instance of Types.
    /// </summary>
    public interface IFuzzTypes
    {
        /// <summary>
        /// Generates an instance of a type T.
        /// </summary>
        /// <returns>An instance with string properties set to random strings</returns>
        T GenerateInstance<T>();
    }
}