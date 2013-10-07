using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationDouble : IBinaryInformation<Double>
    {
        private readonly BinaryConverterDouble _converter;

        public BinaryInformationDouble()
        {
            _converter = new BinaryConverterDouble();
        }

        public IBinaryConverter<Double> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 8; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Double value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
