using System;

namespace Fuzzers
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz
    {
        Random Random { get; set; }
        int GenerateInteger(int minValue, int maxValue);
        int GenerateInteger();
        int GeneratePositiveInteger();
        decimal GeneratePositiveDecimal();
    }
}