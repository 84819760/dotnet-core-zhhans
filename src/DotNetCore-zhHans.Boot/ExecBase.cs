using System.Net.Http;

namespace DotNetCore_zhHans.Boot;

abstract class ExecBase
{
    private const HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
    protected readonly string url = "http://www.wyj55.cn/download/DotNetCorezhHans10";

    public ExecBase(ViewModel viewModel) => ViewModel = viewModel;

    public ViewModel ViewModel { get; }

    public CancellationToken CancellationToken => ViewModel.cancellation.Token;

    public abstract void Run();

    protected async Task<(string file, Exception? exception)> DownloadFile(string url, string file
      , CancellationToken token
      , Action<(long? length, long received)>? action = null)
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
     , Action<(long? length, long received)>? action = null)
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
            action?.Invoke((length, position));
            await fileStream.WriteAsync(buffer.AsMemory(0, count));
        }
    }
}
