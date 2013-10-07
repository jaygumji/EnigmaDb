using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class Int16Key : BinaryKey
    {
        private readonly Int16 _value;
        public Int16Key(Int16 value) : base(BitConverter.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
