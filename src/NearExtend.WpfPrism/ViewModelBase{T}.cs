using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExtend.WpfPrism
{
    public class ViewModelBase<T> : ViewModelBase, IDataErrorInfo
    {
        public string Error { get; }

        public string this[string columnName] => ClassCache<T>
            .TestValidationProperty(this, columnName, out var propValue)
            ? this.GetDataError(columnName, propValue) : default;

    }
}
