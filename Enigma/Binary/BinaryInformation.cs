using Enigma.Binary.Information;
using System;
using System.Collections.Generic;
namespace Enigma.Binary
{
    public static class BinaryInformation
    {

        public static readonly BinaryInformationInt16 Int16 = new BinaryInformationInt16();
        public static readonly BinaryInformationInt32 Int32 = new BinaryInformationInt32();
        public static readonly BinaryInformationInt64 Int64 = new BinaryInformationInt64();
        public static readonly BinaryInformationUInt16 UInt16 = new BinaryInformationUInt16();
        public static readonly BinaryInformationUInt32 UInt32 = new BinaryInformationUInt32();
        public static readonly BinaryInformationUInt64 UInt64 = new BinaryInformationUInt64();
        public static readonly BinaryInformationSingle Single = new BinaryInformationSingle();
        public static readonly BinaryInformationDouble Double = new BinaryInformationDouble();
        public static readonly BinaryInformationDateTime DateTime = new BinaryInformationDateTime();
        public static readonly BinaryInformationTimeSpan TimeSpan = new BinaryInformationTimeSpan();
        public static readonly BinaryInformationByte Byte = new BinaryInformationByte();
        public static readonly BinaryInformationSByte SByte = new BinaryInformationSByte();
        public static readonly BinaryInformationGuid Guid = new BinaryInformationGuid();
        public static readonly BinaryInformationDecimal Decimal = new BinaryInformationDecimal();
        public static readonly BinaryInformationString String = new BinaryInformationString();
        public static readonly BinaryInformationBoolean Boolean = new BinaryInformationBoolean();


        private static readonly Dictionary<Type, IBinaryInformation> Lookup;

        static BinaryInformation()
        {
            Lookup = new Dictionary<Type, IBinaryInformation> {
                {typeof (Int16), Int16},
                {typeof (Int32), Int32},
                {typeof (Int64), Int64},
                {typeof (UInt16), UInt16},
                {typeof (UInt32), UInt32},
                {typeof (UInt64), UInt64},
                {typeof (Single), Single},
                {typeof (Double), Double},
                {typeof (DateTime), DateTime},
                {typeof (TimeSpan), TimeSpan},
                {typeof (Byte), Byte},
                {typeof (SByte), SByte},
                {typeof (Guid), Guid},
                {typeof (Decimal), Decimal},
                {typeof (String), String},
                {typeof (Boolean), Boolean}
            };
        }

        public static IBinaryInformation<T> Of<T>()
        {
            return (IBinaryInformation<T>)Of(typeof(T));
        }

        public static IBinaryInformation Of(Type type)
        {
            IBinaryInformation information;
            if (!Lookup.TryGetValue(type, out information))
                throw new ArgumentException("No binary information has been specified for type " + type.FullName);

            return information;
        }

    }
}
