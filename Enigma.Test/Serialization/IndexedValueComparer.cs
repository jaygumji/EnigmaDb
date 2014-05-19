using System.Collections.Generic;

namespace Enigma.Test.Serialization
{
    internal class IndexedValueComparer : IEqualityComparer<KeyValuePair<string, int>>
    {
        public bool Equals(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
        {
            return x.Key == y.Key && x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair<string, int> obj)
        {
            return obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
        }
    }
}