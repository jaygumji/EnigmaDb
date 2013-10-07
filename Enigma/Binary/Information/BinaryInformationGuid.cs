using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationGuid : IBinaryInformation<Guid>
    {
        private readonly BinaryConverterGuid _converter;

        public BinaryInformationGuid()
        {
            _converter = new BinaryConverterGuid();
        }

        public IBinaryConverter<Guid> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 16; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(Guid value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
