using Nordril.Functional.Data;
using System;
using System.Collections.Generic;

namespace Nordril.HedgingEngine.Logic.Collections
{
    /// <summary>
    /// A finite, 1:1 mapping between values.
    /// </summary>
    /// <typeparam name="TLeft">The left value.</typeparam>
    /// <typeparam name="TRight">The right value.</typeparam>
    public interface IBijection<TLeft, TRight> : IDictionary<TLeft, TRight>
    {
        /// <summary>
        /// Gets or sets the right key associated with the left key.
        /// </summary>
        /// <param name="key">The right key associated with the left key.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and the key is not found.</exception>
        /// <exception cref="KeyAlreadyPresentException">The property was set, but the value was already associated with another left value.</exception>
        new TRight this[TLeft key] { get; set; }

        /// <summary>
        /// Gets or sets the left key associated with the right key.
        /// </summary>
        /// <param name="key">The right key associated with the left key.</param>
        /// <param name="tag">A type-level tag to resolve ambiguity with indexer of <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and the key is not found.</exception>
        /// <exception cref="KeyAlreadyPresentException">The property was set, but the value was already associated with another right value.</exception>
        TLeft this[TRight key, TagRight tag] { get; set; }


        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> contains an element
        /// with the specified right key. This is the efficient version of <see cref="ICollection{T}.Contains(T)"/>
        /// which uses indexing.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        bool ContainsRightKey(TRight key);

        /// <summary>
        /// Removes the element with the specified right key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        bool RemoveRight(TRight key);

        /// <summary>
        /// Gets the left key associated with the specified right key.
        /// </summary>
        /// <param name="key">The right key whose associated left key to get.</param>
        /// <param name="value">The left key, if present; otherwise, the type's default value is returned.</param>
        bool TryGetLeftValue(TRight key, out TLeft value);
    }
}