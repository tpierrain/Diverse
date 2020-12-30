using System.Reflection;

namespace Diverse
{
    /// <summary>
    /// Key used by the internal Memoizer
    /// (enabled if AvoidDuplication is set to <b>true</b>).
    /// </summary>
    internal readonly struct MemoizerKey
    {
        public MethodBase FuzzerMethod { get; }

        public int ArgumentsHashCode { get; }

        public MemoizerKey(MethodBase fuzzerMethod, int argumentsHashCode)
        {
            FuzzerMethod = fuzzerMethod;
            ArgumentsHashCode = argumentsHashCode;
        }
        public override string ToString()
        {
            return $"MemoizerKey: '{FuzzerMethod}' - {ArgumentsHashCode}";
        }
    }
}