namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Two types that reference each other (A ↔ B cycle).
    /// Used to test that the TypeFuzzer handles mutual references without stack overflow.
    /// </summary>
    public class TypeA
    {
        public string Name { get; }
        public int Value { get; }
        public TypeB Partner { get; }

        public TypeA(string name, int value, TypeB partner)
        {
            Name = name;
            Value = value;
            Partner = partner;
        }
    }

    public class TypeB
    {
        public string Label { get; }
        public TypeA Friend { get; }

        public TypeB(string label, TypeA friend)
        {
            Label = label;
            Friend = friend;
        }
    }
}
