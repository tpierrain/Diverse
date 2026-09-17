using System;
using System.IO;
using System.Text;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// A <see cref="TextWriter"/> that throws on every write, to stand for a Console the test
    /// runner has already torn down (a disposed or closed writer left behind by
    /// <see cref="Console.SetOut"/>).
    /// <remarks>
    ///     A broken sink and a broken Console are correlated failures, not independent ones: the
    ///     very teardown that invalidates xUnit's ITestOutputHelper is what closes the writer the
    ///     runner had redirected the Console to.
    /// </remarks>
    /// </summary>
    public class AlwaysFailingTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            throw new ObjectDisposedException(nameof(AlwaysFailingTextWriter));
        }

        public override void Write(string value)
        {
            throw new ObjectDisposedException(nameof(AlwaysFailingTextWriter));
        }

        public override void WriteLine(string value)
        {
            throw new ObjectDisposedException(nameof(AlwaysFailingTextWriter));
        }
    }
}
