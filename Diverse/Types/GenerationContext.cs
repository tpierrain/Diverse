using System;
using System.Collections.Generic;
using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Tracks the ancestry chain of types during recursive instance generation.
    /// Detects cycles (self-referencing or mutual references) and adjusts collection
    /// sizes to prevent geometric explosion and stack overflow.
    /// </summary>
    internal class GenerationContext
    {
        private readonly Stack<Type> _typeAncestry = new Stack<Type>();

        /// <summary>
        /// Maximum number of times a type can appear in the ancestry chain
        /// before generation stops for that type (returns null/default).
        /// </summary>
        public int MaxDepthForRecursiveTypes { get; }

        /// <summary>
        /// Maximum number of elements to generate in collections at the top level.
        /// This is reduced for recursive types to prevent geometric explosion.
        /// </summary>
        public int MaxCollectionSize { get; }

        /// <summary>
        /// Instantiates a <see cref="GenerationContext"/>.
        /// </summary>
        /// <param name="maxDepthForRecursiveTypes">
        /// Maximum number of times a type can appear in the ancestry chain
        /// before generation stops. Default: 2 (root + one level of children).
        /// </param>
        /// <param name="maxCollectionSize">
        /// Maximum number of elements to generate in collections at the top level. Default: 5.
        /// </param>
        public GenerationContext(int maxDepthForRecursiveTypes = 2, int maxCollectionSize = 5)
        {
            MaxDepthForRecursiveTypes = maxDepthForRecursiveTypes;
            MaxCollectionSize = maxCollectionSize;
        }

        /// <summary>
        /// Pushes a type onto the ancestry stack before generating an instance of it.
        /// Must be paired with <see cref="PopType"/> in a try/finally block.
        /// </summary>
        /// <param name="type">The type about to be generated.</param>
        public void PushType(Type type)
        {
            _typeAncestry.Push(type);
        }

        /// <summary>
        /// Pops a type from the ancestry stack after generation is complete.
        /// </summary>
        public void PopType()
        {
            _typeAncestry.Pop();
        }

        /// <summary>
        /// Counts how many times a type already appears in the current ancestry chain.
        /// </summary>
        /// <param name="type">The type to count occurrences of.</param>
        /// <returns>The number of occurrences of the type in the ancestry chain.</returns>
        public int CountOccurrencesOf(Type type)
        {
            var count = 0;
            foreach (var ancestor in _typeAncestry)
            {
                if (ancestor == type)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Determines whether generation should stop for a given type
        /// because it has already appeared too many times in the ancestry chain.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><b>true</b> if the type has reached its maximum allowed depth, <b>false</b> otherwise.</returns>
        public bool ShouldStopRecursingFor(Type type)
        {
            return CountOccurrencesOf(type) >= MaxDepthForRecursiveTypes;
        }

        /// <summary>
        /// Computes the number of elements to generate in a collection of the given element type.
        /// Returns 0 if the element type has reached its maximum recursion depth.
        /// Returns a reduced size if the element type is already in the ancestry chain
        /// (to prevent geometric explosion like 5^n instances).
        /// Returns the full <see cref="MaxCollectionSize"/> for non-recursive types.
        /// </summary>
        /// <param name="elementType">The type of the elements in the collection.</param>
        /// <returns>The number of elements to generate.</returns>
        public int GetCollectionSizeFor(Type elementType)
        {
            var occurrences = CountOccurrencesOf(elementType);

            if (occurrences >= MaxDepthForRecursiveTypes)
            {
                return 0;
            }

            // Reduce collection size for recursive types to prevent geometric explosion.
            // E.g., with MaxCollectionSize=5: depth 0 → 5 elements, depth 1 → 2, depth 2 → 1
            if (occurrences > 0)
            {
                return Math.Max(1, MaxCollectionSize / (occurrences + 1));
            }

            return MaxCollectionSize;
        }
    }
}
