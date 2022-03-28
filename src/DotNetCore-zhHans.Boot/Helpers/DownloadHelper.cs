using System;
using System.IO;
using System.Net.Http;

namespace DotNetCore_zhHans.Boot;

static class DownloadHelper
{
    private const HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
    public static async Task<Exception?> DownloadFile(string url, string file
        , CancellationToken token
        , Action<double>? progressReport = null
        , Action<long>? lengthReport = null)
    {
        Debug.Print($"下载:{url}");
        Exception? res = null;
        try
        {
            await ExecDownload(url, file, token, progressReport, lengthReport);
        }
        catch (Exception ex)
        {
            res = ex;
        }
        return res;
    }

    private static async Task ExecDownload(string url, string file
        , CancellationToken token
        , Action<double>? progressReport = null
        , Action<long>? lengthReport = null)
    {
        using var hc = new HttpClient();
        using var response = await hc
            .GetAsync(url, httpCompletionOption, token)
            .ConfigureAwait(false);

        var length = response.Content.Headers.ContentLength;
        lengthReport?.Invoke(length ?? 0);

        using var responseStream = await response.Content
            .ReadAsStreamAsync(token)
            .ConfigureAwait(false);

        await ExecDownload(responseStream, file, length, token, progressReport);
    }

    private static async Task ExecDownload(Stream stream
     , string file
     , long? length
     , CancellationToken token
     , Action<double>? progressReport = null)
    {
        using var fileStream = File.Create(file);
        long position = 0;
        var count = 0;
        var buffer = new byte[8192];

        while ((count = await stream.ReadAsync(buffer, token).ConfigureAwait(false)) != 0)
        {
            position += count;
            if (length.HasValue) progressReport?.Invoke((double)position / length.Value);
            await fileStream.WriteAsync(buffer.AsMemory(0, count));
        }
    }

    private static readonly string[] bs =
        new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB" };

    public static string FormatSize(long length)
    {
        var position = 0;
        double number = length;
        while (Math.Round(number / 1024, 4) >= 1)
        {
            number /= 1024;
            position++;
        }
        return $"{string.Format("{0:0.0}", number)}{bs[position]}".Replace(".0", "");
    }
}
