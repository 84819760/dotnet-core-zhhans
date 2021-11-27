using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Dotnet_Intellisense
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<DataModel>? DataModels { get; set; } = new();

        public bool Enabled { get; set; } = true;

        public string? ProgressTitle { get; set; } = " 使用.Net官方包替换本地文件，作为汉化工具的补充。";

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetIsEnabled(false);
            var items = await JsonBuilder.GetDataModels();
            items.ToList().ForEach(x => DataModels?.Add(x));
            SetIsEnabled(true);
        }

        private void SetIsEnabled(bool isEnabled)
        {
            Button1.IsEnabled = isEnabled;
            Button2.IsEnabled = isEnabled;
        }

        //执行
        private async void Run(object sender, RoutedEventArgs e)
        {
            Enabled = false;
            var items = DataModels!.Where(x => x.IsSelect).Select(CreateFile);
            foreach (var item in items)
            {
                item.Notice += SetProgressTitle;
                try
                {
                    await item.Run();
                }
                catch (Exception)
                {

                }
            }
            Enabled = true;
            SetProgressTitle("更新完成");
        }

        private FileItem CreateFile(DataModel data) => new(data);

        private void SetProgressTitle(string value) => ProgressTitle = value;

        //全选
        private void SelectAll(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataModels!)
            {
                item.IsSelect = !item.IsSelect;
            }
        }
    }
}
