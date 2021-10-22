using Nordril.Functional.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nordril.Collections
{
    /// <summary>
    /// A bijection, i.e. a bidirectional directory.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left keys.</typeparam>
    /// <typeparam name="TRight">The type of the right keys.</typeparam>
    [Obsolete("Use Nordril.Functional.Algebra.IOneToOneRelation instead.")]
    public class Bijection<TLeft, TRight> : IBijection<TLeft, TRight>
    {
        private readonly IDictionary<TLeft, TRight> to = new Dictionary<TLeft, TRight>();
        private readonly IDictionary<TRight, TLeft> from = new Dictionary<TRight, TLeft>();

        /// <inheritdoc />
        public TRight this[TLeft key]
        {
            get => to[key];
            set
            {

                var leftPresent = to.TryGetValue(key, out var right);
                var rightPresent = from.ContainsKey(value);

                //both are present already -> fail if they aren't paired, NO-OP otherwise.
                if (leftPresent && rightPresent)
                {
                    if (!right.Equals(value))
                        throw new KeyAlreadyPresentException(key.ToString());
                }
                //only the left is present -> overwrite the right.
                else if (leftPresent)
                {
                    to[key] = value;
                    from[value] = key;
                }
                //only the right is present -> fail because the existing right is part of a pair, and we now want to attach a 2nd left to it.
                else if (rightPresent)
                    throw new KeyAlreadyPresentException(value.ToString());
                //neither are present -> add as usual.
                else
                    Add(key, value);
            }
        }

        /// <inheritdoc />
        public TLeft this[TRight key, TagRight tag]
        {
            get => from[key];
            set
            {

                var leftPresent = to.ContainsKey(value);
                var rightPresent = from.TryGetValue(key, out var left);

                //both are present already -> fail if they aren't paired, NO-OP otherwise.
                if (leftPresent && rightPresent)
                {
                    if (!left.Equals(value))
                        throw new KeyAlreadyPresentException(key.ToString());
                }
                //only the right is present -> overwrite the left.
                else if (rightPresent)
                {
                    to[value] = key;
                    from[key] = value;
                }
                //only the left is present -> fail because the existing left is part of a pair, and we now want to attach a 2nd right to it.
                else if (leftPresent)
                    throw new KeyAlreadyPresentException(value.ToString());
                //neither are present -> add as usual.
                else
                    Add(value, key);
            }
        }

        /// <inheritdoc />
        IEnumerable<TLeft> IReadOnlyDictionary<TLeft, TRight>.Keys => to.Keys;

        /// <inheritdoc />
        IEnumerable<TRight> IReadOnlyDictionary<TLeft, TRight>.Values => from.Keys;

        /// <summary>
        /// Creates a new, empty instance.
        /// </summary>
        public Bijection()
        {
        }

        /// <summary>
        /// Creates a new instance from a collection of key-value-pairs.
        /// </summary>
        /// <param name="xs">The collection of key-value-pairs.</param>
        public Bijection(IEnumerable<KeyValuePair<TLeft, TRight>> xs)
        {
            to = new Dictionary<TLeft, TRight>(xs);
            from = new Dictionary<TRight, TLeft>(xs.Select(x => KeyValuePair.Create(x.Value, x.Key)));
        }

        /// <inheritdoc />
        public bool ContainsRightKey(TRight key) => from.ContainsKey(key);

        /// <inheritdoc />
        public bool RemoveRight(TRight key)
        {
            if (from.TryGetValue(key, out var left))
                return from.Remove(key) && to.Remove(left);
            else
                return false;
        }

        /// <inheritdoc />
        public bool TryGetLeftValue(TRight key, out TLeft value) => from.TryGetValue(key, out value);

        /// <inheritdoc />
        public void Add(TLeft key, TRight value)
        {
            var leftPresent = to.ContainsKey(key);
            var rightPresent = from.ContainsKey(value);

            if (!leftPresent && !rightPresent)
            {
                to.Add(key, value);
                from.Add(value, key);
            }
            else if (leftPresent && rightPresent)
                throw new ArgumentException("The left and right key were already present in the bijection.");
            else if (leftPresent)
                throw new ArgumentException("The left key was already present in the bijection.", nameof(key));
            else if (rightPresent)
                throw new ArgumentException("The right key was already present in the bijection.", nameof(value));
        }

        /// <inheritdoc />
        public bool ContainsKey(TLeft key) => to.ContainsKey(key);

        /// <inheritdoc />
        public bool Remove(TLeft key)
        {
            if (to.TryGetValue(key, out var right))
                return to.Remove(key) && from.Remove(right);
            else
                return false;
        }

        /// <inheritdoc />
        public bool TryGetValue(TLeft key, out TRight value) => to.TryGetValue(key, out value);

        /// <inheritdoc />
        public ICollection<TLeft> Keys => to.Keys;

        /// <inheritdoc />
        public ICollection<TRight> Values => from.Keys;

        /// <inheritdoc />
        public void Add(KeyValuePair<TLeft, TRight> item) => Add(item.Key, item.Value);

        /// <inheritdoc />
        public void Clear()
        {
            to.Clear();
            from.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TLeft, TRight> item) => to.ContainsKey(item.Key) && from.ContainsKey(item.Value);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TLeft, TRight>[] array, int arrayIndex)
            => to.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TLeft, TRight> item)
        {
            if (to.ContainsKey(item.Key) && from.ContainsKey(item.Value))
                return to.Remove(item.Key) && from.Remove(item.Value);
            else
                return false;
        }

        /// <inheritdoc />
        public int Count => to.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator() => to.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => to.GetEnumerator();
    }
}
