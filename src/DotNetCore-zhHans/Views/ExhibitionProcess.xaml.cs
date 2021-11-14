using System.Windows;
using System.Windows.Controls;
using DotNetCorezhHans.Messages;
using DotNetCorezhHans.ViewModels;

namespace DotNetCorezhHans.Views
{
    using ProcessMessage = Message<ExhibitionProcessViewModel>;

    /// <summary>
    /// ProcessControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionProcess : UserControl
    {
        private readonly ProcessMessage processMsg = ProcessMessage.Instance;

        public ExhibitionProcess() => InitializeComponent();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
{
            processMsg.Publish(new() { Target = "RequestInstance", IsRequest = true });
        }
    }
}
