using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationByte : IBinaryInformation<Byte>
    {
        private readonly BinaryConverterByte _converter;

        public BinaryInformationByte()
        {
            _converter = new BinaryConverterByte();
        }

        public IBinaryConverter<Byte> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 1; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Byte value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
