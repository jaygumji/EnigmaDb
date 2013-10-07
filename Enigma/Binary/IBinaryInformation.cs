namespace Enigma.Binary
{
    public interface IBinaryInformation
    {
        bool IsFixedLength { get; }
        int FixedLength { get; }
        IBinaryConverter Converter { get; }

        int LengthOf(object value);
    }

    public interface IBinaryInformation<T> : IBinaryInformation
    {
        new IBinaryConverter<T> Converter { get; }
        
        int LengthOf(T value);
    }

}
