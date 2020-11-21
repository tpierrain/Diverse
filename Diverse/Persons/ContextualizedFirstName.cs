namespace Diverse
{
    public class ContextualizedFirstName
    {
        public ContextualizedFirstName(string firstName, Continent origin)
        {
            FirstName = firstName;
            Origin = origin;
        }

        public string FirstName { get; }

        public Continent Origin { get; }
    }
}