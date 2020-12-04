namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Player for any game (dummy class for testing purpose).
    /// </summary>
    internal class Player
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int Age { get; private set; }

        /// <summary>
        /// Useless constructor but interesting for our tests.
        /// </summary>
        private Player()
        {

        }

        /// <summary>
        /// Protected constructor interesting for our tests.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="age"></param>
        protected Player(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
    }
}