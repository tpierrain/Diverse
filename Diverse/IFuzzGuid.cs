using System;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="Guid"/>.
    /// </summary>
    public interface IFuzzGuid
    {
        /// <summary>
        /// Generates a random <see cref="Guid"/>
        /// </summary>
        /// <returns>A random <see cref="Guid"/>.</returns>
        Guid GenerateGuid();
    }
}