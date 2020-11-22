using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// Naive test only to be used to publish valid C# code samples in our Readme.md documentation file.
    /// </summary>
    [TestFixture]
    public class FuzzerShouldDocument
    {
        [Test]
        public void Classical_usage()
        {
            var fuzzer = new Fuzzer();
            
            // Uses the Fuzzer
            var person = fuzzer.GenerateAPerson(); // speed up the creation of someone with random values
            var password = fuzzer.GeneratePassword(); // avoid always using the same hard-coded values
            TestContext.WriteLine($"password: {password}");
            var invalidPhoneNumber = "";

            // Do your domain stuff
            var signUpRequest = new SignUpRequest(login: person.EMail, password: password, 
                                                    firstName: person.FirstName, lastName: person.LastName, 
                                                    phoneNumber : invalidPhoneNumber);

            var signUpResponse = new AccountService().SignUp(signUpRequest);

            // Assert
            Check.That(signUpResponse.Login).IsEqualTo(person.EMail);
            Check.That(signUpResponse.Status).IsEqualTo(SignUpStatus.InvalidPhoneNumber);
        }
    }
}