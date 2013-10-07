using Enigma.IO;
using Enigma.Modelling;
using Enigma.Store.Indexes;
using System.Collections.Generic;
using System.IO;

namespace Enigma.Store.FileSystem
{
    class FileSystemIndexConfigurator : IIndexConfigurator
    {
        private readonly string _directory;
        private readonly Model _model;
        private readonly IEntityMap _entityMap;
        private readonly List<IndexConfiguration> _indexes;

        public FileSystemIndexConfigurator(string directory, Model model, string name)
        {
            _directory = directory;
            _model = model;
            _entityMap = model.GetEntity(name);
            _indexes = new IndexConfigurationConverter(model, _entityMap).Convert();
        }

        public IEnumerable<IndexConfiguration> Indexes { get { return _indexes; } }

        public IStreamProvider GetStreamProvider(string name)
        {
            var fileName = string.Concat("IX_", name, ".idx");
            var path = Path.Combine(_directory, fileName);
            return new FileSystemStreamProvider(path);
        }
    }
}
