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

        private void Button_Click(object sender, RoutedEventArgs e) => ViewModel.Start();
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

        internal void Start() =>
            new ImportHandler(this, @"D:\tmp\TranslData.db", @"TranslData.db").Run();
    }
}

