using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Runs actions on real threads and brings whatever they threw back to the test thread.
    /// <remarks>
    ///     An exception escaping the body of a raw <see cref="Thread"/> does not fail the test that
    ///     started it: it tears the whole test host down, losing the verdict of every other test of
    ///     the run. Every concurrency test here must thus catch on the thread and re-assert here.
    /// </remarks>
    /// </summary>
    public static class Concurrently
    {
        /// <summary>
        /// Runs every given action on its own thread, and returns what they threw (if anything).
        /// </summary>
        /// <param name="actions">The actions to run concurrently.</param>
        /// <returns>One description per failure ("ExceptionType: message"), empty when all went well.</returns>
        public static IReadOnlyList<string> Run(params Action[] actions)
        {
            var failures = new ConcurrentQueue<string>();

            var threads = actions.Select(action => new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    failures.Enqueue($"{exception.GetType().Name}: {exception.Message}");
                }
            })).ToList();

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return failures.ToList();
        }

        /// <summary>
        /// Runs the very same action on <paramref name="numberOfThreads"/> threads, and returns
        /// what they threw (if anything).
        /// </summary>
        /// <param name="numberOfThreads">The number of threads to race with.</param>
        /// <param name="action">The action every thread has to run.</param>
        /// <returns>One description per failure ("ExceptionType: message"), empty when all went well.</returns>
        public static IReadOnlyList<string> Run(int numberOfThreads, Action action)
        {
            return Run(Enumerable.Range(0, numberOfThreads).Select(_ => action).ToArray());
        }
    }
}
