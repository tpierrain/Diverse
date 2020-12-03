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

        /// <summary>
        /// Instantiates a <see cref="StringFuzzer"/>.
        /// </summary>
        /// <param name="fuzzerPrimitives"></param>
        public StringFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
        }

        /// <summary>
        /// Generates a random adjective based on a feeling.
        /// </summary>
        /// <param name="feeling">The expected feeling of the adjective</param>
        /// <returns>An adjective based on a particular feeling or random one if not provided</returns>
        public string GenerateAdjective(Feeling? feeling = null)
        {
            string[] adjectives;
            if(feeling.HasValue)
            {
                adjectives = Adjectives.PerFeeling[feeling.Value];

            }
            else
            {
                bool isPositive = _fuzzerPrimitives.HeadsOrTails();
                feeling = isPositive ? Feeling.Positive : Feeling.Negative;
                adjectives = Adjectives.PerFeeling[feeling.Value];
            }
            
            var randomLocalIndex = _fuzzerPrimitives.Random.Next(0, adjectives.Length);

            return adjectives[randomLocalIndex];
        }
    }
}