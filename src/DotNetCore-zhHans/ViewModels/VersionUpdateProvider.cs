using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NearCoreExtensions;

namespace DotNetCorezhHans.ViewModels
{
    internal class VersionUpdateProvider
    {
        private static readonly string currentDirectory = Directory.GetCurrentDirectory();
        private const string updateExe = "DotNetCorezhHans.Update.exe";
        private static readonly string downloadPath = Path
          .Combine(currentDirectory, "Download");

        private bool isDownload;

        public Action<int> SetProgress { get; init; }

        public Action<string> SetTitle { get; init; }

        public Action FirstRun { get; init; }

        private string GetFilePath() => Path.Combine(downloadPath, $"{Guid.NewGuid()}.tmp");

        public async Task DownloadFile(string updateUrl)
        {
            if (isDownload) return;
            isDownload = true;
            FirstRun.Invoke();
            SetTitle?.Invoke("开始下载");

            var filePath = GetFilePath();
            await HttpDownloadRun(filePath, updateUrl);
            SetTitle?.Invoke("执行更新");

            ExecUpdate(filePath);
        }

        private async static void ExecUpdate(string filePath)
        {
            var updateExeSoruce = GetUpdateExe();
            var newUpdateExe = Path.Combine(downloadPath, updateExe);
            await FileCopy(updateExeSoruce, newUpdateExe);
            Process.Start(newUpdateExe, $"{Application.ExecutablePath} {filePath}");
            Environment.Exit(0);
        }

        private static async Task FileCopy(string updateExe, string newUpdateExe)
        {
            while (true)
            {
                try
                {
                    File.Copy(updateExe, newUpdateExe, true);
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }

        private async Task HttpDownloadRun(string filePath, string updateUrl)
        {
            var hd = new HttpDownload(updateUrl, filePath);
            hd.ProgressEvent += e => SetProgress?.Invoke(e);
            await hd.Download();
        }

        private static string GetUpdateExe() => new[]
        {
            Path.Combine(currentDirectory, "lib", updateExe),
            Path.Combine(currentDirectory, updateExe),
        }.Where(File.Exists).First();
    }
}
