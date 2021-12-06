using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetCore_zhHans.Db.Import;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    internal MainWindowViewModel ViewModel { get; } = new();

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CancellationTokenSource.Cancel();
        SetEnd();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (ViewModel.Task.IsCompleted) return;
        if (e != null) e.Cancel = true;
        MessageBox.Show("先停止才能关闭");
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Start();
        SetEnd();
    }

    private async void SetEnd()
    {
        //button1.IsEnabled = false;
        //button1.Content = "停止中";
        //await ViewModel.DisposeAsync();
        //button1.Content = "已结束";
        //ViewModel.Current = ViewModel.Count;
        //ViewModel.WindowsProgress = 1;
    }
}

internal class MainWindowViewModel : NotifyPropertyChanged, IAsyncDisposable
{

    private ImportHandler importHandler = null!;

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    public bool IsCancell => CancellationTokenSource.IsCancellationRequested;

    public ProgressData ReadProgress { get; } = new();

    public ProgressData WriteProgress { get; } = new();

    public Task Task { get; private set; } = null!;

    internal Task Start() => Task = Task.Run(CreateImportHandler);

    private async Task CreateImportHandler()
    {
        importHandler = new ImportHandler(this, App.Source, App.Target);
        await importHandler.Run();
    }

    public ValueTask DisposeAsync() => importHandler.DisposeAsync();
}