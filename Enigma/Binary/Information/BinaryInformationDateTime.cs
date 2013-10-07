using Enigma.Binary.Converters;
using System;
namespace Enigma.Binary.Information
{
    public class BinaryInformationDateTime : IBinaryInformation<DateTime>
    {
        private readonly BinaryConverterDateTime _converter;

        public BinaryInformationDateTime()
        {
            _converter = new BinaryConverterDateTime();
        }

        public IBinaryConverter<DateTime> Converter { get { return _converter; } }

        public bool IsFixedLength { get { return true; } }

        public int FixedLength { get { return 8; } }

        IBinaryConverter IBinaryInformation.Converter { get { return _converter; } }

        public int LengthOf(DateTime value)
        {
            return FixedLength;
        }

        public int LengthOf(object value)
        {
            return FixedLength;
        }

    }
}
