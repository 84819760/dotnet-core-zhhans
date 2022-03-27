using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using SevenZip;
namespace DotNetCore_zhHans.Boot;
using func = Func<string, string, Task>;
using zip = System.IO.Compression;
class ZipHelper
{
    public static void Init()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var target = Path.Combine(currentDir, "7z.dll");
        var libDir = Path.Combine(currentDir, "lib");
        var final = Path.Combine(libDir, "7z.dll"); ;
        if (File.Exists(target))
        {
            Directory.CreateDirectory(libDir);
            try
            {
                File.Move(target, final, true);
            }
            finally { }
        }
        SevenZipBase.SetLibraryPath(final);
    }

    public ZipHelper(Action<double>? action = null)
    {
        if (action is not null) Progress += action;
    }


    public event Action<double> Progress = null!;

    public async Task Zip(string filePath, string zipFile)
    {
        func handler = zipFile.EndsWith(".zip") ? CompressZip : CompressPpmd;
        await handler(filePath, zipFile).ConfigureAwait(false);
    }

    public async Task UnZip(string zipFile, string filePath)
    {
        func handler = zipFile.EndsWith(".zip") ? DecompressZip : DecompressPpmd;
        await handler(zipFile, filePath).ConfigureAwait(false);
    }

    private static async Task CompressZip(string filePath, string zipFile)
    {
        using var source = File.OpenRead(filePath);
        using var target = File.Create(zipFile);
        using var zipStream = new GZipStream(target, zip.CompressionMode.Compress);
        await source.CopyToAsync(zipStream).ConfigureAwait(false);
    }

    private static async Task DecompressZip(string zipFile, string filePath)
    {
        using var source = File.OpenRead(zipFile);
        using var target = File.Create(filePath);
        using var zipStream = new GZipStream(source, zip.CompressionMode.Decompress);
        await zipStream.CopyToAsync(target).ConfigureAwait(false);
    }

    private async Task CompressPpmd(string filePath, string zipFile)
    {
        var compressor = new SevenZipCompressor()
        {
            CompressionLevel = SevenZip.CompressionLevel.High,
            CompressionMethod = CompressionMethod.Ppmd,
        };
        compressor.Compressing += (s, e) => Progress?.Invoke((double)e.PercentDone / 100);
        await compressor.CompressFilesAsync(zipFile, filePath).ConfigureAwait(false);
    }

    private async Task DecompressPpmd(string zipFile, string filePath)
    {
        using var target = File.Create(filePath);
        using var zip = new SevenZipExtractor(zipFile);
        zip.Extracting += (s, e) => Progress?.Invoke((double)e.PercentDone / 100);
        await zip.ExtractFileAsync(0, target).ConfigureAwait(false);
    }
}
