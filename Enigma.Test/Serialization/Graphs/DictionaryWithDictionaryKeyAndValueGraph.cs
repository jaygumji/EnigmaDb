using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithDictionaryKeyAndValueGraph
    {
        public Dictionary<Dictionary<string, int>, Dictionary<int, string>> Value { get; set; }
    }
}