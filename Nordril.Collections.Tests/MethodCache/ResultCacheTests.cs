using Nordril.Collections.MethodCache;
using Nordril.Functional;
using Nordril.Functional.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests.MethodCache
{
    public class ResultCacheTests
    {
        public enum Errors { Red, Orange, Yellow }

        public static IEnumerable<object[]> IntResultCreationData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 6 };
        }

        public static IEnumerable<object[]> StringResultCreationData()
        {
            yield return new object[] { "xyz" };
        }

        public static IEnumerable<object[]> IntListResultCreationData()
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

        public static IEnumerable<object[]> IntListResultErrorCreationData()
        {
            yield return new object[]
            {
                new Error[] { },
                ResultClass.BadRequest
            };

            yield return new object[]
            {
                new Error[] { new Error("uh-oh!", Errors.Red) },
                ResultClass.BadRequest
            };

            yield return new object[]
            {
                new Error[] { new Error("uh-oh!", Errors.Red), new Error("boo!", Errors.Orange, "that field", new ArgumentException("arg")) },
                ResultClass.NotFound
            };
        }

        private static bool ResultEqual<T>(Result<T> x, Result<T> y)
        {
            if (x.IsOk != y.IsOk)
                return false;

            if (x.IsOk)
            {
                return x.Value().Equals(y.Value());
            } else
            {
                var xErrs = x.Errors();
                var yErrs = y.Errors();

                if (x.ResultClass != y.ResultClass)
                    return false;

                if (xErrs.Count != yErrs.Count)
                    return false;

                var zipped = xErrs.Zip(yErrs, (a, b) => a.Equals(b)).All();

                return zipped;
            }
        }

        [Theory]
        [MemberData(nameof(IntResultCreationData))]
        public static void IntResultCreation(int x)
        {
            var expected = Result.Ok(x);
            var cache = new ResultCache(1);
            var actual = (Result<int>)cache.RetrieveOrCacheOk(typeof(int), x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StringResultCreationData))]
        public static void StringResultCreation(string x)
        {
            var expected = Result.Ok(x);
            var cache = new ResultCache(1);
            var actual = (Result<string>)cache.RetrieveOrCacheOk(typeof(string), x);

            Assert.True(ResultEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(IntListResultCreationData))]
        public static void IntListResultCreation(List<int> x)
        {
            var expected = Result.Ok(x);
            var cache = new ResultCache(1);
            var actual = (Result<List<int>>)cache.RetrieveOrCacheOk(typeof(List<int>), x);

            Assert.True(ResultEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(IntListResultErrorCreationData))]
        public static void IntListResultErrorCreation(IEnumerable<Error> errors, ResultClass resultClass)
        {
            var expected = Result.WithErrors<List<int>>(errors, resultClass);
            var cache = new ResultCache(1);
            var actual = (Result<List<int>>)cache.RetrieveOrCacheWithErrors(typeof(List<int>), errors, resultClass);

            Assert.True(ResultEqual(expected, actual));
        }
    }
}
