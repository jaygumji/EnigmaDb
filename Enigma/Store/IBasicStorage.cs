using Enigma.Store.Maintenance;
using System.Collections.Generic;
namespace Enigma.Store
{
    public interface IBasicStorage
    {
        ITableOfContent TableOfContent { get; }
        IStorageMaintenance Maintenance { get; }

        bool TryAdd(IKey key, byte[] content);
        bool TryUpdate(IKey key, byte[] content);
        bool TryRemove(IKey key);
        bool TryGet(IKey key, out byte[] content);


        IEnumerable<byte[]> All();
    }
}
