using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class Int32Key : BinaryKey
    {
        private readonly Int32 _value;
        public Int32Key(Int32 value) : base(BitConverter.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
