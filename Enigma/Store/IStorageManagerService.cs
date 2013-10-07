using System.Collections.Generic;
namespace Enigma.Store
{
    public interface IStorageManagerService
    {
        void ApplyTruncatedData(byte[] tableOfContent, byte[] content);
        byte[] GetContent(Entry entry);
}
}
