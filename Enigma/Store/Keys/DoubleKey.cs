using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class DoubleKey : BinaryKey
    {
        private readonly Double _value;
        public DoubleKey(Double value) : base(BitConverter.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }

    }
}
