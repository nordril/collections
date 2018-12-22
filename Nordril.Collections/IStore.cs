using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A mutable store which has the same interface as a dictionary and which can be copied.
    /// </summary>
    /// <typeparam name="TKey">The keys of the stored elements.</typeparam>
    /// <typeparam name="TValue">The elements which are stored.</typeparam>
    public interface IStore<TKey, TValue> : IReadOnlyStore<TKey, TValue>
    {
        /// <summary>
        /// Adds a new entry to the store.
        /// </summary>
        /// <param name="key">The key of the new element.</param>
        /// <param name="value">The value of the new element.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Adds many entries to the store at once. Depending on the implemention, this
        /// operation may be more efficient than repeatedly calling <see cref="Add(TKey, TValue)"/>.
        /// </summary>
        /// <param name="values">The elements to add.</param>
        void AddMany(IEnumerable<(TKey, TValue)> values);
    }
}
