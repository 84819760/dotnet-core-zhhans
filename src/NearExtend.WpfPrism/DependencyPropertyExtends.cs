using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NearExtend.WpfPrism
{
    public static class DependencyPropertyExtends
    {
        /// <summary>
        /// 注册依赖属性
        /// </summary>
        /// <typeparam name="TData">属性数据类型</typeparam>
        /// <typeparam name="TUser">拥有属性的类</typeparam>
        /// <param name="propName">属性名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="callBack">修改值时回调函数</param>
        public static DependencyProperty RegisterProperty<TData, TUser>(this string propName
            , TData defaultValue = default
            , PropertyChangedCallback callBack = null)
        {
            var propData = CreateFrameworkPropertyMetadata(defaultValue, callBack);
            return DependencyProperty.Register(propName, typeof(TData), typeof(TUser), propData);
        }

        private static FrameworkPropertyMetadata CreateFrameworkPropertyMetadata<T>(T defaultValue
            , PropertyChangedCallback callBack)
        {
            return new FrameworkPropertyMetadata(defaultValue)
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = callBack,
            };
        }

        public static T GetDataContext<T>(this DependencyObject dependencyObject)
            where T : class, new()
        {
            return (dependencyObject as FrameworkElement)?.DataContext as T ?? new T();
        }

        public static void SetViewModelValue<TViewModel>(this DependencyObject dependencyObject
            , Action<TViewModel> setter)
            where TViewModel : class, new()
        {
            setter(dependencyObject.GetDataContext<TViewModel>());
        }

        public static void SetViewModelValue(this DependencyObject dependencyObject
           , Action<dynamic> setter)
        {
            setter(dependencyObject.GetDataContext<dynamic>());
        }


        public static DependencyProperty FindDependencyProperty(this DependencyObject dp, string name)
        {
            var field = GetDependencyPropertyField(dp.GetType(), name);
            return field.GetValue(dp) as DependencyProperty;
        }

        private static FieldInfo GetDependencyPropertyField(Type type, string name)
        {
            if (type is null) return default;
            var filed = type.GetField(name);
            return filed ?? GetDependencyPropertyField(type.BaseType, name);
        }
    }
}
