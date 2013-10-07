using Enigma.Store.Indexes;
namespace Enigma.Db.Linq
{
    public class EnigmaIndexOperation
    {
        public string UniqueName { get; set; }
        public CompareOperation Operation { get; set; }
        public object Value { get; set; }
    }
}
