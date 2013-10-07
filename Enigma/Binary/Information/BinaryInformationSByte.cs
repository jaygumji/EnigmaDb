using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationSByte : IBinaryInformation<SByte>
    {
        private readonly BinaryConverterSByte _converter;

        public BinaryInformationSByte()
        {
            _converter = new BinaryConverterSByte();
        }

        public IBinaryConverter<SByte> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 1; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(SByte value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
