﻿using System.Net.Http;
using System.Text.Json.Nodes;

namespace DotNetCore_zhHans.Boot;

abstract class ExecBase
{
    private const string defaultUrl = "http://www.wyj55.cn/download/DotNetCorezhHans20";
    protected readonly ViewModel vm;
    private bool isInitZip = false;

    public ExecBase(ViewModel viewModel)
    {
        vm = viewModel;
        CurrentDirectory = Directory.GetCurrentDirectory();
        UrlPack = GetUrlPack();
        UrlRoot = UrlPack.Replace("/packs/_pack.json", "");
    }

    public string UrlPack { get; }

    public string UrlRoot { get; }

    /// <summary>
    /// 当前目录
    /// </summary>
    public string CurrentDirectory { get; }
    /// <summary>
    /// 目标目录
    /// </summary>
    public string LibDirectory => CreateSubDirectory(Path.Combine(CurrentDirectory, "lib"));
    /// <summary>
    /// 下载目录
    /// </summary>
    public string DownloadDirectory => CreateSubDirectory(Path.Combine(LibDirectory, "_download"));

    public CancellationToken Token => vm.cancellation.Token;

    private void InitZip()
    {
        if (isInitZip) return;
        var zipDll = Path.Combine(LibDirectory, "7z.dll");
        if (File.Exists(zipDll))
        {
            isInitZip = true;
            ZipHelper.Init();
        }
    }

    public abstract void Run();

    /// <summary>
    /// 在当前目录下创建子目录
    /// </summary>
    protected static string CreateSubDirectory(string dirName)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), dirName);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private string GetUrlPack()
    {
        var defUrl = $"{defaultUrl}/packs/_pack.json";
        var jsonPath = GetConfigJson() ?? Path.Combine(CurrentDirectory, "DotNetCore-zhHans.Config.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            var jObj = JsonNode.Parse(json)!;
            return jObj["PackagesUrl"]?.GetValue<string>() ?? defUrl;
        }
        return defUrl;

        static string? GetConfigJson() => App.Args
            .FirstOrDefault(x => x.EndsWith("DotNetCore-zhHans.Config.json"));
    }

    protected async Task<FileInfo[]> GetPackJson()
    {
        using var hc = new HttpClient();
        var json = await hc.GetStringAsync(UrlPack);
        return JsonSerializer.Deserialize<FileInfo[]>(json)?
            .OrderByDescending(x => x.Index).ToArray()
            ?? Array.Empty<FileInfo>();
    }

    protected virtual string? GetUrl((FileInfo info, double progress) v)
    {
        InitZip();
        return v.info.DownloadUrl;
    }

    protected virtual void FileDownloadLengthChange((FileInfo info, long length) v) { }
    protected virtual void FileDownloadProgressChange((FileInfo info, double progress) v) { }
    protected virtual void UnZipProgressChange((FileInfo info, double progress) v) { }
    protected virtual void Complete((FileInfo info, string file) v) { }

    protected FileInfoDownloadAndUnZipHelper CreateDownloadAndUnZip(IEnumerable<FileInfo> files) =>
    new(DownloadDirectory, files, Token)
    {
        UrlProvider = GetUrl,
        FileDownloadLengthChange = FileDownloadLengthChange,
        FileDownloadProgressChange = FileDownloadProgressChange,
        UnZipProgressChange = UnZipProgressChange,
        FileComplete = Complete,
    };

    protected void RunMain()
    {
        var exe = Path.Combine(LibDirectory, "DotNetCorezhHansMain.exe");
        Process.Start(exe, "--run");
        Environment.Exit(0);
    }

}