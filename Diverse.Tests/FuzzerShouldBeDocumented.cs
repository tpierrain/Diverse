using System.Collections.Generic;
using System.Linq;
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
        public void Simplest_usage()
        {
            // Instantiates the Fuzzer
            var fuzzer = new Fuzzer();

            var results = new List<int>();
            for (int i = 0; i < 50; i++)
            {
                results.Add(fuzzer.GeneratePositiveInteger(maxValue: 3));
            }

            var greater = results.Any(x => x > 3);
            var withMaxValue = results.Any(x => x == 3);
            Check.That(greater).IsFalse();
            Check.That(withMaxValue).IsTrue();



            // Uses it for various usages
            var randomPositiveNumber = fuzzer.GeneratePositiveInteger(maxValue:8);
            TestContext.WriteLine($"Positive Number: {randomPositiveNumber}");

            var password = fuzzer.GeneratePassword(); // avoid always using the same hard-coded values
            TestContext.WriteLine($"password: {password}");

            var person = fuzzer.GenerateAPerson(); // speed up the creation of someone with random values
            TestContext.WriteLine($"First name: {person.FirstName}");
            TestContext.WriteLine($"Last name: {person.LastName}");
            TestContext.WriteLine($"Gender: {person.Gender}");
            TestContext.WriteLine($"Age: {person.Age}");
            TestContext.WriteLine($"IsMarried: {person.IsMarried}");
        }

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