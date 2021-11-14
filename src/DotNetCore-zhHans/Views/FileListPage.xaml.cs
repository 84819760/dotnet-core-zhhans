using System.Windows;
using System.Windows.Controls;
using DotNetCorezhHans.Messages;
using DotNetCorezhHans.ViewModels;

namespace DotNetCorezhHans.Views
{
    using FileListPageMsg = Message<FileListPageViewModel>;

    /// <summary>
    /// FolderList.xaml 的交互逻辑
    /// </summary>
    public partial class FileListPage : UserControl
    {
        private readonly FileListPageMsg fileListPage = FileListPageMsg.Instance;

        public FileListPage() => InitializeComponent();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            fileListPage.Publish(new() { Target = "RequestInstance", IsRequest = true });
        }
    }
}
