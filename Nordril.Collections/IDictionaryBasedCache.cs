using System;
using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A dictionary-based, thread-safe cache which uses a least-recently-used (LRU) caching strategy for its keys.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDictionaryBasedCache<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Sets the maximum cache size. No more entries than <paramref name="size"/> will be stored. If the <see cref="IDictionaryBasedCache{TKey, TValue}"/> contains more than <paramref name="size"/> elements at the time of this call, the oldest elements will be purged first, until the following holds:
        /// <code>
        ///    this.Count == size
        /// </code>
        /// </summary>
        /// <param name="size">The maximum size of the cahce.</param>
        /// <exception cref="NegativeSizeException">If <paramref name="size"/> is negative.</exception>
        void SetCacheSize(int size);

        /// <summary>
        /// Either retrieves the cached value for a key or registers a new value <paramref name="newValue"/> and returns that.
        /// </summary>
        /// <param name="key">The key whose value to retrieve.</param>
        /// <param name="newValue">The factory for the new value, which is called if the key isn't found and the result of which is registered if the key isn't found.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>The result of the caching operation.</returns>
        CacheResult RetrieveOrCache(TKey key, Func<TValue> newValue, out TValue value);
    }

    /// <summary>
    /// The result of a caching operation (trying to retrieve an item from cache and optionally entering it into the cache).
    /// </summary>
    public enum CacheResult
    {
        /// <summary>
        /// The element was present in cache and the cached value was returned.
        /// </summary>
        WasFoundCached,
        /// <summary>
        /// The element was not present and a fresh value was inserted into the cache.
        /// </summary>
        WasInserted,
        /// <summary>
        /// The element was not present, but a new value was not cached due to cache limitations.
        /// </summary>
        WasMissingAndNotCached
    }
}
