using System.Threading.Tasks;
using System;
using DotNetCorezhHans.Messages;
using DotNetCoreZhHans.Service;

namespace DotNetCorezhHans.TranslTasks
{
    /// <summary>
    /// 负责文件扫描
    /// </summary>
    internal class SecnFileTask : TaskBase
    {
        private readonly XmlFileProvider xmlFileProvider;

        public SecnFileTask(TranslManager translManager) : base(translManager)
        {
            xmlFileProvider = new(translManager.Config, translManager.Token);
            xmlFileProvider.SendAddXmlFilePath += SendAddXmlFilePathHander;
            xmlFileProvider.SendScanPath += SendScanPathHander;
        }

        private void SendScanPathHander(string path)
        {
            if (IsCancel) return;
            Progress.Files.ScanContent = path;
        }

        private void SendAddXmlFilePathHander(string path)
        {
            if (translManager.IsCancel) return;
            Progress.Files.AddFile(path);
        }

        public override async Task<PageControlType> Run()
        {
            await xmlFileProvider.ScanFiles();
            translManager.Progress.Files.InitFileItems();
            return translManager.IsCancel
                ? PageControlType.TerminationTask
                : await new TranslTask(translManager).Run();
        }   
    }
}
