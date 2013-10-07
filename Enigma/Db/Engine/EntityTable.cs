using Enigma.Modelling;
using Enigma.Store;
using Enigma.Store.Indexes;
namespace Enigma.Db.Engine
{
    public class EntityTable
    {

        private readonly IStorage _storage;
        private readonly IIndexCollection _indexes;

        public EntityTable(IStorage storage, IIndexCollection indexes)
        {
            _storage = storage;
            _indexes = indexes;
        }

        public IStorage Storage { get { return _storage; } }
        public IIndexCollection Indexes { get { return _indexes; } }

    }
}
