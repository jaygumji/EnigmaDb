using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithCollectionKeyAndValueGraph
    {
        public Dictionary<List<int>, List<string>> Value { get; set; }
    }
}