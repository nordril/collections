using Nordril.Collections.MethodCache;
using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests.MethodCache
{
    public class FuncDictionaryCacheTests
    {
        public static IEnumerable<object[]> IntIntDictCreationData()
        {
            yield return new object[] { new (int, int)[0] };
            yield return new object[] { new (int, int)[] { (4,6) } };
            yield return new object[] { new (int, int)[] { (6,6), (4, 87), (33,21), (10,5), (8,0), (5, -1), (-4, 3) } };
        }

        public static IEnumerable<object[]> StringStringDictCreationData()
        {
            yield return new object[] { new (string, string)[0] };
            yield return new object[] { new (string, string)[] { ("a", "b") } };
            yield return new object[] { new (string, string)[] { ("a", "a"), ("c", "xxx"), ("v", "aaaa"), ("aaaa", "ba"), ("hah", "aha") } };
        }

        public static IEnumerable<object[]> StringIntDictCreationData()
        {
            yield return new object[] { new (string, int)[0] };
            yield return new object[] { new (string, int)[] { ("d", 6) } };
            yield return new object[] { new (string, int)[] { ("f", 6), ("d", 87), ("ghjg", 21), ("j", 5), ("h", 0), ("e", -1), ("-sdjh", 3) } };
        }

        [Theory]
        [MemberData(nameof(IntIntDictCreationData))]
        public static void IntIntDictCreation((int, int)[] xs)
        {
            var expected = xs.ToDictionary(x => x.Item1, x => x.Item2);
            var cache = new FuncDictionaryCache(1);
            var actual = (FuncDictionary<int, int>)cache.RetrieveOrCacheCreate(typeof(int), typeof(int), xs.Select(x => ((object)x.Item1, (object)x.Item2)));

            var expectedOrdered = expected.OrderBy(x => x.Key);
            var actualOrdered = actual.OrderBy(x => x.Key);

            Assert.Equal(expectedOrdered, actualOrdered);
        }

        [Theory]
        [MemberData(nameof(StringStringDictCreationData))]
        public static void StringStringDictCreation((string, string)[] xs)
        {
            var expected = xs.ToDictionary(x => x.Item1, x => x.Item2);
            var cache = new FuncDictionaryCache(1);
            var actual = (FuncDictionary<string, string>)cache.RetrieveOrCacheCreate(typeof(string), typeof(string), xs.Select(x => ((object)x.Item1, (object)x.Item2)));

            var expectedOrdered = expected.OrderBy(x => x.Key);
            var actualOrdered = actual.OrderBy(x => x.Key);

            Assert.Equal(expectedOrdered, actualOrdered);
        }

        [Theory]
        [MemberData(nameof(StringIntDictCreationData))]
        public static void StringIntDictCreation((string, int)[] xs)
        {
            var expected = xs.ToDictionary(x => x.Item1, x => x.Item2);
            var cache = new FuncDictionaryCache(1);
            var actual = (FuncDictionary<string, int>)cache.RetrieveOrCacheCreate(typeof(string), typeof(int), xs.Select(x => ((object)x.Item1, (object)x.Item2)));

            var expectedOrdered = expected.OrderBy(x => x.Key);
            var actualOrdered = actual.OrderBy(x => x.Key);

            Assert.Equal(expectedOrdered, actualOrdered);
        }
    }
}
