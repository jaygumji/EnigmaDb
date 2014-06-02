using System;
using System.Collections.Generic;

namespace Enigma
{
    public class BlobComparer : IComparer<byte[]>
    {
        public int Compare(byte[] left, byte[] right)
        {
            return CompareBlobs(left, right);
        }

        public static int CompareBlobs(byte[] left, byte[] right)
        {
            if (left == null) {
                if (right == null) return 0;
                return -1;
            }
            if (right == null) return 1;

            if (left.Length != right.Length)
                return left.Length - right.Length;

            for (var index = 0; index < left.Length; index++) {
                var leftByte = left[index];
                var rightByte = right[index];
                if (leftByte != rightByte)
                    return leftByte - rightByte < 0 ? -1 : 1;
            }
            return 0;
        }

        public static bool AreEqual(byte[] left, byte[] right)
        {
            return CompareBlobs(left, right) == 0;
        }

        public static int GetBlobHashCode(byte[] value)
        {
            if (value == null) return 0;

            var index = 0;
            var hash = 0;
            if (value.Length >= 4) {
                hash = BitConverter.ToInt32(value, 0);
                for (index = 4; index < value.Length - 3; index += 4)
                    hash ^= BitConverter.ToInt32(value, index);
            }

            if (index >= value.Length) return hash;

            var lastPart = (int) value[index];
            if (index + 1 < value.Length)
                lastPart |= value[index + 1] << 8;
            if (index + 2 < value.Length)
                lastPart |= value[index + 2] << 16;

            return hash ^ lastPart;
        }
    }
}
