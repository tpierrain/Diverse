namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Player for any game. Dummy class for testing purpose (for Constructor-based Fuzzing).
    /// What matters here is to have at least one non-empty constructor.
    /// </summary>
    internal class PlayerWithProtectedConstructor
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int Age { get; private set; }

        /// <summary>
        /// Useless constructor but interesting for our tests.
        /// </summary>
        private PlayerWithProtectedConstructor()
        {

        }

        /// <summary>
        /// Protected constructor interesting for our tests.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="age"></param>
        protected PlayerWithProtectedConstructor(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
    }
}