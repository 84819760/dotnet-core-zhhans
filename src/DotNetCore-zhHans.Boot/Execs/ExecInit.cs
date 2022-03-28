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

        vm.Title = "初始化组件";
        vm.Context = "下载组件";
        vm.IsIndeterminate = true;

        await CreateDownloadAndUnZip(list).DownloadFileAsync();
        MessageBox.Show("完成");
        RunMain();
    }

    protected override void Complete((FileInfo info, string file) v)
    {
        var libFile = Path.Combine(LibDirectory, v.info.SourceName);
        File.Move(v.file, libFile, true);
    }

    private void TryAddDb(List<FileInfo> list)
    {
        var name = "TranslData.db";
        var target = Path.Combine(LibDirectory, name);
        if (File.Exists(target)) return;
        list.Add(new()
        {
            ExtensionName = ".7z",
            PackUrl = "http://www.wyj55.cn/download/DotNetCorezhHans20/",
            SourceName = name,
            Md5 = Guid.NewGuid().ToString(),
            ShowMsg = "下载数据库"
        });
    }


}