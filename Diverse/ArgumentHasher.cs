using System.Collections;

namespace Diverse
{
    /// <summary>
    /// Hash arguments (so that we can Memoize).
    /// </summary>
    public class ArgumentHasher
    {
        public static int HashArguments(params object[] parameters)
        {
            var hash = 17;
            foreach (var parameter in parameters)
            {
                if (parameter is IEnumerable)
                {
                    var parameterHashCode = GetByValueHashCode(parameter as IEnumerable);
                    hash = hash * 23 + parameterHashCode;
                }
                else
                {
                    var parameterHashCode = parameter?.GetHashCode() ?? 17;
                    hash = hash * 23 + parameterHashCode;
                }
            }

            return hash;
        }

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