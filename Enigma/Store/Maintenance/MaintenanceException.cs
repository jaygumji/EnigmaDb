using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Store.Maintenance
{
    public class MaintenanceException : Exception
    {

        public MaintenanceException(string message) : base(message) { }

    }
}
