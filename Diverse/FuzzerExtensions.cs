using System.Linq;

namespace Diverse
{
    public static class FuzzerExtensions
    {
        /// <summary>
        /// Randomly pick one element from a given collection.
        /// </summary>
        /// <param name="fuzzer"></param>
        /// <param name="candidates"></param>
        /// <returns>One of the elements from the candidates collection.</returns>
        public static T PickOneFrom<T>(this IFuzzFromCollections fuzzer, params T[] candidates)
        {
            return fuzzer.PickOneFrom(candidates.ToList());
        }
    }
}