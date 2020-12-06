using System;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    public class SelfReferencingTypeWithACollectionOfItself
    {
        public string Name { get; }
        public DateTime Birthday { get; }

        public IEnumerable<SelfReferencingTypeWithACollectionOfItself> Friends { get; }

        public SelfReferencingTypeWithACollectionOfItself(string name, DateTime birthday, IEnumerable<SelfReferencingTypeWithACollectionOfItself> friends)
        {
            Name = name;
            Birthday = birthday;
            Friends = friends;
        }
    }
}