namespace Diverse.Tests.Utils
{
    public class UserProfile
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Password { get; }
        public int Age { get; }

        public UserProfile(string firstName, string lastName, string email, string password, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Age = age;
        }
    }
}
