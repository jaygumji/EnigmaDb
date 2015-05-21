using System.Collections.Generic;

namespace Enigma.Test.Serialization.Graphs
{
    public class DictionaryWithCollectionValueGraph
    {
        public Dictionary<string, List<int>> Value { get; set; }
    }
}