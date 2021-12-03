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
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        //private readonly ImportHandler importHandler;

        public MainWindowViewModel()
        {
            //importHandler = new(this);
        }

        public CancellationTokenSource CancellationTokenSource { get; set; } = new();

        public bool IsCancell => CancellationTokenSource.IsCancellationRequested;

        public CancellationToken Token => CancellationTokenSource.Token;

        public int Count { get; set; }

        public int Current { get; set; }


    }
}

