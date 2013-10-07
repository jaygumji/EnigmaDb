using System;
namespace Enigma.Store.Indexes
{
    public class IndexConfiguration
    {
        public string UniqueName { get; set; }
        public Type Type { get; set; }
        public Type EntityType { get; set; }
    }
}
