using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Db
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(object id) : base("Entity with id " + id.ToString() + " was not found")
        {
        }
    }
}
