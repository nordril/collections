using Nordril.Functional.Data;
using System;
using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A finite, 1:1 mapping between values.
    /// </summary>
    /// <typeparam name="TLeft">The left value.</typeparam>
    /// <typeparam name="TRight">The right value.</typeparam>
    [Obsolete("Use Nordril.Functional.Algebra.IOneToOneRelation instead.")]
    public interface IBijection<TLeft, TRight> : IDictionary<TLeft, TRight>, IReadOnlyBijection<TLeft, TRight>
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
        new TLeft this[TRight key, TagRight tag] { get; set; }

        /// <summary>
        /// Removes the element with the specified right key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        bool RemoveRight(TRight key);
    }
}