using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class StringKey : BinaryKey
    {
        private readonly String _value;
        public StringKey(String value) : base(Encoding.UTF8.GetBytes(value))
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
