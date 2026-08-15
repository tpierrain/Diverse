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
    ///     <see cref="AllTestFixtures"/> registers once for the whole assembly. It must thus be
    ///     saved and restored around each test, and this fixture must never be [Parallelizable].
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerLoggingShould
    {
        private const string SeparatorLine =
            "----------------------------------------------------------------------------------------------------------------------";

        private Action<string> _previousStaticLog;

        [SetUp]
        public void SaveTheStaticLog()
        {
            _previousStaticLog = Fuzzer.Log;
        }

        [TearDown]
        public void RestoreTheStaticLog()
        {
            Fuzzer.Log = _previousStaticLog;
        }

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
            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                $"--- Fuzzer (\"{((Fuzzer)derivedFuzzer).Name}\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Log_the_banner_of_a_derived_NoDuplication_Fuzzer_only_once_it_is_used)}()",
                SeparatorLine);
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

            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerLoggingShould)}.{nameof(Resolve_the_name_of_the_test_even_when_the_Fuzzer_was_built_outside_of_the_test_method)}()",
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
