# Diverse

Diverse, the Fuzzer pico library you need to make your .NET tests more *Diverse*.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-wave.jpg?raw=true)   

---

![.NET Core](https://github.com/42skillz/Diverse/workflows/.NET%20Core/badge.svg)  -  thomas@42skillz.com - ![twitter icon](https://github.com/42skillz/Diverse/blob/main/Images/Twitter_icon.gif?raw=true) [use case driven on twitter](https://twitter.com/tpierrain)  

---

## Diverse added-value

Diverse:

 - Provides __a set of primitives__ that speed up the usage and/or the build of __*inclusive Fuzzers*__ in your tests
	
 - Provides __a way to easy reproduce any failing test in a deterministic manner__ whereas the initial bad conditions have been found randomly

	- Important when your Fuzzer make you discover randomly a case that breaks one of your test
 
 - Is a __pico library__ with __no external dependency__, and is nonetheless __compliant with all test frameworks__

 - Is __easily extensible__ through .NET extension methods over a simple *IFuzz* interface


## What are Fuzzers and why should I use them?

*Fuzzers* are tiny utilities generating data/cases for you and your tests.
Instead of hard-coding *'john@doe.com'* in all your tests (or using 42 as default integer ;-)
a *Fuzzer* will generate any random value for you. Thus, you will more easily discover
issues in your production code or discover that a test is badly written.

Every time your test will run, it will use different random values for what matters in your *Domain*.

The whole idea of a good *Fuzzer* is to be as easy to use as putting an hard-coded value.


## Why the name Diverse?

Thanks to ask ;-) Well... It is a matter of fact that the software industry is still not really a super inclusive place right now.

### *Karens* are in minority here ;-)
Indded, Diverse will help you to make your tests more inclusive and more diverse by picking other things that *Karen* of *John* as default first names for instance.

Diverse will help you to quickly generate diverse names or persons from various genders, countries, cultures, etc.

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Diverse-icon-small.jpg?raw=true)

Just pick the primitives you want and check by yourself ;-)


## A (possibly) deterministic Fuzzer?!?

Use extensible fuzzers that randomize the values for you, but that can be replayed deterministically if any of your tests failed one day in a specific configuration. 

I explained this here in that thread: 

![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread1-550.png?raw=true)
![twitter screen](https://github.com/42skillz/Diverse/blob/main/Images/DiverseThread2-550.png?raw=true)

[see the thread on twitter here](https://twitter.com/tpierrain/status/1328962675074850819)

#### How to deterministically reproduce a test that has failed but only in a very specific case (picked randomly)

 1. Do ...
 1. Do ...


## Code Sample


     TBD







