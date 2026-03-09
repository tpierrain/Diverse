using System;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// A deep hierarchy of types with no cycle: Level1 → Level2 → Level3 → Level4 → Level5.
    /// Tests that the TypeFuzzer handles deep non-cyclic hierarchies without issue.
    /// </summary>
    public class Level1
    {
        public string Name { get; }
        public int Value { get; }
        public Level2 Child { get; }

        public Level1(string name, int value, Level2 child)
        {
            Name = name;
            Value = value;
            Child = child;
        }
    }

    public class Level2
    {
        public string Name { get; }
        public DateTime Date { get; }
        public Level3 Child { get; }

        public Level2(string name, DateTime date, Level3 child)
        {
            Name = name;
            Date = date;
            Child = child;
        }
    }

    public class Level3
    {
        public string Name { get; }
        public decimal Amount { get; }
        public Level4 Child { get; }

        public Level3(string name, decimal amount, Level4 child)
        {
            Name = name;
            Amount = amount;
            Child = child;
        }
    }

    public class Level4
    {
        public string Name { get; }
        public bool Active { get; }
        public Level5 Child { get; }

        public Level4(string name, bool active, Level5 child)
        {
            Name = name;
            Active = active;
            Child = child;
        }
    }

    public class Level5
    {
        public string Name { get; }
        public long Code { get; }

        public Level5(string name, long code)
        {
            Name = name;
            Code = code;
        }
    }
}
