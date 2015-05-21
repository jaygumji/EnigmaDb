using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithDictionaryValueGraph
    {
        public Dictionary<string, Dictionary<int, string>> Value { get; set; }
    }
}