using System;

namespace Diverse
{
    public class FuzzerException : Exception
    {
        public FuzzerException(string message): base(message)
        {
        }
    }
}