using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationDecimal : IBinaryInformation<Decimal>
    {
        private readonly BinaryConverterDecimal _converter;

        public BinaryInformationDecimal()
        {
            _converter = new BinaryConverterDecimal();
        }

        public IBinaryConverter<Decimal> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 16; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Decimal value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
