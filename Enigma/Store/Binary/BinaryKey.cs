﻿using System;
using System.Linq;

namespace Enigma.Store.Binary
{
    /// <summary>
    /// Key in binary format
    /// </summary>
    /// <remarks>
    /// <para>Although keys can be in many different formats, they are always treated as binary keys.
    /// This is to prevent the store from the need to have knowledge what type the key was originally in and thus simplifying the management of keys.
    /// And yet making it easy to compare with typed keys like int32 by comparing with the binary format of int32.</para>
    /// <para>Currently the binary format can be different between machines that uses little endian repsectively big endian encoding of data.</para>
    /// </remarks>
    public class BinaryKey : IKey
    {
        private readonly byte[] _value;

        public BinaryKey(byte[] value)
        {
            _value = value;
        }

        byte[] IKey.Value { get { return _value; } }

        public static readonly IKey Null = new BinaryKey(new byte[] { });

        public override bool Equals(object obj)
        {
            var other = obj as IKey;
            
            if (other == null) return false;
            if (other.Value.Length != _value.Length) return false;
            return other.Value.SequenceEqual(_value);
        }

        public override int GetHashCode()
        {
            var index = 0;
            var hashCode = 0;
            while (index + 4 < _value.Length)
            {
                hashCode ^= BitConverter.ToInt32(_value, index);
                index += 4;
            }

            while (index < _value.Length)
            {
                hashCode ^= _value[index];
                index++;
            }

            return hashCode;
        }

        public override string ToString()
        {
            if (_value.Length == 0) return "null";
            return BitConverter.ToString(_value);
        }

    }
}
