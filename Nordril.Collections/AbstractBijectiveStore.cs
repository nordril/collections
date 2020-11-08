using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nordril.Functional.Data;

namespace Nordril.Collections
{
    /// <summary>
    /// An abstract store which can be subclassed to obtain concrete stores.
    /// A bijective stroe is a bijection with an immutable/mutable-split: in its immutable version, it can only be read from, and one can make a mutable deep copy. This mutable deep copy can then be freely modified without affecting the original.
    /// </summary>
    /// <typeparam name="TKey">The key of the stored elements.</typeparam>
    /// <typeparam name="TValue">The elements which are stored.</typeparam>
    [Obsolete]
    public abstract class AbstractBijectiveStore<TKey, TValue> : IBijectiveStore<TKey, TValue>
    {
        /// <summary>
        /// The underlying dictionary of values.
        /// </summary>
        protected Bijection<TKey, TValue> Store { get; private set; } = new Bijection<TKey, TValue>();

        /// <summary>
        /// The copying-function for the store's values, in case a copy of the store is requested.
        /// </summary>
        protected Func<TValue, TValue> Copier { get; set; }

        /// <summary>
        /// Creates a new store with the supplied values <paramref name="values"/>. The values are copied via <paramref name="copier"/>.
        /// </summary>
        /// <param name="values">The values to put into the store.</param>
        /// <param name="copier">The copier which will be applied to every value, to store a copy in this store.</param>
        protected AbstractBijectiveStore(IEnumerable<KeyValuePair<TKey, TValue>> values, Func<TValue, TValue> copier)
        {
            Copier = copier;
            if (values == null)
                values = new KeyValuePair<TKey, TValue>[0];
            Store = new Bijection<TKey, TValue>(values.Select(kv => new KeyValuePair<TKey, TValue>(kv.Key, copier(kv.Value))));
        }

        /// <summary>
        /// Creates a new store with the supplied values <paramref name="values"/>. The values are copied via <paramref name="copier"/>.
        /// </summary>
        /// <param name="values">The values to put into the store.</param>
        /// <param name="copier">The copier which will be applied to every value, to store a copy in this store.</param>
        protected AbstractBijectiveStore(IEnumerable<(TKey, TValue)> values, Func<TValue, TValue> copier)
        {
            Copier = copier;
            Store = new Bijection<TKey, TValue>(values.Select(kv => new KeyValuePair<TKey, TValue>(kv.Item1, copier(kv.Item2))));
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => Store.ContainsKey(key);

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) => Store.TryGetValue(key, out value);

        /// <inheritdoc />
        public TValue this[TKey key] => Store[key];

        /// <inheritdoc />
        public IEnumerable<TKey> Keys => Store.Keys;

        /// <inheritdoc />
        public IEnumerable<TValue> Values => Store.Values;

        /// <inheritdoc />
        public int Count => Store.Count;

        /// <inheritdoc />
        public TKey this[TValue key, TagRight tag] => Store[key, tag];

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Store.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Store.GetEnumerator();

        /// <inheritdoc />
        public void Add(TKey key, TValue value) => Store.Add(key, value);

        /// <inheritdoc />
        public void AddMany(IEnumerable<(TKey, TValue)> values)
        {
            foreach (var kv in values)
                Store.Add(kv.Item1, kv.Item2);
        }

        /// <summary>
        /// Creates a deep copy of the store.
        /// </summary>
        /// <remarks>
        /// Implementors can use the protected constructor of <see cref="AbstractStore{TKey, TValue}"/> to have the underlying dictionary copied.
        /// </remarks>
        public abstract IBijectiveStore<TKey, TValue> CopyMutable();

        /// <inheritdoc />
        public bool ContainsRightKey(TValue key) => Store.ContainsRightKey(key);

        /// <inheritdoc />
        public bool TryGetLeftValue(TValue key, out TKey value) => Store.TryGetLeftValue(key, out value);
    }
}
