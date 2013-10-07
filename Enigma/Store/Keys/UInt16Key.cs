using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class UInt16Key : BinaryKey
    {
        private readonly UInt16 _value;
        public UInt16Key(UInt16 value) : base(BitConverter.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
