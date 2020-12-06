using System;

namespace Diverse
{
    /// <summary>
    /// List a set of Types that are...
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// Gets a set of <see cref="Type"/> that are covered by a Fuzzer.
        /// </summary>
        public static readonly Type[] CoveredByAFuzzer = new Type[]
        {
            typeof(DateTime), 
            typeof(int),
            typeof(long),
            typeof(bool),
            typeof(decimal),
            typeof(string),
        };
    }
}