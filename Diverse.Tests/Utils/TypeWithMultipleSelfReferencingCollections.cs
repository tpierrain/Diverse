using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// A type with multiple collections of itself.
    /// Tests that the TypeFuzzer handles multiple self-referencing collections
    /// without geometric explosion.
    /// </summary>
    public class TypeWithMultipleSelfReferencingCollections
    {
        public string Name { get; }
        public IEnumerable<TypeWithMultipleSelfReferencingCollections> Children { get; }
        public IEnumerable<TypeWithMultipleSelfReferencingCollections> Siblings { get; }

        public TypeWithMultipleSelfReferencingCollections(
            string name,
            IEnumerable<TypeWithMultipleSelfReferencingCollections> children,
            IEnumerable<TypeWithMultipleSelfReferencingCollections> siblings)
        {
            Name = name;
            Children = children;
            Siblings = siblings;
        }
    }
}
