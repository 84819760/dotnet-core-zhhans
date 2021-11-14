using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExtend.WpfPrism
{
    internal static class ClassCache<T>
    {
        private static readonly bool isContainValidation = GetIsContainValidationProperty();
        private static readonly Dictionary<string, Func<object, object>> funcMap = new();
        private static Func<object> viewModel;

        public static object GetValue(object instance, string propName)
        {
            if (!funcMap.TryGetValue(propName, out var func))
                funcMap[propName] = func = ExpressionHelper.CreatePropGet<T>(propName);
            return func(instance);
        }

        public static bool TestValidationProperty(object instance
            , string propName, out object value)
        {
            value = isContainValidation ? GetValue(instance, propName) : null;
            return isContainValidation;
        }

        private static bool GetIsContainValidationProperty() => typeof(T)
            .GetProperties()
            .SelectMany(x => x.GetCustomAttributes(true))
            .OfType<ValidationAttribute>().Any();


        public static object GetViewModel() => (viewModel ??= GetViewModelFunc())();

        private static Func<object> GetViewModelFunc()
        {
            return ClassCache.ViewModelMap.TryGetValue(typeof(T), out var type)
                ? ExpressionHelper.CreateInstance(type)
                : () => null;
        }

    }
}
