using Nordril.Collections.MethodCache;
using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests.MethodCache
{
    public static class MaybeCacheTests
    {
        public static IEnumerable<object[]> IntMaybeCreationData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 6 };
        }

        public static IEnumerable<object[]> StringMaybeCreationData()
        {
            yield return new object[] { "xyz" };
        }

        public static IEnumerable<object[]> IntListMaybeCreationData()
        {
            yield return new object[] {
                new List<int>()
            };
            yield return new object[] {
                new List<int>{ 5, 6 }
            };
            yield return new object[] {
                new List<int> { 8, 8, 3, 2, 1 }
            };
        }

        [Theory]
        [MemberData(nameof(IntMaybeCreationData))]
        public static void IntMaybeCreation(int x)
        {
            var expected = Maybe.Just(x);
            var cache = new MaybeCache(1);
            var actual = (Maybe<int>)cache.RetrieveOrCacheJust(typeof(int), x);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void IntNothingCreation()
        {
            var expected = Maybe.Nothing<int>();
            var cache = new MaybeCache(1);
            var actual = (Maybe<int>)cache.RetrieveOrCacheNothing(typeof(int));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StringMaybeCreationData))]
        public static void StringMaybeCreation(string x)
        {
            var expected = Maybe.Just(x);
            var cache = new MaybeCache(1);
            var actual = (Maybe<string>)cache.RetrieveOrCacheJust(typeof(string), x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IntListMaybeCreationData))]
        public static void IntLisMaybeCreation(List<int> x)
        {
            var expected = Maybe.Just(x);
            var cache = new MaybeCache(1);
            var actual = (Maybe<List<int>>)cache.RetrieveOrCacheJust(typeof(List<int>), x);

            Assert.Equal(expected, actual);
        }
    }
}
