using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetCorezhHans.Update
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string zipFile;
        private readonly string call;
        public MainWindow()
        {
            InitializeComponent();
            call = App.Args.First();
            zipFile = App.Args.Last();
            DataContext = this;
        }


        public string Text { get; set; } = "检查进程";

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                if (TestCall()) break;
                await Task.Delay(1000);
            }
            Text = "更新文件";
            UnZipFile();
        }

        private bool TestCall()
        {
            var fileName = Path.GetFileNameWithoutExtension(call);
            return Process.GetProcessesByName(fileName).Length is 0;
        }

        private async void UnZipFile()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(call)).Parent.FullName;
            await Task.Run(() => new FastZip().ExtractZip(zipFile, dir, ""));
            CleanTmp();
            Process.Start(call, "update");
            Environment.Exit(0);
        }

        private async void CleanTmp()
        {
            var dir = Path.GetDirectoryName(zipFile);
            var files = Directory.GetFiles(dir, "*.tmp");
            foreach (var item in files) await DeleteFile(item);
        }

        private static async Task DeleteFile(string path)
        {
            while (true)
            {
                try
                {
                    File.Delete(path);
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}