using System;

namespace Diverse
{
    /// <summary>
    /// An exception related to Fuzzing.
    /// </summary>
    public class FuzzerException : Exception
    {
        /// <summary>
        /// Instantiates a <see cref="FuzzerException"/>.
        /// </summary>
        /// <param name="message">The message of the <see cref="FuzzerException"/>.</param>
        public FuzzerException(string message): base(message)
        {
        }
    }
}