using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Default locations of people in Diverse lib.
    /// </summary>
    internal class Locations
    {
        public static Continent FindContinent(string firstName)
        {
            Continent continent;
            var contextualizedFirstName = Male.ContextualizedFirstNames.FirstOrDefault(c => c.FirstName == firstName);
            if (contextualizedFirstName != null)
            {
                continent = contextualizedFirstName.Origin;
            }
            else
            {
                contextualizedFirstName = Female.ContextualizedFirstNames.FirstOrDefault(c => c.FirstName == firstName);
                if (contextualizedFirstName != null)
                {
                    continent = contextualizedFirstName.Origin;
                }
                else
                {
                    continent = Continent.Africa;
                }
            }

            return continent;
        }
    }
}