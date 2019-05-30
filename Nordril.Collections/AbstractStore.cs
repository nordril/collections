using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nordril.Collections
{
    /// <summary>
    /// An abstract store which can be subclassed to obtain concrete stores.
    /// A store is a dictionary with an immutable/mutable-split: in its immutable version, it can only be read from, and one can make a mutable deep copy. This mutable deep copy can then be freely modified without affecting the original.
    /// </summary>
    /// <typeparam name="TKey">The key of the stored elements.</typeparam>
    /// <typeparam name="TValue">The elements which are stored.</typeparam>
    public abstract class AbstractStore<TKey, TValue> : IStore<TKey, TValue>
    {
        /// <summary>
        /// The underlying dictionary of values.
        /// </summary>
        protected Dictionary<TKey, TValue> Store { get; private set; } = new Dictionary<TKey, TValue>();

        /// <summary>
        /// The copying-function for the store's values, in case a copy of the store is requested.
        /// </summary>
        protected Func<TValue, TValue> Copier { get; set; }

        /// <summary>
        /// Creates a new store with the supplied values <paramref name="values"/>. The values are copied via <paramref name="copier"/>.
        /// </summary>
        /// <param name="values">The values to put into the store.</param>
        /// <param name="copier">The copier which will be applied to every value, to store a copy in this store.</param>
        protected AbstractStore(IEnumerable<KeyValuePair<TKey, TValue>> values, Func<TValue, TValue> copier)
        {
            Copier = copier;
            if (values == null)
                values = new KeyValuePair<TKey, TValue>[0];
#if NETCORE
            Store = new Dictionary<TKey, TValue>(values.Select(kv => new KeyValuePair<TKey, TValue>(kv.Key, copier(kv.Value))));
#elif NETFULL
            Store = new Dictionary<TKey, TValue>();

            foreach (var x in values)
                Store.Add(x.Key, copier(x.Value));
#endif
        }

        /// <summary>
        /// Creates a new store with the supplied values <paramref name="values"/>. The values are copied via <paramref name="copier"/>.
        /// </summary>
        /// <param name="values">The values to put into the store.</param>
        /// <param name="copier">The copier which will be applied to every value, to store a copy in this store.</param>
        protected AbstractStore(IEnumerable<(TKey, TValue)> values, Func<TValue, TValue> copier)
        {
            Copier = copier;
#if NETCORE
            Store = new Dictionary<TKey, TValue>(values.Select(kv => new KeyValuePair<TKey, TValue>(kv.Item1, copier(kv.Item2))));
#elif NETFULL
            Store = new Dictionary<TKey, TValue>();

            foreach (var x in values)
                Store.Add(x.Item1, copier(x.Item2));
#endif
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
        public abstract IStore<TKey, TValue> CopyMutable();
    }
}
