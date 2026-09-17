using System;
using System.Threading;
using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about the logger one can give to a single <see cref="Fuzzer"/> instance, instead of
    /// relying on the process-wide static <see cref="Fuzzer.Log"/>.
    /// <remarks>
    ///     Every test here mutates the static <see cref="Fuzzer.Log"/> that
    ///     <see cref="AllTestFixtures"/> registers once for the whole assembly, hence the
    ///     <see cref="LogMutatingFixture"/> base class that puts it back after each test. None of
    ///     these fixtures may ever be [Parallelizable].
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerWithItsOwnLoggerShould : LogMutatingFixture
    {
        private const int NumberOfRacingThreads = 8;

        [Test]
        public void Send_its_seed_banner_to_its_own_logger_instead_of_the_static_one()
        {
            var staticSpy = new LogSpy();
            var instanceSpy = new LogSpy();
            Fuzzer.Log = staticSpy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1").WithLogger(instanceSpy.Sink);

            fuzzer.GenerateInteger();

            Check.That(instanceSpy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Send_its_seed_banner_to_its_own_logger_instead_of_the_static_one)}()",
                SeparatorLine);
            Check.That(staticSpy.Lines).IsEmpty();
        }

        [Test]
        public void Fall_back_on_the_static_Log_when_no_instance_logger_was_provided()
        {
            var staticSpy = new LogSpy();
            Fuzzer.Log = staticSpy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            fuzzer.GenerateInteger();

            Check.That(staticSpy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Fall_back_on_the_static_Log_when_no_instance_logger_was_provided)}()",
                SeparatorLine);
        }

        [Test]
        public void Emit_to_the_static_sink_that_was_registered_when_it_was_built()
        {
            // The shape of parallel xUnit classes: another one overwrites the process-wide static
            // between our construction and our first generated value.
            var sinkOfTheTestThatBuiltTheFuzzer = new LogSpy();
            var sinkOfAnotherTestRunningInParallel = new LogSpy();
            Fuzzer.Log = sinkOfTheTestThatBuiltTheFuzzer.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            Fuzzer.Log = sinkOfAnotherTestRunningInParallel.Sink;
            fuzzer.GenerateInteger();

            Check.That(sinkOfTheTestThatBuiltTheFuzzer.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Emit_to_the_static_sink_that_was_registered_when_it_was_built)}()",
                SeparatorLine);
            Check.That(sinkOfAnotherTestRunningInParallel.Lines).IsEmpty();
        }

        [Test]
        public void Trace_its_seed_again_when_it_is_given_a_new_logger_after_its_first_generated_value()
        {
            // The natural xUnit ordering for a Fuzzer living longer than one test: it is handed a
            // fresh ITestOutputHelper per test, and each test deserves its seed.
            var staticSpy = new LogSpy();
            var laterSpy = new LogSpy();
            Fuzzer.Log = staticSpy.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");
            fuzzer.GenerateInteger();

            fuzzer.WithLogger(laterSpy.Sink);
            fuzzer.GenerateInteger();
            fuzzer.GenerateFirstName();

            Check.That(laterSpy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Trace_its_seed_again_when_it_is_given_a_new_logger_after_its_first_generated_value)}()",
                SeparatorLine);
            Check.That(staticSpy.Lines).HasSize(4);
        }

        [Test]
        [Repeat(200)]
        public void Log_its_seed_banner_exactly_once_when_several_threads_race_on_its_first_generated_value()
        {
            Fuzzer.Log = null;
            var spy = new LogSpy();
            var sharedFuzzer = new Fuzzer(seed: 42, name: "shared").WithLogger(spy.Sink);

            var startLine = new Barrier(NumberOfRacingThreads);
            var failures = Concurrently.Run(NumberOfRacingThreads, () =>
            {
                startLine.SignalAndWait();
                sharedFuzzer.HeadsOrTails();
            });

            Check.That(failures).IsEmpty();
            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"shared\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Log_its_seed_banner_exactly_once_when_several_threads_race_on_its_first_generated_value)}()",
                SeparatorLine);
        }

        [Test]
        [Repeat(200)]
        public void Keep_the_logs_of_two_concurrent_Fuzzers_separated_when_each_has_its_own_logger()
        {
            // No static sink at all: whatever we get, we got it through the instance loggers.
            Fuzzer.Log = null;
            var spyOfTheFirstOne = new LogSpy();
            var spyOfTheSecondOne = new LogSpy();
            var firstFuzzer = new Fuzzer(seed: 1, name: "first").WithLogger(spyOfTheFirstOne.Sink);
            var secondFuzzer = new Fuzzer(seed: 2, name: "second").WithLogger(spyOfTheSecondOne.Sink);

            var failures = Concurrently.Run(
                () => firstFuzzer.GenerateInteger(),
                () => secondFuzzer.GenerateInteger());

            Check.That(failures).IsEmpty();

            // Both Fuzzers were built on the stack of this very test, so they name it, even though
            // their first value was generated from a thread that has no test method on its stack.
            var expectedTestNameLine =
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Keep_the_logs_of_two_concurrent_Fuzzers_separated_when_each_has_its_own_logger)}()";
            Check.That(spyOfTheFirstOne.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"first\") instantiated from a provided seed (1)",
                expectedTestNameLine,
                SeparatorLine);
            Check.That(spyOfTheSecondOne.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"second\") instantiated from a provided seed (2)",
                expectedTestNameLine,
                SeparatorLine);
        }

        [Test]
        public void Inherit_the_logger_given_to_its_parent_even_after_it_has_been_derived()
        {
            // No static sink to fall back on, and the logger arrives *after* the derivation: a
            // child that snapshots its parent's logger would throw a FuzzerException here.
            Fuzzer.Log = null;
            var spy = new LogSpy();
            var parentFuzzer = new Fuzzer(seed: 42, name: "parent");
            var derivedFuzzer = parentFuzzer.GenerateNoDuplicationFuzzer();

            parentFuzzer.WithLogger(spy.Sink);
            derivedFuzzer.GenerateInteger(1, 5);

            Check.That(spy.Lines).HasSize(4);
            Check.That(spy.Lines[1]).Matches(@"^--- Fuzzer \(""fuzzer\d+""\) instantiated from a provided seed \(42\)$");
            Check.That(spy.Lines[2]).IsEqualTo(
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Inherit_the_logger_given_to_its_parent_even_after_it_has_been_derived)}()");
        }

        [Test]
        public void Pass_its_own_logger_down_to_the_Fuzzer_it_derives_for_the_NoDuplication_mode()
        {
            // No static sink to fall back on: a derived Fuzzer that would not inherit the logger
            // of the instance it comes from would throw a FuzzerException here.
            Fuzzer.Log = null;
            var spy = new LogSpy();
            var parentFuzzer = new Fuzzer(seed: 42, name: "parent").WithLogger(spy.Sink);

            var derivedFuzzer = parentFuzzer.GenerateNoDuplicationFuzzer();
            derivedFuzzer.GenerateInteger(1, 5);

            // The name of a derived Fuzzer is generated, so it is pinned by its shape here: reading
            // it back from the object under test would make this half of the assertion unfailable.
            Check.That(spy.Lines).HasSize(4);
            Check.That(spy.Lines[0]).IsEqualTo(SeparatorLine);
            Check.That(spy.Lines[1]).Matches(@"^--- Fuzzer \(""fuzzer\d+""\) instantiated from a provided seed \(42\)$");
            Check.That(spy.Lines[2]).IsEqualTo(
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Pass_its_own_logger_down_to_the_Fuzzer_it_derives_for_the_NoDuplication_mode)}()");
            Check.That(spy.Lines[3]).IsEqualTo(SeparatorLine);
        }

        [Test]
        public void Refuse_a_null_logger()
        {
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            Check.ThatCode(() => fuzzer.WithLogger(null))
                .Throws<ArgumentNullException>()
                .WithProperty(exception => exception.ParamName, "logger");
        }
    }
}
