using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationSingle : IBinaryInformation<Single>
    {
        private readonly BinaryConverterSingle _converter;

        public BinaryInformationSingle()
        {
            _converter = new BinaryConverterSingle();
        }

        public IBinaryConverter<Single> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 4; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Single value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
