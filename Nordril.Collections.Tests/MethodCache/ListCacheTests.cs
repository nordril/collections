using Nordril.Collections.MethodCache;
using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests.MethodCache
{
    public class ListCacheTests
    {
        public static IEnumerable<object[]> IntListCreationData()
        {
            yield return new object[] { new int[0] };
            yield return new object[] { new int[] { 6 } };
            yield return new object[] { new int[] { 6, 87, 21, 5, 0, -1, 3 } };
        }

        public static IEnumerable<object[]> StringListCreationData()
        {
            yield return new object[] { new string[0] };
            yield return new object[] { new string[] { "xyz" } };
            yield return new object[] { new string[] { "a", "h", "hjkh", "7xb", "9fbs", "abcz" } };
        }

        public static IEnumerable<object[]> IntListListCreationData()
        {
            yield return new object[] {
                new List<int>[0]
            };
            yield return new object[] {
                new List<int>[]
                {
                    new List<int>(),
                    new List<int>{ 5, 6 }
                }
            };
            yield return new object[] {
                new List<int>[]
                {
                    new List<int>{ 7, 7, 7 },
                    new List<int>{ 5, 6 },
                    new List<int>{ },
                    new List<int> { 8, 8, 3, 2, 1 }
                }
            };
        }

        [Theory]
        [MemberData(nameof(IntListCreationData))]
        public static void IntListCreation(int[] xs)
        {
            var expected = xs.ToList();
            var cache = new ListCache(1);
            var actual = (FuncList<int>)cache.RetrieveOrCacheCreate(typeof(int), xs.Cast<object>());

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StringListCreationData))]
        public static void StringListCreation(string[] xs)
        {
            var expected = xs.ToList();
            var cache = new ListCache(1);
            var actual = (FuncList<string>)cache.RetrieveOrCacheCreate(typeof(string), xs.Cast<object>());

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IntListListCreationData))]
        public static void IntListListCreation(List<int>[] xs)
        {
            var expected = xs.ToList();
            var cache = new ListCache(1);
            var actual = (FuncList<List<int>>)cache.RetrieveOrCacheCreate(typeof(List<int>), xs.Cast<object>());

            Assert.Equal(expected, actual);
        }
    }
}
