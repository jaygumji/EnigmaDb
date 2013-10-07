using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationInt16 : IBinaryInformation<Int16>
    {
        private readonly BinaryConverterInt16 _converter;

        public BinaryInformationInt16()
        {
            _converter = new BinaryConverterInt16();
        }

        public IBinaryConverter<Int16> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 2; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Int16 value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
