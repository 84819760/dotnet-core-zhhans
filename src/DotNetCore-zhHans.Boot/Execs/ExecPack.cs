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

    public async override void Run()
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
        var packDir = Path.Combine(dir, "_packs");
        Directory.CreateDirectory(packDir);
        SevenZipBase.SetLibraryPath("7z.dll");
        var files = fileProvider.GetFileInfos();
        await RunPack(files, dir);
        var json = JsonSerializer.Serialize(files, Share.JsonOptions);
        File.WriteAllText(@"_packs/_pack.json", json);
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
        var (source, pack) = fileInfo.GetFullPath(dir, "_packs");
        if (Isjson(fileInfo, source, pack) || await Is7zdll(fileInfo, source, pack)) return;
        fileInfo.ExtensionName = ".7z";
        fileInfo.Index = fileInfo.SourceName == "DotNetCoreZhHans.exe" ? -1024 : 0;


        await new ZipHelper(v => vm.SubProgress = v)
            .Zip(source, pack);
    }

    private static bool Isjson(FileInfo fileInfo, string source, string pack)
    {
        if (!fileInfo.SourceName.ToLower().EndsWith(".json")) return false;
        File.Copy(source, pack, true);
        return true;
    }

    private static async Task<bool> Is7zdll(FileInfo fileInfo, string source, string pack)
    {
        if (fileInfo.SourceName != "7z.dll") return false;
        fileInfo.ExtensionName = ".zip";
        fileInfo.Index = 1024;
        fileInfo.Cmd = "ZipInit";
        await new ZipHelper().Zip(source, pack);
        return true;
    }
}
