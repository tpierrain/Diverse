using System;
using System.IO;
using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// All about what happens when the log sink we were given is broken: tracing a seed is a
    /// service Diverse renders, never a reason to fail the test of an end-user.
    /// <remarks>
    ///     Every test here mutates the static <see cref="Fuzzer.Log"/> that
    ///     <see cref="AllTestFixtures"/> registers once for the whole assembly, hence the
    ///     <see cref="LogMutatingFixture"/> base class that puts it back after each test. None of
    ///     these fixtures may ever be [Parallelizable].
    /// </remarks>
    /// </summary>
    [TestFixture]
    public class FuzzerWithAFailingLogSinkShould : LogMutatingFixture
    {
        [Test]
        public void Not_break_the_test_when_the_registered_log_sink_throws()
        {
            var brokenSink = new TestOutputHelperOutsideOfAnActiveTestStub();
            Fuzzer.Log = brokenSink.WriteLine;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            Check.ThatCode(() => fuzzer.GenerateInteger()).DoesNotThrow();

            // We really did try the sink we were given (and gave up on it afterwards).
            Check.That(brokenSink.CallCount).IsEqualTo(1);

            fuzzer.GenerateInteger();
            fuzzer.GenerateInteger();
            fuzzer.GenerateInteger();

            Check.That(brokenSink.CallCount).IsEqualTo(1);
        }

        [Test]
        public void Fall_back_on_the_Console_when_the_registered_log_sink_throws()
        {
            var brokenSink = new TestOutputHelperOutsideOfAnActiveTestStub();
            Fuzzer.Log = brokenSink.WriteLine;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            var previousConsoleOut = Console.Out;
            var capturedConsole = new StringWriter();
            try
            {
                Console.SetOut(capturedConsole);
                fuzzer.GenerateInteger();
            }
            finally
            {
                Console.SetOut(previousConsoleOut);
            }

            var consoleLines = capturedConsole.ToString()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Check.That(consoleLines).ContainsExactly(
                "--- Diverse: the registered log sink threw InvalidOperationException (\"There is no currently active test.\"). Falling back to the Console for the rest of this Fuzzer's seed trace.",
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)",
                $"--- from the test: {nameof(FuzzerWithAFailingLogSinkShould)}.{nameof(Fall_back_on_the_Console_when_the_registered_log_sink_throws)}()",
                SeparatorLine);
        }

        [Test]
        public void Not_break_the_test_when_the_log_sink_and_the_Console_are_both_broken()
        {
            // Correlated failures, not independent ones: the very teardown that invalidates a test
            // output helper is what closes the writer the runner redirected the Console to.
            var brokenSink = new TestOutputHelperOutsideOfAnActiveTestStub();
            Fuzzer.Log = brokenSink.WriteLine;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            var previousConsoleOut = Console.Out;
            try
            {
                Console.SetOut(new AlwaysFailingTextWriter());

                Check.ThatCode(() => fuzzer.GenerateInteger()).DoesNotThrow();
            }
            finally
            {
                Console.SetOut(previousConsoleOut);
            }
        }

        [Test]
        public void Replay_on_the_Console_only_the_banner_lines_the_sink_did_not_take()
        {
            var halfBrokenSink = new PartiallyFailingLogSinkStub(numberOfLinesToAcceptBeforeFailing: 2);
            Fuzzer.Log = halfBrokenSink.Sink;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            var previousConsoleOut = Console.Out;
            var capturedConsole = new StringWriter();
            try
            {
                Console.SetOut(capturedConsole);
                fuzzer.GenerateInteger();
            }
            finally
            {
                Console.SetOut(previousConsoleOut);
            }

            var consoleLines = capturedConsole.ToString()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Check.That(halfBrokenSink.AcceptedLines).ContainsExactly(
                SeparatorLine,
                "--- Fuzzer (\"fuzzer1\") instantiated from a provided seed (42)");

            // Reprinting what the sink already took would show the seed twice, in two places,
            // leaving the reader unable to tell which trace is the authoritative one.
            Check.That(consoleLines).ContainsExactly(
                "--- Diverse: the registered log sink threw InvalidOperationException (\"There is no currently active test.\"). Falling back to the Console for the rest of this Fuzzer's seed trace.",
                $"--- from the test: {nameof(FuzzerWithAFailingLogSinkShould)}.{nameof(Replay_on_the_Console_only_the_banner_lines_the_sink_did_not_take)}()",
                SeparatorLine);
        }

        [Test]
        public void Keep_telling_the_user_a_log_sink_is_missing_even_when_generating_an_instance_of_a_type()
        {
            // Generating an instance walks constructors behind catch-all handlers: a wiring mistake
            // must not be swallowed there and turned into a null the user gets much later.
            Fuzzer.Log = null;
            var fuzzer = new Fuzzer(seed: 42, name: "fuzzer1");

            Check.ThatCode(() => fuzzer.GenerateInstanceOf<SignUpRequest>())
                .Throws<FuzzerException>()
                .WhichMember(exception => exception.Message)
                .Contains("OneTimeSetUp", "ITestOutputHelper", nameof(Fuzzer.WithLogger));
        }

        [Test]
        public void Throw_an_explicit_FuzzerException_only_when_a_value_is_generated_and_no_log_sink_was_registered()
        {
            Fuzzer.Log = null;
            Fuzzer fuzzer = null;

            // An absent sink is a wiring mistake one has to fix, unlike a broken one we can survive.
            // But it must not blow up at construction time anymore: that is the whole point of #11.
            Check.ThatCode(() => fuzzer = new Fuzzer(seed: 42, name: "fuzzer1")).DoesNotThrow();

            Check.ThatCode(() => fuzzer.GenerateInteger())
                .Throws<FuzzerException>()
                .WhichMember(exception => exception.Message)
                .Contains("OneTimeSetUp", "ITestOutputHelper", nameof(Fuzzer.WithLogger));
        }
    }
}
