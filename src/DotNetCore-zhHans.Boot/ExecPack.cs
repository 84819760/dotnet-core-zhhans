using System.Windows;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 打包
/// </summary>
partial class ExecPack : ExecBase
{
    public ExecPack(ViewModel viewModel) : base(viewModel) { }

    public override void Run()
    {
        var dir = @"D:\_Code\01_Git\dot-net-core-zh-hans-open\src\DotNetCore-zhHans\bin\Release\net6.0-windows\publish\win-x64";
        var packName = "packs";
        var data = new FileInfoProvider() { DirectoryPath = dir }.GetFileInfos();
        data.ToList().ForEach(x => x.CreateZip(dir, packName));

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true });
        var packFile = Path.Combine(dir, packName, "_packs.json");
        if (File.Exists(packFile)) File.Delete(packFile);
        File.WriteAllText(packFile, json);
        MessageBox.Show("完成");
        Environment.Exit(0);
    }
}
