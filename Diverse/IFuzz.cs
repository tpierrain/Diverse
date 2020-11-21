using System;

namespace Diverse
{
    /// <summary>
    /// Interface to build your own <see cref="Fuzzer"/> through extension methods.
    /// </summary>
    public interface IFuzz
    {
        /// <summary>
        /// Gets a <see cref="Random"/> instance to use if you want your extensible Fuzzer to be deterministic when providing a seed.
        /// </summary>
        Random Random { get; }

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        int GenerateInteger(int minValue, int maxValue);

        /// <summary>
        /// Generates a random integer value.
        /// </summary>
        /// <returns>An integer value generated randomly.</returns>
        int GenerateInteger();

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <returns>A positive integer value generated randomly.</returns>
        int GeneratePositiveInteger();

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <returns>A positive decimal value generated randomly.</returns>
        decimal GeneratePositiveDecimal();

        /// <summary>
        /// Generates a 'Diverse' first name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="gender">The <see cref="Gender"/> to be used as indication (optional).</param>
        /// <returns>A 'Diverse' first name.</returns>
        string GenerateFirstName(Gender? gender = null);

        /// <summary>
        /// Generates a 'Diverse' last name (i.e. from all around the world and different cultures).
        /// </summary>
        /// <param name="firstName">The first name of this person.</param>
        /// <returns>A 'Diverse' last name.</returns>
        string GenerateLastName(string firstName);

        /// <summary>
        /// Generates a 'Diverse' <see cref="Person"/> (i.e. from all around the world and different cultures). 
        /// </summary>
        /// <param name="gender">The (optional) <see cref="Gender"/> of this <see cref="Person"/></param>
        /// <returns>A 'Diverse' <see cref="Person"/> instance.</returns>
        Person GenerateAPerson(Gender? gender = null);

        /// <summary>
        /// Generates a random Email.
        /// </summary>
        /// <param name="firstName">The (optional) first name for this Email</param>
        /// <param name="lastName">The (option) last name for this Email.</param>
        /// <returns>A random Email.</returns>
        string GenerateEMail(string firstName = null, string lastName = null);
        
        /// <summary>
        /// Flips a coin.
        /// </summary>
        /// <returns><b>True</b> if Heads; <b>False</b> otherwise (i.e. Tails).</returns>
        bool HeadsOrTails();
    }
}