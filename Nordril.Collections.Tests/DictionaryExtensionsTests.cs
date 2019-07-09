using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests
{
    public class DictionaryExtensionsTests
    {
        public static IEnumerable<object[]> UpdateData()
        {
            //Dictionary
            yield return new object[]
            {
                new Dictionary<int, string>(),
                5,
                false
            };

            yield return new object[]
            {
                new Dictionary<int, string>{ { 8, "h" } },
                5,
                false
            };

            yield return new object[]
            {
                new Dictionary<int, string>{ { 8, "h" } },
                8,
                true
            };

            yield return new object[]
            {
                new Dictionary<int, string>{ { 8, "h" }, { 15, "o" }, { 7, "g" } },
                15,
                true
            };

            //FuncDictionary
            yield return new object[]
            {
                new FuncDictionary<int, string>(),
                5,
                false
            };

            yield return new object[]
            {
                new FuncDictionary<int, string>{ { 8, "h" } },
                5,
                false
            };

            yield return new object[]
            {
                new FuncDictionary<int, string>{ { 8, "h" } },
                8,
                true
            };

            yield return new object[]
            {
                new FuncDictionary<int, string>{ { 8, "h" }, { 15, "o" }, { 7, "g" } },
                15,
                true
            };
        }

        [Theory]
        [MemberData((nameof(UpdateData)))]
        public static void UpdateTest(IDictionary<int, string> dict, int key, bool isPresent)
        {
            var oldSize = dict.Count;
            var oldCopy = new Dictionary<int, string>(dict);

            dict.Update(key, s => s + s);

            Assert.Equal(oldSize, dict.Count);

            if (isPresent)
                Assert.Equal(oldCopy[key] + oldCopy[key], dict[key]);
            else
                Assert.False(dict.ContainsKey(key));
        }

        [Theory]
        [MemberData((nameof(UpdateData)))]
        public static void UpdateWithTest(IDictionary<int, string> dict, int key, bool isPresent)
        {
            var oldSize = dict.Count;
            var oldCopy = new Dictionary<int, string>(dict);

            dict.Update(key, "xxx", s => s + s);

            if (isPresent)
            {
                Assert.Equal(oldSize, dict.Count);
                Assert.Equal(oldCopy[key] + oldCopy[key], dict[key]);
            }
            else
            {
                Assert.Equal(oldSize+1, dict.Count);
                Assert.True(dict.ContainsKey(key));
                Assert.Equal("xxx", dict[key]);
            }
        }

        [Theory]
        [MemberData((nameof(UpdateData)))]
        public static void UpdateWithTest2(IDictionary<int, string> dict, int key, bool isPresent)
        {
            var oldSize = dict.Count;
            var oldCopy = new Dictionary<int, string>(dict);

            dict.TryGetValue(key, out var _);
            dict.Update(key, "xxx", s => s + s);

            if (isPresent)
            {
                Assert.Equal(oldSize, dict.Count);
                Assert.Equal(oldCopy[key] + oldCopy[key], dict[key]);
            }
            else
            {
                Assert.Equal(oldSize + 1, dict.Count);
                Assert.True(dict.ContainsKey(key));
                Assert.Equal("xxx", dict[key]);
            }
        }
    }
}
