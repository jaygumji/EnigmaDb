using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationInt32 : IBinaryInformation<Int32>
    {
        private readonly BinaryConverterInt32 _converter;

        public BinaryInformationInt32()
        {
            _converter = new BinaryConverterInt32();
        }

        public IBinaryConverter<Int32> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 4; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Int32 value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
