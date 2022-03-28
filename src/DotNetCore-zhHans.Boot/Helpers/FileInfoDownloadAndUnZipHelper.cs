using System;
using System.IO;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// 负责下载和解压
/// </summary>
class FileInfoDownloadAndUnZipHelper
{
    private const string unZipExtension = ".zip .7z";
    private readonly CancellationToken token;
    private readonly FileInfo[] files;

    public FileInfoDownloadAndUnZipHelper(string DownloadDirectory
        , IEnumerable<FileInfo> files
        , CancellationToken token)
    {
        this.files = files.ToArray();
        this.DownloadDirectory = DownloadDirectory;
        this.token = token;
    }

    /// <summary>
    /// 下载控制，返回值不为空时下载(附加内容和进度)
    /// </summary>
    public Func<(FileInfo info, double progress), string?> UrlProvider { get; set; } = null!;
    public Action<(FileInfo info, long length)> FileDownloadLengthChange { get; set; } = null!;
    /// <summary>
    /// 解压进度
    /// </summary>
    public Action<(FileInfo info, double progress)> UnZipProgressChange { get; set; } = null!;

    /// <summary>
    /// 文件下载进度
    /// </summary>
    public Action<(FileInfo info, double progress)> FileDownloadProgressChange { get; set; } = null!;

    /// <summary>
    /// 下载和解压完成时
    /// </summary>
    public Action<(FileInfo info, string file)> FileComplete { get; set; } = null!;

    public string DownloadDirectory { get; }

    public async Task DownloadFileAsync()
    {
        var count = files.Length;
        for (int i = 0; i < count; i++)
        {
            var item = files[i];
            var progress = (double)(i + 1) / count;
            await DownloadFileAsync(item, progress);
        }
    }

    private async Task DownloadFileAsync(FileInfo fileInfo, double progress)
    {
        var url = UrlProvider?.Invoke((fileInfo, progress));
        if (url is null) return;
        var downloadFile = Path.Combine(DownloadDirectory, fileInfo.UrlName);

        for (int i = 0; i < 3; i++)
        {
            if (token.IsCancellationRequested) return;
            var ex = await DownloadHelper.DownloadFile(url, downloadFile, token
            , p => FileDownloadProgressChange?.Invoke((fileInfo, p))
            , l => FileDownloadLengthChange?.Invoke((fileInfo, l)));

            if (ex is null) break;
        }      
        await UnZipFileAsync(fileInfo, downloadFile);
    }

    private async Task UnZipFileAsync(FileInfo fileInfo, string downloadFile)
    {
        var file = Path.Combine(DownloadDirectory, fileInfo.SourceName);
        if (unZipExtension.Contains(Path.GetExtension(downloadFile))) 
        {
            await new ZipHelper(p => UnZipProgressChange?.Invoke((fileInfo, p)))
                 .UnZip(downloadFile, file);
            File.Delete(downloadFile);
        }      
        FileComplete?.Invoke((fileInfo, file));
    }
}
