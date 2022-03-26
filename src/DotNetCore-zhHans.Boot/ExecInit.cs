using System.Net.Http;
using System.Windows;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 首次更新
/// </summary>
partial class ExecInit : ExecBase
{
    public ExecInit(ViewModel viewModel) : base(viewModel) { }

    async public override void Run()
    {
        var vm = ViewModel;
        vm.Title = "首次使用";
        vm.Context = "初始化下载";
        var libDirectory = GetLibDirectory();
        var downloadDirectory = GetDownloadDirectory();

        var fileList = await DownloadPackJson();
        for (int i = 0; i < fileList.Length; i++)
        {
            if (vm.cancellation.Token.IsCancellationRequested) break;
            var item = fileList[i];
            vm.Details = $"{item.SourceName}{item.ExtensionName}";
            vm.UpdateProgress = (double)(i + 1) / fileList.Length;
            vm.Details = $"{item.SourceName}{item.ExtensionName}";
            var file = $"{downloadDirectory}\\{vm.Details}";
            var url = $"{this.url}/packs/{vm.Details}";
            await Download(url, file, libDirectory);
        }

        vm.Context = "初始化数据库";
        vm.Details = $"TranslData.zip";
        var urlDb = $"{url}/{vm.Details }";
        var dbfile = $"{downloadDirectory}\\{vm.Details}";
        await Download(urlDb, dbfile, libDirectory);
        MessageBox.Show("完成");
    }

    private async Task<FileInfo[]> DownloadPackJson()
    {
        var url = $"{this.url}/packs/_packs.json";
        using var hc = new HttpClient();
        var json = await hc.GetStringAsync(url);
        return JsonSerializer.Deserialize<FileInfo[]>(json) ?? Array.Empty<FileInfo>();
    }

    private static string GetLibDirectory()
    {
        var res = Path.Combine(Directory.GetCurrentDirectory(), "lib");
        Directory.CreateDirectory(res);
        return res;
    }

    private static string GetDownloadDirectory()
    {
        var res = Path.Combine(Directory.GetCurrentDirectory(), "Download");
        Directory.CreateDirectory(res);
        return res;
    }


    private async Task Download(string url, string file, string libPath)
    {
        var res = await DownloadFile(url, file, CancellationToken, x =>
        {
            ViewModel.DownloadProgress = (double)x.received / (x.length ?? 0);
        });
    }
}
