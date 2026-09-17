using System;
using NUnit.Framework;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Base class of every fixture that mutates the static <see cref="Fuzzer.Log"/>.
    /// <remarks>
    ///     <see cref="AllTestFixtures"/> registers that static once for the whole assembly, so a
    ///     fixture playing with it must put it back as it found it. None of them may ever be
    ///     [Parallelizable]: NUnit's default serial execution is what protects that static.
    /// </remarks>
    /// </summary>
    public abstract class LogMutatingFixture
    {
        /// <summary>
        /// The separator line opening and closing every seed banner (kept in sync with the one of
        /// the <see cref="Fuzzer"/> by the tests asserting the banner).
        /// </summary>
        protected const string SeparatorLine =
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
    }
}
