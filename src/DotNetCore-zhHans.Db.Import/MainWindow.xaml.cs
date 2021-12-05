﻿using System;
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

    public MainWindowViewModel ViewModel { get; } = new();

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

public class MainWindowViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ImportHandler importHandler = null!;

    public MainWindowViewModel() => PropertyChanged += PropertyChangedHandler;

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    public bool IsCancell => CancellationTokenSource.IsCancellationRequested;

    public double ProgressMaximum { get; set; } = 1;

    public double ProgressValue { get; set; } = 0;

    public double Progress { get; set; } = 0;

    public double WindowsProgress { get; set; }

    public int WriteCount { get; set; }

    public int CacheCount { get; set; }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        var prop = e.PropertyName;

    }

    //internal void UpdateCurrent(int value)
    //{
    //    //if (IsCancell) return;
    //    Current = value;
    //    var ps = Current / Count;
    //    WindowsProgress = ps;
    //    SetCurrentString((int)(ps * 100));
    //}

    //internal void UpdateWriteTitle(int vlaue)
    //{
    //    writeCount += vlaue;
    //    WriteTitle = $"写入 : {writeCount} 行";
    //}

    //internal void UpdateWriteTitle(string value) => WriteTitle = value;

    //private void SetCurrentString(double progress = 0) =>
    //    CurrentString = $"读取:{Current}({progress}%)";

    public Task Task { get; private set; } = null!;

    internal Task Start() => Task = Task.Run(CreateImportHandler);

    private async Task CreateImportHandler()
    {
        importHandler = new ImportHandler(this, App.Source, App.Target);
        await importHandler.Run();
    }

    public ValueTask DisposeAsync() => importHandler.DisposeAsync();
}