using Enigma.Store.Binary;
using Enigma.Store.Memory;
using Enigma.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace Enigma.Store.Maintenance
{
    public class StorageMaintenance : IStorageMaintenance, IDisposable
    {

        private MaintenanceMode _mode;
        private readonly IStorageFragment _fragment;
        private readonly IStorageManagerService _service;
        private readonly ExclusiveLock _lock;

        public StorageMaintenance(IStorageFragment fragment, IStorageManagerService service)
        {
            _mode = MaintenanceMode.Normal;
            _service = service;
            _fragment = fragment;
            _lock = new ExclusiveLock();
        }

        public bool IsFragmented { get; set; }

        public MaintenanceMode Mode { get { return _mode; } }

        public ILock Lock { get { return _lock; } }

        public void Truncate()
        {
            if (!IsFragmented) return;

            _mode = MaintenanceMode.Lockdown;
            _lock.EnterExclusive();
            try
            {
                using (var memoryStore = new MemoryBinaryStore())
                {
                    var entries = _fragment.TableOfContent.Entries;
                    var memoryEntries = new List<Entry>();

                    foreach (var entry in entries)
                    {
                        var content = _service.GetContent(entry);
                        long storeOffset;
                        if (!memoryStore.TryWrite(content, out storeOffset))
                            throw new MaintenanceException("Failed to write data block to memory store, " + entry.Key.ToString());

                        memoryEntries.Add(new Entry
                        {
                            Key = entry.Key,
                            ValueLength = content.Length,
                            ValueOffset = storeOffset + 16
                        });
                    }
                    byte[] tableOfContent;
                    using (var stream = new MemoryStream())
                    {
                        var converter = new EntryBinaryConverter();
                        foreach (var entry in memoryEntries)
                            converter.ConvertTo(entry, stream);

                        tableOfContent = stream.ToArray();
                    }
                    var data = memoryStore.ToArray();
                    _service.ApplyTruncatedData(tableOfContent, data);

                }
            }
            finally
            {
                _mode = MaintenanceMode.Normal;
                _lock.ExitExclusive();
            }
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}
