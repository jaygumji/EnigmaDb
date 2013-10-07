using System;

namespace Enigma.Modelling
{
    public class RegistrationAlreadyExistsException : Exception
    {
        public RegistrationAlreadyExistsException(string name) : base("Registration with name " + name + " already exists")
        {
        }
    }
}
