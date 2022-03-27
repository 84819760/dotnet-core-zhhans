using System.Windows;
using System.Windows.Markup;
using SevenZip;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 打包
/// </summary>
partial class ExecPack : ExecBase
{
    public ExecPack(ViewModel viewModel) : base(viewModel) { }

    protected override void InitZip() { }

    public override async void Run()
    {
        await Task.Run(RunPack);
        MessageBox.Show("打包完成");
        Environment.Exit(0);
    }

    private async Task RunPack()
    {
        vm.Details = "读取文件";
        vm.Context = vm.Title = "创建更新包";
        var dir = Directory.GetCurrentDirectory();
        var fileProvider = new FileInfoProvider(dir);
        var packDir = Path.Combine(dir, "_pack");
        Directory.CreateDirectory(packDir);
        SevenZipBase.SetLibraryPath("7z.dll");
        var files = fileProvider.GetFileInfos();
        await RunPack(files, dir);
        var json = JsonSerializer.Serialize(files, Share.JsonOptions);
        File.WriteAllText(@"_pack/_pack.json", json);
    }

    private async Task RunPack(FileInfo[] files, string dir)
    {
        var count = files.Length;
        for (int i = 0; i < count; i++)
        {
            var p = (double)(i + 1) / count;
            var item = files[i];
            await RunPack(item, dir);
            vm.Progress = p;
            vm.Details = $"{item.SourceName}";
        }
    }

    private async Task RunPack(FileInfo fileInfo, string dir)
    {
        if (Isjson(fileInfo, dir) || await Is7zdll(fileInfo, dir)) return;
        fileInfo.ExtensionName = ".7z";
        var (source, pack) = fileInfo.GetFullPath(dir, "_pack");

        await new ZipHelper(v => vm.SubProgress = v)
            .Zip(source, pack);
    }

    private static bool Isjson(FileInfo fileInfo, string dir)
    {
        if (!fileInfo.SourceName.ToLower().EndsWith(".json")) return false;
        var (source, pack) = fileInfo.GetFullPath(dir, "_pack");
        File.Copy(source, pack, true);
        return true;
    }

    private static async Task<bool> Is7zdll(FileInfo fileInfo, string dir)
    {
        if (fileInfo.SourceName != "7z.dll") return false;
        fileInfo.ExtensionName = ".zip";
        fileInfo.Index = 1024;
        fileInfo.Cmd = "ZipInit";
        var (source, pack) = fileInfo.GetFullPath(dir, "_pack");
        await new ZipHelper().Zip(source, pack);
        return true;
    }
}
