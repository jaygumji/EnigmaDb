using System;

namespace Enigma.Scheduling
{
    public class InvalidSchedulerConfigurationException : Exception
    {
        public InvalidSchedulerConfigurationException(string message) : base(message)
        {
        }
    }
}