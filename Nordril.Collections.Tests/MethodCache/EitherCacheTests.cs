using Nordril.Collections.MethodCache;
using Nordril.Functional.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests.MethodCache
{
    public static class EitherCacheTests
    {
        [Fact]
        public static void IntEither1Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Identity<int>(14);
            var actual = (Identity<int>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int) }, 0, 14);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void ListStringEither1Creation()
        {
            var cache = new EitherCache(20);
            var list = new List<string> { "abc" };

            var expected = new Identity<List<string>>(list);
            var actual = (Identity<List<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(List<string>) }, 0, list);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void IntEither2Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, string>(Either.One(14));
            var actual = (Either<int, string>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(string) }, 0, 14);

            Assert.Equal(expected, actual);

            expected = new Either<int, string>(Either.Two("abc"));
            actual = (Either<int, string>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(string) }, 1, "abc");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void ListStringEither2Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, FuncList<string>>(Either.One(14));
            var actual = (Either<int, FuncList<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(FuncList<string>) }, 0, 14);

            Assert.Equal(expected, actual);

            expected = new Either<int, FuncList<string>>(Either.Two(new FuncList<string> { "abc" }));
            actual = (Either<int, FuncList<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(FuncList<string>) }, 1, new FuncList<string> { "abc" });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either3Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>) }, 0, 14);

            Assert.Equal(expected, actual);

            var list = new List<bool> { false, false, true };
            expected = new Either<int, List<bool>, FuncList<string>>(Either.Two(list));
            actual = (Either<int, List<bool>, FuncList<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>) }, 1, list);

            Assert.Equal(expected, actual);

            expected = new Either<int, List<bool>, FuncList<string>>(Either.Three(new FuncList<string> { "a", "b", "c" }));
            actual = (Either<int, List<bool>, FuncList<string>>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>) }, 2, new FuncList<string> { "a", "b", "c" });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either4Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>, float>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float) }, 0, 14);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either5Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float, bool>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>, float, bool>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool) }, 0, 14);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either6Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float, bool, short>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>, float, bool, short>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool), typeof(short) }, 0, 14);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either7Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float, bool, short, byte>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>, float, bool, short, byte>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool), typeof(short), typeof(byte) }, 0, 14);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either8Creation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>(Either.One(14));
            var actual = (Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool), typeof(short), typeof(byte), typeof(Type) }, 0, 14);

            Assert.Equal(expected, actual);

             expected = new Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>(Either.Eight(typeof(int)));
            actual = (Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>)cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool), typeof(short), typeof(byte), typeof(Type) }, 7, typeof(int));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Either8FuncCreation()
        {
            var cache = new EitherCache(20);
            var expected = new Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>(Either.One(14));
            var actualConstructor = cache.RetrieveOrCacheCreate(new List<Type> { typeof(int), typeof(List<bool>), typeof(FuncList<string>), typeof(float), typeof(bool), typeof(short), typeof(byte), typeof(Type) });

            var actual = (Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>)actualConstructor(0, 14);

            Assert.Equal(expected, actual);

            expected = new Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>(Either.Eight(typeof(int)));
            actual = (Either<int, List<bool>, FuncList<string>, float, bool, short, byte, Type>)actualConstructor(7, typeof(int));

            Assert.Equal(expected, actual);

            Assert.Throws<IndexOutOfRangeException>(() => actualConstructor(8, 4));
        }

        [Fact]
        public static void ErrorTest()
        {
            var cache = new EitherCache(20);
            Assert.Throws<ArgumentException>(() => cache.RetrieveOrCacheCreate(null, 0, 5));
            Assert.Throws<ArgumentException>(() => cache.RetrieveOrCacheCreate(Enumerable.Repeat(typeof(int), 9).ToList(), 0, 5));
            Assert.Throws<IndexOutOfRangeException>(() => cache.RetrieveOrCacheCreate(Enumerable.Repeat(typeof(int), 5).ToList(), 5, 5));
            Assert.Throws<IndexOutOfRangeException>(() => cache.RetrieveOrCacheCreate(Enumerable.Repeat(typeof(int), 5).ToList(), -1, 5));
        }
    }
}
