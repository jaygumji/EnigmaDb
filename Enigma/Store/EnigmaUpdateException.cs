using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Store
{
    public class EnigmaUpdateException : Exception
    {

        public EnigmaUpdateException(string message) : base(message) { }

        public static EnigmaUpdateException UpdateFailed(IKey key)
        {
            var message = "Unable to update entry, a critical error occured at binary store level. Entry key was " + key.ToString();
            return new EnigmaUpdateException(message);
        }

    }
}
