namespace DotNetCore_zhHans.Boot;

public class FileInfo
{
    public string SourceName { get; init; } = null!;

    public string? ExtensionName { get; set; } = null!;

    public string? Md5 { get; set; } = null!;

    private bool IsJson => Path.GetExtension(SourceName) == ".json";

    public FileInfo InitMd5(string directory)
    {
        Md5 = Share.GetMd5(Path.Combine(directory, SourceName));
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
}
