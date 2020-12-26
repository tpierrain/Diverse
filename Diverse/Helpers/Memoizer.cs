using System.Collections.Generic;

namespace Diverse
{
    internal class Memoizer
    {
        private readonly Dictionary<MemoizerKey, SortedSet<object>> _memoizer = new Dictionary<MemoizerKey, SortedSet<object>>();

        public Memoizer()
        {
        }

        public SortedSet<object> GetAlreadyProvidedValues(MemoizerKey memoizerKey)
        {
            SortedSet<object> alreadyProvidedValues = null;
            
            if (!_memoizer.ContainsKey(memoizerKey))
            {
                alreadyProvidedValues = new SortedSet<object>();
                _memoizer[memoizerKey] = alreadyProvidedValues;
            }
            else
            {
                alreadyProvidedValues = _memoizer[memoizerKey];
            }

            return alreadyProvidedValues;
        }
    }
}