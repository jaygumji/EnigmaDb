using System;

namespace Enigma.Scheduling
{
    public class NeverConfiguration : IDateTimeConfiguration
    {

        public static readonly NeverConfiguration Instance = new NeverConfiguration();

        private NeverConfiguration() { }

        DateTime IDateTimeConfiguration.GetNext(DateTime @from)
        {
            return DateTime.MinValue;
        }
    }
}