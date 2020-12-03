namespace Diverse
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz : IProvideCorePrimitivesToFuzzer, IFuzzNumbers, IFuzzPersons, IFuzzDatesAndTime
    {
    }
}