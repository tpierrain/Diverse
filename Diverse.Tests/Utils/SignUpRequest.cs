namespace Diverse.Tests.Utils
{
    public class SignUpRequest
    {
        public string Login { get; }
        public string EMail { get; }
        public string Password { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PhoneNumber { get; }

        public SignUpRequest(string login, string password, string firstName, string lastName, string phoneNumber)
        {
            Login = login;
            EMail = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }
    }
}