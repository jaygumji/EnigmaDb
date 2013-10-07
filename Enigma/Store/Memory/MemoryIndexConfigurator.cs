using Enigma.IO;
using Enigma.Modelling;
using Enigma.Store.Indexes;
using System.Collections.Generic;

namespace Enigma.Store.Memory
{
    class MemoryIndexConfigurator : IIndexConfigurator
    {
        private readonly Model _model;
        private readonly IEntityMap _entityMap;
        private readonly List<IndexConfiguration> _indexes;

        public MemoryIndexConfigurator(Model model, string name)
        {
            _model = model;
            _entityMap = model.GetEntity(name);
            _indexes = new IndexConfigurationConverter(model, _entityMap).Convert();
        }

        public IEnumerable<IndexConfiguration> Indexes { get { return _indexes; } }

        public IStreamProvider GetStreamProvider(string name)
        {
            return new MemoryStreamProvider((int) DataSize.FromKB(5).Value);
        }
    }
}
