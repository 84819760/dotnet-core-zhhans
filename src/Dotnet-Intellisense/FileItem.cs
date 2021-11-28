using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;
using NearCoreExtensions;

namespace Dotnet_Intellisense
{
    internal class FileItem
    {
        private readonly string tempPath = Path.GetTempPath();

        public FileItem(DataModel data)
        {
            Data = data;
            ZipFile = Path.Combine("Download", $"{Name}.zip");
        }

        public event Action<string>? Notice;

        public DataModel Data { get; }

        public string Name => Data.Name!;

        public string? ZipFile { get; set; }

        public string XmlFiles => $"{ZipFile}XmlFiles";

        internal async Task Run()
        {
            Notice?.Invoke($"更新{Name}");
            await InitZipFile();
            await ReplaceFile();
            Data.IsSelect = false;
        }

        private async Task ReplaceFile()
        {
            if (!File.Exists(ZipFile)) return;
            Notice?.Invoke($"更新文件{Name}");
            await Task.Run(ReplaceXmlFile);
        }

        private async Task ReplaceXmlFile()
        {
            ExtractZip();
            foreach (var item in Data?.DataDetails!)
                await ReplaceXmlFile(item);
            try { Directory.Delete(XmlFiles, true); }
            catch (Exception) { }
        }

        private void ExtractZip() => new FastZip().ExtractZip(ZipFile, XmlFiles, "");

        private async Task ReplaceXmlFile(DataDetailsModel detail)
        {
            var sorce = Path.Combine(XmlFiles, detail.Source!);
            if (!Directory.Exists(sorce))
            {
                MessageBox.Show($"找不到路径{sorce}");
                return;
            }

            foreach (var item in Directory.GetFiles(sorce))
            {
                var fileName = Path.GetFileName(item);
                await ReplaceXmlFile(item, $"{detail.Target}\\{fileName}");
            }
        }

        private async Task ReplaceXmlFile(string sorce, string target)
        {
            var txt = $"替换{target}";
            CreateDirectory(target);
            try
            {
                Notice?.Invoke(txt);
                File.Move(sorce, target, true);
            }
            catch (Exception ex)
            {
                Notice?.Invoke($"{txt}时发生异常:\r\n{ex.Message}");
                await Task.Delay(1000);
            }
        }

        private async Task InitZipFile()
        {
            if (File.Exists(ZipFile)) return;
            CreateDirectory(ZipFile!);
            var zipFile = Path.Combine(tempPath, $"{Guid.NewGuid()}.zip");
            var hd = new HttpDownload(Data.Url, zipFile);
            hd.ProgressEvent += p => Notice?.Invoke($"下载{Name} ({p}%)");
            var isOk = await hd.Download();
            InitZipFile(isOk, zipFile);
        }

        private void InitZipFile(bool isOk, string zipFile)
        {
            if (!isOk)
            {
                MessageBox.Show($"{Name}下载失败");
                return;
            }

            File.Move(zipFile, ZipFile!);
        }

        private static void CreateDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir)) return;
            Directory.CreateDirectory(dir!);
        }
    }
}
