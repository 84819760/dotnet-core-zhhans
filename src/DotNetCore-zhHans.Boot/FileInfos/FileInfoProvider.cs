using System.Security.Cryptography;
using System.Text;

namespace DotNetCore_zhHans.Boot;

class FileInfoProvider
{
    public FileInfoProvider(string directoryPath) => DirectoryPath = directoryPath;

    public string DirectoryPath { get; }

    public FileInfo[] GetFileInfos()
    {
        return Directory.GetFiles(DirectoryPath)
            .Where(x => Path.GetExtension(x) != ".pdb")
            .Where(x => !x.Contains("DotNetCore-zhHans.Config.json"))
            .Select(CreateFileInfo).ToArray();

        FileInfo CreateFileInfo(string path) =>
            new FileInfo()
            {
                SourceName = Path.GetFileName(path),
                PackUrl= "http://www.wyj55.cn/download/DotNetCorezhHans20/packs"
            }.InitMd5(DirectoryPath);
    }
}
