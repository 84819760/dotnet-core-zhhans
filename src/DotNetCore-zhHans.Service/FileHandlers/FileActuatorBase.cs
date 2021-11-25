using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.FileHandlers.FileActuators;
using DotNetCoreZhHans.Service.XmlFileProviders;

namespace DotNetCoreZhHans.Service.FileHandlers
{
    /// <summary>
    /// XML文件分类执行器(文件导入还是翻译)
    /// </summary>
    internal abstract class FileActuatorBase : ITransmitData
    {
        protected FileActuatorBase(ITransmitData transmits)
        {
            Transmits = transmits;
            Token = transmits.Token;
        }

        public ITransmitData Transmits { get; }

        public CancellationToken Token { get; }

        public abstract Task Run();

        public static Task CreateFactory(ITransmitData transmits)
        {
            var test = new XmlFileTest(transmits);
            return TestIsChinese(test, transmits);
        }

        /// <summary>
        /// 测试是否为中文或非英语内容
        /// </summary>
        private static async Task TestIsChinese(XmlFileTest test, ITransmitData transmits)
        {
            if (!test.IsChinese) await TestZhHansFile(test, transmits);
            else AddError(test, transmits);
        }

        private static void AddError(XmlFileTest test, ITransmitData transmits)
        {
            var error = new Exception($"检测到非英语内容,执行跳过：({test.SkipReason.Trim()})");
            transmits.AddError(error, new FilePath() { Path = test.FilePath }, 0);
        }

        /// <summary>
        /// 测试是否包含ZhHans文件
        /// </summary>
        private static Task TestZhHansFile(XmlFileTest test, ITransmitData transmits)
        {
            var zhHansFile = test.CreateZhHansXmlFile();
            return zhHansFile is not null
                ? TestHistorical(zhHansFile, transmits)
                : RequestTranslation(test, transmits);
        }

        /// <summary>
        /// 测试是否为历史生成
        /// </summary>
        private static Task TestHistorical(XmlFileTest test, ITransmitData transmits)
        {
            var data = (XmlFileTestZhHans)test;
            return data.IsDotNetCoreZhHans ? TestCover(data, transmits) : ImportCheck(data, transmits);
        }

        /// <summary>
        /// 测试是否覆盖
        /// </summary>
        private static Task TestCover(XmlFileTestZhHans test, ITransmitData transmits) =>
            test.IsCover ? RequestTranslation(test, transmits) : Task.CompletedTask;

        /// <summary>
        /// 导入检查
        /// </summary>
        private static Task ImportCheck(XmlFileTestZhHans test, ITransmitData transmits)
        {
            if (!transmits.Config.EnableImport) return Task.CompletedTask;
            throw new NotImplementedException("导入功能未实现");
        }

        /// <summary>
        /// 请求翻译
        /// </summary>
        private static Task RequestTranslation(XmlFileTest _, ITransmitData transmits) =>
            new TranslateFileActuator(transmits).Run();

        #region Transmits       

        public virtual ConfigManager Config => Transmits.Config;

        public IProgress Progress => Transmits.Progress;

        public IFileProgress File => Transmits.File;

        public string Version => Transmits.Version;

        public GcManager GcManager => Transmits.GcManager;

        public CancellationTokenSource CancellationTokenSource => Transmits.CancellationTokenSource;

        public string this[string key] { get => Transmits[key]; set => Transmits[key] = value; }

        public ReaderWriterLockSlim DbLock => Transmits.DbLock;

        public Exception Interrupt { get => Transmits.Interrupt; set => Transmits.Interrupt = value; }

        public void AddError(Exception exception, IFilePath file, int index) => Transmits.AddError(exception, file, index);

        #endregion
    }
}