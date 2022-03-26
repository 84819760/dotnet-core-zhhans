using System.Windows;
namespace DotNetCore_zhHans.Boot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string exe = Path.Combine
        (Directory.GetCurrentDirectory(), "lib", "DotNetCorezhHans.Manager.exe");

    private readonly ViewModel viewModel = new();

    private bool ExistsExe => File.Exists(exe);

    public MainWindow()
    {
        DataContext = viewModel;
        var exec = TestPack() ?? TestUpdate() ?? TestInit();
        if (exec is null)
        {
            if (ExistsExe) Process.Start(exe);
            Environment.Exit(0);
        }
        else
        {
            InitializeComponent();
            exec();
        }
    }

    private void Window_Closed(object sender, EventArgs e) => viewModel.WindowClosed();

    //打包命令
    private Action? TestPack() => TestArgs("pack", () => viewModel.CreatePack());

    //更新
    private Action? TestUpdate() => TestArgs("pack", () => viewModel.Update());

    //首次使用
    private Action? TestInit() => ExistsExe ? default : viewModel.Init;

    private Action? TestArgs(string cmd, Action action) =>
        App.Args.Any(x => x?.ToLower() == cmd) ? action : default;
}


public partial class ViewModel
{
    public readonly CancellationTokenSource cancellation = new();

    [AddNotifyProperty]
    private string title = "标题";

    /// <summary>
    /// 自行细节
    /// </summary>
    [AddNotifyProperty]
    private string details = "自行细节";

    /// <summary>
    /// 初始化下载
    /// </summary>
    [AddNotifyProperty]
    private string context = "初始化下载";

    /// <summary>
    /// 更新进度
    /// </summary>
    [AddNotifyProperty]
    private double updateProgress;

    /// <summary>
    /// 下载进度
    /// </summary>
    [AddNotifyProperty]
    private double downloadProgress;

    internal void WindowClosed() => cancellation.Cancel();

    public void Init() => new ExecInit(this).Run();
    public void CreatePack() => new ExecPack(this).Run();
    internal void Update() => new ExecUpdate(this).Run();
}