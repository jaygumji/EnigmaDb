using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationTimeSpan : IBinaryInformation<TimeSpan>
    {
        private readonly BinaryConverterTimeSpan _converter;

        public BinaryInformationTimeSpan()
        {
            _converter = new BinaryConverterTimeSpan();
        }

        public IBinaryConverter<TimeSpan> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 8; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(TimeSpan value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
