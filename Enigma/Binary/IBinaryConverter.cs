namespace Enigma.Binary
{
    public interface IBinaryConverter
    {
        object Convert(byte[] value);
        object Convert(byte[] value, int startIndex);
        object Convert(byte[] value, int startIndex, int length);
        byte[] Convert(object value);
        void Convert(object value, byte[] buffer);
        void Convert(object value, byte[] buffer, int offset);
    }

    public interface IBinaryConverter<T> : IBinaryConverter
    {
        new T Convert(byte[] value);
        new T Convert(byte[] value, int startIndex);
        new T Convert(byte[] value, int startIndex, int length);
        byte[] Convert(T value);
        void Convert(T value, byte[] buffer);
        void Convert(T value, byte[] buffer, int offset);
    }
}
