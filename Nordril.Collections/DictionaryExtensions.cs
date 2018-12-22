using System;
using System.Collections.Generic;
using System.Linq;

namespace Nordril.HedgingEngine.Logic.Collections
{
    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Updates a value by applying <paramref name="f"/> if the key <paramref name="key"/> is present.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to update.</param>
        /// <param name="key">The key.</param>
        /// <param name="f">The function to apply to the value.</param>
        /// <returns>true if <paramref name="f"/> was applied, false otherwise.</returns>
        public static bool Update<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TValue> f)
        {
            if (dict.TryGetValue(key, out var oldValue))
            {
                dict[key] = f(oldValue);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Adds a new to a collection-valued dictionary, creating the key with a 1-element collection first if it doesn't exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the values, of which the dictionary contains collections.</typeparam>
        /// <typeparam name="TCollection">The type of the collection which stores the values.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key to which to add another value.</param>
        /// <param name="value">The value to associate with the key.</param>
        public static void AddToCollection<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> dict, TKey key, TValue value)
            where TCollection : class, ICollection<TValue>, new()
        {
            if (dict.TryGetValue(key, out var values))
                values.Add(value);
            else
            {
                var toInsert = new TCollection();
                toInsert.Add(value);
                dict.Add(key, toInsert);
            }
        }

        /// <summary>
        /// Adds a new to a set-valued dictionary, creating the key with a 1-element set first if it doesn't exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the values, of which the dictionary contains sets.</typeparam>
        /// <typeparam name="TSet">The type of the set which stores the values.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key to which to add another value.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <returns>true if the value was not already present in the set associated with the key, false otherwise.</returns>
        public static bool AddToSet<TKey, TValue, TSet>(this IDictionary<TKey, TSet> dict, TKey key, TValue value)
            where TSet : class, ISet<TValue>, new()
        {
            if (dict.TryGetValue(key, out var values))
                return values.Add(value);
            else
            {
                var toInsert = new TSet();
                toInsert.Add(value);
                dict.Add(key, toInsert);
                return true;
            }
        }

        /// <summary>
        /// Updates a value by applying <paramref name="f"/> if the key <paramref name="key"/> is present. If not, <paramref name="def"/> is inserted.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to update.</param>
        /// <param name="key">The key.</param>
        /// <param name="def">The default value to insert if the key was not present.</param>
        /// <param name="f">The function to apply to the value.</param>
        /// <returns>true if <paramref name="f"/> was applied, false otherwise.</returns>
        public static bool Update<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue def, Func<TValue, TValue> f)
        {
            if (dict.TryGetValue(key, out var oldValue))
            {
                dict[key] = f(oldValue);
                return true;
            }
            else
            {
                dict[key] = def;
                return false;
            }
        }

        /// <summary>
        /// Applies <paramref name="f"/> to all values in a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to update.</param>
        /// <param name="f">The function to apply to the value.</param>
        public static void MapValues<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue, TValue> f)
        {
            //The .ToList() call here important because we can't change a dictionary's values if we're using its iterator.
            foreach (var k in dict.Keys.ToList())
                dict[k] = f(dict[k]);
        }

        /// <summary>
        /// Applies <paramref name="f"/> to all values in a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to update.</param>
        /// <param name="f">The function to apply to the value.</param>
        public static void MapValuesWithKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, TValue, TValue> f)
        {
            //The .ToList() call here important because we can't change a dictionary's values if we're using its iterator.
            foreach (var k in dict.Keys.ToList())
                dict[k] = f(k, dict[k]);
        }
    }
}
