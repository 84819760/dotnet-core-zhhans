using System.Windows;
using DotNetCore_zhHans.Boot.Execs;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string exe = Path.Combine
        (Directory.GetCurrentDirectory(), "lib", "DotNetCorezhHansMain.exe");

    private readonly ViewModel viewModel = new ViewModel().Init();

    private bool IsExistsExec => File.Exists(exe);

    public MainWindow()
    {
        DataContext = viewModel;
        var exec = TestPack() ?? TestUpdate() ?? TestUpdateFile() ?? TestInit();
        if (exec is not null)
        {
            InitializeComponent();
            exec();
        }
        else if (IsExistsExec)
        {
            Process.Start(exe, "--run");
            Environment.Exit(0);
        }
    }

    private void Window_Closed(object sender, EventArgs e) => viewModel.WindowClosed();

    //打包命令
    private Action? TestPack() => TestArgs("--pack", () => viewModel.CreatePack());

    //检查
    private Action? TestUpdate() => TestArgs("--update-file-check", () => viewModel.Update());

    //移动文件
    private Action? TestUpdateFile() => TestArgs("--update-file-move", () => viewModel.UpdateFile());

    //首次使用
    private Action? TestInit() => IsExistsExec ? default : viewModel.InitFile;

    private static Action? TestArgs(string cmd, Action action) =>
        App.Args.Any(x => x?.ToLower() == cmd) ? action : default;
}

[AddINotifyPropertyChangedInterface]
public partial class ViewModel
{
    public readonly CancellationTokenSource cancellation = new();

    public ViewModel() { }

    public ViewModel Init()
    {
        Title = Details = Context = "";
        Length = null;
        Progress = SubProgress = 0;
        IsIndeterminate = false;
        return this;
    }

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
    public double Progress { get; set; } = 0.5;

    /// <summary>
    /// 下载进度
    /// </summary>
    public double SubProgress { get; set; } = 0.5;

    public bool IsIndeterminate { get; set; }

    public string? Length { get; set; } = "100kb";

    internal void WindowClosed() => cancellation.Cancel();

    //下载、解压、移动到lib ，db初始化
    public void InitFile() => new ExecInit(this).Run();

    //下载、解压、退出，
    internal void Update() => new ExecUpdate(this).Run();

    //更新文件
    internal void UpdateFile() => new ExecMoveFile(this).Run();

    public void CreatePack() => new ExecPack(this).Run();
}