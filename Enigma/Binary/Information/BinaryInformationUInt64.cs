using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationUInt64 : IBinaryInformation<UInt64>
    {
        private readonly BinaryConverterUInt64 _converter;

        public BinaryInformationUInt64()
        {
            _converter = new BinaryConverterUInt64();
        }

        public IBinaryConverter<UInt64> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 8; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(UInt64 value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
