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

    protected override async void OnClosing(CancelEventArgs e)
    {
        if (ViewModel.Task.IsCompleted) return;
        e.Cancel = true;
        if (!ViewModel.IsCancell)
        {
            ViewModel.CancellationTokenSource.Cancel();         
            Title = "等待线程停止";
            await SetEnd();
        }       
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Start();
        await SetEnd();
    }
    
    private async Task SetEnd()
    {
        await ViewModel.DisposeAsync();
        ViewModel.WriteProgress.Value = 0;
        Title = "完成";
        if (ViewModel.IsCancell) Close();
        else MessageBox.Show("完成");
    }
}

internal class MainWindowViewModel : NotifyPropertyChanged, IAsyncDisposable
{
    private ImportHandler importHandler = null!;
    private DateTime dateTime;

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    public bool IsCancell => CancellationTokenSource.IsCancellationRequested;

    public ProgressData ReadProgress { get; } = new();

    public ProgressData WriteProgress { get; } = new() { Digits = 2 };

    public string TimeSpan { get; set; } = "0分0秒";

    public int WriteCount { get; set; }

    public Task Task { get; private set; } = null!;

    internal Task Start() => Task = Task.Run(CreateImportHandler);

    private async Task CreateImportHandler()
    {
        dateTime = DateTime.Now;
        var time = new System.Timers.Timer(1000) { AutoReset = true, Enabled = true };
        time.Elapsed += Timer_Elapsed;
        importHandler = new ImportHandler(this, App.Source, App.Target);
        await importHandler.Run();
        time.Enabled = false;
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var timeSpan = DateTime.Now - dateTime;
        TimeSpan = $"{timeSpan.Minutes}分{timeSpan.Seconds}秒";
    }

    public ValueTask DisposeAsync() => (importHandler?.DisposeAsync()).GetValueOrDefault();
}