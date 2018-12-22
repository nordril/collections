using Nordril.Base;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Nordril.HedgingEngine.Logic.Collections.MethodCache
{
    /// <summary>
    /// A cache of typed constructors for <see cref="Result{T}"/>. This is useful if you have value-level types (<see cref="Type"/>) and dynamically want to call <see cref="Result.Ok{T}(T)"/>/<see cref="Result.WithErrors{T}(IEnumerable{Error}, ResultClass)"/> with them as type argument.
    /// This class uses dynamic method compilation to provide high-performance access to the constructors of <see cref="Result"/>, avoiding the high, repeated runtime cost of reflection.
    /// </summary>
    public class ResultCache : DictionaryCache<Type, (Func<IEnumerable<Error>, ResultClass, object>, Func<object, object>)>
    {
        private static readonly Func<Type, Func<IEnumerable<Error>, ResultClass, object>> makeWithErrorsMethod = t =>
        {
            var m = new DynamicMethod("WithErrorsDynamic", typeof(object), new Type[] { typeof(object), typeof(ResultClass) });

            var generator = m.GetILGenerator();

            generator.DeclareLocal(typeof(object));

            generator.Emit(OpCodes.Ldarg_0); //[] -> [x:stack]
            generator.Emit(OpCodes.Ldarg_1); //[x:stack] -> [x:stack, rc:stack]
            generator.EmitCall(OpCodes.Call, typeof(Result).GetMethod(nameof(Result.WithErrors)).MakeGenericMethod(t), null); //[x:stack, rc:stack] -> [ret:stack]
            generator.Emit(OpCodes.Box, typeof(Result<>).MakeGenericType(t)); //[ret:stack] -> [ret:heap]
            generator.Emit(OpCodes.Ret); //[ret:heap] -> []

            return (Func<IEnumerable<Error>, ResultClass, object>)m.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(IEnumerable<Error>), typeof(ResultClass), typeof(object)));
        };

        private static readonly Func<Type, Func<object, object>> makeOkMethod = t =>
        {
            var m = new DynamicMethod("OkDynamic", typeof(object), new Type[] { typeof(object) });

            var generator = m.GetILGenerator();

            generator.DeclareLocal(typeof(object));

            generator.Emit(OpCodes.Ldarg_0); //[] -> [x:stack]
            generator.Emit(OpCodes.Unbox_Any, t); //[x:stack -> x:stack|type:t]
            generator.EmitCall(OpCodes.Call, typeof(Result).GetMethod(nameof(Result.Ok)).MakeGenericMethod(t), null); //[x:stack] -> [ret:stack]
            generator.Emit(OpCodes.Box, typeof(Result<>).MakeGenericType(t)); //[ret:stack] -> [ret:heap]
            generator.Emit(OpCodes.Ret); //[ret:heap] -> []

            return (Func<object, object>)m.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(object), typeof(object)));
        };

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="cacheSize">The maximum cache size.</param>
        public ResultCache(int cacheSize) : base(cacheSize)
        {
        }

        /// <summary>
        /// Retrieves <see cref="Result.Ok{T}(T)"/>-method and caches both both methods for type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <param name="rc">The <see cref="ResultClass"/>.</param>
        /// <typeparam name="T">The type of the contents.</typeparam>
        public Result<T> RetrieveOrCacheWithErrors<T>(IEnumerable<Error> errors, ResultClass rc) => (Result<T>)RetrieveOrCacheWithErrors(typeof(T), errors, rc);

        /// <summary>
        /// Retrieves <see cref="Result.Ok{T}(T)"/>-method and caches both methods for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the contents.</typeparam>
        public Result<T> RetrieveOrCacheOk<T>(T value) => (Result<T>)RetrieveOrCacheOk(typeof(T), value);

        /// <summary>
        /// Retrieves <see cref="Result.WithErrors{T}(IEnumerable{Error}, ResultClass)"/>-method and caches both both methods for type <paramref name="t"/>.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <param name="rc">The <see cref="ResultClass"/>.</param>
        /// <param name="t">The type of the contents.</param>
        public object RetrieveOrCacheWithErrors(Type t, IEnumerable<Error> errors, ResultClass rc)
        {
            RetrieveOrCache(t, () => (makeWithErrorsMethod(t), makeOkMethod(t)), out var methods);
            return methods.Item1(errors, rc);
        }

        /// <summary>
        /// Retrieves <see cref="Result.WithErrors{T}(IEnumerable{Error}, ResultClass)"/>-method and caches both both methods for type <paramref name="t"/>.
        /// </summary>
        /// <param name="value">The result.</param>
        /// <param name="t">The type of the contents.</param>
        public object RetrieveOrCacheOk(Type t, object value)
        {
            RetrieveOrCache(t, () => (makeWithErrorsMethod(t), makeOkMethod(t)), out var methods);
            return methods.Item2(value);
        }
    }
}
