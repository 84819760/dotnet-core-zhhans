using System.Windows;

namespace DotNetCore_zhHans.Boot;

partial class ExecUpdate : ExecBase
{
    public ExecUpdate(ViewModel viewModel) : base(viewModel) { }

    public async override void Run()
    {
        vm.Title = $"更新 {App.Version}";
        vm.Details = "获取更新配置";
        vm.Context = "更新";
        var list = (await GetJsonFileInfos()).ToList();
        TryAddDb(list);
        await CreateDownloadAndUnZip(list).DownloadFileAsync();
        End();
    }

    private void End()
    {
        Process.Start(Path.Combine(CurrentDirectory, Share.RootExe), "--updateOk");
        Environment.Exit(0);
    }

    protected override async void Complete((FileInfo info, string file) v) => await MoveFile(v.info, v.file);

    private async Task MoveFile(FileInfo info, string file)
    {
        var target = Path.Combine(LibDirectory, info.SourceName);
        Exception? error = null;
        for (int i = 0; i < 3; i++)
        {
            try
            {
                File.Move(file, target, true);
                return;
            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
                error = ex;
            }         
        }
        if (error is null) return;
        throw error;
    }
}
