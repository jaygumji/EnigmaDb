namespace Enigma.Store.Indexes
{
    public class IndexEntry<T>
    {
        public int Offset { get; set; }
        public T Value { get; set; }
    }
}
