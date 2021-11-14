using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    public class ListModuleViewModel : ViewModelBase<ListModuleViewModel>
    {
        public object AddContent { get; set; }

        public string Title { get; set; } = "标题";

        public string Icon { get; set; } = "ListStatus";

        public string RegionName { get; set; }

        public HorizontalAlignment ContentHorizontalAlignment { get; set; }

    }
}
