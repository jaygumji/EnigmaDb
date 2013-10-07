namespace Enigma.Store
{
    public interface IStorageFragment : IBasicStorage
    {

        bool IsSpaceAvailable(long size);

    }
}
