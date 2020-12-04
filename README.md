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


## Sample

Example of a typical test using Fuzzers. Here, a test for the SignUp process of an API:

```csharp

[Test]
public void Return_InvalidPhoneNumber_status_when_SignUp_with_an_empty_PhoneNumber()
{
    var fuzzer = new Fuzzer();
            
    // Uses the Fuzzer
    var person = fuzzer.GeneratePerson(); // speed up the creation of someone with random values
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

## What are Fuzzers and why should I use them?

*Fuzzers* are tiny utilities generating data/cases for you and your tests.
Instead of hard-coding *'john@doe.com'* in all your tests (or using 42 as default integer ;-)
a *Fuzzer* will generate any random value for you. Diverse will provide you random but relevant/credible data but may soon provide invalid or unexpected ones too.
Thus, you will more easily discover issues in your production code or discover that a test is badly written.

#### Disclaimer

Diverse is not a Property-based testing framework nor an advanced Fuzzer.

There are debates about what is really a Fuzzer or not. 
In Wikipedia, one can found __[the following definition for Fuzzing](https://en.wikipedia.org/wiki/Fuzzing)__: 

> *"Fuzzing or fuzz testing is an automated software testing technique that involves __providing invalid, unexpected, or random data__ as inputs to a computer program."*

So far the lib will only provide credible random data and not invalid or unexpected ones. 

But it will soon probably do the second part too.

#### How does Diverse looks like

```csharp

// avoid using always the same hard-coded values in your tests code => use Fuzzers instead
var age = fuzzer.GenerateInteger(minValue: 17, maxValue: 54);

// speed up the creation of something relatively 'complicated' and stay *intent driven*
// with random but coherent values (here, the Diverse Person)
var person = fuzzer.GeneratePerson(); 

// or other specific stuffs
var password = fuzzer.GeneratePassword();

// any enum value
var ingredient = fuzzer.GenerateEnum<Ingredient>();

// or dates
var dateTime = fuzzer.GenerateDateTime();
var dateTimeInRange = fuzzer.GenerateDateTimeBetween("1974/06/08", "2020/11/01");
var otherDateTimeInRange = fuzzer.GenerateDateTimeBetween(new DateTime(1974,6,8), new DateTime(2020, 11, 1));

// or any type actually (either, class, enum)
var player = fuzzer.GenerateInstanceOf<ChessPlayer>();

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

Ms. Valeria DENILSON (Female) vdenilson@microsoft.com (age: 62 years)
Ms. Kirsten BREKKE (Female) kbrekke@gmail.com (age: 76 years)
Mr. John BRAND (Male) john.brand@gmail.com (married - age: 86 years)
Mx. Ashok KHATRI (NonBinary) ashok.khatri@yahoo.fr (married - age: 32 years)
Ms. Fatima SELASSIE (Female) fatima.selassie@aol.com (age: 68 years)
Mx. Demba ADOMAKO (NonBinary) demba.adomako@gmail.com (age: 34 years)
Mrs. Erika MADSEN (Female) emadsen@gmail.com (married - age: 24 years)
Ms. Antje JOHNSON (Female) antje.johnson@protonmail.com (age: 37 years)
Ms. Isabella AMBRÍZ (Female) isabella.ambriz@microsoft.com (age: 60 years)
Mr. Arjun YOON (Male) ayoon@42skillz.com (age: 53 years)


```

## Fully Random, but deterministic when needed! (for debugging)

Use extensible fuzzers that randomize the values for you, but that can be replayed deterministically if any of your tests failed one day (in a specific configuration). 

I explained this here in that thread: 

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread1-550.png?raw=true)
![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread2-550.png?raw=true)

[see the thread on twitter here](https://twitter.com/tpierrain/status/1328962675074850819)

#### How to deterministically reproduce a test that has failed but only in a very specific case (picked randomly)

### 1. First, ensure that Diverse's logs will be traced down wherever you want. 

All you have to do is to call once the Fuzzer.Log setter:

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


### 2. Consult the report of a failing test and Copy the Seed that was used for it. 

Note: Diverse traces the seed used for every test ran. It will look like this:

```
 ----------------------------------------------------------------------------------------------------------------------
--- Fuzzer ("fuzzer1265") instantiated with the seed (1248680008)
--- from the test: FuzzerWithNumbersShould.GeneratePositiveInteger_with_an_inclusive_upper_bound()
--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions
-----------------------------------------------------------------------------------------------------------------------

```

### 3. Change your failing test to provide the copied Seed to its fuzzer:

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


## How to extend Diverse with your own Fuzzing methods

__Fortunately, Diverse is extensible.__ You will be able to add any specific method you want.

All you have to do is to __add your own .NET extension methods onto the *'IFuzz'* interface__

  - This will automatically add your own method to any Fuzzer instance
  
  - This will allow your methods to have access to the *'Random'* instance of the Fuzzer 
    
     - Interesting to leverage & compose with any other existing Fuzzing function! 
     
     - Mandatory to benefits from the Deterministic capabilities of the library



### Example of method extension for IFuzz

The *IFuzz* interface implemented by all our Fuzzer instances is made to be extended.
Let's say that we want to Add a new fuzzing method to dynamically generate an 'Age' instance
(part of our specific domain).

All we have to do in our project is to add a new GenerateAVerySpecificAge() extension method that may looks like this:

```csharp

public static class FuzzerExtensions
{
    public static Age GenerateAVerySpecificAge(this IFuzz fuzzer)
    {
        // Here, we can have access to all the existing methods 
        // exposed by the IFuzz interface
        int years = fuzzer.GeneratePositiveInteger(97);

        // or this one (very useful)
        bool isConfidential = fuzzer.HeadsOrTails();

        // For very specific needs, you have to use the
        // Random property of the Fuzzer
        double aDoubleForInstance = fuzzer.Random.NextDouble();

        return new Age(years, isConfidential);
    }
}

```

So that we can now see it and call it onto any of the Diverse Fuzzers like this in a test:

```csharp

[TestFixture]
public class FuzzerThatIsExtensibleShould
{
    [Test]
    public void Be_able_to_have_extension_methods()
    {
        var fuzzer = new Fuzzer(1245650948);

        // we have access to all our extension methods on our Fuzzer
        // (here: GenerateAVerySpecificAge())
        Age age = fuzzer.GenerateAVerySpecificAge();

        Check.That(age.Confidential).IsTrue();
        Check.That(age.Years).IsEqualTo(59);
    }
}

```

- - -

__*"I hope you will enjoy & benefit from adding more and more *Fuzzers* in your tests, but also to introduce some diversity in your code bases"*__

__*Thomas PIERRAIN*__










