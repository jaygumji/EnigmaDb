using Enigma.Modelling;
using Enigma.Store.Indexes;
using System.IO;
namespace Enigma.Store.FileSystem
{
    public class FileSystemStorageFactoryService : IStorageFactoryService
    {

        private readonly string _baseDirectory;
        private readonly string _modelPath;
        private readonly CompositeStorageConfiguration _configuration;
        private Model _model;

        public FileSystemStorageFactoryService(string baseDirectory, CompositeStorageConfiguration configuration)
        {
            _baseDirectory = baseDirectory;
            if (!Directory.Exists(_baseDirectory)) Directory.CreateDirectory(_baseDirectory);
            _modelPath = Path.Combine(_baseDirectory, "Model.xml");
            if (File.Exists(_modelPath))
                _model = Model.Load(_modelPath);

            _configuration = configuration;
        }

        public Model Model { get { return _model; } }

        public IStorage CreateStorage(string name)
        {
            var directory = Path.Combine(_baseDirectory, name);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var storagePath = Path.Combine(directory, name + ".dat");

            var storageConfigurator = new FileSystemCompositeStorageConfigurator(storagePath, _configuration);
            var storage = new CompositeStorage(storageConfigurator);
            return storage;
        }

        public IIndexCollection CreateIndexes(string name)
        {
            var directory = Path.Combine(_baseDirectory, name);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return new IndexCollection(new FileSystemIndexConfigurator(directory, _model, name));
        }

        public void SynchronizeModel(Model model)
        {
            if (_model == null)
            {
                _model = model;
                _model.Save(_modelPath);
                return;
            }

            var newModelHash = model.GetHashCode();
            var oldModelHash = _model.GetHashCode();

            if (newModelHash == oldModelHash) return;

            model.CopyFrom(_model);
            _model = model;
            _model.Save(_modelPath);
        }


    }
}
