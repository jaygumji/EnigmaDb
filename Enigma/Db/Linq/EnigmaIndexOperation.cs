using Enigma.Store.Indexes;

namespace Enigma.Db.Linq
{
    public class EnigmaIndexOperation
    {
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public CompareOperation Operation { get; set; }
        public object Value { get; set; }
    }
}
