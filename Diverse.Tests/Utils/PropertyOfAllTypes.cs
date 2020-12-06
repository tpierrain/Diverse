using System;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    public class PropertyOfAllTypes
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public IEnumerable<int> FavoriteNumbers { get; set; }

        public PropertyOfAllTypes()
        {
        }
    }
}