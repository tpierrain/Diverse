using System;
using System.Collections.Generic;

namespace Diverse
{
    /// <summary>
    /// Accumulates generated values during constructor parameter fuzzing,
    /// enabling cross-parameter dependencies (e.g., lastName can use the firstName
    /// generated just before to produce a culturally coherent last name).
    /// </summary>
    internal class ConstructorFuzzingContext
    {
        private readonly Dictionary<string, object> _generatedValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Stores a generated value for later retrieval by other parameters.
        /// </summary>
        /// <param name="parameterName">The parameter or property name.</param>
        /// <param name="value">The generated value.</param>
        public void Store(string parameterName, object value)
        {
            if (parameterName != null)
            {
                _generatedValues[parameterName] = value;
            }
        }

        /// <summary>
        /// Tries to retrieve a previously generated value by parameter name.
        /// </summary>
        /// <param name="parameterName">The parameter name to look up.</param>
        /// <param name="value">The previously generated value, if found.</param>
        /// <returns><b>true</b> if a value was found, <b>false</b> otherwise.</returns>
        public bool TryGet(string parameterName, out object value)
        {
            return _generatedValues.TryGetValue(parameterName, out value);
        }
    }
}
