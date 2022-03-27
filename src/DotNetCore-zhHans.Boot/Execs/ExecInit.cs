using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using SevenZip;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 首次更新
/// </summary>
partial class ExecInit : ExecBase
{
    protected readonly string url = "http://www.wyj55.cn/download/DotNetCorezhHans20";

    public ExecInit(ViewModel viewModel) : base(viewModel) { }

    protected override void InitZip() { }

    public record class DirectoryData(string LibDirectory, string DownloadDirectory)
    {
        public FileInfo FileInfo { get; set; } = null!;

        public string LibFile => Path.Combine(LibDirectory, FileInfo.SourceName);

        public string UnZipFile => Path.Combine(DownloadDirectory, $"{FileInfo.SourceName}");

        public string DownloadFile =>
            Path.Combine(DownloadDirectory, $"{FileInfo.SourceName}{FileInfo.ExtensionName}");

        public bool IsExists => File.Exists(LibFile);

        public bool TestMd5 => Share.GetMd5(LibFile) == FileInfo.Md5;

        public void Move()
        {
            File.Move(UnZipFile, LibFile, true);
            File.Delete(DownloadFile);
        }
    }

    async public override void Run()
    {
        var vm = base.vm;
        vm.Title = "初始化组件";
        vm.Context = "下载组件";

        await Init7z();
        var dirData = new DirectoryData(CreateSubDirectory("lib"), CreateSubDirectory("_tmp"));
        var datas = await DownloadPackJson();
        var count = datas.Length;
        for (int i = 0; i < count; i++)
        {
            if (CancellationToken.IsCancellationRequested) return;
            var item = datas[i];
            vm.Progress = (double)(i + 1) / count;
            vm.Details = item.SourceName;
            await Run(dirData with { FileInfo = item });
        }
        await InitDb();
        vm.Context = "下载完成";
        try
        {
            Directory.Delete(dirData.DownloadDirectory);
        }
        finally { }

        MessageBox.Show("完成");
    }

    private async Task Init7z()
    {
        var name = "7z.dll";
        var zipName = $"{name}.zip";
        var url = $"{this.url}/pack/{zipName}";
        var tmpPath = Path.Combine(Directory.GetCurrentDirectory(), zipName);
        var libFile = Path.Combine(CreateSubDirectory("lib"), name);
        await DownloadAndUnZip(url, tmpPath, libFile);
        base.InitZip();
    }

    private async Task InitDb()
    {
        var name = "TranslData.db";
        var libFile = Path.Combine(CreateSubDirectory("lib"), name);
        if (File.Exists(libFile)) return;
        var zipName = $"{name}.7z";
        var url = $"{this.url}/{zipName}";
        var tmpPath = Path.Combine(Directory.GetCurrentDirectory(), "_tmp", zipName);
        await DownloadAndUnZip(url, tmpPath, libFile
            , dv =>
            {
                vm.Context = "下载数据库";
                vm.Details = "初始化数据库";
                vm.SubProgress = dv;
            }, zv =>
            {
                vm.Context = "解压数据库";
                vm.SubProgress = zv;
            });
    }



    private async Task Run(DirectoryData data)
    {
        if (data.IsExists && data.TestMd5) return;
        var url = $"{this.url}/pack/{data.FileInfo.UrlName}";

        await DownloadFile(url, data.DownloadFile, CancellationToken, x => vm.SubProgress = x);
        await UnZip(data);
    }

    private async Task UnZip(DirectoryData data)
    {
        if (IsJson(data)) return;
        await new ZipHelper(p =>
        {
            vm.SubProgress = p;
            vm.Details = $"解压:{data.FileInfo.SourceName}";
        }).UnZip(data.DownloadFile, data.UnZipFile);
        data.Move();
    }

    private static bool IsJson(DirectoryData data)
    {
        if (!data.DownloadFile.EndsWith(".json")) return false;
        data.Move();
        return true;
    }

    private async Task<FileInfo[]> DownloadPackJson()
    {
        var url = $"{this.url}/pack/_pack.json";
        using var hc = new HttpClient();
        var json = await hc.GetStringAsync(url);
        return JsonSerializer.Deserialize<FileInfo[]>(json) ?? Array.Empty<FileInfo>();
    }
}
