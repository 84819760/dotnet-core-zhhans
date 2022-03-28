namespace DotNetCore_zhHans.Boot;

public class FileInfo
{
    public string SourceName { get; init; } = null!;

    public string? ExtensionName { get; set; } = null!;

    public int Index { get; set; }

    public string? Cmd { get; set; }

    public string? Md5 { get; set; } = null!;

    public string PackUrl { get; init; } = null!;

    private bool IsJson => Path.GetExtension(SourceName) == ".json";

    public string? ShowMsg { get; set; }

    private string? GetMD5Value(string directory)=> Share.GetMd5(Path.Combine(directory, SourceName));

    public FileInfo InitMd5(string directory)
    {
        Md5 = GetMD5Value(directory);
        return this;
    }

    public override string ToString() => $"{nameof(FileInfo)}: {SourceName}{ExtensionName} : {Md5}";


    public (string source, string target) GetFullPath(string directory, string targetDirectory)
    {
        var source = Path.Combine(directory, SourceName);
        var pack = Path.Combine(directory, targetDirectory, $"{SourceName}{ExtensionName}");
        return (source, pack);
    }

    public string UrlName => $"{SourceName}{ExtensionName}";

    public string DownloadUrl => $"{PackUrl}/{UrlName}";

    public bool TestMd5(string libDir) => Md5 == GetMD5Value(libDir);
}
