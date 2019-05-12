using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests
{
    public class ListExtensionsTests
    {
        public static IEnumerable<object[]> BinarySearchIndexOfData()
        {
            yield return new object[]
            {
                new List<int>{ },
                5,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 4 },
                5,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 5 },
                5,
                Maybe.Just(0)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,5,6,7 },
                5,
                Maybe.Just(4)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                5,
                Maybe.Nothing<int>()
            };
        }

        public static IEnumerable<object[]> BinarySearchIndexOfApproximateData()
        {
            yield return new object[]
            {
                new List<int>{ },
                5,
                0,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 4 },
                5,
                0,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 5 },
                5,
                0,
                Maybe.Just(0)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,5,6,7 },
                5,
                -1,
                Maybe.Just(4)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                6,
                1,
                Maybe.Just(4)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                5,
                1,
                Maybe.Just(4)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                4,
                -1,
                Maybe.Just(3)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                9,
                -1,
                Maybe.Just(6)
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                9,
                0,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                9,
                1,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                -6,
                -1,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                -6,
                0,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                -6,
                1,
                Maybe.Just(0)
            };

            yield return new object[]
{
                new List<int>{ 1,2,3,4,6,7,8 },
                15,
                -1,
                Maybe.Just(6)
};

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                16,
                0,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 1,2,3,4,6,7,8 },
                15,
                1,
                Maybe.Nothing<int>()
            };

            yield return new object[]
            {
                new List<int>{ 10,20,30,40,60,70,80 },
                30,
                0,
                Maybe.Just(2)
            };
        }

        [Theory]
        [MemberData(nameof(BinarySearchIndexOfData))]
        public void BinarySearchIndexOfTest(IList<int> xs, int elem, Maybe<int> expected)
        {
            var actual = xs.BinarySearchIndexOf(elem);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(BinarySearchIndexOfApproximateData))]
        public void BinarySearchIndexOfApproximateTest(IList<int> xs, int elem, int roundingDirection, Maybe<int> expected)
        {
            var actual = xs.BinarySearchIndexOfApproximate(elem, roundingDirection, x => x);

            Assert.Equal(expected.HasValue, actual.HasValue);

            if (expected.HasValue)
                Assert.Equal(expected.Value(), actual.Value());
        }
    }
}
