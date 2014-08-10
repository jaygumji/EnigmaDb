using System;

namespace Enigma.Caching
{
    /// <summary>
    /// Contract of a cache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Set the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="content">The content</param>
        void Set(object key, object content);

        /// <summary>
        /// Set the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="content">The content</param>
        /// <param name="policy">The policy that controls when the content expires</param>
        void Set(object key, object content, ICachePolicy policy);

        /// <summary>
        /// Try to set content given a key, not overwriting existing content
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="contentGetter">A method that takes the key as input and expects the new content as output</param>
        /// <returns>The cached content</returns>
        /// <remarks>
        /// <para>If the cache already contains the content given the key, it will return that content.
        /// On the other hand if the cache does not contain the content given the key,
        /// it will set it by using the contentGetter function to retrieve the content value</para>
        /// </remarks>
        object TrySet(object key, Func<object, object> contentGetter);

        /// <summary>
        /// Try to set content given a key, not overwriting existing content
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="contentGetter">A method that takes the key as input and expects the new content as output</param>
        /// <param name="policy">The policy that controls when the content expires</param>
        /// <returns>The cached content</returns>
        /// <remarks>
        /// <para>If the cache already contains the content given the key, it will return that content.
        /// On the other hand if the cache does not contain the content given the key,
        /// it will set it by using the contentGetter function to retrieve the content value</para>
        /// </remarks>
        object TrySet(object key, Func<object, object> contentGetter, ICachePolicy policy);

        /// <summary>
        /// Get the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The content</returns>
        /// <exception cref="CachedContentNotFoundException">Will be cast if no content exists that corresponds to the given key</exception>
        object Get(object key);

        /// <summary>
        /// Try to get the content given a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool TryGet(object key, out object content);
    }

    /// <summary>
    /// Contract of a cache
    /// </summary>
    public interface ICache<TKey, TContent> : ICache
    {
        /// <summary>
        /// Set the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="content">The content</param>
        void Set(TKey key, TContent content);

        /// <summary>
        /// Set the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="content">The content</param>
        /// <param name="policy">The policy that controls when the content expires</param>
        void Set(TKey key, TContent content, ICachePolicy policy);
        
        /// <summary>
        /// Try to set content given a key, not overwriting existing content
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="contentGetter">A method that takes the key as input and expects the new content as output</param>
        /// <returns>The cached content</returns>
        /// <remarks>
        /// <para>If the cache already contains the content given the key, it will return that content.
        /// On the other hand if the cache does not contain the content given the key,
        /// it will set it by using the contentGetter function to retrieve the content value</para>
        /// </remarks>
        TContent TrySet(TKey key, Func<TKey, TContent> contentGetter);

        /// <summary>
        /// Try to set content given a key, not overwriting existing content
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="contentGetter">A method that takes the key as input and expects the new content as output</param>
        /// <param name="policy">The policy that controls when the content expires</param>
        /// <returns>The cached content</returns>
        /// <remarks>
        /// <para>If the cache already contains the content given the key, it will return that content.
        /// On the other hand if the cache does not contain the content given the key,
        /// it will set it by using the contentGetter function to retrieve the content value</para>
        /// </remarks>
        TContent TrySet(TKey key, Func<TKey, TContent> contentGetter, ICachePolicy policy);

        /// <summary>
        /// Get the content given a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The content</returns>
        /// <exception cref="CachedContentNotFoundException">Will be cast if no content exists that corresponds to the given key</exception>
        TContent Get(TKey key);

        /// <summary>
        /// Try to get the content given a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool TryGet(TKey key, out TContent content);
    }
}