namespace Diverse.Tests.Utils
{
    public class SignUpResponse
    {
        public SignUpResponse(string login)
        {
            Login = login;
        }

        public string Login { get; }
        public SignUpStatus Status { get; set; }
    }
}