using Enigma.Store.Binary;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Store
{
    public class StorageFragmentCollection : IEnumerable<IStorageFragment>
    {
        private List<IStorageFragment> _fragments;
        private readonly ICompositeStorageConfigurator _configurator;
        private readonly object _fragmentsLock = new object();

        public StorageFragmentCollection(ICompositeStorageConfigurator configurator)
        {
            _configurator = configurator;
            _fragments = new List<IStorageFragment>(configurator.GetFragments());
        }

        public IStorageFragment GetNextAvailableFragment(IKey key, long length)
        {
            var size = EntryBinaryConverter.GetLength(key) + length;
            var fragments = _fragments;
            foreach (var fragment in fragments)
                if (fragment.IsSpaceAvailable(size))
                    return fragment;

            lock (_fragmentsLock)
            {
                if (fragments.Count < _fragments.Count)
                {
                    fragments = _fragments;
                    return fragments[fragments.Count - 1];
                }

                var newFragment = _configurator.CreateFragment();
                var newFragments = _fragments.ToList();
                newFragments.Add(newFragment);
                _fragments = newFragments;
                return newFragment;
            }
        }

        public IEnumerator<IStorageFragment> GetEnumerator()
        {
            return _fragments.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
