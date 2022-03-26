using System.IO.Compression;
using System.Security.Cryptography;

namespace DotNetCore_zhHans.Boot;

public class FileInfo
{
    public string SourceName { get; init; } = null!;

    public string ExtensionName { get; set; } = null!;

    public string? Md5 { get; set; } = null!;

    private bool IsJson => Path.GetExtension(SourceName) == ".json";

    private static string? GetMd5(string? filePath)
    {
        if (filePath is null || !File.Exists(filePath)) return default;
        try
        {
            using var file = new FileStream(filePath, FileMode.Open);
            var md5 = MD5.Create();
            var hashValues = md5.ComputeHash(file);
            var hashStr = hashValues.Select(x => x.ToString("X2"));
            return string.Join("", hashStr);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public FileInfo InitMd5(string directory)
    {
        Md5 = GetMd5(Path.Combine(directory, SourceName));
        return this;
    }

    public override string ToString() => $"{nameof(FileInfo)}: {SourceName} : {Md5} :{ExtensionName}";

    public void CreateZip(string sourceDirectory, string packDirectoryName)
    {
        var outDirectory = Path.Combine(sourceDirectory, packDirectoryName);
        ExtensionName = JsonHandler(sourceDirectory, outDirectory)
            ?? ZipHandler(sourceDirectory, outDirectory);
    }

    private (string source, string target) GetPath(string sourceDirectory, string outDirectory)
    {
        var source = Path.Combine(sourceDirectory, SourceName);
        var target = Path.Combine(outDirectory, SourceName);
        return (source, target);
    }

    private string? JsonHandler(string sourceDirectory, string outDirectory)
    {
        if (!IsJson) return default;
        var (source, target) = GetPath(sourceDirectory, outDirectory);
        File.Copy(source, target, true);
        return "";
    }

    private string ZipHandler(string sourceDirectory, string outDirectory)
    {
        var (source, target) = GetPath(sourceDirectory, outDirectory);
        Compress(source, target + ".gz");
        return ".gz";
    }

    private static void Compress(string file, string gzFile)
    {
        using var source = File.OpenRead(file);
        using var target = File.Create(gzFile);
        using var ZipStream = new GZipStream(target, CompressionMode.Compress);
        source.CopyTo(ZipStream);
    }
}
