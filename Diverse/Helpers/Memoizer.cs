using System.Collections.Generic;

namespace Diverse
{
    internal class Memoizer
    {
        private readonly Dictionary<MemoizerKey, HashSet<object>> _memoizer = new Dictionary<MemoizerKey, HashSet<object>>();

        public Memoizer()
        {
        }

        public HashSet<object> GetAlreadyProvidedValues(MemoizerKey memoizerKey)
        {
            HashSet<object> alreadyProvidedValues = null;
            
            if (!_memoizer.ContainsKey(memoizerKey))
            {
                alreadyProvidedValues = new HashSet<object>();
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