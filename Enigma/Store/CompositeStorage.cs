using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Enigma.Store.Binary;
using Enigma.Store.Maintenance;
using Enigma.Store.Indexes;

namespace Enigma.Store
{

    public class CompositeStorage : IStorage
    {

        private readonly ICompositeStorageConfigurator _configurator;
        private readonly StorageFragmentCollection _fragments;
        private readonly ITableOfContent _tableOfContent;
        private readonly IStorageMaintenance _maintenance;

        public CompositeStorage(ICompositeStorageConfigurator storageConfigurator)
        {
            _configurator = storageConfigurator;
            _fragments = new StorageFragmentCollection(storageConfigurator);
            _tableOfContent = new CompositeTableOfContent(_fragments);
            _maintenance = new CompositeStorageMaintenance(_fragments);
        }

        public ITableOfContent TableOfContent { get { return _tableOfContent; } }
        public IStorageMaintenance Maintenance { get { return _maintenance; } }

        public bool TryAdd(IKey key, byte[] content)
        {
            var fragments = _fragments;
            foreach (var fragment in fragments)
                if (fragment.TableOfContent.Contains(key))
                    return false;

            var availableFragment = _fragments.GetNextAvailableFragment(key, content.Length);
            if (availableFragment.TryAdd(key, content)) return true;

            availableFragment = _fragments.GetNextAvailableFragment(key, content.Length);
            return availableFragment.TryAdd(key, content);
        }

        public bool TryUpdate(IKey key, byte[] content)
        {
            var fragments = _fragments;
            foreach (var fragment in fragments)
            {
                if (!fragment.TableOfContent.Contains(key)) continue;

                if (fragment.TryUpdate(key, content))
                    return true;

                if (!fragment.TryRemove(key))
                    return false;

                var availableFragment = _fragments.GetNextAvailableFragment(key, content.Length);
                if (availableFragment.TryAdd(key, content)) return true;

                availableFragment = _fragments.GetNextAvailableFragment(key, content.Length);
                if (!TryAdd(key, content))
                    throw EnigmaUpdateException.UpdateFailed(key);
            }

            return false;
        }

        public bool TryRemove(IKey key)
        {
            var fragments = _fragments;
            foreach (var fragment in fragments)
                if (fragment.TableOfContent.Contains(key))
                    return fragment.TryRemove(key);

            return false;
        }

        public bool TryGet(IKey key, out byte[] content)
        {
            foreach (var fragment in _fragments)
                if (fragment.TryGet(key, out content))
                    return true;

            content = null;
            return false;
        }

        public IEnumerable<byte[]> All()
        {
            return _fragments.SelectMany(f => f.All()).ToList();
        }
    }
}
