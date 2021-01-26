using System.Collections;

namespace Diverse
{
    /// <summary>
    /// Hash arguments (so that we can Memoize).
    /// </summary>
    public class ArgumentHasher
    {
        /// <summary>
        /// Hash provided arguments.
        /// </summary>
        /// <param name="arguments">the </param>
        /// <returns></returns>
        public static int HashArguments(params object[] arguments)
        {
            var hash = 17;
            foreach (var argument in arguments)
            {
                if (argument is IEnumerable enumerable)
                {
                    var parameterHashCode = GetByValueHashCode(enumerable);
                    hash = hash * 23 + parameterHashCode;
                }
                else
                {
                    var parameterHashCode = argument?.GetHashCode() ?? 17;
                    hash = hash * 23 + parameterHashCode;
                }
            }

            return hash;
        }

        /// <summary>
        /// Handle the case of collections (where we want to compare their content and not the reference of the collection).
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static int GetByValueHashCode(IEnumerable collection)
        {
            var hash = 17;

            foreach (var element in collection)
            {
                var parameterHashCode = element?.GetHashCode() ?? 17;
                hash = hash * 23 + parameterHashCode;
            }

            return hash;
        }
    }
}