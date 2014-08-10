using System;

namespace Enigma.Scheduling
{
    public interface IDateTimeConfiguration
    {
        DateTime GetNext(DateTime from);
    }
}