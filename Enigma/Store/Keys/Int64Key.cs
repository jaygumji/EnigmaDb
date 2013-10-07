using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class Int64Key : BinaryKey
    {
        private readonly Int64 _value;
        public Int64Key(Int64 value) : base(BitConverter.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
