using System;

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
        /// <returns>An instance of type T with some fuzzed properties.</returns>
        T GenerateInstance<T>();

        /// <summary>
        /// Generates an instance of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Enum"/></typeparam>
        /// <returns>An random value of the specified <see cref="Enum"/> type.</returns>
        T GenerateEnum<T>();
    }
}