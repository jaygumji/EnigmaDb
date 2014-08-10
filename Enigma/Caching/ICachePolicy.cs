using System;

namespace Enigma.Caching
{
    public interface ICachePolicy
    {
        DateTime CalculateExpiration(DateTime createdAt, DateTime lastAccessedAt);
    }
}