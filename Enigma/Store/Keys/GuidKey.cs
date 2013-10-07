using Enigma.Store.Binary;
using System;
using System.Text;

namespace Enigma.Store.Keys
{
    public class GuidKey : BinaryKey
    {
        private readonly Guid _value;
        public GuidKey(Guid value) : base(value.ToByteArray())
        {
            _value = value;
        }

        public override string ToString()
        {
            return String.Concat(_value, ", Binary: ", base.ToString());
        }
    }
}
