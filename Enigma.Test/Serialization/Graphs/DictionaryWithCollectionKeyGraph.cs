using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithCollectionKeyGraph
    {
        public Dictionary<List<int>, string> Value { get; set; }
    }
}