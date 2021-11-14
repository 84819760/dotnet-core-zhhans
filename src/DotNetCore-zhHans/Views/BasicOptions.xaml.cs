using System.Windows.Controls;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// BasicOptions.xaml 的交互逻辑
    /// </summary>
    public partial class BasicOptions : UserControl
    {
        public BasicOptions()
        {
            InitializeComponent();
        }

        public ConfigManager Config
        {
            get => DataContext as ConfigManager;
            init => DataContext = value;
        }
    }
}
