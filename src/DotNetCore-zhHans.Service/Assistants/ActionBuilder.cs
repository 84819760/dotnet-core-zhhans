using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service
{
    using DictType = ConcurrentDictionary<MemberInfo, Action<object, object>>;
    internal class ActionBuilder
    {
        private static readonly DictType dic = new();

        public static Action<object, object> GetAction(MemberInfo member)
        {
            if (!dic.TryGetValue(member, out var res))
                dic[member] = res = CreateAction(member);
            return res;
        }

        private static Action<object, object> CreateAction(MemberInfo member) => typeof(object)
            .CreateParameter(out var instance)
            .Convert(member.DeclaringType)
            .Property(member.Name)
            .Assign(typeof(object).CreateParameter(out var value).Convert(GetMemberType(member)))
            .CreateLambda<Action<object, object>>(instance, value)
            .Compile();

        private static Type GetMemberType(MemberInfo memberInfo) => memberInfo switch
        {
            PropertyInfo propInof => propInof.PropertyType,
            MethodInfo methodInfo => methodInfo.ReturnType,
            _ => throw new NotImplementedException("未处理类型"),
        };
    }
}
