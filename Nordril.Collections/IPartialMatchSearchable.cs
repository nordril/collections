using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A data structure that supports search based on partial queries. A partial query is one which only specifies a subset of the data structure's key to be a certain value.
    /// </summary>
    /// <typeparam name="TKey">The type of the structure's key parts. The keys are composed of an array of key parts.</typeparam>
    /// <typeparam name="TValue">The type of the structure's values.</typeparam>
    public interface IPartialMatchSearchable<TKey, TValue>
    {
        /// <summary>
        /// Returns the number of parts of which the keys of the items are composed.
        /// </summary>
        int Dimensions { get; }

        /// <summary>
        /// Finds all items which match the partial query <paramref name="query"/>. An item <code>X</code> is presumed to have an ordered, 0-indexed list of key-parts and it matches the query <code>query = {(k_1 -&gt; v_1),...,(k_n -&gt; v_n)}</code> iff, for all <code> i in {1,...,n}, value_of_keypart(k_i, X) = v_i</code>.
        /// <br />
        /// As special cases, this means that a query which has values for all key parts is analogous <see cref="IDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/> (an exact match), and a query which has no values specified is analogous to <see cref="IEnumerable{T}.GetEnumerator"/> (getting all stored values).
        /// </summary>
        /// <param name="query">The query to match against the structure. The keys are the indexes of the key parts and the values are the values which those key parts have to have.</param>
        IEnumerable<(TKey[], TValue)> GetValueByPartialQuery(IDictionary<int, TKey> query);

        /// <summary>
        /// See <see cref="GetValueByPartialQuery(IDictionary{int, TKey})"/>.
        /// </summary>
        /// <param name="query">The query to match against the structure.</param>
        IEnumerable<(TKey[], TValue)> GetValueByPartialQuery(params (int, TKey)[] query);
    }
}
