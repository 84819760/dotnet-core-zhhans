using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Prism.Events;
using Prism.Mvvm;

namespace NearExtend.WpfPrism
{
    public abstract class ViewModelBase : BindableBase
    {
        private DependencyObject parentUi;
        private Window window;

        public DependencyObject UiElement { get; set; }

        public static bool IsDesignMode => Extends.IsDesignMode;

        public FrameworkElement FrameworkElement => UiElement as FrameworkElement;

        public Window Window => TestUiElement(ref window, () => Window.GetWindow(UiElement));

        public DependencyObject ParentUiElement =>
            TestUiElement(ref parentUi, () => VisualTreeHelper.GetParent(UiElement));

        private T TestUiElement<T>(ref T value, Func<T> func) where T : class
        {
            if (value is not null) return value;
            return UiElement is null ? default : value ??= func();
        }

        public T FindName<T>(string name) where T : class => FrameworkElement?.FindName(name) as T;

        public IEnumerable<T> FindNames<T>(params string[] name)
        {
            foreach (var item in name)
            {
                var res = FrameworkElement?.FindName(item);
                if (res is T tres) yield return tres;
            }
        }

        public T FindParentViewModel<T>(Func<T, bool> where = null)
            where T : ViewModelBase
        {
            if (ParentUiElement is not FrameworkElement fe) return default;
            where ??= x => true;
            return FindParentViewModel(fe, where);
        }

        private static T FindParentViewModel<T>(FrameworkElement frameworkElement
            , Func<T, bool> where) where T : ViewModelBase
        {
            var dc = frameworkElement.DataContext as ViewModelBase;
            return dc is T res && where(res) ? res : dc?.FindParentViewModel(where);
        }

        /// <summary>
        /// 返回指定Key的字典
        /// </summary>
        public T FindResource<T>(string key)
        {
            var res = FrameworkElement?.FindResource(key) ?? FindResource(key);
            return res is T tRes ? tRes : default;
        }

        private static object FindResource(string key)
        {
            try
            {
                return Application.Current.FindResource(key);
            }
            catch (ResourceReferenceKeyNotFoundException) { }

            return default;
        }

        /// <summary>
        /// 返回指定名称资源
        /// </summary>
        protected static T GetResources<T>(string key)
        {
            var res = Application.Current.Resources[key];
            return res is null ? default : (T)res;
        }

        protected T GetUiElement<T>() where T : class => UiElement as T;

        public void CallStory(string storyName, Action action = null)
        {
            UiElement.CallStory(storyName);
            action?.Invoke();
        }

        public void SetDesign(Action action)
        {
            if (!IsDesignMode) return;
            action();
        }
    }
}