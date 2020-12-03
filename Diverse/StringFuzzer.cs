using System;
using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Fuzz <see cref="string"/> values.
    /// </summary>
    public class StringFuzzer : IFuzzStrings
    {
        private readonly IProvideCorePrimitivesToFuzzer _fuzzerPrimitives;

        public StringFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
        }

        private bool HeadsOrTails()
        {
            return _fuzzerPrimitives.Random.Next(0, 2) == 1;
        }

        public string GenerateAdjective(Feeling? feeling = null)
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
            
            var randomLocalIndex = _fuzzerPrimitives.Random.Next(0, adjectives.Length);

            return adjectives[randomLocalIndex];
        }
    }
}