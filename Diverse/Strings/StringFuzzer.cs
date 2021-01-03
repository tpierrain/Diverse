using System.Collections.Generic;
using System.Text;

namespace Diverse.Strings
{
    /// <summary>
    /// Fuzz <see cref="string"/> values.
    /// </summary>
    internal class StringFuzzer : IFuzzStrings
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="StringFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public StringFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates a random adjective based on a feeling.
        /// </summary>
        /// <param name="feeling">The expected feeling of the adjective</param>
        /// <returns>An adjective based on a particular feeling or random one if not provided</returns>
        public string GenerateAdjective(Feeling? feeling = null)
        {
            if (!feeling.HasValue)
            {
                var isPositive = _fuzzer.HeadsOrTails();
                feeling = isPositive ? Feeling.Positive : Feeling.Negative;
            }

            var adjectives = Adjectives.PerFeeling[feeling.Value];

            var randomLocalIndex = _fuzzer.Random.Next(0, adjectives.Length);

            return adjectives[randomLocalIndex];
        }

        /// <summary>
        /// Generates a string from a given 'diverse' format.
        /// </summary>
        /// <param name="diverseFormat">The 'diverse' format to use.</param>
        /// <returns>A randomly generated string followin the 'diverse' format.</returns>
        public string GenerateFromPattern(string diverseFormat)
        {
            var builder = new StringBuilder(diverseFormat.Length);
            foreach (var c in diverseFormat.ToCharArray())
            {
                switch (c)
                {
                    case '#':
                        builder.Append(_fuzzer.GenerateInteger(0,9));
                        break;

                    case 'X': builder.Append(_fuzzer.GenerateLetter().ToString().ToUpper());
                        break;

                    case 'x': builder.Append(_fuzzer.GenerateLetter());
                        break;

                    default: builder.Append(c);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}