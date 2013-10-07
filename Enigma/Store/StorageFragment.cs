using Enigma.Store.Binary;
using Enigma.Binary;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Enigma.Store.Maintenance;

namespace Enigma.Store
{
    public class StorageFragment : IStorageFragment, IStorageManagerService
    {

        private ITableOfContentManager _tableOfContent;
        private readonly StorageMaintenance _maintenance;
        private readonly IBinaryStore _tableOfContentStore;
        private readonly IBinaryStore _store;
        private long _reserved;
        private readonly object _reservationLock = new object();
 
        public StorageFragment(IDualBinaryStore store)
        {
            bool isFragmented;
            _tableOfContent = Store.TableOfContent.From(store.Right, out isFragmented);
            _maintenance = new StorageMaintenance(this, this) { IsFragmented = isFragmented };
            _store = store.Left;
            _tableOfContentStore = store.Right;
        }

        public ITableOfContent TableOfContent { get { return _tableOfContent; } }
        public IStorageMaintenance Maintenance { get { return _maintenance; } }

        private bool TryReserve(long size)
        {
            if (!_store.IsSpaceAvailable(size + _reserved)) return false;

            lock (_reservationLock){
                if (!_store.IsSpaceAvailable(size + _reserved)) return false;
                _reserved += size;
            }
            return true;
        }

        private void ReleaseReservation(long size)
        {
            lock (_reservationLock)
                _reserved -= size;
        }

        public bool IsSpaceAvailable(long size)
        {
            return _store.IsSpaceAvailable(size + _reserved);
        }

        public bool TryAdd(IKey key, byte[] content)
        {
            if (_maintenance.Mode == MaintenanceMode.Lockdown)
                return false;

            if (!_maintenance.Lock.TryEnter())
                return false;

            try
            {

                var size = (long)EntryBinaryConverter.GetLength(key) + content.Length;
                if (!TryReserve(size))
                    return false;

                Entry entry;
                if (!_tableOfContent.TryReserve(key, out entry))
                {
                    ReleaseReservation(size);
                    return false;
                }

                long valueOffset;
                if (!_store.TryWrite(content, out valueOffset))
                {
                    _tableOfContent.TryRemove(key);
                    ReleaseReservation(size);
                    return false;
                }

                entry.ValueLength = content.Length;
                entry.ValueOffset = valueOffset;

                _tableOfContent.Enable(entry);
                ReleaseReservation(size);
                return true;
            }
            finally
            {
                _maintenance.Lock.Exit();
            }
        }

        public bool TryUpdate(IKey key, byte[] content)
        {
            using (_maintenance.Lock.Enter())
            {
                IEntry entry;
                if (!_tableOfContent.TryGet(key, out entry))
                    return false;

                if (entry.ValueLength == content.Length)
                {
                    _store.Write(entry.ValueOffset, content);
                    return true;
                }
                else
                {
                    var size = EntryBinaryConverter.GetLength(key) + content.Length;
                    if (!TryReserve(size))
                        return false;

                    if (!TryRemove(key))
                    {
                        ReleaseReservation(size);
                        return false;
                    }

                    Entry replaceEntry;
                    if (!_tableOfContent.TryReserve(key, out replaceEntry))
                    {
                        ReleaseReservation(size);
                        return false;
                    }

                    long valueOffset;
                    if (!_store.TryWrite(content, out valueOffset))
                    {
                        _tableOfContent.TryRemove(key);
                        ReleaseReservation(size);
                        return false;
                    }

                    replaceEntry.ValueLength = content.Length;
                    replaceEntry.ValueOffset = valueOffset;
                    _tableOfContent.Enable(replaceEntry);
                    ReleaseReservation(size);
                    return true;
                }
            }
        }

        public bool TryRemove(IKey key)
        {
            if (_maintenance.Mode == MaintenanceMode.Lockdown)
                if (!_tableOfContent.Contains(key))
                    return false;

            using (_maintenance.Lock.Enter())
            {
                if (_tableOfContent.TryRemove(key))
                {
                    _maintenance.IsFragmented = true;
                    return true;
                }
                return false;
            }
        }

        public bool TryGet(IKey key, out byte[] content)
        {
            if (_maintenance.Mode == MaintenanceMode.Lockdown)
            {
                if (!_tableOfContent.Contains(key))
                {
                    content = null;
                    return false;
                }
            }

            using (_maintenance.Lock.Enter())
            {
                IEntry entry;
                if (!_tableOfContent.TryGet(key, out entry))
                {
                    content = null;
                    return false;
                }

                content = _store.Read(entry.ValueOffset, entry.ValueLength);
                return true;
            }
        }

        public IEnumerable<byte[]> All()
        {
            using (_maintenance.Lock.Enter())
                return _tableOfContent.Entries.Select(e => _store.Read(e.ValueOffset, e.ValueLength)).ToList();
        }

        byte[] IStorageManagerService.GetContent(Entry entry)
        {
            return _store.Read(entry.ValueOffset, entry.ValueLength);
        }

        void IStorageManagerService.ApplyTruncatedData(byte[] tableOfContent, byte[] content)
        {
            _tableOfContentStore.TruncateTo(tableOfContent);
            _store.TruncateTo(content);
            _tableOfContent = Store.TableOfContent.From(_tableOfContentStore);
        }
    }
}
