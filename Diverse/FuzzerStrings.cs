using System;
using System.Linq;

namespace Diverse
{
    public class FuzzerStrings : IFuzzStrings
    {
        private Random _internalRandom;

        public FuzzerStrings(Random random)
        {
            _internalRandom = random;
        }

        private bool HeadsOrTails()
        {
            return _internalRandom.Next(0, 2) == 1;
        }

        public string GenerateString(Feeling? feeling = null)
        {
            string[] adjectives;
            if(feeling.HasValue)
            {
                adjectives = Adjectives.PerFeeling[feeling.Value];

            }
            else
            {
                bool isPositive = HeadsOrTails();
                feeling = isPositive ? Feeling.Positive : Feeling.Negative;
                adjectives = Adjectives.PerFeeling[feeling.Value];
            }
            
            var randomLocalIndex = _internalRandom.Next(0, adjectives.Length);

            return adjectives[randomLocalIndex];
        }
    }
}