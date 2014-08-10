using System;

namespace Enigma.Caching
{
    internal class CacheContent<T>
    {
        private readonly DateTime _createdAt;
        private readonly DateTime _lastAccessedAt;
        private readonly T _content;
        private readonly ICachePolicy _policy;

        public CacheContent(T content, ICachePolicy policy)
        {
            _content = content;
            _policy = policy;
            _createdAt = DateTime.Now;
            _lastAccessedAt = DateTime.Now;
        }

        public DateTime CreatedAt { get { return _createdAt; } }

        public DateTime LastAccessedAt { get { return _lastAccessedAt; } }

        public T Content { get { return _content; } }
        public DateTime ExpiresAt { get { return _policy.CalculateExpiration(_createdAt, _lastAccessedAt); } }

        public bool Validate()
        {
            var expiresAt = _policy.CalculateExpiration(_createdAt, _lastAccessedAt);
            return expiresAt >= DateTime.Now;
        }

    }
}