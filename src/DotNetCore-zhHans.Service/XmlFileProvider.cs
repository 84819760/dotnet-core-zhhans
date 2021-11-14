using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 负责扫描xml文档文件
    /// </summary>
    public class XmlFileProvider
    {
        private readonly CancellationToken token;
        private readonly ConfigManager config;

        public event Action<string> SendAddXmlFilePath;
        public event Action<string> SendScanPath;

        /// <summary>
        /// <inheritdoc cref="XmlFileProvider"/>
        /// </summary>
        public XmlFileProvider(ConfigManager config, CancellationToken token)
        {
            this.config = config;
            this.token = token;
        }

        public Task ScanFiles() => Task.Run(GetXmlFilePaths, token);

        public void GetXmlFilePaths() => _ = config.Directorys
            .Where(ExistsDirectory)
            .Select(FileExtensions.ExpandEnvironmentVariables)
            .SelectMany(GetFiles).Count();

        private bool ExistsDirectory(string directory) => Directory.Exists(directory);

        private IEnumerable<string> GetFiles(string directory) =>
             TestAndGetFiles(directory).Concat(TestAndGetDirectories(directory));

        private IEnumerable<string> TestAndGetFiles(string directory) =>
             TryGet(directory, GetXmlFiles, Array.Empty<string>);

        private IEnumerable<string> TestAndGetDirectories(string directory) =>
            TryGet(directory, GetDirectories, Array.Empty<string>);

        private IEnumerable<string> GetXmlFiles(string directory) => Directory
            .EnumerateFiles(directory, "*.xml")
            .Where(ExistDll)
            .Where(TryIsDoc)
            .Select(SendPath)
            .Select(SendFilePath);

        private IEnumerable<string> GetDirectories(string directory) => Directory
            .EnumerateDirectories(directory)
            .Select(SendPath)
            .SelectMany(GetFiles);

        private bool TryIsDoc(string path) => TryGet(path, IsDoc, () => false);

        private bool IsDoc(string path) => File.ReadLines(path).Take(2).Contains("<doc>");

        private string SendPath(string path)
        {
            SendScanPath?.Invoke(path);
            return path;
        }

        private string SendFilePath(string path)
        {
            SendAddXmlFilePath?.Invoke(path);
            return path;
        }

        private TResult TryGet<TResult>(string value
            , Func<string, TResult> func
            , Func<TResult> defaultValue)
        {
            try
            {
                return token.IsCancellationRequested ? defaultValue() : func(value);
            }
            catch (Exception)
            {
                return defaultValue();
            }
        }

        private static bool ExistDll(string path)
        {
            var fileName = $"{Path.GetFileNameWithoutExtension(path)}.dll";
            var dllFile = Path.Combine(Path.GetDirectoryName(path), fileName);
            return File.Exists(dllFile);
        }
    }
}
