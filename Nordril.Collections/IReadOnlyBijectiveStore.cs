using System.Collections.Generic;

namespace Nordril.Collections
{
    /// <summary>
    /// A read-only store which has the same interface as a bijection, but which can create mutable copies of itself.
    /// </summary>
    /// <typeparam name="TKey">The key of the stored elements.</typeparam>
    /// <typeparam name="TValue">The elements which are stored.</typeparam>
    public interface IReadOnlyBijectiveStore<TKey, TValue> : IReadOnlyBijection<TKey, TValue>
    {
        /// <summary>
        /// Creates a mutable copy of this store.
        /// </summary>
        IBijectiveStore<TKey, TValue> CopyMutable();
    }
}
