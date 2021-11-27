using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearCoreExtensions;


namespace Dotnet_Intellisense
{
    internal class RunHelper
    {
        private const string download = "Download";
     
        public static async Task Run(DataModel item, Action<string> setTitle)
        {

            var title = $"下载{item.Name}";
            setTitle(title);
            InitDownload();
            await DownloadFile(item, setTitle, title);
        }

        private static async Task DownloadFile(DataModel item, Action<string> setTitle, string title)
        {
            var target = Path.Combine(download, $"{item.Name}.zip");
            if (File.Exists(target)) return;
            Directory.CreateDirectory(Path.Combine(download, "tmp"));
            await DownloadFile(item.Url!, target, setTitle, title);
        }

        private static async Task DownloadFile(string url, string file, Action<string> setTitle, string title)
        {
            
        }

        private static void InitDownload()
        {
            if (Directory.Exists(download)) return;
            Directory.CreateDirectory(download);
        }
    }
}
