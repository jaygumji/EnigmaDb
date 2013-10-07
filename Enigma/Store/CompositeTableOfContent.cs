using System.Collections.Generic;
using System.Linq;

namespace Enigma.Store
{
    public class CompositeTableOfContent : ITableOfContent
    {
        private StorageFragmentCollection _fragments;

        public CompositeTableOfContent(StorageFragmentCollection fragments)
        {
            _fragments = fragments;
        }
        public int Count
        {
            get { return _fragments.Sum(f => f.TableOfContent.Count); }
        }

        public IEntry Get(IKey key)
        {
            IEntry entry;
            if (TryGet(key, out entry))
                return entry;

            throw new EntryNotFoundException(key);
        }

        public bool TryGet(IKey key, out IEntry entry)
        {
            foreach (var fragment in _fragments)
                if (fragment.TableOfContent.TryGet(key, out entry))
                    return true;

            entry = null;
            return false;
        }

        public bool Contains(IKey key)
        {
            return _fragments.Any(f => f.TableOfContent.Contains(key));
        }


        public IEnumerable<Entry> Entries { get { return _fragments.SelectMany(f => f.TableOfContent.Entries); } }
    }
}
