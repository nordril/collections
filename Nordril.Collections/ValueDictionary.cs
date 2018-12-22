using Nordril.Functional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nordril.Collections
{
    /// <summary>
    /// A wrapper around an arbitrary <see cref="IDictionary{TKey, TValue}"/> which determines equality based on the contained keys and values instead of by reference. Two <see cref="ValueDictionary{TKey, TValue}"/>-object are equal if they contain the same set of keys and, for every contained key, <see cref="object.Equals(object)"/> of the corresponding values returns true.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public struct ValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEquatable<ValueDictionary<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        /// <summary>
        /// The underlying dictionary field, which <see cref="Dict"/> returns.
        /// </summary>
        private IDictionary<TKey, TValue> dict;

        /// <summary>
        /// The underlying dictionary.
        /// </summary>
        public IDictionary<TKey, TValue> Dict
        {
            get
            {
                if (dict == null)
                    dict = new Dictionary<TKey, TValue>();
                return dict;
            }
            private set { dict = value; }

        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value) => Dict.Add(key, value);

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => Dict.ContainsKey(key);

        /// <inheritdoc />
        public bool Remove(TKey key) => Dict.Remove(key);

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) => Dict.TryGetValue(key, out value);

        /// <inheritdoc />
        public TValue this[TKey key] { get => Dict[key]; set { Dict[key] = value; } }

        /// <summary>
        /// Creates a new <see cref="ValueDictionary{TKey, TValue}"/> from a pair of keys and values.
        /// </summary>
        /// <param name="pairs">The pairs to put into the dictionary.</param>
        public ValueDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            dict = new Dictionary<TKey, TValue>(pairs);
        }

        /// <summary>
        /// Creates a new <see cref="ValueDictionary{TKey, TValue}"/> from a pair of keys and values.
        /// </summary>
        /// <param name="pairs">The pairs to put into the dictionary.</param>
        public ValueDictionary(IEnumerable<(TKey, TValue)> pairs) : this(pairs.Select(p => new KeyValuePair<TKey, TValue>(p.Item1, p.Item2)))
        {
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys => Dict.Keys;

        /// <inheritdoc />
        public ICollection<TValue> Values => Dict.Values;

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) => Dict.Add(item);

        /// <inheritdoc />
        public void Clear() => Dict.Clear();

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) => Dict.Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Dict.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item) => Dict.Remove(item);

        /// <inheritdoc />
        public int Count => Dict.Count;

        /// <inheritdoc />
        public bool IsReadOnly => Dict.IsReadOnly;

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dict.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Dict.GetEnumerator();

        /// <summary>
        /// Determines equality based on the contents of two <see cref="ValueDictionary{TKey, TValue}"/>-objects. Two objects are equal if they have the same set of keys and if, for every contained key, <see cref="IEquatable{T}.Equals(T)"/> of the values returns true.
        /// </summary>
        /// <param name="other">The other <see cref="ValueDictionary{TKey, TValue}"/>.</param>
        public bool Equals(ValueDictionary<TKey, TValue> other)
        {
            var onlyOtherKeys = other.Keys.Except(Dict.Keys);

            if (onlyOtherKeys.Empty())
            {
                var thisSame = Dict.All(kv =>
                {
                    if (other.TryGetValue(kv.Key, out var otherValue))
                        return kv.Value.Equals(otherValue);
                    else
                        return false;
                });

                return thisSame;
            }
            else
                return false;
        }

        /// <summary>
        /// See <see cref="ValueDictionary{TKey, TValue}.Equals(ValueDictionary{TKey, TValue})"/>.
        /// </summary>
        /// <param name="obj">The other oject.</param>
        public override bool Equals(object obj)
            => obj is ValueDictionary<TKey, TValue> && Equals((ValueDictionary<TKey, TValue>)obj);

        /// <summary>
        /// See <see cref="ValueDictionary{TKey, TValue}.Equals(ValueDictionary{TKey, TValue})"/>.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        public static bool operator ==(ValueDictionary<TKey, TValue> left, ValueDictionary<TKey, TValue> right)
            => left.Equals(right);

        /// <summary>
        /// See <see cref="ValueDictionary{TKey, TValue}.Equals(ValueDictionary{TKey, TValue})"/>.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        public static bool operator !=(ValueDictionary<TKey, TValue> left, ValueDictionary<TKey, TValue> right)
            => !(left == right);

        /// <summary>
        /// Computes the hash based on <see cref="Functional.CollectionExtensions.HashElements{T}(IEnumerable{T})"/>.
        /// </summary>
        public override int GetHashCode() => this.HashElements();

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Dict.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Dict.Values;
    }
}
