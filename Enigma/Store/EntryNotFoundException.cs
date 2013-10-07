using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Store
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException(IKey key) : base(string.Format("Entry with key {0} was not found", key))
        {
        }
    }
}
