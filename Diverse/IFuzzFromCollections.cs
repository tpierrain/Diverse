using System.Collections.Generic;

namespace Diverse
{
    /// <summary>
    /// Fuzz from collections.
    /// </summary>
    public interface IFuzzFromCollections
    {
        /// <summary>
        /// Randomly pick one element from a given collection.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns>One of the elements from the candidates collection.</returns>
        T PickOneFrom<T>(IList<T> candidates);
    }
}