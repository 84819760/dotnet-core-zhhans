using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Prism.Regions;

namespace NearExtend.WpfPrism
{
    public static class Extends
    {

        #region IDataErrorInfo
        public static string GetDataError(this IDataErrorInfo instance
            , string propName, object propValue)
        {
            var vc = new ValidationContext(instance) { MemberName = propName };
            return GetDataError(vc, propValue);
        }

        private static string GetDataError(ValidationContext validationContext, object propValue)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(propValue, validationContext, validationResults);
            return validationResults.FirstOrDefault()?.ErrorMessage;
        }

        #endregion

        #region IRegionManager

        public static IRegionManager RegisterViewWithRegion<T>(this IRegionManager region)
        {
            var type = typeof(T);
            return region.RegisterViewWithRegion(type.Name, type);
        }
        #endregion

        #region Design设计模式

        public static bool GetIsDesignMode(this FrameworkElement _) => GetIsDesignMode();

        public static void SetDesignData<T>(this FrameworkElement view, Action<object> set = null)
          where T : new()
        {
            if (!IsDesignMode) return;
            view.DataContext = new T();
            set?.Invoke(view.DataContext);
        }

        public static bool IsDesignMode { get; } = GetIsDesignMode();

        private static bool GetIsDesignMode() =>
         GetIsDesignMode(DesignerProperties.IsInDesignModeProperty);

        private static bool GetIsDesignMode(DependencyProperty dependencyProperty) =>
            (bool)DependencyPropertyDescriptor
              .FromProperty(dependencyProperty, typeof(FrameworkElement))
              .Metadata.DefaultValue;
        #endregion

        #region DependencyObject

        /// <summary>
        /// 查找子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="parent">父控件依赖对象</param>
        /// <param name="lstT">子控件列表</param>
        public static IEnumerable<T> GetChilds<T>(this DependencyObject parent) where T : class
        {
            var list = new List<T>();
            AddChildsToList(parent, list);
            return list;
        }

        private static void AddChildsToList<T>(DependencyObject parent, List<T> list) where T : class
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var v = VisualTreeHelper.GetChild(parent, i);
                if (v != null)
                {
                    if (v is T tv) list.Add(tv);
                    AddChildsToList(v, list);
                }
            }
        }
        #endregion
    }
}