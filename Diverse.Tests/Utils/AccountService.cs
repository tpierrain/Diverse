using System.Drawing;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Dummy implementation only made to support tests for documentation purpose.
    /// </summary>
    public class AccountService
    {
        public SignUpResponse SignUp(SignUpRequest signUpRequest)
        {
            var response = new SignUpResponse(signUpRequest.Login);

            if (string.IsNullOrWhiteSpace(signUpRequest.PhoneNumber))
            {
                response.Status = SignUpStatus.InvalidPhoneNumber;
            }

            return response;
        }
    }
}