using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows;

namespace NearExtend.WpfPrism
{
    public class CallMethodTrigger : CallMethodAction
    {
        protected override void Invoke(object parameter)
        {
            var method = TargetObject?.GetType()
                .GetMethods()
                .Where(x => x.Name == MethodName)
                .FirstOrDefault(x => x.GetParameters().Length == 1 
                                && x.GetParameters().First().ParameterType == typeof(object));

            method?.Invoke(TargetObject, new[] { Param });
        }

        #region DependencyProperty:Param 
        public object Param
        {
            get => GetValue(ParamProperty);
            set => SetValue(ParamProperty, value);
        }

        public static readonly DependencyProperty ParamProperty = "Param"
            .RegisterProperty<object, CallMethodTrigger>();
        #endregion
    }
}
