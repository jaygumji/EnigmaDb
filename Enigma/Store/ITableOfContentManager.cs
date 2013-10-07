namespace Enigma.Store
{
    public interface ITableOfContentManager : ITableOfContent
    {
        bool TryReserve(IKey key, out Entry entry);
        void Enable(Entry entry);
        bool TryRemove(IKey key);
    }
}
