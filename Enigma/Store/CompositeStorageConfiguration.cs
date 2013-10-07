namespace Enigma.Store
{
    public class CompositeStorageConfiguration
    {

        public CompositeStorageConfiguration()
        {
            FragmentSize = DataSize.FromMB(5);
        }

        public DataSize FragmentSize { get; set; }
    }
}
