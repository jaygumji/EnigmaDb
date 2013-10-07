namespace Enigma
{
    public class Int32KeyGenerator
    {

        private int _id;
        private readonly object _lockObject = new object();

        public Int32KeyGenerator()
        {
        }

        public Int32KeyGenerator(int start)
        {
            _id = start - 1;
        }

        public int Next()
        {
            lock (_lockObject)
                return ++_id;
        }

    }
}
