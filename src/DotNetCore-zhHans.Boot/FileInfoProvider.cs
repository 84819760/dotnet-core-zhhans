using System.Security.Cryptography;
using System.Text;

namespace DotNetCore_zhHans.Boot;

class FileInfoProvider
{
    public string DirectoryPath { get; init; } = null!;

    public FileInfo[] GetFileInfos()
    {
        return Directory.GetFiles(DirectoryPath)
            .Where(x => ".xml.exe.dll.json".Contains(Path.GetExtension(x)))
            .Where(x => !x.Contains("DotNetCore-zhHans.Config.json"))
            .Select(CreateFileInfo).ToArray();

        FileInfo CreateFileInfo(string path) =>
            new FileInfo() { SourceName = Path.GetFileName(path) }.InitMd5(DirectoryPath);
    }
}
