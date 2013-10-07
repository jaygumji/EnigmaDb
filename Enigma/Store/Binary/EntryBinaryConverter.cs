using Enigma.Binary;
using Enigma.Store.Keys;
using System;
using System.IO;

namespace Enigma.Store.Binary
{
    public class EntryBinaryConverter : IBinaryConverter<Entry>
    {

        public const int LengthValueOffset = 8;
        public const int IsActiveValueOffset = 16;
        public const int KeySizeValueOffset = 17;
        public const int KeyValueOffset = 18;

        public const int ConstantSizePart = 18;

        public Entry Convert(byte[] value)
        {
            return Convert(value, 0);
        }

        private static readonly IBinaryConverter<Entry> Singleton = new EntryBinaryConverter();
        public static IBinaryConverter<Entry> Instance { get { return Singleton; } }

        public static bool IsActive(byte[] value, int startIndex)
        {
            return BitConverter.ToBoolean(value, startIndex + IsActiveValueOffset);
        }

        public static int GetLength(IKey key)
        {
            return ConstantSizePart + key.Value.Length;
        }

        public Entry Convert(byte[] value, int startIndex)
        {
            var entry = new Entry();

            entry.ValueOffset = BitConverter.ToInt64(value, startIndex);
            entry.ValueLength = BitConverter.ToInt64(value, startIndex + LengthValueOffset);
            var keySize = value[startIndex + KeySizeValueOffset];
            entry.Key = Key.Create(value, startIndex + KeyValueOffset, keySize);

            return entry;
        }

        public Entry Convert(byte[] value, int startindex, int length)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Convert(Entry value)
        {
            var keyValue = value.Key.Value;
            var keySize = (byte) keyValue.Length;

            var result = new byte[ConstantSizePart + keySize];

            var offset = BitConverter.GetBytes(value.ValueOffset);
            Array.Copy(offset, 0, result, 0, 8);

            var length = BitConverter.GetBytes(value.ValueLength);
            Array.Copy(length, 0, result, LengthValueOffset, 8);

            var isActive = BitConverter.GetBytes(true);
            result[IsActiveValueOffset] = isActive[0];
            result[KeySizeValueOffset] = keySize;

            Array.Copy(keyValue, 0, result, KeyValueOffset, keySize);

            return result;
        }

        public void ConvertTo(Entry value, Stream stream)
        {
            var keyValue = value.Key.Value;
            var keySize = (byte)keyValue.Length;

            var offset = BitConverter.GetBytes(value.ValueOffset);
            stream.Write(offset, 0, offset.Length);
            var length = BitConverter.GetBytes(value.ValueLength);
            stream.Write(length, 0, length.Length);

            var isActive = BitConverter.GetBytes(true);
            stream.WriteByte(isActive[0]);
            stream.WriteByte(keySize);

            stream.Write(keyValue, 0, keySize);
        }

        object IBinaryConverter.Convert(byte[] value)
        {
            return Convert(value, 0);
        }

        object IBinaryConverter.Convert(byte[] value, int startIndex)
        {
            return Convert(value, startIndex);
        }

        object IBinaryConverter.Convert(byte[] value, int startIndex, int length)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Convert(object value)
        {
            throw new System.NotImplementedException();
        }

        public void Convert(Entry value, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Convert(Entry value, byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        public void Convert(object value, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
