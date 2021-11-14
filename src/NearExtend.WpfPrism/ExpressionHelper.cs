using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NearCoreExtensions;

namespace NearExtend.WpfPrism
{
    internal static class ExpressionHelper
    {
        private static Expression<Func<TIn, TOut>> LambdaFunc<TIn, TOut>(this Expression exp
            , ParameterExpression parameter)
        {
            return Expression.Lambda<Func<TIn, TOut>>(exp, parameter);
        }

        private static Expression<Func<T>> LambdaFunc<T>(this Expression exp) =>
            Expression.Lambda<Func<T>>(exp);

        private static Expression<Action<TIn, TOut>> LambdaAction<TIn, TOut>(this Expression exp
            , params ParameterExpression[] parameter)
        {
            return Expression.Lambda<Action<TIn, TOut>>(exp, parameter);
        }

        public static Func<object, object> CreatePropGet<T>(string propName)
        {
            var x = Expression.Parameter(typeof(object), "x");
            return x.Convert<T>()
                .Property(propName)
                .Convert<object>()
                .LambdaFunc<object, object>(x)
                .Compile();
        }

        public static Action<object, object> CreatePropSet<T>(string propName)
        {
            var i = Expression.Parameter(typeof(object), "i");
            var v = Expression.Parameter(typeof(object), "v");
            return i.Convert<T>()
                .Property(propName, out var propType)
                .Assign(v.Convert(propType))
                .LambdaAction<object, object>(i, v)
                .Compile();
        }

        public static Func<object> CreateInstance(Type type) =>
            type.New().LambdaFunc<object>().Compile();      
    }
}
