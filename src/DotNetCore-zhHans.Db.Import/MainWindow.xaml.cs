using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace DotNetCore_zhHans.Db.Import
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainWindowViewModel ViewModel { get; } = new();

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button!.IsEnabled = false;
            await ViewModel.Start();
            button!.IsEnabled = true;
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public CancellationTokenSource CancellationTokenSource { get; set; } = new();

        public bool IsCancell => CancellationTokenSource.IsCancellationRequested;

        public CancellationToken Token => CancellationTokenSource.Token;

        public double Count { get; set; }

        public double Current { get; set; }

        public string? Title { get; set; }

        internal Task Start()
        {
            var source = @"D:\tmp\TranslData.db";
            var target = @"D:\tmp\TranslData2.db";
            return Task.Run(() => new ImportHandler(this, source, target).Run());
        }
    }
}

