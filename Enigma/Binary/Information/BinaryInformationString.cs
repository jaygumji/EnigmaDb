using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationString : IBinaryInformation<String>
    {
        private readonly BinaryConverterString _converter;

        public BinaryInformationString()
        {
            _converter = new BinaryConverterString();
        }

        public IBinaryConverter<String> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return false; } }

        public int FixedLength { get { return -1; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(String value)
        {
            return value == null ? 0 : -1;
        }

        public int LengthOf(object value)
        {
            return LengthOf((String) value);
        }

    }
}
