# Diverse

thomas@42skillz.com

Diverse, the .NET Fuzzer you need to make your tests more *Diverse*.

## Diverse added-value

Diverse:

 - Provides a set of primitives that speed up your usage of Fuzzers in your tests

 - Provides a way to easy reproduce in a deterministic manner any test scenario initially fully random

	- Important when your Fuzzer make you discover a case that breaks one of your test
 
	- Has no external dependency and is compliant with pretty much all test frameworks

 - Is easily extensible through .NET extension methods over an interface: *IFuzz*


## What are Fuzzers and why should I use them?

*Fuzzers* are tiny utilities generating data/cases for you and your tests.
Instead of hard-coding *'john@doe.com'* in all your tests (or using 42 as default integer ;-)
a *Fuzzer* will generate any random value for you. Thus, you will more easily discover
issues in your production code or discover that a test is badly written.

Every time your test will run, it will use different random values for what matters in your *Domain*.

The whole idea of a good *Fuzzer* is to be as easy to use as putting an hard-coded value.


## Why the name Diverse?

Thanks to ask ;-) It's a matter of fact that IT is still not really an inclusive place right now.
Diverse will help you to make your test more inclusive and diverse by picking other things that *Karen* of *John* as default first names for instance.

Diverse will help you to quickly generate diverse persons from various genders, countries, cultures, etc.
Just pick the primitives you want.

## How to deterministically reproduce a test that has failed but only in a very specific case (picked randomly)?

Use extensible fuzzers that randomize the values for you, but can be replayed deterministically if any of your tests ever fail one day in a specific configuration. 

Easy. First thing

Link toward my tweet thread on that topic.



## Code Sample

TBD






