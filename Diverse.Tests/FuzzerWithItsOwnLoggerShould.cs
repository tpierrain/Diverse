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
    ///     <see cref="AllTestFixtures"/> registers once for the whole assembly. It must thus be
    ///     saved and restored around each test, and this fixture must never be [Parallelizable].
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerWithItsOwnLoggerShould
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
        [Repeat(20)]
        public void Keep_the_logs_of_two_concurrent_Fuzzers_separated_when_each_has_its_own_logger()
        {
            // No static sink at all: whatever we get, we got it through the instance loggers.
            Fuzzer.Log = null;
            var spyOfTheFirstOne = new LogSpy();
            var spyOfTheSecondOne = new LogSpy();
            var firstFuzzer = new Fuzzer(seed: 1, name: "first").WithLogger(spyOfTheFirstOne.Sink);
            var secondFuzzer = new Fuzzer(seed: 2, name: "second").WithLogger(spyOfTheSecondOne.Sink);

            var firstThread = new Thread(() => firstFuzzer.GenerateInteger());
            var secondThread = new Thread(() => secondFuzzer.GenerateInteger());
            firstThread.Start();
            secondThread.Start();
            firstThread.Join();
            secondThread.Join();

            // "(not found)" is the honest expectation here: a raw thread has no test method on its stack.
            Check.That(spyOfTheFirstOne.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"first\") instantiated from a provided seed (1)",
                "--- from the test: (not found)()",
                SeparatorLine);
            Check.That(spyOfTheSecondOne.Lines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"second\") instantiated from a provided seed (2)",
                "--- from the test: (not found)()",
                SeparatorLine);
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

            Check.That(spy.Lines).ContainsExactly(
                SeparatorLine,
                $"--- Fuzzer (\"{((Fuzzer)derivedFuzzer).Name}\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithItsOwnLoggerShould)}.{nameof(Pass_its_own_logger_down_to_the_Fuzzer_it_derives_for_the_NoDuplication_mode)}()",
                SeparatorLine);
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
