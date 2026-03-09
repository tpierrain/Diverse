namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Three types forming an indirect cycle: CycleX → CycleY → CycleZ → CycleX.
    /// Used to test that the TypeFuzzer detects indirect cycles without stack overflow.
    /// </summary>
    public class CycleX
    {
        public string Name { get; }
        public CycleY Next { get; }

        public CycleX(string name, CycleY next)
        {
            Name = name;
            Next = next;
        }
    }

    public class CycleY
    {
        public string Name { get; }
        public CycleZ Next { get; }

        public CycleY(string name, CycleZ next)
        {
            Name = name;
            Next = next;
        }
    }

    public class CycleZ
    {
        public string Name { get; }
        public CycleX Next { get; }

        public CycleZ(string name, CycleX next)
        {
            Name = name;
            Next = next;
        }
    }
}
