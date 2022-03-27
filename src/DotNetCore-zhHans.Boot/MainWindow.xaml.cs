using System.Windows;
namespace DotNetCore_zhHans.Boot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string exe = Path.Combine
        (Directory.GetCurrentDirectory(), "lib", "DotNetCorezhHansMain.exe");

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
    private Action? TestPack() => TestArgs("--pack", () => viewModel.CreatePack());

    //更新
    private Action? TestUpdate() => TestArgs("--update", () => viewModel.Update());

    //首次使用
    private Action? TestInit() => ExistsExe ? default : viewModel.Init;

    private Action? TestArgs(string cmd, Action action) =>
        App.Args.Any(x => x?.ToLower() == cmd) ? action : default;
}

[AddINotifyPropertyChangedInterface]
public partial class ViewModel
{
    public readonly CancellationTokenSource cancellation = new();


    public string Title { get; set; } = "标题";

    /// <summary>
    /// 自行细节
    /// </summary>
    public string Details { get; set; } = "加载配置";

    /// <summary>
    /// 初始化下载
    /// </summary>
    public string Context { get; set; } = "初始化下载";

    /// <summary>
    /// 主进度
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// 下载进度
    /// </summary>
    public double SubProgress { get; set; }

    internal void WindowClosed() => cancellation.Cancel();

    public void Init() => new ExecInit(this).Run();
    public void CreatePack() => new ExecPack(this).Run();
    internal void Update() => new ExecUpdate(this).Run();
}