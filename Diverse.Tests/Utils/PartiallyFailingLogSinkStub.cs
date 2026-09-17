using System;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// A log sink that accepts a given number of lines and then throws for every subsequent one.
    /// <remarks>
    ///     The realistic shape of a sink dying *in the middle* of a banner: a test output helper
    ///     whose test ends while we are writing, a rate-limited logger, a writer hitting a buffer
    ///     limit. <see cref="TestOutputHelperOutsideOfAnActiveTestStub"/> always throws on its very
    ///     first call, so it cannot express that case.
    /// </remarks>
    /// </summary>
    public class PartiallyFailingLogSinkStub
    {
        private readonly List<string> _acceptedLines = new List<string>();

        public PartiallyFailingLogSinkStub(int numberOfLinesToAcceptBeforeFailing)
        {
            // Cached once, so that the very same delegate instance is registered every time.
            Sink = line =>
            {
                if (_acceptedLines.Count >= numberOfLinesToAcceptBeforeFailing)
                {
                    throw new InvalidOperationException("There is no currently active test.");
                }

                _acceptedLines.Add(line);
            };
        }

        /// <summary>
        /// Gets the sink to register (either on <see cref="Fuzzer.Log"/> or via WithLogger()).
        /// </summary>
        public Action<string> Sink { get; }

        /// <summary>
        /// Gets the lines the sink did accept, in the order they were received.
        /// </summary>
        public IReadOnlyList<string> AcceptedLines => _acceptedLines;
    }
}
