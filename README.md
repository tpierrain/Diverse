# Diverse ![.NET Core](https://github.com/42skillz/Diverse/workflows/.NET%20Core/badge.svg)

Diverse, the Fuzzer pico library you need to make your .NET tests more *Diverse*.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-wave.jpg?raw=true)   

  

  
# 
![twitter icon](https://github.com/42skillz/Diverse/blob/main/Images/Twitter_icon.gif?raw=true) [use case driven on twitter](https://twitter.com/tpierrain) - (thomas@42skillz.com)


## Diverse added-value

Diverse:

 - Provides __a set of primitives__ to easy *Fuzz* the data in your .NET tests in a *Diverse* manner
	
 - Provides __fully randomed values, but__ every case can be reproduced after in a __deterministic__ manner __when needed__ (e.g.: specific test case, debugging)
 
 - Is a __pico library__ with __no external dependency__, and is nonetheless __compliant with all test frameworks__

 - Is __easily extensible__ through .NET extension methods over a simple *IFuzz* interface


## What are Fuzzers and why should I use them?

*Fuzzers* are tiny utilities generating data/cases for you and your tests.
Instead of hard-coding *'john@doe.com'* in all your tests (or using 42 as default integer ;-)
a *Fuzzer* will generate any random value for you. Thus, you will more easily discover
issues in your production code or discover that a test is badly written.

```csharp

        var person = fuzzer.GenerateAPerson(); // speed up the creation of someone with random values
        var password = fuzzer.GeneratePassword(); // avoid always using the same hard-coded values

```


Every time your test will run, it will use different random values for what matters in your *Domain*.

The whole idea of a good *Fuzzer* is to be as easy to use as putting an hard-coded value.


## Why the name Diverse?

Thanks to ask ;-) Well... It is a matter of fact that the software industry is still not really a super inclusive place right now.

### *Karens* are in minority here ;-)
Indded, Diverse will help you to make your tests more inclusive and more diverse by picking other things that *Karen* of *John* as default first names for instance.

Diverse will help you to quickly generate diverse names or persons from various genders, countries, cultures, etc.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-small.jpg?raw=true)

Just pick the primitives you want and check by yourself ;-)


## Fully Random, but deterministic when needed! (for debugging)

Use extensible fuzzers that randomize the values for you, but that can be replayed deterministically if any of your tests failed one day (in a specific configuration). 

I explained this here in that thread: 

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread1-550.png?raw=true)
![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread2-550.png?raw=true)

[see the thread on twitter here](https://twitter.com/tpierrain/status/1328962675074850819)

#### How to deterministically reproduce a test that has failed but only in a very specific case (picked randomly)

 1. Do ...
 1. Do ...


## Code Sample


     TBD







