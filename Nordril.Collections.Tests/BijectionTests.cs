using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests
{
    public class BijectionTests
    {
        public static IEnumerable<object[]> RightGetData()
        {
            yield return new object[]
            {
                new Bijection<int, string> {{1,"a" }},
                1,
                "a"
            };

            yield return new object[]
            {
                new Bijection<int, string> {{1,"a" }, {2,"b" },{ 3,"c"} },
                1,
                "a"
            };

            yield return new object[]
            {
                new Bijection<int, string> {{1,"a" }, {2,"b" },{ 3,"c"} },
                3,
                "c"
            };
        }

        [Theory]
        [MemberData(nameof(RightGetData))]
        public void RightGetTests(IBijection<int, string> bijection, int key, string expected)
        {
            var actual = bijection[key];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RightGetNotFound()
        {
            var bijection = new Bijection<int, string> { { 1,"a"} };

            Assert.Throws<KeyNotFoundException>(() => bijection[2]);
        }

        [Fact]
        public void RightSetConflict()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, {2,"b" } };

            Assert.Throws<KeyAlreadyPresentException>(() => bijection[2] = "a");
            Assert.Throws<KeyAlreadyPresentException>(() => bijection[3] = "a");
            Assert.Throws<KeyAlreadyPresentException>(() => bijection[1] = "b");
        }

        [Fact]
        public void RightSetOk()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" } };

            bijection[1] = "c";

            Assert.Equal("b", bijection[2]);
            Assert.Equal("c", bijection[1]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(1, bijection["c", TagRight.Value]);
        }

        [Theory]
        [MemberData(nameof(RightGetData))]
        public void LeftGetTests(IBijection<int, string> bijection, int expected, string key)
        {
            var actual = bijection[key, TagRight.Value];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LeftGetNotFound()
        {
            var bijection = new Bijection<int, string> { { 1, "a" } };

            Assert.Throws<KeyNotFoundException>(() => bijection["b", TagRight.Value]);
        }

        [Fact]
        public void LeftSetConflict()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" } };

            Assert.Throws<KeyAlreadyPresentException>(() => bijection["b", TagRight.Value] = 1);
            Assert.Throws<KeyAlreadyPresentException>(() => bijection["c", TagRight.Value] = 1);
            Assert.Throws<KeyAlreadyPresentException>(() => bijection["a", TagRight.Value] = 2);
        }

        [Fact]
        public void LeftSetOk()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" } };

            bijection["a", TagRight.Value] = 3;

            Assert.Equal("a", bijection[3]);
            Assert.Equal("b", bijection[2]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(3, bijection["a", TagRight.Value]);
        }

        [Fact]
        public void RemoveRightTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" }, {3,"c" } };

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("b", bijection[2]);
            Assert.Equal("c", bijection[3]);

            bijection.RemoveRight("b");

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("c", bijection[3]);

            Assert.False(bijection.ContainsKey(2));
            Assert.False(bijection.ContainsRightKey("b"));

            bijection.RemoveRight("a");
            bijection.RemoveRight("c");

            Assert.Empty(bijection);

            bijection.RemoveRight("a");
        }

        [Fact]
        public void RemoveTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" }, { 3, "c" } };

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("b", bijection[2]);
            Assert.Equal("c", bijection[3]);

            bijection.Remove(2);

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("c", bijection[3]);

            Assert.False(bijection.ContainsKey(2));
            Assert.False(bijection.ContainsRightKey("b"));

            bijection.Remove(1);
            bijection.Remove(3);

            Assert.Empty(bijection);

            bijection.Remove(1);
        }

        [Fact]
        public void ContainsKeyTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" }, { 3, "c" } };

            Assert.True(bijection.ContainsKey(2));
            Assert.False(bijection.ContainsKey(4));

            Assert.True(bijection.ContainsRightKey("b"));
            Assert.False(bijection.ContainsRightKey("d"));

            bijection[5] = "e";

            Assert.True(bijection.ContainsKey(5));
            Assert.True(bijection.ContainsRightKey("e"));
        }

        [Fact]
        public void RemovePairTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" }, { 3, "c" } };

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("b", bijection[2]);
            Assert.Equal("c", bijection[3]);

            bijection.Remove(KeyValuePair.Create(2, "b"));
            bijection.Remove(KeyValuePair.Create(1, "x"));

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(3, bijection["c", TagRight.Value]);
            Assert.Equal("a", bijection[1]);
            Assert.Equal("c", bijection[3]);

            Assert.False(bijection.ContainsKey(2));
            Assert.False(bijection.ContainsRightKey("b"));

            bijection.Remove(1);
            bijection.Remove(3);

            Assert.Empty(bijection);

            bijection.Remove(KeyValuePair.Create(1, "a"));
        }

        [Fact]
        public void CountTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" }, { 3, "c" } };

            Assert.Equal(3, bijection.Count);
            bijection.Remove(3);
            Assert.Equal(2, bijection.Count);
            bijection.Remove(3);
            Assert.Equal(2, bijection.Count);
            bijection.Remove(2);
            Assert.Single(bijection);
            bijection.Remove(1);
            Assert.Empty(bijection);
        }

        [Fact]
        public void AddTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" } };

            bijection.Add(KeyValuePair.Create(5, "c"));

            Assert.Equal(1, bijection["a", TagRight.Value]);
            Assert.Equal(2, bijection["b", TagRight.Value]);
            Assert.Equal(5, bijection["c", TagRight.Value]);

            Assert.Equal("a", bijection[1]);
            Assert.Equal("b", bijection[2]);
            Assert.Equal("c", bijection[5]);

            Assert.Throws<ArgumentException>(() => bijection.Add(KeyValuePair.Create(5, "x")));
            Assert.Throws<ArgumentException>(() => bijection.Add(KeyValuePair.Create(9, "a")));
            Assert.Throws<ArgumentException>(() => bijection.Add(KeyValuePair.Create(1, "a")));

            Assert.Equal(3, bijection.Count);
        }

        [Fact]
        public void ClearTest()
        {
            var bijection = new Bijection<int, string> { { 1, "a" }, { 2, "b" } };

            bijection.Clear();
            Assert.Empty(bijection);
            bijection.Clear();
            Assert.Empty(bijection);

            bijection[1] = "a";
            bijection[3] = "c";

            Assert.Equal(2, bijection.Count);

            bijection.Clear();

            Assert.Empty(bijection);
        }


    }
}
