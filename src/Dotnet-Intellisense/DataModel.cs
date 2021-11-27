using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Dotnet_Intellisense
{
    public class DataModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name { get; set; }

        public string? Url { get; set; }

        public bool IsSelect { get; set; }

        public DataDetailsModel[]? DataDetails { get; set; }

    }
}
