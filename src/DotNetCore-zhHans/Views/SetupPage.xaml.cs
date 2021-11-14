using System.Windows.Controls;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// SetupPage.xaml 的交互逻辑
    /// </summary>
    public partial class SetupPage : UserControl
    {
        public SetupPage()
        {
            InitializeComponent();
            this.SetDesignData<SetupPageViewModel>();
        }

        private void UserControl1_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var config = App.GetConfigManager();
            BasicOptions.SetShow(new BasicOptions() { Config = config });
            ApiPage.SetShow(new ApiPage() { Config = config });
            FolderPage.SetShow(new FolderPage());
        }
    }
}
