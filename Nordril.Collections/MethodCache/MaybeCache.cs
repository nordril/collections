using Nordril.Functional.Data;
using System;
using System.Reflection.Emit;

namespace Nordril.Collections.MethodCache
{
    /// <summary>
    /// A cache of typed constructors for <see cref="Maybe{T}"/>. This is useful if you have value-level types (<see cref="Type"/>) and dynamically want to call <see cref="Maybe.Nothing{T}"/>/<see cref="Maybe.Just{T}(T)"/> with them as type argument.
    /// This class uses dynamic method compilation to provide high-performance access to the constructors of <see cref="Maybe"/>, avoiding the high, repeated runtime cost of reflection.
    /// </summary>
    public class MaybeCache : DictionaryBasedCache<Type, (Func<object>, Func<object, object>)>
    {
        private static readonly Func<Type, Func<object>> makeNothingMethod = t =>
        {
            /*
             * Generated code:
             * Type parameters in brackets denote template parameters.
             * 
             * Maybe<T> JustDynamic[T](object x)
             * {
             *    return Maybe.Just<T>((T)x);
             * }
             */

            var m = new DynamicMethod("NothingDynamic", typeof(object), new Type[0]);

            var generator = m.GetILGenerator();

            //declare a local for the return of the call
            generator.DeclareLocal(typeof(object));

            generator.EmitCall(OpCodes.Call, typeof(Maybe).GetMethod(nameof(Maybe.Nothing), 1, new Type[0]).MakeGenericMethod(t), null); //[] ->[ret:stack]
            generator.Emit(OpCodes.Box, typeof(Maybe<>).MakeGenericType(t)); //[ret:stack] -> [ret:heap]
            generator.Emit(OpCodes.Ret); //[ret:head] -> []

            return (Func<object>)m.CreateDelegate(typeof(Func<>).MakeGenericType(typeof(object)));
        };

        private static readonly Func<Type, Func<object, object>> makeJustMethod = t =>
        {
            /*
             * Generated code:
             * Type parameters in brackets denote template parameters.
             * 
             * Maybe<T> NothingDynamic[T]()
             * {
             *    return Maybe.Nothing<T>((T)x);
             * }
             */

            var m = new DynamicMethod("JustDynamic", typeof(object), new Type[] { typeof(object) });

            var generator = m.GetILGenerator();

            generator.DeclareLocal(typeof(object));

            generator.Emit(OpCodes.Ldarg_0); //[] -> [x:stack]
            generator.Emit(OpCodes.Unbox_Any, t); //[x:stack -> x:stack|type:t]
            generator.EmitCall(OpCodes.Call, typeof(Maybe).GetMethod(nameof(Maybe.Just)).MakeGenericMethod(t), null); //[x:stack] -> [ret:stack]
            generator.Emit(OpCodes.Box, typeof(Maybe<>).MakeGenericType(t)); //[ret:stack] -> [ret:heap]
            generator.Emit(OpCodes.Ret); //[ret:heap] -> []

            return (Func<object, object>)m.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(object), typeof(object)));
        };

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="cacheSize">The maximum cache size.</param>
        public MaybeCache(int cacheSize) : base(cacheSize)
        {
        }

        /// <summary>
        /// Retrieves <see cref="Maybe.Nothing{T}"/>-method and caches both <see cref="Maybe.Nothing{T}"/> and <see cref="Maybe.Just{T}(T)"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the contents.</typeparam>
        public Maybe<T> RetrieveOrCacheNothing<T>() => (Maybe<T>)RetrieveOrCacheNothing(typeof(T));

        /// <summary>
        /// Retrieves <see cref="Maybe.Just{T}(T)"/>-method and caches both <see cref="Maybe.Nothing{T}"/> and <see cref="Maybe.Just{T}(T)"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the contents.</typeparam>
        public Maybe<T> RetrieveOrCacheJust<T>(T value) => (Maybe<T>)RetrieveOrCacheJust(typeof(T), value);

        /// <summary>
        /// Retrieves <see cref="Maybe.Nothing{T}"/>-method and caches both <see cref="Maybe.Nothing{T}"/> and <see cref="Maybe.Just{T}(T)"/> for type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type of the contents.</param>
        public object RetrieveOrCacheNothing(Type t)
        {
            RetrieveOrCache(t, () => (makeNothingMethod(t), makeJustMethod(t)), out var methods);
            return methods.Item1();
        }

        /// <summary>
        /// Retrieves <see cref="Maybe.Just{T}(T)"/>-method and caches both <see cref="Maybe.Nothing{T}"/> and <see cref="Maybe.Just{T}(T)"/> for type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type of the contents.</param>
        /// <param name="value">The contents.</param>
        public object RetrieveOrCacheJust(Type t, object value)
        {
            RetrieveOrCache(t, () => (makeNothingMethod(t), makeJustMethod(t)), out var methods);
            return methods.Item2(value);
        }
    }
}
