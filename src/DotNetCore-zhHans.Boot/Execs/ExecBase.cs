using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace DotNetCore_zhHans.Boot;

abstract class ExecBase
{
    private const HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
    protected readonly ViewModel vm;


    public ExecBase(ViewModel viewModel)
    {
        vm = viewModel;
        UrlPack = GetUrlPack();
        UrlRoot = UrlPack.Replace("/pack/_pack.json", "");
        CurrentDirectory = GetCurrentDirectory();
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
    public string DownloadDirectory => CreateSubDirectory(Path.Combine(CurrentDirectory, "_download"));

    public CancellationToken CancellationToken => vm.cancellation.Token;

    protected virtual void InitZip() => ExecInitZip();

    private static void ExecInitZip() => ZipHelper.Init();

    public abstract void Run();

    protected static async Task<(string file, Exception? exception)> DownloadFile(string url, string file
      , CancellationToken token
      , Action<double>? action = null)
    {
        Debug.Print($"下载:{url}");
        var fileTmp = $"{file}.tmp";
        new[] { fileTmp, file }.Where(File.Exists).ToList().ForEach(File.Delete);
        Exception? resException = null;
        try
        {
            await ExecDownload(url, fileTmp, token, action);
        }
        catch (Exception ex)
        {
            resException = ex;
        }
        File.Move(fileTmp, file);
        return (file, resException);
    }

    private static async Task ExecDownload(string url, string file
     , CancellationToken token
     , Action<double>? action = null)
    {
        using var fileStream = File.Create(file);
        using var hc = new HttpClient();
        using var response = await hc.GetAsync(url, httpCompletionOption, token)
            .ConfigureAwait(false);
        long position = 0;
        var length = response.Content.Headers.ContentLength;
        using var responseStream = await response.Content
            .ReadAsStreamAsync(token)
            .ConfigureAwait(false);

        var buffer = new byte[102400];
        var count = 0;
        while ((count = await responseStream.ReadAsync(buffer, token).ConfigureAwait(false)) != 0)
        {
            position += count;
            if (length.HasValue) action?.Invoke((double)position / length.Value);
            await fileStream.WriteAsync(buffer.AsMemory(0, count));
        }
    }

    /// <summary>
    /// 在当前目录下创建子目录
    /// </summary>
    protected static string CreateSubDirectory(string dirName)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), dirName);
        Directory.CreateDirectory(dir);
        return dir;
    }

    /// <summary>
    /// 下载并解压
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tmpPath">临时文件位置</param>
    /// <param name="outPath">解压输出位置</param>
    protected async Task DownloadAndUnZip(string url, string tmpPath, string outPath
       , Action<double>? downloadProgress = null
       , Action<double>? unzipProgress = null)
    {
        await DownloadFile(url, tmpPath, CancellationToken, downloadProgress);
        await new ZipHelper(unzipProgress).UnZip(tmpPath, outPath);
        File.Delete(tmpPath);
    }

    protected async Task Init7z()
    {
        var name = "7z.dll";
        var zipName = $"{name}.zip";
        var url = $"{UrlRoot}/pack/{zipName}";
        var tmpPath = Path.Combine(Directory.GetCurrentDirectory(), zipName);
        var libFile = Path.Combine(CreateSubDirectory("lib"), name);
        await DownloadAndUnZip(url, tmpPath, libFile);
        ExecInitZip();
    }

    protected static string? GetConfigJson() => App.Args.FirstOrDefault(x => x.EndsWith("DotNetCore-zhHans.Config.json"));

    private static string GetCurrentDirectory()
    {
        var jsonDir = GetConfigJson();
        if (jsonDir != null) jsonDir = Path.GetDirectoryName(jsonDir);
        return jsonDir ?? Directory.GetCurrentDirectory();
    }

    private static string GetUrlPack()
    {
        var defaultUrl = "http://www.wyj55.cn/download/DotNetCorezhHans20/pack/_pack.json";
        var jsonPath = GetConfigJson() ?? Path.Combine(GetCurrentDirectory(), "DotNetCore-zhHans.Config.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            var jObj = JsonNode.Parse(json)!;
            return jObj[" PackagesUrl "]?.GetValue<string>() ?? defaultUrl;
        }
        return defaultUrl;
    }

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
}
