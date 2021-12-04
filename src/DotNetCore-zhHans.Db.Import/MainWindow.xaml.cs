using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DotNetCore_zhHans.Db.Import;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    public MainWindowViewModel ViewModel { get; } = new();

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        button1.IsEnabled = false;
        button1.Content = "停止中";
        ViewModel.CancellationTokenSource?.Cancel();
        await ViewModel.Task;
        SetEnd();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (ViewModel.IsCancell) return;
        if (e != null) e.Cancel = true;
        MessageBox.Show("先停止才能关闭");
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CancellationTokenSource = new();
        await ViewModel.Start();
        ViewModel.CancellationTokenSource?.Cancel();
        SetEnd();
    }

    private void SetEnd()
    {
        button1.IsEnabled = false;
        button1.Content = "已结束";
        ViewModel.Current = ViewModel.Count;
        ViewModel.WindowsProgress = 1;
    }
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private int writeCount = 0;

    public CancellationTokenSource? CancellationTokenSource { get; set; }

    public bool IsCancell => (CancellationTokenSource?.IsCancellationRequested).GetValueOrDefault();

    public CancellationToken Token => (CancellationTokenSource?.Token).GetValueOrDefault();

    public double Count { get; set; } = 100;

    public double Current { get; set; }

    public string WriteTitle { get; set; } = "写入 : 0 行";

    public string CurrentString { get; set; } = "读取:0(0%)";

    public double WindowsProgress { get; set; }

    internal void UpdateCurrent(int value)
    {
        if (IsCancell) return;
        Current = value;  
        var ps = Current / Count ;
        WindowsProgress = ps;
        SetCurrentString((int)(ps * 100));
    }

    internal void UpdateWriteTitle(int vlaue)
    {
        writeCount += vlaue;
        WriteTitle = $"累计写入 : {writeCount} 行";
    }

    internal void UpdateWriteTitle(string value) => WriteTitle = value;

    private void SetCurrentString(double progress = 0) =>
        CurrentString = $"读取:{Current}({progress}%)";

    public Task Task { get; private set; } = null!;

    internal Task Start() => Task = Task.Run(StartImportHandler);

    private async Task StartImportHandler()
    {
        await using var handler = new ImportHandler(this, App.Source, App.Target);
        await handler.Run();
    }
}
