using Enigma.IO;
using Enigma.Store.Binary;
using System;
using System.Collections.Generic;
using System.IO;
namespace Enigma.Store.Memory
{
    public class MemoryCompositeStorageConfigurator : ICompositeStorageConfigurator
    {

        private readonly CompositeStorageConfiguration _configuration;

        public MemoryCompositeStorageConfigurator()
        {
            _configuration = new CompositeStorageConfiguration { FragmentSize = DataSize.FromKB(5) };
        }

        public CompositeStorageConfiguration Configuration
        {
            get { return _configuration; }
        }

        public IEnumerable<IStorageFragment> GetFragments()
        {
            return new List<StorageFragment>();
        }

        public IStorageFragment CreateFragment()
        {
            var streamProvider = new MemoryStreamProvider((int)_configuration.FragmentSize.Value);
            var binaryStore = new DualBinaryStore(streamProvider, 0, _configuration.FragmentSize.Value);
            return new StorageFragment(binaryStore);
        }
    }
}
