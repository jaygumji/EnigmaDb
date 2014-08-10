using System;

namespace Enigma.Caching
{
    /// <summary>
    /// Thrown when requesting a content with a key that didn't exist in the cache
    /// </summary>
    public class CachedContentNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="CachedContentNotFoundException"/>
        /// </summary>
        /// <param name="key">The requested cache key</param>
        public CachedContentNotFoundException(object key) : base(string.Format("Content was not found, supplied key was {0}", key)) { }
    }
}