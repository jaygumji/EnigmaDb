using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Enigma.Scheduling;

namespace Enigma.Caching
{
    public class MemoryCache<TKey, TContent> : ICache<TKey, TContent>, IDisposable
    {
        private static readonly TimeSpan Disabled = new TimeSpan(0, 0, 0, 0, -1);

        private readonly ICachePolicy _policy;
        private readonly Timer _timer;
        private readonly DateTimeQueue<TKey> _queue;
        private readonly ConcurrentDictionary<TKey, CacheContent<TContent>> _contents;
        private DateTime _nextTimerExecution;

        public MemoryCache(ICachePolicy policy) : this(policy, EqualityComparer<TKey>.Default)
        {
        }

        public MemoryCache(ICachePolicy policy, IEqualityComparer<TKey> keyComparer)
        {
            _policy = policy;
            _timer = new Timer(CacheValidationCallback, null, Disabled, Disabled);
            _queue = new DateTimeQueue<TKey>();
            _contents = new ConcurrentDictionary<TKey, CacheContent<TContent>>(keyComparer);
            _nextTimerExecution = DateTime.MaxValue;
        }

        private void CacheValidationCallback(object state)
        {
            IEnumerable<TKey> keys;
            while (_queue.TryDequeue(out keys)) {
                foreach (var key in keys) {
                    CacheContent<TContent> content;
                    if (!_contents.TryGetValue(key, out content)) continue;

                    if (content.Validate()) {
                        var expiresAt = content.ExpiresAt;
                        if (expiresAt != DateTime.MaxValue)
                            _queue.Enqueue(content.ExpiresAt, key);
                    }
                    else
                        _contents.TryRemove(key, out content);
                }
            }

            DateTime nextAt;
            if (_queue.TryPeekNextEntryAt(out nextAt))
                SetTimer(DateTime.MaxValue, nextAt);
        }

        private void SetTimer(DateTime currentAt, DateTime expiresAt)
        {
            if (currentAt <= expiresAt) return;
            lock (_timer) {
                if (currentAt <= expiresAt) return;

                _nextTimerExecution = expiresAt;
                _timer.Change(expiresAt.Subtract(DateTime.Now), Disabled);
            }
        }

        private void UpdateExpiryControl(DateTime expiresAt, TKey key)
        {
            if (expiresAt == DateTime.MaxValue) return;
            _queue.Enqueue(expiresAt, key);
            SetTimer(_nextTimerExecution, expiresAt);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        void ICache.Set(object key, object content)
        {
            ((ICache) this).Set(key, content, _policy);
        }

        void ICache.Set(object key, object content, ICachePolicy policy)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (content == null) throw new ArgumentNullException("content");

            if (!(key is TKey))
                throw new ArgumentException("The parameter key must be of type " + typeof(TKey).FullName);

            if (!(content is TContent))
                throw new ArgumentException("The parameter content must be of type " + typeof(TContent).FullName);

            Set((TKey)key, (TContent)content, policy);
        }

        object ICache.TrySet(object key, Func<object, object> contentGetter)
        {
            return ((ICache) this).TrySet(key, contentGetter, _policy);
        }

        object ICache.TrySet(object key, Func<object, object> contentGetter, ICachePolicy policy)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (contentGetter == null) throw new ArgumentNullException("contentGetter");

            if (!(key is TKey))
                throw new ArgumentException("The parameter key must be of type " + typeof(TKey).FullName);

            return TrySet((TKey) key, k => {
                var content = contentGetter(key);
                if (content == null) throw new ArgumentException("The content getter delegate may not return null");

                if (!(content is TContent))
                    throw new ArgumentException("The content must be of type " + typeof (TContent).FullName);

                return (TContent) content;
            });
        }

        object ICache.Get(object key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (!(key is TKey))
                throw new ArgumentException("The parameter key must be of type " + typeof(TKey).FullName);

            return Get((TKey) key);
        }

        bool ICache.TryGet(object key, out object content)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (!(key is TKey))
                throw new ArgumentException("The parameter key must be of type " + typeof(TKey).FullName);

            TContent typedContent;
            var res = TryGet((TKey) key, out typedContent);

            content = typedContent;
            return res;
        }

        public void Set(TKey key, TContent content)
        {
            Set(key, content, _policy);
        }

        public void Set(TKey key, TContent content, ICachePolicy policy)
        {
            _contents.AddOrUpdate(key,
                k => {
                    var cached = new CacheContent<TContent>(content, policy);
                    UpdateExpiryControl(cached.ExpiresAt, k);
                    return cached;
                },
                (k, c) => {
                    var cached = new CacheContent<TContent>(content, policy);
                    UpdateExpiryControl(cached.ExpiresAt, k);
                    return cached;
                });
        }

        public TContent TrySet(TKey key, Func<TKey, TContent> contentGetter)
        {
            return TrySet(key, contentGetter, _policy);
        }

        public TContent TrySet(TKey key, Func<TKey, TContent> contentGetter, ICachePolicy policy)
        {
            var cached = _contents.GetOrAdd(key, k => {
                var c = new CacheContent<TContent>(contentGetter(k), policy);
                UpdateExpiryControl(c.ExpiresAt, k);
                return c;
            });
            return cached.Content;
        }

        public TContent Get(TKey key)
        {
            CacheContent<TContent> cached;
            if (!_contents.TryGetValue(key, out cached))
                throw new KeyNotFoundException("The given key was not found in the cache");

            return cached.Content;
        }

        public bool TryGet(TKey key, out TContent content)
        {
            CacheContent<TContent> cached;
            if (!_contents.TryGetValue(key, out cached)) {
                content = default(TContent);
                return false;
            }

            content = cached.Content;
            return true;
        }
    }
}
