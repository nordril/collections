using System;
using System.Collections.Generic;
using System.Text;

namespace Nordril.Collections
{
    /// <summary>
    /// A store based on a bijection.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IBijectiveStore<TKey, TValue> : IReadOnlyBijectiveStore<TKey, TValue>
    {
        /// <summary>
        /// Adds a new entry to the store.
        /// </summary>
        /// <param name="key">The key of the new element.</param>
        /// <param name="value">The value of the new element.</param>
        /// <exception cref="KeyAlreadyPresentException">If either key already exists.</exception>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Adds many entries to the store at once. Depending on the implemention, this
        /// operation may be more efficient than repeatedly calling <see cref="Add(TKey, TValue)"/>.
        /// </summary>
        /// <param name="values">The elements to add.</param>
        /// <exception cref="KeyAlreadyPresentException">If either key already exists.</exception>
        void AddMany(IEnumerable<(TKey, TValue)> values);
    }
}
