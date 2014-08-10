using System;

namespace Enigma.Scheduling
{
    public interface IDateConfiguration
    {
        DateTime NextAt(DateTime from);
    }
}