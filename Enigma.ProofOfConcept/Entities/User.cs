using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.ProofOfConcept.Entities
{
    public class User
    {

        public User()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nick { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
    }
}
