using System.Net.Http;

namespace DotNetCore_zhHans.Boot;

abstract class ExecBase
{
    private const HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
    protected readonly ViewModel vm;

    public ExecBase(ViewModel viewModel) => vm = viewModel;
    public CancellationToken CancellationToken => vm.cancellation.Token;

    protected virtual void InitZip() => ZipHelper.Init();

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
}
