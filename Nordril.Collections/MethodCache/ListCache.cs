using Nordril.Functional.Data;
using Sigil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nordril.Collections.MethodCache
{
    /// <summary>
    /// A cache of typed constructors for <see cref="List{T}"/>. This is useful if you have value-level types (<see cref="Type"/>) and dynamically want to call the <see cref="List{T}"/>-constructor with them as type argument.
    /// This class uses dynamic method compilation to provide high-performance access to the constructor of <see cref="List{T}"/>, avoiding the high, repeated runtime cost of reflection.
    /// </summary>
    public class ListCache : DictionaryCache<Type, Func<IEnumerable<object>, object>>
    {
        private static readonly Func<Type, Func<IEnumerable<object>, object>> makeCreateMethod = t =>
        {
            var enumType = typeof(IEnumerable<>).MakeGenericType(t);
            var flType = typeof(FuncList<>).MakeGenericType(t);

            var getEnumeratorMethod = typeof(IEnumerable<object>).GetMethods().First(me => me.Name == nameof(IEnumerable<object>.GetEnumerator) && me.GetParameters().Length == 0);
            var moveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
            var getCurrentMethod = typeof(IEnumerator<object>).GetProperty(nameof(IEnumerator<object>.Current)).GetMethod;
            var disposeMethod = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));
            var addMethod = flType.GetMethod(nameof(FuncList<object>.Add));
            var constr = flType.GetConstructor(new Type[] { enumType });

            //below:
            //Ultimate Chaos, at whose centre sprawls the blind idiot god Azathoth, Lord of All Things, encircled by his flopping horde of mindless and amorphous dancers, and lulled by the thin monotonous piping of a daemoniac flute held in nameless paws.
            //Azathoth have mercy! I can see everything with a monstrous sense that is not sight!

            var generator = Emit<Func<IEnumerable<object>, object>>.NewDynamicMethod("CreateDynamic");

            var locEnum = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(typeof(object))); //enum
            var locFl = generator.DeclareLocal(flType); //fl
            var locCur = generator.DeclareLocal(t); //cur

            generator.LoadLocalAddress(locFl); //[] -> [fl*:stack]
            generator.InitializeObject(flType); //[fl*:stack] -> []; fl <= newobj
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
            generator.UnboxAny(t); //[newcur:stack] -> [newcur:t:stack]
            generator.StoreLocal(locCur); //[newcur:t:stack] -> []; cur <= newcur
            //We need a pointer to the FuncList-struct, not the value directly, to call Add on it.
            generator.LoadLocalAddress(locFl); //[] -> [fl*:stack]
            generator.LoadLocal(locCur); //[fl*:stack] -> [fl*:stack, cur:stack]
            generator.Call(addMethod); //[fl*:stack, cur:stack] -> []

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
            generator.LoadLocal(locFl); //[] -> [fl:stack]
            generator.Box(flType); //[fl:stack] -> [fl:heap]
            generator.Return();

            var del = generator.CreateDelegate();

            return del;
        };

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="cacheSize">The maximum cache size.</param>
        public ListCache(int cacheSize) : base (cacheSize)
        {
        }

        /// <summary>
        /// Retrieves the <see cref="FuncList{T}"/>-constructor and caches itfor type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="elements">The list of elements.</param>
        /// <typeparam name="T">The type of the contents.</typeparam>
        public FuncList<T> RetrieveOrCacheCreate<T>(IEnumerable<T> elements) => (FuncList<T>)RetrieveOrCacheCreate(typeof(T), elements.Cast<object>());

        /// <summary>
        /// Retrieves the <see cref="FuncList{T}"/>-constructor and caches itfor type <paramref name="t"/>.
        /// </summary>
        /// <param name="elements">The list of elements.</param>
        /// <param name="t">The type of the contents.</param>
        public object RetrieveOrCacheCreate(Type t, IEnumerable<object> elements)
        {
            RetrieveOrCache(t, () => makeCreateMethod(t), out var m);
            return m(elements);
        }
    }
}
