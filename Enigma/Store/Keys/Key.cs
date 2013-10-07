using Enigma.Store.Binary;
using System;
namespace Enigma.Store.Keys
{
    public static class Key
    {
        public static IKey Create(object id)
        {
            if (id == null) return BinaryKey.Null;

            if (id is Int32) return new Int32Key((Int32)id);
            if (id is Int16) return new Int16Key((Int16)id);
            if (id is Int64) return new Int64Key((Int64)id);
            if (id is UInt32) return new UInt32Key((UInt32)id);
            if (id is UInt16) return new UInt16Key((UInt16)id);
            if (id is UInt64) return new UInt64Key((UInt64)id);
            if (id is Double) return new DoubleKey((Double)id);
            if (id is Single) return new SingleKey((Single)id);
            if (id is Guid) return new GuidKey((Guid)id);
            if (id is String) return new StringKey((String)id);

            throw new ArgumentException("Unable to create a key for parameter id");
        }

        public static IKey Create(byte[] buffer, int startIndex, int length)
        {
            var keyBytes = new byte[length];
            Array.Copy(buffer, startIndex, keyBytes, 0, length);
            return new BinaryKey(keyBytes);
        }

        public static IKey[] EmptyKeys = new IKey[] { };

    }
}
