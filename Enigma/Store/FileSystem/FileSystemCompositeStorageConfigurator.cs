using Enigma.IO;
using Enigma.Modelling;
using Enigma.Store;
using Enigma.Store.Binary;
using Enigma.Store.FileSystem;
using Enigma.Store.Indexes;
using System.Collections.Generic;
using System.IO;

namespace Enigma.Store.FileSystem
{
    public class FileSystemCompositeStorageConfigurator : ICompositeStorageConfigurator
    {

        private readonly string _path;
        private readonly CompositeStorageConfiguration _configuration;

        public FileSystemCompositeStorageConfigurator(string path) : this(path, new CompositeStorageConfiguration())
        {
        }

        public FileSystemCompositeStorageConfigurator(string path, CompositeStorageConfiguration configuration)
        {
            _path = path;
            _configuration = configuration;
        }

        public CompositeStorageConfiguration Configuration { get { return _configuration; } }

        public IEnumerable<IStorageFragment> GetFragments()
        {
            var fragments = new List<IStorageFragment>();

            var fileInfo = new FileInfo(_path);
            if (!fileInfo.Exists) return fragments;

            var fragmentCount = fileInfo.Length / _configuration.FragmentSize.Value;

            for (var i = 0; i < fragmentCount; i++)
            {
                var start = i * _configuration.FragmentSize.Value;
                var store = new DualBinaryStore(new FileSystemStreamProvider(_path), start, _configuration.FragmentSize.Value);
                fragments.Add(new StorageFragment(store));
            }
            return fragments;
        }

        public IStorageFragment CreateFragment()
        {
            var fileInfo = new FileInfo(_path);
            var start = fileInfo.Exists ? fileInfo.Length : 0;
            var store = new DualBinaryStore(new FileSystemStreamProvider(_path), start, _configuration.FragmentSize.Value);
            return new StorageFragment(store);
        }
    }
}
