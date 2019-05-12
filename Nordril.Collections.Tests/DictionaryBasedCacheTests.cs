using Nordril.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nordril.Collections.Tests
{
    public class DictionaryBasedCacheTests
    {
        public static IEnumerable<object[]> RetrieveOrCacheData()
        {
            //0-size caches. These should always return "missing & not cached"
            yield return new object[]
            {
                0,
                new (int, string)[] { },
                new (int, string, CacheResult)[] { },
                new (int, string)[] { }
            };

            yield return new object[]
            {
                0,
                new (int, string)[] { },
                new (int, string, CacheResult)[] { (1, "a", CacheResult.WasMissingAndNotCached) },
                new (int, string)[] { }
            };

            yield return new object[]
            {
                0,
                new (int, string)[] { },
                new (int, string, CacheResult)[] {
                    (1, "a", CacheResult.WasMissingAndNotCached),
                    (1, "a", CacheResult.WasMissingAndNotCached),
                    (-4, "z", CacheResult.WasMissingAndNotCached),
                },
                new (int, string)[] { }
            };

            yield return new object[]
            {
                0,
                new (int, string)[] { (1,"x") },
                new (int, string, CacheResult)[] {
                    (1, "a", CacheResult.WasMissingAndNotCached),
                    (1, "a", CacheResult.WasMissingAndNotCached),
                    (-4, "z", CacheResult.WasMissingAndNotCached),
                },
                new (int, string)[] { }
            };

            //The value is cached -> retrieve it
            yield return new object[]
            {
                1,
                new (int, string)[] { (1, "a") },
                new (int, string, CacheResult)[] {
                    (1, "b", CacheResult.WasFoundCached),
                },
                new (int, string)[] { (1,"a") }
            };

            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2, "b") },
                new (int, string, CacheResult)[] {
                    (1, "b", CacheResult.WasFoundCached),
                    (2, "b", CacheResult.WasFoundCached),
                    (1, "c", CacheResult.WasFoundCached),
                },
                new (int, string)[] { (1,"a"), (2, "b") }
            };

            //The value is not cached and can be cached because there's room in the cache.
            yield return new object[]
            {
                2,
                new (int, string)[] { (1, "a") },
                new (int, string, CacheResult)[] {
                    (2, "b", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a"), (2,"b") }
            };

            yield return new object[]
            {
                2,
                new (int, string)[] { (5, "xxx") },
                new (int, string, CacheResult)[] {
                    (1, "yzuvw", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"yzuvw"), (5,"xxx") }
            };

            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b") },
                new (int, string, CacheResult)[] {
                    (3, "c", CacheResult.WasInserted),
                    (4, "d", CacheResult.WasInserted),
                    (5, "e", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a"), (2,"b"), (3,"c"), (4,"d"),(5,"e") }
            };

            yield return new object[]
            {
                99,
                new (int, string)[] { (1, "a"), (2,"b") },
                new (int, string, CacheResult)[] {
                    (3, "c", CacheResult.WasInserted),
                    (4, "d", CacheResult.WasInserted),
                    (5, "e", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a"), (2,"b"), (3,"c"), (4,"d"),(5,"e") }
            };

            //The value is not cached and can be cached by removing an old element
            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d"),(5,"e") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                },
                new (int, string)[] { (2,"b"), (3,"c"), (4,"d"),(5,"e"),(6,"f") }
            };

            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d"),(5,"e") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                    (1,"a2", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a2"), (3,"c"), (4,"d"),(5,"e"),(6,"f") }
            };

            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d"),(5,"e") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                    (1,"a2", CacheResult.WasInserted),
                    (2,"b2", CacheResult.WasInserted),
                    (3,"c2", CacheResult.WasInserted),
                    (4,"d2", CacheResult.WasInserted),
                    (5,"e2", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a2"), (2,"b2"), (3,"c2"), (4,"d2"),(5,"e2") }
            };

            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d"),(5,"e") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                    (1,"a2", CacheResult.WasInserted),
                    (2,"b2", CacheResult.WasInserted),
                    (3,"c2", CacheResult.WasInserted),
                    (4,"d2", CacheResult.WasInserted),
                    (5,"e2", CacheResult.WasInserted),
                    (6,"f2", CacheResult.WasInserted),
                    (1,"a3", CacheResult.WasInserted),
                    (2,"b3", CacheResult.WasInserted),
                    (3,"c3", CacheResult.WasInserted),
                    (4,"d3", CacheResult.WasInserted),
                    (5,"e3", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a3"), (2,"b3"), (3,"c3"), (4,"d3"),(5,"e3") }
            };

            //Mixed cases
            yield return new object[]
            {
                5,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d"),(5,"e") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                    (1,"a2", CacheResult.WasInserted),
                    (2,"b2", CacheResult.WasInserted),
                    (3,"c2", CacheResult.WasInserted),
                    (4,"d2", CacheResult.WasInserted),
                    (5,"e2", CacheResult.WasInserted),
                    (6,"f2", CacheResult.WasInserted),
                    (1,"a3", CacheResult.WasInserted),
                    (2,"b3", CacheResult.WasInserted),
                    (3,"c3", CacheResult.WasInserted),
                    (1,"a9", CacheResult.WasFoundCached),
                    (4,"d3", CacheResult.WasInserted),
                    (4,"d9", CacheResult.WasFoundCached),
                    (5,"e3", CacheResult.WasInserted),
                },
                new (int, string)[] { (1,"a3"), (2,"b3"), (3,"c3"), (4,"d3"),(5,"e3") }
            };

            yield return new object[]
            {
                4,
                new (int, string)[] { (1, "a"), (2,"b"), (3,"c"),(4,"d") },
                new (int, string, CacheResult)[] {
                    (6, "f", CacheResult.WasInserted),
                    (1,"a2", CacheResult.WasInserted),
                    (4,"d2", CacheResult.WasFoundCached),
                },
                new (int, string)[] { (1,"a2"), (3,"c"), (4,"d"), (6,"f") }
            };
        }

        [Theory]
        [MemberData(nameof(RetrieveOrCacheData))]
        public void RetrieveOrCacheTest(
            int cacheSize,
            IEnumerable<(int, string)> initial,
            IEnumerable<(int, string, CacheResult)> inserts,
            IEnumerable<(int, string)> expected)
        {
            var cache = new DictionaryBasedCache<int, string>(cacheSize);

            foreach (var (k,v) in initial)
                cache.RetrieveOrCache(k, () => v, out var _);

            foreach (var (k,v,r) in inserts)
            {
                var actualR = cache.RetrieveOrCache(k, () => v, out var actualV);
                Assert.Equal(r, actualR);
            }

            var actual = cache.OrderBy(kv => kv.Key).Select(kv => (kv.Key, kv.Value)).ToList();
            var expectedOrd = expected.OrderBy(kv => kv.Item1).ToList();

            Assert.Equal(expectedOrd.Count, actual.Count);

            foreach (var (e,a) in expectedOrd.Zip(actual))
            {
                Assert.Equal(e.Item1, a.Key);
                Assert.Equal(e.Item2, a.Value);
            }
        }
    }
}
