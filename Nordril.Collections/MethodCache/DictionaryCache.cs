using Nordril.TypeToolkit;
using Sigil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nordril.Collections.MethodCache
{
    /// <summary>
    /// A cache of typed constructors for <see cref="Dictionary{TKey, TValue}"/>. This is useful if you have value-level types (<see cref="Type"/>) and dynamically want to call the <see cref="Dictionary{TKey, TValue}"/>-constructor with them as type argument.
    /// This class uses dynamic method compilation to provide high-performance access to the constructor of <see cref="Dictionary{TKey, TValue}"/>, avoiding the high, repeated runtime cost of reflection.
    /// </summary>
    public class DictionaryCache : DictionaryBasedCache<(Type, Type), Func<IEnumerable<(object, object)>, object>>
    {
        private static readonly MethodInfo selectMethod = typeof(Enumerable).GetMethods().First(m => {
            if (m.Name != nameof(Enumerable.Select))
                return false;

            var ps = m.GetParameters();

            if (ps.Length != 2 || ps[1].ParameterType.GetGenericTypeDefinitionSafe() != typeof(Func<,>) || !m.IsGenericMethodDefinition)
                return false;
            else
                return true;
        });

        private static readonly Func<Type, Type, Func<IEnumerable<(object, object)>, object>> makeCreateMethod = (tkey, tvalue) =>
        {
            /*
             * Generated code:
             * Type parameters in brackets denote template parameters.
             * 
             * Dictionary<TKey, TValue> CreateDynamic[Tkey, TValue](IEnumerable<(object, object)> xs)
             * {
             *    List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
             *    
             *    foreach (var x in xs)
             *    {
             *       list.Add(new KeyValuePair<TKey, TValue>((TKey)x.Item1, (TValue)x.Item2));
             *    }
             *    
             *    return new Dictionary<TKey, TValue>(list);
             * }
             */

            var dictType = typeof(Dictionary<,>).MakeGenericType(tkey, tvalue);
            var tupleObjectType = typeof(ValueTuple<,>).MakeGenericType(typeof(object), typeof(object));
            var kvType = typeof(KeyValuePair<,>).MakeGenericType(tkey, tvalue);
            var enumType = typeof(IEnumerable<>).MakeGenericType(kvType);
            var listType = typeof(List<>).MakeGenericType(kvType);

            var dictConstr = dictType.GetConstructor(new Type[] { enumType });

            var getEnumeratorMethod = typeof(IEnumerable<(object, object)>).GetMethods().First(me => me.Name == nameof(IEnumerable<object>.GetEnumerator) && me.GetParameters().Length == 0);
            var moveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
            var getCurrentMethod = typeof(IEnumerator<(object, object)>).GetProperty(nameof(IEnumerator<object>.Current)).GetMethod;
            var disposeMethod = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));
            var addMethod = listType.GetMethod(nameof(List<object>.Add));
            var fieldItem1 = tupleObjectType.GetField(nameof(ValueTuple<object, object>.Item1));
            var fieldItem2 = tupleObjectType.GetField(nameof(ValueTuple<object, object>.Item2));
            var listConstr = listType.GetConstructor(new Type[0]);
            var kvConstr = kvType.GetConstructor(new Type[] { tkey, tvalue });

            var generator = Emit<Func<IEnumerable<(object, object)>, object>>.NewDynamicMethod("CreateDynamic");

            var locEnum = generator.DeclareLocal(typeof(IEnumerator<(object, object)>)); //enum
            var locList = generator.DeclareLocal(listType); //list
            var locCur = generator.DeclareLocal(typeof(ValueTuple<object, object>)); //cur

            generator.NewObject(listConstr); //[] -> [newlist:stack]
            generator.StoreLocal(locList); //[newlist:stack] -> []
            generator.LoadArgument(0); //[] -> [arg0:stack] (the IEnumerable<object> argument)
            generator.CallVirtual(getEnumeratorMethod); //[arg0:stack] -> [newenum:stack]
            generator.StoreLocal(locEnum); //[fnewenum:stack] -> []; enum <= newenum
            var tryLbl = generator.BeginExceptionBlock(); //try

            var loopBody = generator.DefineLabel();
            //Jump past .Add and directly to MoveNext on the first iteration.
            generator.Branch(loopBody);

            //Loop body. foreach (x in xs)
            var loopBegin = generator.DefineLabel();
            generator.MarkLabel(loopBegin);
            generator.LoadLocal(locEnum); //[] -> [enum:stack]
            generator.CallVirtual(getCurrentMethod); //[enum: stack] -> [newcur:stack]
            generator.StoreLocal(locCur); //[newcur:stack] -> []; cur <= newcur

            //We need this later for Add, but we perform some calculations further up the stack in the meanwhile.
            generator.LoadLocal(locList); //[] -> [list:stack]
            generator.LoadLocal(locCur); //[list:stack] -> [list:stack, cur:stack]
            generator.LoadField(fieldItem1); //[list:stack, cur:stack] -> [list:stack, cur.Item1:stack]
            generator.UnboxAny(tkey); //[list:stack, cur.Item1:stack] -> [list:stack, cur.Item1:tkey:stack]
            generator.LoadLocal(locCur); //[list:stack, cur.Item1:tkey:stack] -> [list:stack, cur.Item1:tkey:stack, cur:stack]
            generator.LoadField(fieldItem2); //[list:stack, cur.Item1:tkey:stack, cur:stack] -> [list:stack, cur.Item1:tkey:stack, cur.Item2:stack]
            generator.UnboxAny(tvalue); //[list:stack, cur.Item1:tkey:stack, cur.Item2:stack] -> [list:stack, cur.Item1:stack, cur.Item2:tvalue:stack]
            generator.NewObject(kvConstr); //[list:stack, cur.Item1:tkey:stack, cur.Item2:tvalue:stack] -> [list:stack, newkv:stack]
            generator.Call(addMethod); //[list:stack, newkv:stack] -> []

            generator.MarkLabel(loopBody);
            generator.LoadLocal(locEnum); //[] -> [enum:stack]
            generator.CallVirtual(moveNextMethod); //[enum:stack] -> [hasNext:stack]
            generator.BranchIfTrue(loopBegin); //[hasNext:next] -> []

            //Leave try and jump to return;
            var endLbl = generator.DefineLabel();
            generator.Leave(endLbl);

            //Finally
            var lblFinally = generator.BeginFinallyBlock(tryLbl);

            generator.LoadLocal(locEnum); //[] -> [enum:stack]
            generator.CallVirtual(disposeMethod); //[enum:stack] -> []

            generator.EndFinallyBlock(lblFinally);
            generator.EndExceptionBlock(tryLbl);

            //Return
            generator.MarkLabel(endLbl);
            generator.LoadLocal(locList); //[] -> [list:stack]
            generator.NewObject(dictConstr); //[list:stack] -> [newdict:stack]
            generator.Return();

            var del = generator.CreateDelegate();

            return del;
        };

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="cacheSize">The maximum cache size.</param>
        public DictionaryCache(int cacheSize) : base(cacheSize)
        {

        }

        /// <summary>
        /// Retrieves <see cref="Dictionary{TKey, TValue}"/>-constructor, specialized to <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The type of the keys.</param>
        /// <param name="value">The type of the values.</param>
        public Func<IEnumerable<(object, object)>, object> RetrieveOrCacheCreate(Type key, Type value)
        {
            RetrieveOrCache((key, value), () => makeCreateMethod(key, value), out var method);
            return method;
        }

        /// <summary>
        /// Retrieves <see cref="Dictionary{TKey, TValue}"/>-constructor-method and caches both both methods for type <typeparamref name="TKey"/> and <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="elements">The list of elements.</param>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        public IDictionary<TKey, TValue> RetrieveOrCacheCreate<TKey, TValue>(IEnumerable<(TKey, TValue)> elements)
            => (Dictionary<TKey, TValue>)RetrieveOrCacheCreate(typeof(TKey), typeof(TValue), elements.Select(e => ((object)e.Item1, (object)e.Item2)));

        /// <summary>
        /// Retrieves <see cref="Dictionary{TKey, TValue}"/>-constructor-method and caches both both methods for type <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="elements">The list of elements.</param>
        /// <param name="key">The type of the keys.</param>
        /// <param name="value">The type of the values.</param>
        public object RetrieveOrCacheCreate(Type key, Type value, IEnumerable<(object, object)> elements)
        {
            RetrieveOrCache((key, value), () => makeCreateMethod(key, value), out var method);
            return method(elements);
        }
    }
}
