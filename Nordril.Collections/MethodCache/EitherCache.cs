using Nordril.Functional;
using Nordril.Functional.Data;
using Sigil;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nordril.Collections.MethodCache
{
    /// <summary>
    /// A cache of typed constructors for the <see cref="Either{TLeft, TRight}"/>-family (from 1 to 8 parameters). This is useful if you have value-level types (<see cref="Type"/>) and dynamically want to call the either-constructor with them as type argument.
    /// This class uses dynamic method compilation to provide high-performance access to the constructors of <see cref="Either{TLeft, TRight}"/>, avoiding the high, repeated runtime cost of reflection.
    /// </summary>
    public class EitherCache : DictionaryBasedCache<IList<Type>, Func<int, object, object>>
    {
        private static readonly IList<(string label, Type eitherN, Type constructor)> eitherConstructors = new List<(string, Type, Type)>
        {
            ("One", typeof(Either1<>), typeof(Identity<>)),
            ("Two", typeof(Either2<>), typeof(Either<,>)),
            ("Three", typeof(Either3<>), typeof(Either<,,>)),
            ("Four",typeof(Either4<>), typeof(Either<,,,>)),
            ("Five", typeof(Either5<>), typeof(Either<,,,,>)),
            ("Six", typeof(Either6<>), typeof(Either<,,,,,>)),
            ("Seven", typeof(Either7<>), typeof(Either<,,,,,,>)),
            ("Eight", typeof(Either8<>), typeof(Either<,,,,,,,>)),
        };

        private static readonly Func<IList<Type>, Func<int, object, object>> makeCreateMethod = types =>
        {
            /*
             * Generated coe:
             * Type parameters in brackets denote template parameters.
             * 
             * Either<T1,...,Tn> CreateDynamic[T1,...,Tn](int index, object element)
             * {
             *    switch(index) {
             *       case 0: return new Either<T1,...Tn>(new Either1<T1>((T1)element));
             *       case 1: return new Either<T1,...Tn>(new Either2<T2>((T2)element));
             *       case 2: return new Either<T1,...Tn>(new Either3<T3>((T3)element));
             *       case 3: return new Either<T1,...Tn>(new Either4<T4>((T4)element));
             *       case 4: return new Either<T1,...Tn>(new Either5<T5>((T5)element));
             *       case 5: return new Either<T1,...Tn>(new Either6<T6>((T6)element));
             *       case 6: return new Either<T1,...Tn>(new Either7<T7>((T7)element));
             *       case 7: return new Either<T1,...Tn>(new Either8<T8>((T8)element));
             *       default: throw new ArgumentException();
             *    }
             * }
             */

            var generator = Emit<Func<int, object, object>>.NewDynamicMethod("CreateDynamic");

            var eitherInfos = types.Zip(eitherConstructors).Select(x => (eitherNType: x.First, x.Second.label, x.Second.eitherN, x.Second.constructor, label: generator.DefineLabel(x.Second.label))).ToArray();

            var defaultLabel = generator.DefineLabel("defaultCase");

            generator.LoadArgument(0);
            generator.Switch(eitherInfos.Select(x => x.label).ToArray());

            //default-case
            generator.Branch(defaultLabel);

            //cases
            foreach (var (caseInfo,i) in eitherInfos.ZipWithStream(0, i => i + 1))
            {
                generator.MarkLabel(caseInfo.label);
                generator.LoadArgument(1);

                if (caseInfo.eitherNType.IsValueType)
                    generator.UnboxAny(caseInfo.eitherNType);
                else
                    generator.CastClass(caseInfo.eitherNType);

                if (eitherInfos.Length == 1)
                {
                    var concreteType = caseInfo.constructor.MakeGenericType(types[0]);
                    var concreteConstructor = concreteType.GetConstructor(new Type[] { types[0] });

                    generator.NewObject(concreteConstructor);
                    generator.Box(concreteType);
                }
                else
                {
                    var concreteType = eitherInfos[^1].constructor.MakeGenericType(types.Take(eitherInfos.Length).ToArray());
                    var eitherN = caseInfo.eitherN.MakeGenericType(caseInfo.eitherNType);
                    var concreteConstructor = concreteType.GetConstructor(new Type[] { eitherN });

                    generator.NewObject(eitherN.GetConstructor(new Type[] { caseInfo.eitherNType }));
                    generator.NewObject(concreteConstructor);
                    generator.Box(concreteType);
                }

                generator.Return();
            }

            generator.MarkLabel(defaultLabel);
            generator.NewObject<ArgumentException>();
            generator.Throw();

            var del = generator.CreateDelegate();

            return del;
        };

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="cacheSize">The maximum cache size.</param>
        public EitherCache(int cacheSize) : base(cacheSize)
        {
        }

        /// <summary>
        /// Creates a 1-8-element <see cref="Either{TLeft, TRight}"/> based on the type parameters of <paramref name="types"/>, containing the element <paramref name="element"/> in position <paramref name="index"/>.
        /// </summary>
        /// <param name="types">The types of the contents.</param>
        /// <param name="index">The 0-based index of the element, meaning which component of the resultant Either has a value..</param>
        /// <param name="element">The contents.</param>
        /// <exception cref="ArgumentException">If <paramref name="types"/> is null or if too many types were specified.</exception>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> lay outside the bounds of <paramref name="types"/>.</exception>
        public object RetrieveOrCacheCreate(IList<Type> types, int index, object element)
        {
            if (types is null)
                throw new ArgumentException();

            if (index < 0 || index >= types.Count)
                throw new IndexOutOfRangeException($"The index {index} was out of bounds (bounds: 0 to {types.Count-1}");

            if (types.Count > 8)
                throw new ArgumentException($"Too many type parameters were specified ({types.Count}). The maximum is 8.");

            RetrieveOrCache(types, () => makeCreateMethod(types), out var method);
            return method(index, element);
        }
    }
}
