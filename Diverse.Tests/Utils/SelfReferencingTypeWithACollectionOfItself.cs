using System;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    public class SelfReferencingTypeWithACollectionOfItself
    {
        public string Name { get; }
        public string FirstName { get; }
        public DateTime Birthday { get; }

        public IEnumerable<SelfReferencingTypeWithACollectionOfItself> Friends { get; }

        public SelfReferencingTypeWithACollectionOfItself(string name, string firstName, DateTime birthday, IEnumerable<SelfReferencingTypeWithACollectionOfItself> friends)
        {
            Name = name;
            FirstName = firstName;
            Birthday = birthday;
            Friends = friends;
        }
    }
}