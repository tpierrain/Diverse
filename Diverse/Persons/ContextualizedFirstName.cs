namespace Diverse
{
    /// <summary>
    /// A first name with some related information (like the <see cref="Origin"/> of it).
    /// </summary>
    public class ContextualizedFirstName
    {
        /// <summary>
        /// Instantiates a <see cref="ContextualizedFirstName"/>.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="origin">The possible origin or the <see cref="Continent"/> where you can find lots of people having this first name.</param>
        public ContextualizedFirstName(string firstName, Continent origin)
        {
            FirstName = firstName;
            Origin = origin;
        }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// The <see cref="Continent"/> where this first name may be originated. It may also be the <see cref="Continent"/> where lots of people have this first name (and not the real Origin for it).
        /// </summary>
        public Continent Origin { get; }
    }
}