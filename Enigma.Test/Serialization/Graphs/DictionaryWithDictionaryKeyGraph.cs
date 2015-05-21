using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithDictionaryKeyGraph
    {
        public Dictionary<Dictionary<int, string>, string> Value { get; set; }
    }
}