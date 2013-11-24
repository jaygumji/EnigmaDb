using System.Collections.Generic;
namespace Enigma.Db.Linq
{
    public class EnigmaCriteria
    {

        public List<EnigmaIndexOperation> IndexOperations { get; private set; }

        public int Skip { get; set; }
        public int Take { get; set; }

        public EnigmaCriteria()
        {
            IndexOperations = new List<EnigmaIndexOperation>();
        }

    }
}
