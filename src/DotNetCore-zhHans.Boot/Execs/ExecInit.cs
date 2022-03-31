using System.Windows;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 首次更新
/// </summary>
partial class ExecInit : ExecBase
{
    public ExecInit(ViewModel viewModel) : base(viewModel) { }
    public async override void Run()
    {
        var list = (await GetJsonFileInfos()).ToList();
        TryAddDb(list);

        vm.Title = $"初始化组件 {App.Version}";
        vm.Context = "下载组件";
        vm.IsIndeterminate = true;

        await CreateDownloadAndUnZip(list).DownloadFileAsync();
        MessageBox.Show("初始化完成",App.Version);
        RunMain();
    }

    protected override void Complete((FileInfo info, string file) v)
    {
        var libFile = Path.Combine(LibDirectory, v.info.SourceName);
        File.Move(v.file, libFile, true);
    }  
}