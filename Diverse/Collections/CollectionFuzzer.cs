using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Diverse.Collections
{
    /// <summary>
    /// Fuzz from collections.
    /// </summary>
    internal class CollectionFuzzer : IFuzzFromCollections
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="CollectionFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public CollectionFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Randomly pick one element from the given collection.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns>One of the elements from the candidates collection.</returns>
        public T PickOneFrom<T>(IList<T> candidates)
        {
            ThrowIfNullOrEmpty(candidates);

            var randomIndex = _fuzzer.Random.Next(0, candidates.Count);

            return candidates[randomIndex];
        }

        private static void ThrowIfNullOrEmpty<T>(IList<T> candidates)
        {
            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            if (!candidates.Any())
            {
                throw new ArgumentException($"{nameof(candidates)} list must not be empty.");
            }
        }
    }
}