using System;

namespace Enigma.Caching
{
    internal class InfiniteCachePolicy : ICachePolicy
    {
        public DateTime CalculateExpiration(DateTime createdAt, DateTime lastAccessedAt)
        {
            return DateTime.MaxValue;
        }
    }
}