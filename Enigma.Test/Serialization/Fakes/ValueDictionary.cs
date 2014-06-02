using System.Collections.Generic;

namespace Enigma.Test.Serialization.Fakes
{
    public class ValueDictionary
    {
        public Dictionary<string, int> Test { get; set; }
    }

    public class ValueDictionaryComparer : IEqualityComparer<KeyValuePair<string, int>>
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
