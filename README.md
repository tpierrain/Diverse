# Diverse ![.NET Core](https://github.com/42skillz/Diverse/workflows/.NET%20Core/badge.svg)

Diverse, the Fuzzer pico library you need to make your .NET tests more *Diverse*.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-wave.jpg?raw=true)   
  
# 
![twitter icon](https://github.com/42skillz/Diverse/blob/main/Images/Twitter_icon.gif?raw=true) [use case driven on twitter](https://twitter.com/tpierrain) - (thomas@42skillz.com)


## Diverse added-value

Diverse:

 - Provides __a set of primitives__ to easy *Fuzz* data in your .NET tests in a *Diverse* manner
	
 - Provides __fully randomed values, but__ every case/test run can be reproduced afterwards in a __deterministic__ manner __if needed__ (e.g.: specific test case, debugging)
 
 - Is a __pico library__ with __no external dependency__, and is nonetheless __compliant with all test frameworks__

 - Is __easily extensible__ through .NET extension methods over a simple *IFuzz* interface


## What are Fuzzers and why should I use them?

*Fuzzers* are tiny utilities generating data/cases for you and your tests.
Instead of hard-coding *'john@doe.com'* in all your tests (or using 42 as default integer ;-)
a *Fuzzer* will generate any random -but relevant credible- value for you. Thus, you will more easily discover
issues in your production code or discover that a test is badly written.

```csharp

// avoid using always the same hard-coded values in your tests code => use Fuzzers instead
var age = fuzzer.GenerateInteger(minValue: 17, maxValue: 54);

// speed up the creation of something relatively 'complicated' and stay *intent driven*
// with random but coherent values
var person = fuzzer.GenerateAPerson(); 

// or other specific stuffs
var password = fuzzer.GeneratePassword(); 

// Anything you need for your test cases actually
// Diverse Fuzzers are easily extensible ;-)

```


Every time your test will run, it will use different random values for what matters in your *Domain*.

The whole idea of a good *Fuzzer* is to be as easy to use as it is to put an hard-coded value.


## Why the name Diverse?

Thanks to ask ;-) Well... It is a matter of fact that the software industry is still not really a super inclusive place right now.

### *Karens* are in minority here ;-)
Indeed, Diverse will help you to make your tests more inclusive and more diverse by picking other things that *Karen* of *John* as default first names for instance.

Diverse will help you to quickly generate diverse names or persons from various genders, countries, cultures, etc.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-small.jpg?raw=true)

Just pick the primitives you want and check by yourself ;-)

```csharp

// Examples of Persons created with Diverse:

Mr. Mamadou TRAORE (Male) mtraore@kolab.com (is married: False - age: 96 yrs)
Ms. Fatima BA (Female) fba@gmail.com (is married: False - age: 93 yrs) 
Ms. Ida STRØM (Female) ida.strøm@protonmail.com (is married: False - age: 68 yrs)
Mx. Jeremie MATEUDI (NonBinary) jeremie.mateudi@yopmail.com (is married: True - age: 66 yrs) 
Ms. Sarah BEN ACHOUR (Female) sben-achour@kolab.com (is married: False - age: 68 yrs)
Mr. Connor BAKER (Male) cbaker@gmail.com (is married: False - age: 20 yrs)
Mrs. Mériem MWANGI (Female) meriem.mwangi@yopmail.com (is married: True - age: 83 yrs) 
Mr. Zhen MADAN (Male) zmadan@yahoo.fr (is married: True - age: 23 yrs)
Mrs. Esther CHAKRABARTI (Female) esther.chakrabarti@ibm.com (is married: True - age: 62 yrs) 
Mr. Javier MUÑOZ (Male) jmunoz@gmail.com (is married: False - age: 66 yrs) 
Mx. Alejandro QUISPE (NonBinary) alejandro.quispe@yahoo.fr (is married: False - age: 76 yrs) 
Ms. Francesca ARELLANO (Female) farellano@ibm.com (is married: False - age: 85 yrs) 
Ms. Ji-yeon WANG (Female) jwang@42skillz.com (is married: False - age: 43 yrs)

```


## Fully Random, but deterministic when needed! (for debugging)

Use extensible fuzzers that randomize the values for you, but that can be replayed deterministically if any of your tests failed one day (in a specific configuration). 

I explained this here in that thread: 

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread1-550.png?raw=true)
![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread2-550.png?raw=true)

[see the thread on twitter here](https://twitter.com/tpierrain/status/1328962675074850819)

#### How to deterministically reproduce a test that has failed but only in a very specific case (picked randomly)

1. First, ensure that Diverse's logs will be traced down wherever you want. All you have to do is to call once the Fuzzer.Log setter:

e.g.: here with NUnit :

 ```csharp

    [SetUpFixture]
    public class AllTestFixtures
    {
        [OneTimeSetUp]
        public void Init()
        {
            // We redirect all Diverse's log into NUnit's TestContext outputs
            Fuzzer.Log = TestContext.WriteLine;
        }
    }

 ```


 2. Consult the report of a failing test and Copy the Seed that was used for it. Note: Diverse traces the seed used for every test ran. It will look like this:
 ```
 ----------------------------------------------------------------------------------------------------------------------
--- Fuzzer ("fuzzer1265") instantiated with the seed (1248680008)
--- from the test: FuzzerWithNumbersShould.GeneratePositiveInteger_with_an_inclusive_upper_bound()
--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions
-----------------------------------------------------------------------------------------------------------------------

 ```

 3. Change your failing test to provide the copied Seed to its fuzzer:

 Instead of:

 ```csharp

 var fuzzer = new Fuzzer();

 ```

 calls:

 ```csharp

 var fuzzer = new Fuzzer(seed: 1248680008);

 ```

That's it! Your test using Diverse fuzzers will be deterministic and always provide the same Fuzzed values.

You can then fix your implementation code to make your test green, or rewrite your badly written test, or keep your test like this so you can have *deterministic values* in it (nice for documentation).

## Sample

Example of a typical test using Fuzzers. Here, a test for the SignUp process of an API:


 ```csharp

[Test]
public void Return_InvalidPhoneNumber_status_when_SignUp_with_an_empty_PhoneNumber()
{
    var fuzzer = new Fuzzer();
            
    // Uses the Fuzzer
    var person = fuzzer.GenerateAPerson(); // speed up the creation of someone with random values
    var password = fuzzer.GeneratePassword(); // avoid always using the same hard-coded values
    var invalidPhoneNumber = "";

    // Do your domain stuff
    var signUpRequest = new SignUpRequest(login: person.EMail, password: password, 
                                            firstName: person.FirstName, lastName: person.LastName, 
                                            phoneNumber : invalidPhoneNumber);

    // Here, the quality of the password won't be a blocker for this
    // SignUp process. We just want to check the behaviour with empty phone number
    var signUpResponse = new AccountService().SignUp(signUpRequest);

    // Assert
    Check.That(signUpResponse.Login).IsEqualTo(person.EMail);
    Check.That(signUpResponse.Status).IsEqualTo(SignUpStatus.InvalidPhoneNumber);
}


 ```

 Note that in a real project, we may have implemented a specific Fuzzer to generate a SignUpRequest.

 __Fortunately, Diverse is extensible.__ You will be able to add any specific method you want.


 ## How to extend Diverse with your own Fuzzing methods

 Easy. All you have to do is to __add your own .NET extension methods onto the *'IFuzz'* interface__

  - This will automatically add your own method to any Fuzzer instance
  
  - This will allow your methods to have access to the *'Random'* instance of the Fuzzer 
    
    - Mandatory to benefits from the Deterministic capabilities of the library
    
    - Interesting to leverage & compose with any other existing Fuzzing function! 


#### Example of method extension for IFuzz


  ```csharp

  TBD

   ```








