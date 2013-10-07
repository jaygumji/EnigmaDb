using Enigma.Binary.Information;
using System;
using System.Collections.Generic;
namespace Enigma.Binary
{
    public static class BinaryInformation
    {

        private static readonly Dictionary<Type, IBinaryInformation> Cache;

        static BinaryInformation()
        {
            Cache = new Dictionary<Type, IBinaryInformation> {
                {typeof(Int16), new BinaryInformationInt16()},
                {typeof(Int32), new BinaryInformationInt32()},
                {typeof(Int64), new BinaryInformationInt64()},
                {typeof(UInt16), new BinaryInformationUInt16()},
                {typeof(UInt32), new BinaryInformationUInt32()},
                {typeof(UInt64), new BinaryInformationUInt64()},
                {typeof(Single), new BinaryInformationSingle()},
                {typeof(Double), new BinaryInformationDouble()},
                {typeof(DateTime), new BinaryInformationDateTime()},
                {typeof(TimeSpan), new BinaryInformationTimeSpan()},
                {typeof(Byte), new BinaryInformationByte()},
                {typeof(SByte), new BinaryInformationSByte()},
                {typeof(Guid), new BinaryInformationGuid()},
                {typeof(Decimal), new BinaryInformationDecimal()},
                {typeof(String), new BinaryInformationString()},
                {typeof(Boolean), new BinaryInformationBoolean()}
            };
        }

        public static IBinaryInformation<T> Of<T>()
        {
            return (IBinaryInformation<T>)Of(typeof(T));
        }

        public static IBinaryInformation Of(Type type)
        {
            IBinaryInformation information;
            if (!Cache.TryGetValue(type, out information))
                throw new ArgumentException("No binary information has been specified for type " + type.FullName);

            return information;
        }

    }
}
