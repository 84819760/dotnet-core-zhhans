using System.Windows;
using System.Windows.Controls;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// ApiPage.xaml 的交互逻辑
    /// </summary>
    public partial class ApiPage : UserControl
    {
        public ApiPage() => InitializeComponent();

        public ConfigManager Config { get; init; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.GetIsDesignMode()) return;         
            (DataContext as ApiPageViewModel).Datas = Config.ApiConfigs;
        }
    }
}
