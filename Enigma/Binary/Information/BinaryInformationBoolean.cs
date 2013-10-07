using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationBoolean : IBinaryInformation<Boolean>
    {
        private readonly BinaryConverterBoolean _converter;

        public BinaryInformationBoolean()
        {
            _converter = new BinaryConverterBoolean();
        }

        public IBinaryConverter<Boolean> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 1; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Boolean value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }
    }
}
