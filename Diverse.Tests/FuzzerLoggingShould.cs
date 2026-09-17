using System;
using System.Threading;
using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about WHEN and HOW a <see cref="Fuzzer"/> traces the seed it uses.
    /// <remarks>
    ///     Every test here mutates the static <see cref="Fuzzer.Log"/> that
    ///     <see cref="AllTestFixtures"/> registers once for the whole assembly, hence the
    ///     <see cref="LogMutatingFixture"/> base class that puts it back after each test. None of
    ///     these fixtures may ever be [Parallelizable].
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerLoggingShould : LogMutatingFixture
    {
        [Test]
        public void Not_log_anything_at_construction_time()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;

            var _ = new Fuzzer(seed: 42, name: "fuzzer1");

            Check.That(spy.Lines).IsEmpty();
        }

        [Test]
        public void Log_the_whole_seed_banner_once_the_first_value_has_been_generated()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            fuzzer.GenerateInteger();

            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Log_the_whole_seed_banner_once_the_first_value_has_been_generated)}()",
                SeparatorLine);
        }

        [Test]
        public void Log_the_reproduction_hint_when_no_seed_was_provided()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var fuzzer = new Fuzzer(name: "fuzzer1");

            fuzzer.GenerateInteger();

            Check.That(spy.Lines).HasSize(5);
            Check.That(spy.Lines[0]).IsEqualTo(SeparatorLine);
            Check.That(spy.Lines[1]).Matches(@"^--- Fuzzer \(""fuzzer1""\) instantiated with the seed \(-?\d+\)$");
            Check.That(spy.Lines[2]).IsEqualTo(
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Log_the_reproduction_hint_when_no_seed_was_provided)}()");
            Check.That(spy.Lines[3]).IsEqualTo(
                "--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions");
            Check.That(spy.Lines[4]).IsEqualTo(SeparatorLine);
        }

        [Test]
        public void Not_log_any_extra_banner_when_using_the_NoDuplication_mode()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1", noDuplication: true);

            fuzzer.GenerateInteger(1, 5);

            // The Fuzzer that the NoDuplication mode uses under the hood is an implementation
            // detail: it must not pollute the output with a second banner of its own.
            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Not_log_any_extra_banner_when_using_the_NoDuplication_mode)}()",
                SeparatorLine);
        }

        [Test]
        public void Log_the_banner_of_a_derived_NoDuplication_Fuzzer_only_once_it_is_used()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var parentFuzzer = new Fuzzer(seed: 42, name: "parent");

            var derivedFuzzer = parentFuzzer.GenerateNoDuplicationFuzzer();

            Check.That(spy.Lines).IsEmpty();

            derivedFuzzer.GenerateInteger(1, 5);

            // A derived Fuzzer is handed over to the end-user: it must trace its own seed
            // (and not the one of the internal Fuzzer it uses for the NoDuplication mode).
            // Its name is generated, so it is pinned by its shape: reading it back from the object
            // under test would make this half of the assertion unfailable.
            Check.That(spy.Lines).HasSize(4);
            Check.That(spy.Lines[0]).IsEqualTo(SeparatorLine);
            Check.That(spy.Lines[1]).Matches(@"^--- Fuzzer \(""fuzzer\d+""\) instantiated from a provided seed \(42\)$");
            Check.That(spy.Lines[2]).IsEqualTo(
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Log_the_banner_of_a_derived_NoDuplication_Fuzzer_only_once_it_is_used)}()");
            Check.That(spy.Lines[3]).IsEqualTo(SeparatorLine);
        }

        [Test]
        public void Keep_the_reproduction_hint_for_a_derived_Fuzzer_when_its_parent_generated_its_own_seed()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var parentFuzzer = new Fuzzer(name: "parent"); // no seed provided: the Fuzzer picked one

            var derivedFuzzer = parentFuzzer.GenerateNoDuplicationFuzzer();
            derivedFuzzer.GenerateInteger(1, 5);

            // A derived Fuzzer must not claim a seed was provided (nobody provided one), and must
            // keep the one line that tells how to reproduce the run -- all the more so since a
            // parent that is never drawn from traces nothing at all.
            Check.That(spy.Lines).HasSize(5);
            Check.That(spy.Lines[1]).Matches(@"^--- Fuzzer \(""fuzzer\d+""\) instantiated with the seed \(-?\d+\)$");
            Check.That(spy.Lines[3]).IsEqualTo(
                "--- Note: you can instantiate another Fuzzer with that very same seed in order to reproduce the exact test conditions");
        }

        [Test]
        public void Resolve_the_name_of_the_test_even_when_the_Fuzzer_was_built_outside_of_the_test_method()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;

            // Builds the Fuzzer from a stack trace holding no test method frame at all: the
            // xUnit-free equivalent of building it from a test class constructor, which is the
            // very case reported in https://github.com/tpierrain/Diverse/issues/11
            Fuzzer fuzzer = null;
            var threadWithoutAnyTestMethodOnItsStack = new Thread(() => fuzzer = new Fuzzer(seed: 42, name: "fuzzer1"));
            threadWithoutAnyTestMethodOnItsStack.Start();
            threadWithoutAnyTestMethodOnItsStack.Join();

            fuzzer.GenerateInteger();

            // Nobody can say this Fuzzer was instantiated *from* this test: it was not. What is
            // true, and useful, is that this test is the one that first used it.
            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- first used by the test: {nameof(FuzzerLoggingShould)}.{nameof(Resolve_the_name_of_the_test_even_when_the_Fuzzer_was_built_outside_of_the_test_method)}()",
                SeparatorLine);
        }

        [Test]
        public void Name_the_test_it_was_built_in_even_when_its_first_value_is_generated_from_another_thread()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1"); // built on the stack of this test

            // An async test resuming after an await, a Parallel.ForEach, a SUT fuzzing from a
            // worker thread: the first generated value very often happens off the test's own stack.
            var threadWithoutAnyTestMethodOnItsStack = new Thread(() => fuzzer.GenerateInteger());
            threadWithoutAnyTestMethodOnItsStack.Start();
            threadWithoutAnyTestMethodOnItsStack.Join();

            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Name_the_test_it_was_built_in_even_when_its_first_value_is_generated_from_another_thread)}()",
                SeparatorLine);
        }

        [Test]
        public void Log_the_seed_banner_only_once_whatever_the_number_of_generated_values()
        {
            var spy = new LogSpy();
            Fuzzer.Log = spy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            fuzzer.GenerateInteger();
            fuzzer.GenerateFirstName();
            fuzzer.GeneratePerson();

            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Log_the_seed_banner_only_once_whatever_the_number_of_generated_values)}()",
                SeparatorLine);
        }
    }
}
