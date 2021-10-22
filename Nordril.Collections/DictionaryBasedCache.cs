using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A thread-safe dictionary-based cache with
    /// <list type="table">
    ///     <listheader>
    ///         <term>Operation</term>
    ///         <description>Runtime</description>
    ///     </listheader>
    ///     
    ///     <item>
    ///         <term>Retrieval of a non-cached element</term>
    ///         <description>O(1).</description>
    ///     </item>
    ///     <item>
    ///         <term>Retrieval of a cached element E</term>
    ///         <description>O(r) where r is the number of elements which were more recently accessed than E.</description>
    ///     </item>
    ///     <item>
    ///         <term>Removal of an old element due to cache-size limits</term>
    ///         <description>O(1).</description>
    ///     </item>
    ///     <item>
    ///         <term>Cache-resizing</term>
    ///         <description>O(max(m-n, n, 1)) where m is the old cache-size and n is the new cache size.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The values to store.</typeparam>
    public class DictionaryBasedCache<TKey, TValue> : IDictionaryBasedCache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> dict;
        private readonly Dictionary<TKey, int> cachePositions;
        private readonly List<TKey> cacheRetrievalHistory;
        private readonly object cacheLock = new object();
        private int cacheSize = 0;

        /// <summary>
        /// Creates a new <see cref="DictionaryBasedCache{TKey, TValue}"/> with the specified cache size.
        /// </summary>
        /// <param name="cacheSize">The cache size.</param>
        public DictionaryBasedCache(int cacheSize)
        {
            dict = new ConcurrentDictionary<TKey, TValue>();
            cachePositions = new Dictionary<TKey, int>(cacheSize);
            cacheRetrievalHistory = new List<TKey>(cacheSize);
            this.cacheSize = cacheSize;
        }

        /// <inheritdoc />
        public void SetCacheSize(int size)
        {
            lock (cacheLock)
            {
                if (size < 0)
                    throw new NegativeSizeException($"Tried to set a negative cache-size on {nameof(DictionaryBasedCache<TKey, TValue>)}.");

                //Case 1: the cache is to be enlarged.
                //Functionally, we could do nothing, but we ensure larger capacities so that we only have to re-size once.
                if (size > cacheSize)
                {
                    cacheRetrievalHistory.Capacity = size;
#if NETCORE
                    cachePositions.EnsureCapacity(size);
#endif
                    cacheSize = size;
                }
                //Case 2: the cache is to be shrunk.
                //Remove as many of the oldest elements as needed.
                else if (size < cacheSize)
                {
                    for (int i = cacheRetrievalHistory.Count-1; i >= size;i--)
                    {
                        var keyToRemove = cacheRetrievalHistory[i];
                        cacheRetrievalHistory.RemoveAt(i);
                        cachePositions.Remove(keyToRemove);
                        dict.TryRemove(keyToRemove, out var _);
                        cacheSize = size;
                    }
                }
                //Case 3: the old and new size are identical -> do nothing.
                else
                    return;
            }
        }

        /// <inheritdoc />
        public CacheResult RetrieveOrCache(TKey key, Func<TValue> newValue, out TValue value)
        {
            lock (cacheLock)
            {
                //Case 1: the value is cached -> return it and put the retrieved key at the front of the history.
                if (dict.TryGetValue(key, out value))
                {
                    var oldPosition = cachePositions[key];
                    cacheRetrievalHistory.RemoveAt(oldPosition);

                    for (int i =0; i< oldPosition;i++)
                        cachePositions.Update(cacheRetrievalHistory[i], x => x + 1);

                    cacheRetrievalHistory.Insert(0, key);
                    cachePositions.Update(key, _ => 0);
                    cachePositions[key] = 0;
                    return CacheResult.WasFoundCached;
                }
                //Case 2: the value is not cached, but we can cache it without deleting any old element.
                else if (dict.Count < cacheSize)
                {
                    cacheRetrievalHistory.Insert(0, key);
                    cachePositions.Add(key, 0);
                    value = newValue();
                    dict.TryAdd(key, value);
                    return CacheResult.WasInserted;
                }
                //Case 3: the value is not cached and we can cache it by removing an old element.
                else if (cacheSize > 0)
                {
                    var keyToRemove = cacheRetrievalHistory[cacheRetrievalHistory.Count - 1];
                    cacheRetrievalHistory.RemoveAt(cacheRetrievalHistory.Count - 1);
                    cachePositions.Remove(keyToRemove);

                    dict.Remove(keyToRemove, out var _);

                    cacheRetrievalHistory.Insert(0, key);
                    cachePositions.Add(key, 0);
                    value = newValue();
                    dict.TryAdd(key, value);
                    return CacheResult.WasInserted;
                }
                //Case 4: the value is not cached and we can't cache it because the cache-size is 0.
                else
                {
                    value = newValue();
                    return CacheResult.WasMissingAndNotCached;
                }
            }
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => dict.ContainsKey(key);

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);

        /// <inheritdoc />
        public TValue this[TKey key] => dict[key];

        /// <inheritdoc />
        public IEnumerable<TKey> Keys => dict.Keys;

        /// <inheritdoc />
        public IEnumerable<TValue> Values => dict.Values;

        /// <inheritdoc />
        public int Count => dict.Count;

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
