using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationUInt16 : IBinaryInformation<UInt16>
    {
        private readonly BinaryConverterUInt16 _converter;

        public BinaryInformationUInt16()
        {
            _converter = new BinaryConverterUInt16();
        }

        public IBinaryConverter<UInt16> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 2; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(UInt16 value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
