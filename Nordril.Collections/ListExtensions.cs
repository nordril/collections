using Nordril.Functional.Data;
using System;
using System.Collections.Generic;

namespace Nordril.HedgingEngine.Logic.Collections
{
    /// <summary>
    /// Extension methods for <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Performs a lookup of an element using binary search. The element's index, if present, is returned.
        /// </summary>
        /// <typeparam name="TOuter">The type of elements in the list.</typeparam>
        /// <typeparam name="T">The type of the elements by which to look up.</typeparam>
        /// <param name="xs">The list to search.</param>
        /// <param name="elem">The element to look up.</param>
        /// <param name="comparer">The comparison function. Negative values indicate "the first argument is smaller than the second", 0 indicates "the arguments are equal" and positive values indicate "the first argument is larger than the second".</param>
        /// <param name="selector">The selector to apply to the elements. If null, no selector is used.</param>
        public static Maybe<int> BinarySearchIndexOf<TOuter, T>(this IList<TOuter> xs, T elem, Func<T, T, int> comparer, Func<TOuter, T> selector = null)
            => BinarySearchIndexOfApproximate(xs, elem, comparer, 0, selector);

        /// <summary>
        /// Performs a lookup of an element using binary search and the <see cref="IComparable{T}"/>-implementation to guide it.
        /// </summary>
        /// <typeparam name="TOuter">The type of elements in the list.</typeparam>
        /// <typeparam name="T">The type of the elements by which to look up.</typeparam>
        /// <param name="xs">The list of search.</param>
        /// <param name="elem">The element to look up.</param>
        /// <param name="selector">The selector to apply to the elements. If null, no selector is used.</param>
        public static Maybe<int> BinarySearchIndexOf<TOuter, T>(this IList<TOuter> xs, T elem, Func<TOuter, T> selector = null) where T : IComparable<T>
            => BinarySearchIndexOf(xs, elem, (x, y) => x.CompareTo(y), selector);

        /// <summary>
        /// Performs a lookup of an element using binary search. The element's index, if present, is returned. If the element isn't found, <paramref name="roundingDirection"/> can be used to return its nearest left or right closest match.
        /// </summary>
        /// <typeparam name="TOuter">The type of elements in the list.</typeparam>
        /// <typeparam name="T">The type of the elements by which to look up.</typeparam>
        /// <param name="xs">The list to search.</param>
        /// <param name="elem">The element to look up.</param>
        /// <param name="comparer">The comparison function. Negative values indicate "the first argument is smaller than the second", 0 indicates "the arguments are equal" and positive values indicate "the first argument is larger than the second".</param>
        /// <param name="roundingDirection">The rounding direction. 0 means "no rounding allowed". Positive values mean "if there's no exact match, get the next larger element, if present" and negative values mean "if there's no exact match, get the next smaller value, if present".</param>
        /// <param name="selector">The selector to apply to the elements. If null, no selector is used.</param>
        public static Maybe<int> BinarySearchIndexOfApproximate<TOuter, T>(
            this IList<TOuter> xs,
            T elem,
            Func<T, T, int> comparer,
            int roundingDirection,
            Func<TOuter, T> selector = null)
        {

            if (selector == null)
                selector = x => (T)(object)x;

            Maybe<int> search(int minIndex, int maxIndex)
            {
                while (true)
                {
                    //The search is finished and we didn't find the element.
                    //Depending on the rounding direction, we can try to return the nearest one.
                    if (minIndex > maxIndex)
                    {
                        //No rounding allows: fail.
                        if (roundingDirection == 0)
                            return Maybe.Nothing<int>();
                        //Rounding allowed: we try to round up or down, and if the result is still a valid index, we return that.
                        else
                        {
                            var index = roundingDirection < 0 ? (minIndex + maxIndex) / 2 : (minIndex + maxIndex + 1) / 2;
                            return Maybe.JustIf(index >= 0 && index <= xs.Count, () => index);
                        }
                    }
                    //Go into the lower or upper half of the list
                    else
                    {
                        var splitPoint = (minIndex + maxIndex) / 2;
                        var comparison = comparer(selector(xs[splitPoint]), elem);

                        //We found the element. Yay!
                        if (comparison == 0)
                            return Maybe.Just(splitPoint);
                        //Go left or get the proximate element.
                        else if (comparison > 0)
                            maxIndex = splitPoint - 1;
                        //Go right.
                        else
                            minIndex = splitPoint + 1;
                    }
                }
            }

            return search(0, xs.Count - 1);
        }

        /// <summary>
        /// Performs a lookup of an element using binary search  and the <see cref="IComparable{T}"/>-implementation to guide it. The element's index, if present, is returned. If the element isn't found, <paramref name="roundingDirection"/> can be used to return its nearest left or right closest match.
        /// </summary>
        /// <typeparam name="TOuter">The type of elements in the list.</typeparam>
        /// <typeparam name="T">The type of the elements by which to look up.</typeparam>
        /// <param name="xs">The list to search.</param>
        /// <param name="elem">The element to look up.</param>
        /// <param name="roundingDirection">The rounding direction. 0 means "no rounding allowed". Positive values mean "if there's no exact match, get the next larger element, if present" and negative values mean "if there's no exact match, get the next smaller value, if present".</param>
        /// <param name="selector">The selector to apply to the elements. If null, no selector is used.</param>
        public static Maybe<int> BinarySearchIndexOfApproximate<TOuter, T>(this IList<TOuter> xs, T elem, int roundingDirection, Func<TOuter, T> selector)
            where T : IComparable<T>
            => BinarySearchIndexOfApproximate(xs, elem, (x, y) => x.CompareTo(y), roundingDirection, selector);
    }
}
