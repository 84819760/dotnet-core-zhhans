using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace NearExtend.WpfPrism
{
    public abstract class ValueConverterBase<T> : MarkupExtension, IValueConverter
    {
        private static T _instance;
        public abstract object Convert(object value, Type targetType
            , object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType
            , object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) =>
            _instance ??= default;
    }
}
