using System;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Captures every line sent to a <see cref="Fuzzer"/> log sink, so that tests can assert
    /// the whole sequence of logged lines (and not only that something was logged).
    /// </summary>
    public class LogSpy
    {
        private readonly List<string> _lines = new List<string>();

        public LogSpy()
        {
            // Cached once, so that registering the sink twice registers the very same delegate instance.
            Sink = line => _lines.Add(line);
        }

        /// <summary>
        /// Gets the log sink to register (either on <see cref="Fuzzer.Log"/> or via WithLogger()).
        /// </summary>
        public Action<string> Sink { get; }

        /// <summary>
        /// Gets every line received so far, in the order they were received.
        /// </summary>
        public IReadOnlyList<string> Lines => _lines;
    }
}
