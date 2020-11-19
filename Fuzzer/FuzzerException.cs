using System;

namespace Fuzzers
{
    public class FuzzerException : Exception
    {
        public FuzzerException(string message): base(message)
        {
        }
    }
}