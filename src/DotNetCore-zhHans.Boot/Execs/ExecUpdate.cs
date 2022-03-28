using System.Windows;

namespace DotNetCore_zhHans.Boot;

partial class ExecUpdate : ExecBase
{
    public ExecUpdate(ViewModel viewModel) : base(viewModel) { }

    public async override void Run()
    {
        vm.Title = "更新";
        vm.Details = "获取更新配置";
        vm.Context = "更新";
        var list = (await GetJsonFileInfos()).ToList();
        await CreateDownloadAndUnZip(list).DownloadFileAsync();
    }

    protected override void Complete((FileInfo info, string file) v)
    {
        if (v.info.SourceName != "DotNetCoreZhHans.exe") return;
        Environment.CurrentDirectory = DownloadDirectory;
        Process.Start(v.file, "--update-file-move");
        Environment.Exit(0);
    }
}
