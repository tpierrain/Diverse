using System;
using System.Collections.Generic;

namespace Diverse
{
    /// <summary>
    /// Represents errors that occur when <see cref="Fuzzer.AvoidDuplication"/> is set to <b>true</b>
    /// and we couldn't find a non-already provided value after the max number
    /// of attempts configured within Diverse. 
    /// </summary>
    public class DuplicationException : Exception
    {
        /// <summary>
        /// Instantiates a <see cref="DuplicationException"/>.
        /// </summary>
        /// <param name="typeRequested">Type to be generated.</param>
        /// <param name="maxAttemptsToFindNotAlreadyProvidedValue">Maximum number of attempts achieved before throwing this exception.</param>
        /// <param name="alreadyProvidedValues">Set of already provided values.</param>
        public DuplicationException(Type typeRequested, int maxAttemptsToFindNotAlreadyProvidedValue,
            SortedSet<object> alreadyProvidedValues) : base($"Couldn't find a non-already provided value of {typeRequested} after {maxAttemptsToFindNotAlreadyProvidedValue} attempts. Already provided values: {string.Join(", ", alreadyProvidedValues) }. In your case, try to increase the value of the {nameof(Fuzzer.MaxFailingAttemptsToFindNotAlreadyProvidedValue)} property for your Fuzzer.")
        {
            
        }
    }
}