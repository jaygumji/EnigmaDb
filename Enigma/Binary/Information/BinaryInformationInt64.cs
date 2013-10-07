using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationInt64 : IBinaryInformation<Int64>
    {
        private readonly BinaryConverterInt64 _converter;

        public BinaryInformationInt64()
        {
            _converter = new BinaryConverterInt64();
        }

        public IBinaryConverter<Int64> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 8; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Int64 value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
