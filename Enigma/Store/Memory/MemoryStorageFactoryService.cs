using Enigma.Modelling;
using Enigma.Store.Indexes;

namespace Enigma.Store.Memory
{
    public class MemoryStorageFactoryService : IStorageFactoryService
    {
        private static Model _model;

        public MemoryStorageFactoryService()
        {
        }

        public Model Model { get { return _model; } }

        public IStorage CreateStorage(string name)
        {
            return new CompositeStorage(new MemoryCompositeStorageConfigurator());
        }

        public IIndexCollection CreateIndexes(string name)
        {
            return new IndexCollection(new MemoryIndexConfigurator(_model, name));
        }

        public void SynchronizeModel(Model model)
        {
            _model = model;
        }

    }
}
